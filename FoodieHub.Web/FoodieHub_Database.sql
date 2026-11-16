-- =========================================================================
-- 1. TẠO CƠ SỞ DỮ LIỆU FOODIEHUB (NẾU CHƯA TỒN TẠI)
-- =========================================================================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'FoodieHubDB')
BEGIN
    CREATE DATABASE [FoodieHubDB];
END
GO

USE [FoodieHubDB];
GO

-- =========================================================================
-- 2. XÓA CÁC BẢNG CŨ NẾU ĐÃ TỒN TẠI (ĐÚNG THỨ TỰ RÀNG BUỘC KHÓA NGOẠI)
-- =========================================================================
IF OBJECT_ID('dbo.OrderDetails', 'U') IS NOT NULL DROP TABLE dbo.OrderDetails;
IF OBJECT_ID('dbo.Orders', 'U')       IS NOT NULL DROP TABLE dbo.Orders;
IF OBJECT_ID('dbo.Products', 'U')     IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID('dbo.Categories', 'U')   IS NOT NULL DROP TABLE dbo.Categories;
IF OBJECT_ID('dbo.Users', 'U')        IS NOT NULL DROP TABLE dbo.Users;
GO

-- =========================================================================
-- 3. KHỞI TẠO CẤU TRÚC 5 BẢNG CHÍNH (TỔNG HỢP TỪ TẤT CẢ MIGRATIONS)
-- =========================================================================

-- 3.1. Bảng Categories (Danh mục món ăn)
CREATE TABLE [dbo].[Categories] (
    [CategoryID]   INT            IDENTITY (1, 1) NOT NULL,
    [CategoryName] NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK_dbo.Categories] PRIMARY KEY CLUSTERED ([CategoryID] ASC)
);
GO

-- 3.2. Bảng Users (Tài khoản Admin & Khách hàng)
CREATE TABLE [dbo].[Users] (
    [UserID]   INT            IDENTITY (1, 1) NOT NULL,
    [Email]    NVARCHAR (MAX) NOT NULL,
    [Password] NVARCHAR (MAX) NOT NULL,
    [FullName] NVARCHAR (MAX) NOT NULL,
    [Phone]    NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.Users] PRIMARY KEY CLUSTERED ([UserID] ASC)
);
GO

-- 3.3. Bảng Products (Sản phẩm / Món ăn)
CREATE TABLE [dbo].[Products] (
    [ProductID]   INT             IDENTITY (1, 1) NOT NULL,
    [ProductName] NVARCHAR (200)  NOT NULL,
    [Price]       DECIMAL (18, 2) NOT NULL,
    [Stock]       INT             NOT NULL,
    [ImageUrl]    NVARCHAR (MAX)  NULL,
    [CategoryID]  INT             NOT NULL,
    CONSTRAINT [PK_dbo.Products] PRIMARY KEY CLUSTERED ([ProductID] ASC),
    CONSTRAINT [FK_dbo.Products_dbo.Categories_CategoryID] FOREIGN KEY ([CategoryID]) 
        REFERENCES [dbo].[Categories] ([CategoryID]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_CategoryID] ON [dbo].[Products]([CategoryID] ASC);
GO

-- 3.4. Bảng Orders (Đơn hàng tổng)
CREATE TABLE [dbo].[Orders] (
    [OrderID]      INT             IDENTITY (1, 1) NOT NULL,
    [UserID]       INT             NULL,
    [CustomerName] NVARCHAR (MAX)  NULL,
    [Address]      NVARCHAR (MAX)  NULL,
    [OrderDate]    DATETIME        NULL,
    [TotalAmount]  DECIMAL (18, 2) NOT NULL,
    [Phone]        NVARCHAR (MAX)  NOT NULL,
    [Email]        NVARCHAR (MAX)  NOT NULL,
    [Status]       NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_dbo.Orders] PRIMARY KEY CLUSTERED ([OrderID] ASC),
    CONSTRAINT [FK_dbo.Orders_dbo.Users_UserID] FOREIGN KEY ([UserID]) 
        REFERENCES [dbo].[Users] ([UserID])
);
GO

CREATE NONCLUSTERED INDEX [IX_UserID] ON [dbo].[Orders]([UserID] ASC);
GO

-- 3.5. Bảng OrderDetails (Chi tiết đơn hàng)
CREATE TABLE [dbo].[OrderDetails] (
    [DetailID]  INT             IDENTITY (1, 1) NOT NULL,
    [OrderID]   INT             NOT NULL,
    [ProductID] INT             NOT NULL,
    [Quantity]  INT             NOT NULL,
    [UnitPrice] DECIMAL (18, 2) NOT NULL,
    CONSTRAINT [PK_dbo.OrderDetails] PRIMARY KEY CLUSTERED ([DetailID] ASC),
    CONSTRAINT [FK_dbo.OrderDetails_dbo.Orders_OrderID] FOREIGN KEY ([OrderID]) 
        REFERENCES [dbo].[Orders] ([OrderID]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.OrderDetails_dbo.Products_ProductID] FOREIGN KEY ([ProductID]) 
        REFERENCES [dbo].[Products] ([ProductID]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_OrderID]   ON [dbo].[OrderDetails]([OrderID] ASC);
CREATE NONCLUSTERED INDEX [IX_ProductID] ON [dbo].[OrderDetails]([ProductID] ASC);
GO

-- =========================================================================
-- 4. NẠP DỮ LIỆU MẪU (SEED DATA TỪ CONFIGURATION.CS)
-- =========================================================================

-- 4.1. Nạp 4 Danh mục món ăn
SET IDENTITY_INSERT [dbo].[Categories] ON;
INSERT INTO [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES
(1, N'Cơm Tấm & Đồ Ăn Trưa'),
(2, N'Trà Sữa & Đồ Uống'),
(3, N'Bún / Phở / Mì'),
(4, N'Ăn Vặt & Ăn Nhẹ');
SET IDENTITY_INSERT [dbo].[Categories] OFF;
GO

-- 4.2. Nạp 9 Món ăn mẫu
INSERT INTO [dbo].[Products] ([ProductName], [Price], [Stock], [ImageUrl], [CategoryID]) VALUES
(N'Cơm Tấm Sườn Bì Chả',           45000.00, 50,  N'https://i-giadinh.vnecdn.net/2024/03/07/7Honthinthnhphm1-1709800144-8583-1709800424.jpg', 1),
(N'Gà Rán Sốt Cay Hàn Quốc',       45000.00, 100, N'https://images.unsplash.com/photo-1562967914-608f82629710', 1),
(N'Cơm Bò Lúc Lắc Hạt Sen',        55000.00, 30,  N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTnUbHlsf7EsOq-XF7MbPOBqSwBccAQPxVwq5dpC4p1AA&s=10', 1),
(N'Trà Sữa Trân Châu Đường Đen',   35000.00, 80,  N'https://images.unsplash.com/photo-1558857563-b371033873b8', 2),
(N'Trà Đào Cam Sả Tươi',           30000.00, 60,  N'https://lypham.vn/wp-content/uploads/2024/09/cach-lam-tra-dao-cam-sa.jpg', 2),
(N'Cà Phê Muối Biển Đặc Biệt',     29000.00, 90,  N'https://images.unsplash.com/photo-1517701604599-bb29b565090c', 2),
(N'Bún Bò Huế Đặc Biệt',           50000.00, 40,  N'https://cdn.hstatic.net/files/1000300839/file/cach-nau-bun-bo-hue-chuan-vi-hue.jpg', 3),
(N'Phở Bò Tái Nạm Hà Nội',         55000.00, 45,  N'https://images.unsplash.com/photo-1582878826629-29b7ad1cdc43', 3),
(N'Bánh Mì Que Hải Phòng',         15000.00, 100, N'https://cdn.nhathuoclongchau.com.vn/unsafe/800x0/banh_mi_que_bao_nhieu_calo_an_banh_mi_que_co_tang_can_khong_2_1bf82300da.jpg', 4);
GO

-- 4.3. Nạp 2 Tài khoản mẫu (1 Admin & 1 Khách hàng)
INSERT INTO [dbo].[Users] ([Email], [Password], [FullName], [Phone]) VALUES
(N'admin@foodiehub.com',  N'123456', N'Quản Trị Viên', N'0901234567'),
(N'nguyenvana@gmail.com', N'123456', N'Nguyễn Văn A',  N'0909876543');
GO