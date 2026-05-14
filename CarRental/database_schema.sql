-- =====================================================================
-- CAR RENTAL DATABASE SCHEMA
-- BLM2058 Veritabanı Yönetimi - Araç Kiralama Sistemi
-- =====================================================================

-- 1. CUSTOMER (Müşteri) Tablosu
CREATE TABLE CUSTOMER (
    customer_id SERIAL PRIMARY KEY,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    license_number VARCHAR(50) UNIQUE NOT NULL,
    birth_date DATE,
    email VARCHAR(150) UNIQUE,
    phone VARCHAR(20),
    created_at TIMESTAMP DEFAULT NOW()
);

-- 2. VEHICLE_CATEGORY (Araç Kategorisi) Tablosu
CREATE TABLE VEHICLE_CATEGORY (
    category_id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    description VARCHAR(255)
);

-- 3. BRANCH (Şube) Tablosu
CREATE TABLE BRANCH (
    branch_id SERIAL PRIMARY KEY,
    city VARCHAR(100) NOT NULL,
    address VARCHAR(255) NOT NULL,
    phone VARCHAR(20)
);

-- 4. STAFF (Personel) Tablosu
CREATE TABLE STAFF (
    staff_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    role VARCHAR(50) NOT NULL,
    CONSTRAINT fk_staff_branch FOREIGN KEY (branch_id) REFERENCES BRANCH(branch_id)
);

-- 5. VEHICLE (Araç) Tablosu
CREATE TABLE VEHICLE (
    vehicle_id SERIAL PRIMARY KEY,
    category_id INT,
    branch_id INT,
    plate_number VARCHAR(20) UNIQUE NOT NULL,
    brand VARCHAR(50) NOT NULL,
    model VARCHAR(50) NOT NULL,
    year INT NOT NULL,
    daily_price DECIMAL(10,2) NOT NULL,
    status VARCHAR(20) CHECK (status IN ('available', 'rented', 'maintenance')),
    mileage INT DEFAULT 0,
    CONSTRAINT fk_vehicle_category FOREIGN KEY (category_id) REFERENCES VEHICLE_CATEGORY(category_id),
    CONSTRAINT fk_vehicle_branch FOREIGN KEY (branch_id) REFERENCES BRANCH(branch_id)
);

-- 6. FEATURE (Özellik) Tablosu
CREATE TABLE FEATURE (
    feature_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description VARCHAR(255)
);

-- 7. VEHICLE_FEATURE (Araç-Özellik Ara Tablosu)
CREATE TABLE VEHICLE_FEATURE (
    vehicle_id INT,
    feature_id INT,
    PRIMARY KEY (vehicle_id, feature_id),
    CONSTRAINT fk_vf_vehicle FOREIGN KEY (vehicle_id) REFERENCES VEHICLE(vehicle_id),
    CONSTRAINT fk_vf_feature FOREIGN KEY (feature_id) REFERENCES FEATURE(feature_id)
);

-- 8. RENTAL (Kiralama) Tablosu
CREATE TABLE RENTAL (
    rental_id SERIAL PRIMARY KEY,
    customer_id INT,
    vehicle_id INT,
    pickup_branch_id INT NOT NULL,
    dropoff_branch_id INT,
    start_date TIMESTAMP NOT NULL,
    end_date TIMESTAMP,
    total_amount DECIMAL(10,2),
    status VARCHAR(20) CHECK (status IN ('pending', 'active', 'completed', 'cancelled')),
    CONSTRAINT fk_rental_customer FOREIGN KEY (customer_id) REFERENCES CUSTOMER(customer_id),
    CONSTRAINT fk_rental_vehicle FOREIGN KEY (vehicle_id) REFERENCES VEHICLE(vehicle_id),
    CONSTRAINT fk_rental_pickup FOREIGN KEY (pickup_branch_id) REFERENCES BRANCH(branch_id),
    CONSTRAINT fk_rental_dropoff FOREIGN KEY (dropoff_branch_id) REFERENCES BRANCH(branch_id)
);

-- 9. PAYMENT (Ödeme) Tablosu
CREATE TABLE PAYMENT (
    payment_id SERIAL PRIMARY KEY,
    rental_id INT UNIQUE,
    amount DECIMAL(10,2) NOT NULL,
    payment_date TIMESTAMP DEFAULT NOW(),
    method VARCHAR(30) CHECK (method IN ('credit_card', 'cash')),
    CONSTRAINT fk_payment_rental FOREIGN KEY (rental_id) REFERENCES RENTAL(rental_id)
);

-- 10. DAMAGE_REPORT (Hasar Raporu) Tablosu
CREATE TABLE DAMAGE_REPORT (
    damage_id SERIAL PRIMARY KEY,
    rental_id INT NOT NULL,
    description VARCHAR(500) NOT NULL,
    repair_cost DECIMAL(10,2),
    report_date DATE DEFAULT CURRENT_DATE,
    CONSTRAINT fk_damage_rental FOREIGN KEY (rental_id) REFERENCES RENTAL(rental_id)
);

-- =====================================================================
-- TETİKLEYİCİ FONKSİYONU VE TETİKLEYİCİ (TRIGGER) TANIMLAMASI
-- =====================================================================

-- Kiralama tamamlandığında araç durumunu ve şubesini güncelleyen fonksiyon
CREATE OR REPLACE FUNCTION fn_update_vehicle_on_rental_complete()
RETURNS TRIGGER AS $$
BEGIN
    -- RENTAL tablosunda status alanı 'completed' değerine güncellendiğinde
    IF NEW.status = 'completed' AND OLD.status IS DISTINCT FROM 'completed' THEN
        UPDATE VEHICLE
        SET status = 'available',
            -- dropoff_branch_id NULL ise mevcut branch_id korunur (null güvenlik kontrolü)
            branch_id = COALESCE(NEW.dropoff_branch_id, branch_id)
        WHERE vehicle_id = NEW.vehicle_id;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- trg_rental_completed Tetikleyicisi
CREATE TRIGGER trg_rental_completed
AFTER UPDATE OF status ON RENTAL
FOR EACH ROW
WHEN (NEW.status = 'completed' AND OLD.status IS DISTINCT FROM 'completed')
EXECUTE FUNCTION fn_update_vehicle_on_rental_complete();
