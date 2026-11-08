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
    // Lớp User đại diện cho thực thể Khách hàng hoặc Quản trị viên trong hệ thống
    public partial class User
    {
        // Hàm khởi tạo mặc định của lớp User
        public User()
        {
            // Khởi tạo tập hợp danh sách các đơn hàng trống để tránh lỗi NullReferenceException khi truy xuất
            Orders = new HashSet<Order>();
        }

        // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key) của bảng Users
        [Key]
        // Khai báo thuộc tính UserID kiểu số nguyên tự động tăng (Identity)
        public int UserID { get; set; }

        // Bắt buộc người dùng phải nhập Email, nếu để trống sẽ xuất thông báo lỗi
        [Required(ErrorMessage = "Email không được để trống")]
        // Kiểm tra và ràng buộc định dạng chuỗi nhập vào phải đúng chuẩn dạng Email
        [EmailAddress(ErrorMessage = "Email sai định dạng")]
        // Khai báo thuộc tính Email kiểu chuỗi lưu địa chỉ hòm thư điện tử
        public string Email { get; set; }

        // Bắt buộc người dùng phải nhập mật khẩu khi đăng ký / tạo tài khoản
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        // Ràng buộc độ dài chuỗi mật khẩu bắt buộc phải đạt tối thiểu 6 ký tự
        [MinLength(6, ErrorMessage = "Mật khẩu ít nhất 6 ký tự")]
        // Khai báo thuộc tính Password kiểu chuỗi lưu mật khẩu đăng nhập
        public string Password { get; set; }

        // Khai báo thuộc tính FullName kiểu chuỗi lưu Họ và tên hiển thị của người dùng
        public string FullName { get; set; }

        // Mối quan hệ 1-Nhiều (Virtual Navigation Property): Một người dùng có thể sở hữu nhiều đơn hàng
        public virtual ICollection<Order> Orders { get; set; }
    }

    // ==========================================
    // 2. BẢNG DANH MỤC SẢN PHẨM (Category)
    // ==========================================
    // Lớp Category đại diện cho thực thể Danh mục phân loại món ăn (Ví dụ: Cơm, Trà sữa, Bún...)
    public partial class Category
    {
        // Hàm khởi tạo mặc định của lớp Category
        public Category()
        {
            // Khởi tạo tập hợp sản phẩm trống ban đầu để không bị lỗi null khi nạp danh sách món ăn
            Products = new HashSet<Product>();
        }

        // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key) của bảng Categories
        [Key]
        // Khai báo thuộc tính CategoryID kiểu số nguyên tự động tăng
        public int CategoryID { get; set; }

        // Ràng buộc không được để trống tên danh mục khi khởi tạo hoặc chỉnh sửa
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        // Khai báo thuộc tính CategoryName kiểu chuỗi lưu tên danh mục món ăn
        public string CategoryName { get; set; }

        // Mối quan hệ 1-Nhiều (Virtual Navigation Property): Một danh mục chứa nhiều sản phẩm món ăn
        public virtual ICollection<Product> Products { get; set; }
    }

    // ==========================================
    // 3. BẢNG SẢN PHẨM / MÓN ĂN (Product)
    // ==========================================
    // Lớp Product đại diện cho thực thể Món ăn / Sản phẩm thương mại trong thực đơn
    public partial class Product
    {
        // Hàm khởi tạo mặc định của lớp Product
        public Product()
        {
            // Khởi tạo tập hợp các dòng chi tiết đơn hàng liên quan tới món ăn này
            OrderDetails = new HashSet<OrderDetail>();
        }

        // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key) của bảng Products
        [Key]
        // Khai báo thuộc tính ProductID kiểu số nguyên tự động tăng (Mã món ăn)
        public int ProductID { get; set; }

        // Ràng buộc bắt buộc phải nhập tên món ăn
        [Required(ErrorMessage = "Tên món ăn không được để trống")]
        // Khai báo thuộc tính ProductName kiểu chuỗi lưu tên hiển thị của món ăn
        public string ProductName { get; set; }

        // Ràng buộc bắt buộc nhập giá bán của món ăn
        [Required]
        // Ràng buộc giá trị số tiền phải nằm trong khoảng hợp lệ từ 0 đến 100.000.000 VNĐ
        [Range(0, 100000000, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        // Khai báo thuộc tính Price kiểu decimal phục vụ tính toán tiền tệ chính xác
        public decimal Price { get; set; }

        // Ràng buộc bắt buộc phải nhập số lượng tồn kho của món ăn
        [Required]
        // Khai báo thuộc tính Stock kiểu số nguyên lưu số lượng món ăn hiện có trong kho
        public int Stock { get; set; }

        // Khai báo thuộc tính ImageUrl kiểu chuỗi lưu đường dẫn ảnh đại diện của món ăn
        public string ImageUrl { get; set; }

        // Khai báo thuộc tính CategoryID kiểu số nguyên đóng vai trò là Khóa ngoại (Foreign Key)
        public int CategoryID { get; set; }

        // Chỉ định rõ ràng thuộc tính CategoryID bên trên là Khóa ngoại liên kết tới bảng Category
        [ForeignKey("CategoryID")]
        // Thuộc tính điều hướng ảo (Virtual) dùng để lấy toàn bộ đối tượng Danh mục tương ứng
        public virtual Category Category { get; set; }

        // Mối quan hệ 1-Nhiều: Một sản phẩm có thể xuất hiện trong nhiều Chi tiết đơn hàng
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }

    // ==========================================
    // 4. BẢNG ĐƠN HÀNG TỔNG (Order)
    // ==========================================
    // Lớp Order đại diện cho thực thể Đơn hàng / Hóa đơn đặt món
    public partial class Order
    {
        // Hàm khởi tạo mặc định của lớp Order
        public Order()
        {
            // Khởi tạo tập hợp danh sách các món ăn chi tiết thuộc hóa đơn này
            OrderDetails = new HashSet<OrderDetail>();
        }

        // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key) của bảng Orders
        [Key]
        // Khai báo thuộc tính OrderID kiểu số nguyên tự động tăng (Mã hóa đơn)
        public int OrderID { get; set; }

        // Khai báo thuộc tính UserID kiểu nullable int (cho phép null nếu là khách mua không cần đăng nhập)
        public int? UserID { get; set; }

        // Khai báo thuộc tính CustomerName kiểu chuỗi lưu tên người nhận hàng
        public string CustomerName { get; set; }

        // Khai báo thuộc tính Address kiểu chuỗi lưu địa chỉ giao hàng
        public string Address { get; set; }

        // Khai báo thuộc tính OrderDate kiểu nullable DateTime lưu thời điểm người dùng bấm đặt hàng
        public DateTime? OrderDate { get; set; }

        // Khai báo thuộc tính TotalAmount kiểu decimal lưu tổng giá trị thanh toán của đơn hàng
        public decimal TotalAmount { get; set; }

        // Đánh dấu thuộc tính UserID là Khóa ngoại kết nối tới bảng Users
        [ForeignKey("UserID")]
        // Thuộc tính điều hướng ảo trỏ đến thông tin tài khoản người dùng đã đặt đơn này
        public virtual User User { get; set; }

        // Mối quan hệ 1-Nhiều: Một đơn hàng chứa nhiều dòng chi tiết món ăn (OrderDetail)
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }

    // ==========================================
    // 5. BẢNG CHI TIẾT ĐƠN HÀNG (OrderDetail)
    // ==========================================
    // Lớp OrderDetail đại diện cho từng dòng sản phẩm chi tiết bên trong một hóa đơn
    public partial class OrderDetail
    {
        // Đánh dấu thuộc tính ngay phía dưới là Khóa chính (Primary Key) của bảng OrderDetails
        [Key]
        // Khai báo thuộc tính DetailID kiểu số nguyên tự động tăng (Mã chi tiết đơn)
        public int DetailID { get; set; }

        // Khai báo thuộc tính OrderID kiểu số nguyên lưu Mã đơn hàng tổng (Khóa ngoại)
        public int OrderID { get; set; }

        // Khai báo thuộc tính ProductID kiểu số nguyên lưu Mã món ăn được mua (Khóa ngoại)
        public int ProductID { get; set; }

        // Khai báo thuộc tính Quantity kiểu số nguyên lưu số lượng món ăn đặt mua
        public int Quantity { get; set; }

        // Khai báo thuộc tính UnitPrice kiểu decimal lưu giá bán món ăn ngay tại thời điểm mua
        public decimal UnitPrice { get; set; }

        // Đánh dấu thuộc tính OrderID là Khóa ngoại kết nối tới bảng Orders
        [ForeignKey("OrderID")]
        // Thuộc tính điều hướng ảo dùng để truy xuất ngược về Đơn hàng tổng
        public virtual Order Order { get; set; }

        // Đánh dấu thuộc tính ProductID là Khóa ngoại kết nối tới bảng Products
        [ForeignKey("ProductID")]
        // Thuộc tính điều hướng ảo dùng để truy xuất thông tin chi tiết món ăn
        public virtual Product Product { get; set; }
    }

    // ==========================================
    // 6. KHỞI TẠO CSDL VÀ TỰ ĐỘNG BƠM DỮ LIỆU MẪU (Initializer)
    // ==========================================
    // Lớp FoodieHubDataInitializer kế thừa từ CreateDatabaseIfNotExists giúp tự động nạp dữ liệu từ Code-First
    public class FoodieHubDataInitializer : CreateDatabaseIfNotExists<FoodieHubDbContext>
    {
        // Ghi đè phương thức Seed để nạp dữ liệu ban đầu mỗi khi CSDL được khởi tạo mới
        protected override void Seed(FoodieHubDbContext context)
        {
            // Tạo danh sách các danh mục món ăn mặc định
            var categories = new List<Category>
            {
                // Danh mục 1: Cơm tấm và các món cơm trưa
                new Category { CategoryName = "Cơm Tấm & Đồ Ăn Trưa" },
                // Danh mục 2: Các loại trà sữa và đồ uống giải khát
                new Category { CategoryName = "Trà Sữa & Đồ Uống" },
                // Danh mục 3: Món nước như bún, phở, mì
                new Category { CategoryName = "Bún / Phở / Mì" },
                // Danh mục 4: Các món ăn vặt nhẹ nhàng
                new Category { CategoryName = "Ăn Vặt & Ăn Nhẹ" }
            };
            // Lặp qua từng danh mục trong danh sách và thêm vào DbSet Categories
            categories.ForEach(c => context.Categories.Add(c));
            // Lưu toàn bộ thay đổi danh mục xuống cơ sở dữ liệu để lấy CategoryID
            context.SaveChanges();

            // Tạo danh sách 9 món ăn mẫu khớp chuẩn với các danh mục trên
            var products = new List<Product>
            {
                // Món 1 thuộc Danh mục 1 (Stock > 0 đảm bảo hiển thị lên giao diện)
                new Product { ProductName = "Cơm Tấm Sườn Bì Chả", Price = 45000, Stock = 50, ImageUrl = "https://i-giadinh.vnecdn.net/2024/03/07/7Honthinthnhphm1-1709800144-8583-1709800424.jpg", CategoryID = 1 },
                // Món 2 thuộc Danh mục 1
                new Product { ProductName = "Gà Rán Sốt Phô Mai", Price = 45000, Stock = 100, ImageUrl = "https://down-vn.img.susercontent.com/file/vn-11134207-7ras8-m1y6320kdfh786", CategoryID = 1 },
                // Món 3 thuộc Danh mục 1
                new Product { ProductName = "Cơm Bò Lúc Lắc", Price = 55000, Stock = 30, ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTnUbHlsf7EsOq-XF7MbPOBqSwBccAQPxVwq5dpC4p1AA&s=10", CategoryID = 1 },
                // Món 4 thuộc Danh mục 2
                new Product { ProductName = "Trà Sữa Trân Châu Đường Đen", Price = 35000, Stock = 80, ImageUrl = "https://images.unsplash.com/photo-1558857563-b371033873b8", CategoryID = 2 },
                // Món 5 thuộc Danh mục 2
                new Product { ProductName = "Trà Đào Cam Sả Tươi", Price = 30000, Stock = 60, ImageUrl = "https://lypham.vn/wp-content/uploads/2024/09/cach-lam-tra-dao-cam-sa.jpg", CategoryID = 2 },
                // Món 6 thuộc Danh mục 2
                new Product { ProductName = "Cà Phê Muối", Price = 29000, Stock = 90, ImageUrl = "https://kingcoffee.com.vn/wp-content/uploads/2026/02/Ca-phe-muoi-la-gi-1024x683.jpg", CategoryID = 2 },
                // Món 7 thuộc Danh mục 3
                new Product { ProductName = "Bún Bò Huế", Price = 50000, Stock = 40, ImageUrl = "https://cdn.hstatic.net/files/1000300839/file/cach-nau-bun-bo-hue-chuan-vi-hue.jpg", CategoryID = 3 },
                // Món 8 thuộc Danh mục 3
                new Product { ProductName = "Phở Bò Tái Nạm", Price = 55000, Stock = 45, ImageUrl = "https://cdn.tgdd.vn/Files/2022/01/25/1412805/cach-nau-pho-bo-nam-dinh-chuan-vi-thom-ngon-nhu-hang-quan-202201250313281452.jpg", CategoryID = 3 },
                // Món 9 thuộc Danh mục 4
                new Product { ProductName = "Bánh Mì Que", Price = 15000, Stock = 100, ImageUrl = "https://cdn.nhathuoclongchau.com.vn/unsafe/800x0/banh_mi_que_bao_nhieu_calo_an_banh_mi_que_co_tang_can_khong_2_1bf82300da.jpg", CategoryID = 4 }
            };
            // Lặp qua từng món ăn và thêm vào DbSet Products
            products.ForEach(p => context.Products.Add(p));
            // Lưu danh sách sản phẩm món ăn xuống cơ sở dữ liệu
            context.SaveChanges();

            // Tạo danh sách tài khoản người dùng mẫu
            var users = new List<User>
            {
                // Tài khoản Admin quản trị hệ thống
                new User { Email = "admin@foodiehub.com", Password = "123456", FullName = "Quản Trị Viên" },
                // Tài khoản Khách hàng mua sắm
                new User { Email = "nguyenvana@gmail.com", Password = "123456", FullName = "Nguyễn Văn A" }
            };
            // Lặp qua từng người dùng và thêm vào DbSet Users
            users.ForEach(u => context.Users.Add(u));
            // Lưu tài khoản mẫu xuống cơ sở dữ liệu
            context.SaveChanges();

            // Gọi phương thức Seed của lớp cơ sở để hoàn tất chu trình khởi tạo
            base.Seed(context);
        }
    }

    // ==========================================
    // 7. LỚP ĐIỀU KHIỂN CSDL CHÍNH (DbContext)
    // ==========================================
    // Lớp FoodieHubDbContext quản lý phiên làm việc với CSDL SQL Server thông qua Entity Framework
    public class FoodieHubDbContext : DbContext
    {
        // Khởi tạo chuỗi kết nối dựa trên định danh "FoodieHubConnection" cấu hình trong Web.config
        public FoodieHubDbContext() : base("name=FoodieHubConnection") { }

        // Khai báo tập hợp bảng Users dùng để truy vấn và thao tác dữ liệu người dùng
        public DbSet<User> Users { get; set; }
        // Khai báo tập hợp bảng Categories dùng để quản lý các danh mục món ăn
        public DbSet<Category> Categories { get; set; }
        // Khai báo tập hợp bảng Products dùng để quản lý toàn bộ thực đơn món ăn
        public DbSet<Product> Products { get; set; }
        // Khai báo tập hợp bảng Orders dùng để lưu trữ các đơn hàng tổng
        public DbSet<Order> Orders { get; set; }
        // Khai báo tập hợp bảng OrderDetails dùng để lưu thông tin chi tiết từng món trong đơn
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}