using System.Data;
using System.Text.RegularExpressions;
using CarRental.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Pages.Reports;

public class QueriesModel : PageModel
{
    private static readonly string[] _forbiddenKeywords =
    [
        "drop", "alter", "truncate", "create",
        "grant", "revoke", "call", "do", "copy"
    ];

    private static readonly string[] _writeKeywords = ["insert", "update", "delete"];

    private static readonly List<QueryDefinition> _queryDefinitions =
    [
        new(
            "available-vehicles",
            "Müsait Araçları Listeleme",
            "Durumu available olan araçları kategori ve şube bilgisiyle listeler.",
            @"SELECT v.vehicle_id, v.plate_number, v.brand, v.model, v.year,
       v.daily_price, vc.name AS category, b.city AS branch
FROM vehicle v
JOIN vehicle_category vc ON v.category_id = vc.category_id
JOIN branch b ON v.branch_id = b.branch_id
WHERE v.status = 'available'
ORDER BY v.daily_price;"),
        new(
            "active-rentals",
            "Aktif Kiralamalar ile Müşteri Bilgileri",
            "Aktif kiralamaları müşteri ve araç detaylarıyla birlikte getirir.",
            @"SELECT r.rental_id, c.first_name || ' ' || c.last_name AS musteri,
       v.plate_number, v.brand || ' ' || v.model AS arac,
       pb.city AS alis_sube, r.start_date,
       EXTRACT(DAY FROM NOW() - r.start_date) AS gun_sayisi
FROM rental r
JOIN customer c ON r.customer_id = c.customer_id
JOIN vehicle v ON r.vehicle_id = v.vehicle_id
JOIN branch pb ON r.pickup_branch_id = pb.branch_id
WHERE r.status = 'active'
ORDER BY r.start_date;"),
        new(
            "branch-revenue",
            "Şubeye Göre Gelir Raporu",
            "Her şube için kiralama sayısı ve toplam gelir bilgisini gösterir.",
            @"SELECT b.city AS sube,
       COUNT(r.rental_id) AS kiralama_sayisi,
       COALESCE(SUM(p.amount), 0) AS toplam_gelir
FROM branch b
LEFT JOIN rental r ON r.pickup_branch_id = b.branch_id
LEFT JOIN payment p ON p.rental_id = r.rental_id
GROUP BY b.branch_id, b.city
ORDER BY toplam_gelir DESC;"),
        new(
            "most-rented-vehicles",
            "En Çok Kiralanan Araçlar",
            "Tamamlanmış kiralamalara göre en çok kiralanan araçları sıralar.",
            @"SELECT v.plate_number, v.brand || ' ' || v.model AS arac,
       COUNT(r.rental_id) AS kiralama_sayisi,
       COALESCE(SUM(p.amount), 0) AS toplam_gelir
FROM vehicle v
JOIN rental r ON v.vehicle_id = r.vehicle_id
LEFT JOIN payment p ON r.rental_id = p.rental_id
WHERE r.status = 'completed'
GROUP BY v.vehicle_id, v.plate_number, v.brand, v.model
ORDER BY kiralama_sayisi DESC
LIMIT 10;"),
        new(
            "vehicle-features",
            " Araç Özelliklerini Listeleme (M:N Sorgu)",
            "Araçların tüm özelliklerini birleştirilmiş şekilde listeler.",
            @"SELECT v.plate_number, v.brand || ' ' || v.model AS arac,
       STRING_AGG(f.name, ', ') AS ozellikler
FROM vehicle v
JOIN vehicle_feature vf ON v.vehicle_id = vf.vehicle_id
JOIN feature f ON vf.feature_id = f.feature_id
GROUP BY v.vehicle_id, v.plate_number, v.brand, v.model
ORDER BY v.plate_number;"),
        new(
            "damage-summary",
            "Hasar Raporu Özeti",
            "Hasar kayıtlarını kiralama, müşteri ve araç bilgileriyle birlikte gösterir.",
            @"SELECT r.rental_id,
       c.first_name || ' ' || c.last_name AS musteri,
       v.plate_number,
       dr.description,
       dr.repair_cost,
       dr.report_date
FROM damage_report dr
JOIN rental r ON dr.rental_id = r.rental_id
JOIN customer c ON r.customer_id = c.customer_id
JOIN vehicle v ON r.vehicle_id = v.vehicle_id
ORDER BY dr.report_date DESC;")
    ];

    private readonly CarRentalContext _context;

    public QueriesModel(CarRentalContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public string SelectedQueryKey { get; set; } = "available-vehicles";

    [BindProperty]
    public string CustomSql { get; set; } = string.Empty;

    public string SelectedQueryTitle { get; private set; } = string.Empty;
    public string SelectedQueryDescription { get; private set; } = string.Empty;
    public string SelectedSql { get; private set; } = string.Empty;
    public string? CustomQueryError { get; private set; }
    public string? CustomQueryResultMessage { get; private set; }

    public List<string> ResultColumns { get; private set; } = [];
    public List<List<string>> ResultRows { get; private set; } = [];

    public IReadOnlyList<QueryDefinition> QueryDefinitions => _queryDefinitions;

    public async Task OnGetAsync()
    {
        await RunSelectedQueryAsync(SelectedQueryKey);
    }

    public async Task<IActionResult> OnPostRunAsync(string queryKey)
    {
        SelectedQueryKey = queryKey;
        CustomQueryError = null;
        await RunSelectedQueryAsync(SelectedQueryKey);
        return Page();
    }

    public async Task<IActionResult> OnPostRunCustomAsync()
    {
        SelectedQueryKey = "custom";
        SelectedQueryTitle = "Özel SQL Sorgusu";
        SelectedQueryDescription = "Kullanıcının yazdığı sorgu sonucu.";
        SelectedSql = CustomSql;
        CustomQueryError = null;
        CustomQueryResultMessage = null;

        if (!TryValidateCustomQuery(CustomSql, out var normalizedSql, out var queryKind, out var errorMessage))
        {
            CustomQueryError = errorMessage;
            ResultColumns = [];
            ResultRows = [];
            return Page();
        }

        CustomSql = normalizedSql;

        if (queryKind == CustomQueryKind.Read)
        {
            // Keep the page responsive by capping custom query output.
            var limitedSql = $"SELECT * FROM ({normalizedSql}) AS q LIMIT 500";
            await ExecuteQueryAsync(limitedSql);
            SelectedSql = normalizedSql;
            CustomQueryResultMessage = "Okuma sorgusu başarıyla çalıştırıldı. Sonuçlar aşağıda listeleniyor.";
        }
        else
        {
            var affectedRows = await ExecuteNonQueryAsync(normalizedSql);
            ResultColumns = [];
            ResultRows = [];
            SelectedSql = normalizedSql;
            CustomQueryResultMessage = affectedRows >= 0
                ? $"Değişiklik sorgusu başarıyla çalıştırıldı. Etkilenen satır sayısı: {affectedRows}."
                : "Değişiklik sorgusu başarıyla çalıştırıldı.";
        }

        return Page();
    }

    private async Task RunSelectedQueryAsync(string? queryKey)
    {
        var selected = _queryDefinitions.FirstOrDefault(q => q.Key == queryKey) ?? _queryDefinitions[0];

        SelectedQueryKey = selected.Key;
        SelectedQueryTitle = selected.Title;
        SelectedQueryDescription = selected.Description;
        SelectedSql = selected.Sql;

        await ExecuteQueryAsync(selected.Sql);
    }

    private async Task ExecuteQueryAsync(string sql)
    {
        ResultColumns = [];
        ResultRows = [];

        var connection = _context.Database.GetDbConnection();
        var shouldCloseConnection = connection.State != ConnectionState.Open;

        try
        {
            if (shouldCloseConnection)
            {
                await connection.OpenAsync();
            }

            await using var command = connection.CreateCommand();
            command.CommandText = sql;

            await using var reader = await command.ExecuteReaderAsync();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                ResultColumns.Add(reader.GetName(i));
            }

            while (await reader.ReadAsync())
            {
                var row = new List<string>();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.IsDBNull(i))
                    {
                        row.Add("-");
                    }
                    else
                    {
                        var value = reader.GetValue(i);
                        row.Add(value.ToString() ?? string.Empty);
                    }
                }

                ResultRows.Add(row);
            }
        }
        finally
        {
            if (shouldCloseConnection)
            {
                await connection.CloseAsync();
            }
        }
    }

    private static bool TryValidateCustomQuery(string? sql, out string normalizedSql, out CustomQueryKind queryKind, out string errorMessage)
    {
        normalizedSql = string.Empty;
        queryKind = CustomQueryKind.Read;
        errorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(sql))
        {
            errorMessage = "Lutfen bir SQL sorgusu yazin.";
            return false;
        }

        var trimmed = sql.Trim();
        while (trimmed.EndsWith(';'))
        {
            trimmed = trimmed[..^1].TrimEnd();
        }

        if (trimmed.Contains(';'))
        {
            errorMessage = "Guvenlik icin tek sorgu calistirilabilir. Birden fazla ifade kullanmayin.";
            return false;
        }

        var firstKeywordMatch = Regex.Match(trimmed, @"^(?<keyword>[a-z_]+)\b", RegexOptions.IgnoreCase);
        if (!firstKeywordMatch.Success)
        {
            errorMessage = "SQL sorgusu taninmadi.";
            return false;
        }

        var firstKeyword = firstKeywordMatch.Groups["keyword"].Value.ToLowerInvariant();

        if (firstKeyword is "select" or "with")
        {
            queryKind = CustomQueryKind.Read;
        }
        else if (_writeKeywords.Contains(firstKeyword))
        {
            queryKind = CustomQueryKind.Write;
        }
        else
        {
            errorMessage = "Sadece SELECT/WITH ile başlayan okuma sorguları veya INSERT/UPDATE/DELETE değişiklik sorguları kabul edilir.";
            return false;
        }

        var keywordPattern = @"\b(" + string.Join("|", _forbiddenKeywords) + @")\b";
        if (Regex.IsMatch(trimmed, keywordPattern, RegexOptions.IgnoreCase))
        {
            errorMessage = "Sorgu, izin verilmeyen bir ifade içeriyor. Sadece tek satırlı INSERT/UPDATE/DELETE veya SELECT/WITH kabul edilir.";
            return false;
        }

        normalizedSql = trimmed;
        return true;
    }

    private async Task<int> ExecuteNonQueryAsync(string sql)
    {
        var connection = _context.Database.GetDbConnection();
        var shouldCloseConnection = connection.State != ConnectionState.Open;

        try
        {
            if (shouldCloseConnection)
            {
                await connection.OpenAsync();
            }

            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            return await command.ExecuteNonQueryAsync();
        }
        finally
        {
            if (shouldCloseConnection)
            {
                await connection.CloseAsync();
            }
        }
    }

    private enum CustomQueryKind
    {
        Read,
        Write
    }

    public sealed record QueryDefinition(string Key, string Title, string Description, string Sql);
}
