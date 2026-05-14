-- =====================================================================
-- SAMPLE DATA FOR CAR RENTAL SYSTEM
-- Insert test data into the database
-- =====================================================================

-- Clear existing data (if running again)
-- DELETE FROM DAMAGE_REPORT;
-- DELETE FROM PAYMENT;
-- DELETE FROM RENTAL;
-- DELETE FROM VEHICLE_FEATURE;
-- DELETE FROM VEHICLE;
-- DELETE FROM STAFF;
-- DELETE FROM FEATURE;
-- DELETE FROM CUSTOMER;
-- DELETE FROM VEHICLE_CATEGORY;
-- DELETE FROM BRANCH;

-- =====================================================================
-- 1. BRANCHES (Şubeler)
-- =====================================================================
INSERT INTO BRANCH (city, address, phone) VALUES
('Ankara', 'Kızılay, Atatürk Caddesi No:45', '+90 312 555 0001'),
('İstanbul', 'Taksim, İstiklal Caddesi No:123', '+90 212 555 0002'),
('İzmir', 'Alsancak, Cumhuriyet Meydanı No:5', '+90 232 555 0003');

-- =====================================================================
-- 2. VEHICLE CATEGORIES (Araç Kategorileri)
-- =====================================================================
INSERT INTO VEHICLE_CATEGORY (name, description) VALUES
('Ekonomi', 'Düşük yakıt tüketimi, uygun fiyat'),
('Orta Sınıf', 'Konforlu ve ekonomik'),
('SUV', 'Geniş iç hacim ve off-road yetenekli'),
('Lüks', 'Premium özelliklere sahip araçlar'),
('Minivan', 'Aile ve grup taşımacılığı için uygun');

-- =====================================================================
-- 3. VEHICLES (Araçlar)
-- =====================================================================
INSERT INTO VEHICLE (category_id, branch_id, plate_number, brand, model, year, daily_price, status, mileage) VALUES
(1, 1, '06 ABC 001', 'Hyundai', 'i10', 2023, 300.00, 'available', 5000),
(1, 1, '06 ABC 002', 'Toyota', 'Yaris', 2023, 350.00, 'available', 3000),
(2, 1, '06 ABC 003', 'Honda', 'Civic', 2022, 500.00, 'available', 15000),
(3, 1, '06 ABC 004', 'Ford', 'EcoSport', 2022, 700.00, 'available', 12000),
(4, 1, '06 ABC 005', 'BMW', '320i', 2023, 1200.00, 'available', 2000),

(1, 2, '34 XYZ 101', 'Fiat', 'Egea', 2023, 300.00, 'rented', 1500),
(2, 2, '34 XYZ 102', 'Renault', 'Clio', 2022, 400.00, 'available', 20000),
(3, 2, '34 XYZ 103', 'Jeep', 'Compass', 2021, 800.00, 'available', 25000),
(5, 2, '34 XYZ 104', 'Peugeot', 'Expert', 2022, 1000.00, 'maintenance', 18000),

(1, 3, '35 LMN 201', 'Skoda', 'Fabia', 2023, 320.00, 'available', 2000),
(2, 3, '35 LMN 202', 'Volkswagen', 'Golf', 2022, 550.00, 'available', 18000),
(3, 3, '35 LMN 203', 'Nissan', 'Qashqai', 2021, 750.00, 'available', 30000);

-- =====================================================================
-- 4. FEATURES (Özellikler)
-- =====================================================================
INSERT INTO FEATURE (name, description) VALUES
('GPS Navigation', 'Navigasyon sistemi ile donanımlı'),
('Klima', 'Hava klimatizasyon sistemi'),
('Bluetooth', 'Kablosuz bağlantı'),
('Çocuk Koltuğu', 'Bebek ve çocuk koltukları'),
('Kamera', 'Arka kamera ile'),
('Sunroof', 'Açılır çatı'),
('Deri Koltuk', 'Lüks deri döşeme'),
('Isıtmalı Koltuk', 'Koltuk ısıtma sistemi');

-- =====================================================================
-- 5. VEHICLE_FEATURE (Araç Özellikleri)
-- =====================================================================
INSERT INTO VEHICLE_FEATURE (vehicle_id, feature_id) VALUES
(1, 1), (1, 2), (1, 3),
(2, 1), (2, 2), (2, 3),
(3, 1), (3, 2), (3, 3), (3, 4),
(4, 1), (4, 2), (4, 3), (4, 5),
(5, 1), (5, 2), (5, 3), (5, 7), (5, 8),
(6, 1), (6, 2), (6, 3),
(7, 1), (7, 2), (7, 3), (7, 4);

-- =====================================================================
-- 6. STAFF (Personel)
-- =====================================================================
INSERT INTO STAFF (branch_id, first_name, last_name, role) VALUES
(1, 'Ahmet', 'Yılmaz', 'Müdür'),
(1, 'Fatih', 'Demir', 'Operatör'),
(2, 'Zeynep', 'Kaya', 'Müdür'),
(2, 'Murat', 'Güneş', 'Operatör'),
(3, 'Ayşe', 'Şahin', 'Müdür'),
(3, 'Berk', 'Ak', 'Operatör');

-- =====================================================================
-- 7. CUSTOMERS (Müşteriler)
-- =====================================================================
INSERT INTO CUSTOMER (first_name, last_name, license_number, birth_date, email, phone) VALUES
('Emine', 'Bülbül', 'TR123456', '1995-03-15', 'emine@example.com', '+90 555 001 0001'),
('Kemal', 'Sucu', 'TR234567', '1988-07-22', 'kemal@example.com', '+90 555 001 0002'),
('Elif', 'Akbay', 'TR345678', '1992-11-10', 'elif@example.com', '+90 555 001 0003'),
('Can', 'Polat', 'TR456789', '1990-05-05', 'can@example.com', '+90 555 001 0004'),
('Nur', 'Çetin', 'TR567890', '1998-09-18', 'nur@example.com', '+90 555 001 0005');

-- =====================================================================
-- 8. RENTALS (Kiralamalar)
-- =====================================================================
INSERT INTO RENTAL (customer_id, vehicle_id, pickup_branch_id, dropoff_branch_id, start_date, end_date, total_amount, status) VALUES
-- Pending rental
(1, 1, 1, 1, NOW() + INTERVAL '2 days', NOW() + INTERVAL '5 days', NULL, 'pending'),

-- Active rental
(2, 6, 2, 2, NOW() - INTERVAL '1 day', NOW() + INTERVAL '2 days', NULL, 'active'),

-- Completed rental
(3, 7, 2, 2, NOW() - INTERVAL '10 days', NOW() - INTERVAL '8 days', 1200.00, 'completed'),

-- Completed rental (different branch return)
(4, 2, 1, 2, NOW() - INTERVAL '5 days', NOW() - INTERVAL '3 days', 1050.00, 'completed'),

-- Cancelled rental
(5, 3, 1, 1, NOW() + INTERVAL '20 days', NOW() + INTERVAL '25 days', NULL, 'cancelled');

-- =====================================================================
-- 9. PAYMENTS (Ödemeler)
-- =====================================================================
INSERT INTO PAYMENT (rental_id, amount, method) VALUES
(3, 1200.00, 'credit_card'),
(4, 1050.00, 'cash');

-- =====================================================================
-- 10. DAMAGE_REPORTS (Hasar Raporları)
-- =====================================================================
INSERT INTO DAMAGE_REPORT (rental_id, description, repair_cost, report_date) VALUES
(3, 'Ön tampon çizilmiş', 500.00, CURRENT_DATE),
(4, 'İç kaplamada leke', 150.00, CURRENT_DATE - INTERVAL '2 days');

-- =====================================================================
-- Verification queries
-- =====================================================================
-- SELECT COUNT(*) as total_customers FROM CUSTOMER;
-- SELECT COUNT(*) as total_vehicles FROM VEHICLE;
-- SELECT COUNT(*) as total_rentals FROM RENTAL;
-- SELECT * FROM VEHICLE WHERE status = 'available';
-- SELECT * FROM RENTAL WHERE status = 'active';
