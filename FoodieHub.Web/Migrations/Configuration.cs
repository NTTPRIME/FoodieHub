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
            // 1. Khởi tạo Danh mục món ăn (An toàn, không xóa dữ liệu cũ)
            context.Categories.AddOrUpdate(c => c.CategoryName,
                new Category { CategoryID = 1, CategoryName = "Cơm Tấm & Đồ Ăn Trưa" },
                new Category { CategoryID = 2, CategoryName = "Trà Sữa & Đồ Uống" },
                new Category { CategoryID = 3, CategoryName = "Bún / Phở / Mì" },
                new Category { CategoryID = 4, CategoryName = "Ăn Vặt & Ăn Nhẹ" }
            );
            context.SaveChanges();

            // 2. Chỉ nạp 9 món ăn mẫu nếu bảng Products đang trống (Tránh lỗi khóa ngoại OrderDetails)
            if (!context.Products.Any())
            {
                context.Products.AddOrUpdate(p => p.ProductName,
                    new Product { ProductName = "Cơm Tấm Sườn Bì Chả", Price = 45000, Stock = 50, ImageUrl = "https://i-giadinh.vnecdn.net/2024/03/07/7Honthinthnhphm1-1709800144-8583-1709800424.jpg", CategoryID = 1 },
                    new Product { ProductName = "Gà Rán Sốt Cay Hàn Quốc", Price = 45000, Stock = 100, ImageUrl = "https://images.unsplash.com/photo-1562967914-608f82629710", CategoryID = 1 },
                    new Product { ProductName = "Cơm Bò Lúc Lắc Hạt Sen", Price = 55000, Stock = 30, ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTnUbHlsf7EsOq-XF7MbPOBqSwBccAQPxVwq5dpC4p1AA&s=10", CategoryID = 1 },
                    new Product { ProductName = "Trà Sữa Trân Châu Đường Đen", Price = 35000, Stock = 80, ImageUrl = "https://images.unsplash.com/photo-1558857563-b371033873b8", CategoryID = 2 },
                    new Product { ProductName = "Trà Đào Cam Sả Tươi", Price = 30000, Stock = 60, ImageUrl = "https://lypham.vn/wp-content/uploads/2024/09/cach-lam-tra-dao-cam-sa.jpg", CategoryID = 2 },
                    new Product { ProductName = "Cà Phê Muối Biển Đặc Biệt", Price = 29000, Stock = 90, ImageUrl = "https://images.unsplash.com/photo-1517701604599-bb29b565090c", CategoryID = 2 },
                    new Product { ProductName = "Bún Bò Huế Đặc Biệt", Price = 50000, Stock = 40, ImageUrl = "https://cdn.hstatic.net/files/1000300839/file/cach-nau-bun-bo-hue-chuan-vi-hue.jpg", CategoryID = 3 },
                    new Product { ProductName = "Phở Bò Tái Nạm Hà Nội", Price = 55000, Stock = 45, ImageUrl = "https://images.unsplash.com/photo-1582878826629-29b7ad1cdc43", CategoryID = 3 },
                    new Product { ProductName = "Bánh Mì Que Hải Phòng", Price = 15000, Stock = 100, ImageUrl = "https://cdn.nhathuoclongchau.com.vn/unsafe/800x0/banh_mi_que_bao_nhieu_calo_an_banh_mi_que_co_tang_can_khong_2_1bf82300da.jpg", CategoryID = 4 }
                );
                context.SaveChanges();
            }

            // 3. Khởi tạo Tài khoản mẫu
            context.Users.AddOrUpdate(u => u.Email,
                new User { Email = "admin@foodiehub.com", Password = "123456", FullName = "Quản Trị Viên", Phone = "0901234567" },
                new User { Email = "nguyenvana@gmail.com", Password = "123456", FullName = "Nguyễn Văn A", Phone = "0909876543" }
            );
            context.SaveChanges();
        }
    }
}