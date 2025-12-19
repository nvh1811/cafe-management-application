-- =============================
-- TẠO DATABASE VÀ SỬ DỤNG
-- =============================
drop database Quanlycoffee
go
CREATE DATABASE Quanlycoffee;
GO
USE Quanlycoffee;
GO

-- =============================
-- BẢNG BÀN (TableFood)
-- =============================
CREATE TABLE dbo.TableFood
(
    id INT IDENTITY PRIMARY KEY,
    name NVARCHAR(100) NOT NULL DEFAULT N'chưa đặt tên',
    status NVARCHAR(100) NOT NULL DEFAULT N'Trống' -- trống || có người 
);
GO

-- =============================
-- BẢNG TÀI KHOẢN (Account)
-- =============================
CREATE TABLE dbo.Account
(
    username NVARCHAR(100) PRIMARY KEY,
    password NVARCHAR(100) NOT NULL DEFAULT 0,
    role NVARCHAR(20) -- 'admin' hoặc 'staff'
);
GO

-- =============================
-- BẢNG LOẠI MÓN (FoodCategory)
-- =============================
CREATE TABLE dbo.FoodCategory
(
    id INT IDENTITY PRIMARY KEY,
    name NVARCHAR(100) NOT NULL DEFAULT N'chưa đặt tên'
);
GO

-- =============================
-- BẢNG MÓN (Food)
-- =============================
CREATE TABLE dbo.Food
(
    id INT IDENTITY PRIMARY KEY,
    name NVARCHAR(100) NOT NULL DEFAULT N'chưa đặt tên', 
    idcategory INT NOT NULL, -- loại nào
    price FLOAT NOT NULL DEFAULT 0, -- giá
    FOREIGN KEY (idcategory) REFERENCES dbo.FoodCategory(id)
);
GO

-- =============================
-- BẢNG HÓA ĐƠN (Bill)
-- =============================
CREATE TABLE dbo.Bill
(
    id INT IDENTITY PRIMARY KEY, 
    datecheckin DATETIME NOT NULL DEFAULT GETDATE(), 
    datecheckout DATETIME,
    idtable INT NOT NULL,
    status INT NOT NULL DEFAULT 0, -- 1: đã thanh toán, 0: chưa thanh toán
    FOREIGN KEY (idtable) REFERENCES dbo.TableFood(id)
);
GO

-- =============================
-- BẢNG CHI TIẾT HÓA ĐƠN (BillInfo)
-- =============================
CREATE TABLE dbo.BillInfo
(
    id INT IDENTITY PRIMARY KEY, 
    idbill INT NOT NULL,
    idfood INT NOT NULL, 
    count INT NOT NULL DEFAULT 0,
    FOREIGN KEY (idbill) REFERENCES dbo.Bill(id),
    FOREIGN KEY (idfood) REFERENCES dbo.Food(id)
);
GO
--====================
-- Thêm tài khoản
insert into dbo.Account values ('Admin', '1234', 'admin')
insert into dbo.Account values ('Staff', '1234', 'staff')
-- =============================
-- KIỂM TRA KẾT QUẢ
-- =============================


--===================
-- Thêm danh mục món
IF NOT EXISTS (SELECT 1 FROM dbo.FoodCategory WHERE name = N'Đồ uống')
BEGIN
    INSERT INTO dbo.FoodCategory(name) VALUES (N'Đồ uống');
END
GO
IF NOT EXISTS (SELECT 1 FROM dbo.FoodCategory WHERE name = N'Đồ ăn')
BEGIN
    INSERT INTO dbo.FoodCategory(name) VALUES (N'Đồ ăn');
END
GO

select * from Account;
go

SELECT * FROM dbo.FoodCategory;
GO
SELECT * FROM dbo.Food;
GO
SELECT * FROM dbo.TableFood;
GO
SELECT * FROM dbo.Bill;
GO
SELECT * FROM dbo.BillInfo;
GO


