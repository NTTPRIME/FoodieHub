namespace FoodieHub.Web.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using FoodieHub.Web.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<FoodieHub.Web.Models.FoodieHubDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FoodieHub.Web.Models.FoodieHubDbContext context)
        {
            // 1. Khởi tạo các Danh mục món ăn
            context.Categories.AddOrUpdate(c => c.CategoryName,
                new Category { CategoryID = 1, CategoryName = "Cơm Tấm & Đồ Ăn Trưa" },
                new Category { CategoryID = 2, CategoryName = "Trà Sữa & Đồ Uống" },
                new Category { CategoryID = 3, CategoryName = "Bún / Phở / Mì" },
                new Category { CategoryID = 4, CategoryName = "Ăn Vặt & Ăn Nhẹ" }
            );
            context.SaveChanges();

            // 2. Xóa toàn bộ sản phẩm cũ để tránh bị trùng lặp do đổi tên món
            context.Database.ExecuteSqlCommand("DELETE FROM Products");

            // 3. Khởi tạo 9 món ăn (Đủ cho 3 hàng x 3 cột hiển thị đẹp mắt)
            context.Products.AddOrUpdate(p => p.ProductName,
                // Nhóm 1: Cơm & Món chính
                new Product { ProductName = "Cơm Tấm Sườn Bì Chả", Price = 45000, Stock = 50, ImageUrl = "https://images.unsplash.com/photo-1546069901-ba9599a7e63c", CategoryID = 1 },
                new Product { ProductName = "Gà Rán Sốt Cay Hàn Quốc", Price = 45000, Stock = 100, ImageUrl = "https://images.unsplash.com/photo-1562967914-608f82629710", CategoryID = 1 },
                new Product { ProductName = "Cơm Bò Lúc Lắc Hạt Sen", Price = 55000, Stock = 30, ImageUrl = "https://images.unsplash.com/photo-1544025162-d76694265947", CategoryID = 1 },

                // Nhóm 2: Đồ uống
                new Product { ProductName = "Trà Sữa Trân Châu Đường Đen", Price = 35000, Stock = 80, ImageUrl = "https://images.unsplash.com/photo-1558857563-b371033873b8", CategoryID = 2 },
                new Product { ProductName = "Trà Đào Cam Sả Tươi", Price = 30000, Stock = 60, ImageUrl = "https://images.unsplash.com/photo-1513558161293-cdaf765ed2fd", CategoryID = 2 },
                new Product { ProductName = "Cà Phê Muối Biển Đặc Biệt", Price = 29000, Stock = 90, ImageUrl = "https://images.unsplash.com/photo-1517701604599-bb29b565090c", CategoryID = 2 },

                // Nhóm 3: Món nước & Ăn vặt
                new Product { ProductName = "Bún Bò Huế Đặc Biệt", Price = 50000, Stock = 40, ImageUrl = "https://images.unsplash.com/photo-1582878826629-29b7ad1cdc43", CategoryID = 3 },
                new Product { ProductName = "Phở Bò Tái Nạm Hà Nội", Price = 55000, Stock = 45, ImageUrl = "https://images.unsplash.com/photo-1582878826629-29b7ad1cdc43", CategoryID = 3 },
                new Product { ProductName = "Bánh Mì Que Hải Phòng", Price = 15000, Stock = 100, ImageUrl = "https://images.unsplash.com/photo-1509722747041-616f39b57569", CategoryID = 4 }
            );
            context.SaveChanges();

            // 4. Khởi tạo Tài khoản mẫu
            context.Users.AddOrUpdate(u => u.Email,
                new User { Email = "admin@foodiehub.com", Password = "123456", FullName = "Quản Trị Viên" },
                new User { Email = "nguyenvana@gmail.com", Password = "123456", FullName = "Nguyễn Văn A" }
            );
            context.SaveChanges();
        }
    }
}