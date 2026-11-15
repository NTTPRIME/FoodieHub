// Nhúng thư viện hệ thống cơ bản của .NET Framework
using System;
// Nhúng thư viện hỗ trợ các cấu trúc danh sách tập hợp (ICollection, HashSet, List)
using System.Collections.Generic;
// Nhúng thư viện kiểm tra tính hợp lệ dữ liệu (Data Annotations validation)
using System.ComponentModel.DataAnnotations;
// Nhúng thư viện thiết lập các ràng buộc cấu trúc CSDL (Schema, Key, ForeignKey, Table)
using System.ComponentModel.DataAnnotations.Schema;
// Nhúng thư viện Entity Framework để giao tiếp dữ liệu với SQL Server
using System.Data.Entity;

// Khai báo không gian tên chứa toàn bộ các lớp Mô hình (Models) của dự án FoodieHub.Web
namespace FoodieHub.Web.Models
{
    // ==========================================
    // 1. BẢNG NGƯỜI DÙNG (User)
    // ==========================================
    public partial class User
    {
        public User()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email sai định dạng")]
        public string Email { get; set; }

        public string Phone { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu ít nhất 6 ký tự")]
        public string Password { get; set; }

        public string FullName { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }

    // ==========================================
    // 2. BẢNG DANH MỤC SẢN PHẨM (Category)
    // ==========================================
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        [Key]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        public string CategoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }

    // ==========================================
    // 3. BẢNG SẢN PHẨM / MÓN ĂN (Product)
    // ==========================================
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        [Key]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Tên món ăn không được để trống")]
        public string ProductName { get; set; }

        [Required]
        [Range(0, 100000000, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

        [Required]
        public int Stock { get; set; }

        public string ImageUrl { get; set; }

        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }

    // ==========================================
    // 4. BẢNG ĐƠN HÀNG TỔNG (Order)
    // ==========================================
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            // Mặc định đơn hàng mới tạo sẽ có trạng thái "Đang xử lý" hoặc "Chờ xác nhận"
            Status = "Đang xử lý";
        }

        [Key]
        public int OrderID { get; set; }

        public int? UserID { get; set; }

        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email sai định dạng")]
        public string Email { get; set; }

        public string Address { get; set; }

        public DateTime? OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        // === THÊM MỚI: CỘT STATUS LƯU TRẠNG THÁI ĐƠN HÀNG CHÍNH THỨC ===
        public string Status { get; set; }
        // ============================================================

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }

    // ==========================================
    // 5. BẢNG CHI TIẾT ĐƠN HÀNG (OrderDetail)
    // ==========================================
    public partial class OrderDetail
    {
        [Key]
        public int DetailID { get; set; }

        public int OrderID { get; set; }

        public int ProductID { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        [ForeignKey("OrderID")]
        public virtual Order Order { get; set; }

        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }
    }

    // ==========================================
    // 6. KHỞI TẠO CSDL VÀ TỰ ĐỘNG BƠM DỮ LIỆU MẪU (Initializer)
    // ==========================================
    public class FoodieHubDataInitializer : CreateDatabaseIfNotExists<FoodieHubDbContext>
    {
        protected override void Seed(FoodieHubDbContext context)
        {
            var categories = new List<Category>
            {
                new Category { CategoryName = "Cơm Tấm & Đồ Ăn Trưa" },
                new Category { CategoryName = "Trà Sữa & Đồ Uống" },
                new Category { CategoryName = "Bún / Phở / Mì" },
                new Category { CategoryName = "Ăn Vặt & Ăn Nhẹ" }
            };
            categories.ForEach(c => context.Categories.Add(c));
            context.SaveChanges();

            var products = new List<Product>
            {
                new Product { ProductName = "Cơm Tấm Sườn Bì Chả", Price = 45000, Stock = 50, ImageUrl = "https://i-giadinh.vnecdn.net/2024/03/07/7Honthinthnhphm1-1709800144-8583-1709800424.jpg", CategoryID = 1 },
                new Product { ProductName = "Gà Rán Sốt Phô Mai", Price = 45000, Stock = 100, ImageUrl = "https://down-vn.img.susercontent.com/file/vn-11134207-7ras8-m1y6320kdfh786", CategoryID = 1 },
                new Product { ProductName = "Cơm Bò Lúc Lắc", Price = 55000, Stock = 30, ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTnUbHlsf7EsOq-XF7MbPOBqSwBccAQPxVwq5dpC4p1AA&s=10", CategoryID = 1 },
                new Product { ProductName = "Trà Sữa Trân Châu Đường Đen", Price = 35000, Stock = 80, ImageUrl = "https://images.unsplash.com/photo-1558857563-b371033873b8", CategoryID = 2 },
                new Product { ProductName = "Trà Đào Cam Sả Tươi", Price = 30000, Stock = 60, ImageUrl = "https://lypham.vn/wp-content/uploads/2024/09/cach-lam-tra-dao-cam-sa.jpg", CategoryID = 2 },
                new Product { ProductName = "Cà Phê Muối", Price = 29000, Stock = 90, ImageUrl = "https://kingcoffee.com.vn/wp-content/uploads/2026/02/Ca-phe-muoi-la-gi-1024x683.jpg", CategoryID = 2 },
                new Product { ProductName = "Bún Bò Huế", Price = 50000, Stock = 40, ImageUrl = "https://cdn.hstatic.net/files/1000300839/file/cach-nau-bun-bo-hue-chuan-vi-hue.jpg", CategoryID = 3 },
                new Product { ProductName = "Phở Bò Tái Nạm", Price = 55000, Stock = 45, ImageUrl = "https://cdn.tgdd.vn/Files/2022/01/25/1412805/cach-nau-pho-bo-nam-dinh-chuan-vi-thom-ngon-nhu-hang-quan-202201250313281452.jpg", CategoryID = 3 },
                new Product { ProductName = "Bánh Mì Que", Price = 15000, Stock = 100, ImageUrl = "https://cdn.nhathuoclongchau.com.vn/unsafe/800x0/banh_mi_que_bao_nhieu_calo_an_banh_mi_que_co_tang_can_khong_2_1bf82300da.jpg", CategoryID = 4 }
            };
            products.ForEach(p => context.Products.Add(p));
            context.SaveChanges();

            var users = new List<User>
            {
                new User { Email = "admin@foodiehub.com", Password = "123456", FullName = "Quản Trị Viên" },
                new User { Email = "nguyenvana@gmail.com", Password = "123456", FullName = "Nguyễn Văn A" }
            };
            users.ForEach(u => context.Users.Add(u));
            context.SaveChanges();

            base.Seed(context);
        }
    }

    // ==========================================
    // 7. LỚP ĐIỀU KHIỂN CSDL CHÍNH (DbContext)
    // ==========================================
    public class FoodieHubDbContext : DbContext
    {
        public FoodieHubDbContext() : base("name=FoodieHubConnection") { }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}