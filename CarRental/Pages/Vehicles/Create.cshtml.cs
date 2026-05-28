using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarRental.Pages.Vehicles;

public class CreateModel : PageModel
{
    private readonly IVehicleService _vehicleService;
    private readonly IBranchService _branchService;
    private readonly IVehicleCategoryService _categoryService;
    private readonly ILogger<CreateModel> _logger;

    [BindProperty]
    public Vehicle Vehicle { get; set; } = new();

    public SelectList BranchSelectList { get; set; } = new SelectList(Array.Empty<object>());
    public SelectList CategorySelectList { get; set; } = new SelectList(Array.Empty<object>());
    public SelectList StatusSelectList { get; set; } = new SelectList(Array.Empty<object>());

    public CreateModel(IVehicleService vehicleService,
                       IBranchService branchService,
                       IVehicleCategoryService categoryService,
                       ILogger<CreateModel> logger)
    {
        _vehicleService = vehicleService;
        _branchService = branchService;
        _categoryService = categoryService;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        await PopulateSelectListsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await PopulateSelectListsAsync();

        // Plaka numarasını normalize et (boşlukları standartlaştır)
        if (!string.IsNullOrWhiteSpace(Vehicle.PlateNumber))
        {
            Vehicle.PlateNumber = NormalizePlateNumber(Vehicle.PlateNumber);
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var existing = await _vehicleService.GetByPlateNumberAsync(Vehicle.PlateNumber);
            if (existing != null)
            {
                ModelState.AddModelError("Vehicle.PlateNumber", "Bu plaka zaten kayıtlı.");
                return Page();
            }

            var created = await _vehicleService.CreateAsync(Vehicle);

            TempData["SuccessMessage"] = "Araç başarıyla eklendi.";
            return RedirectToPage("/Vehicles/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Araç eklenirken hata oluştu");
            ModelState.AddModelError(string.Empty, "Kaydetme sırasında bir hata oluştu.");
            return Page();
        }
    }

    private async Task PopulateSelectListsAsync()
{
    var branches = (await _branchService.GetAllAsync())?.Where(b => b != null).ToList() ?? new List<Models.Branch>();
    var categories = (await _categoryService.GetAllAsync())?.Where(c => c != null).ToList() ?? new List<Models.VehicleCategory>();

    // HATA BURADAYDI: "Name" yerine "City" (veya Branch modelinde ekranda görünmesini istediğin property) olmalı
    BranchSelectList = new SelectList(branches, "BranchId", "City");

    var statuses = new List<string> { "available", "rented", "maintenance" };
    StatusSelectList = new SelectList(statuses);
    
    // Kategoride sorun yok çünkü VEHICLE_CATEGORY tablosunda "Name" kolonu gerçekten var
    CategorySelectList = new SelectList(categories, "CategoryId", "Name");
}
    

    private static string NormalizePlateNumber(string plateNumber)
    {
        if (string.IsNullOrWhiteSpace(plateNumber))
            return plateNumber;

        // Tüm boşlukları kaldır ve büyük harfe çevir
        var cleaned = System.Text.RegularExpressions.Regex.Replace(plateNumber.Trim(), @"\s+", "").ToUpper();

        // Minimum 5 karakter olmalı (2 şehir + 2 harf + 1 numara)
        if (cleaned.Length < 5)
            return plateNumber;

        // İlk 2 karakter şehir kodu
        var cityCode = cleaned.Substring(0, 2);
        var rest = cleaned.Substring(2);

        // Geri kalan kısmından harfleri ayır (baştan itibaren)
        var letters = "";
        var numbers = "";
        bool parsingLetters = true;

        foreach (char c in rest)
        {
            if (parsingLetters && char.IsLetter(c))
            {
                letters += c;
            }
            else
            {
                parsingLetters = false;
                if (char.IsDigit(c))
                {
                    numbers += c;
                }
            }
        }

        // Validasyon: harfler 2 veya 3 karakter, numaralar 2, 3 veya 4 karakter olmalı
        if ((letters.Length < 2 || letters.Length > 3) || (numbers.Length < 2 || numbers.Length > 4))
        {
            return plateNumber; // Geçersiz format, olduğu gibi döndür
        }

        return $"{cityCode} {letters} {numbers}";
    }
}
