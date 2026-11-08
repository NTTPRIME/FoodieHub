// Khai báo các thư viện hệ thống cơ bản của .NET Framework
using System;
// Nhúng thư viện LINQ hỗ trợ các câu lệnh truy vấn dữ liệu (FirstOrDefault, Where...)
using System.Linq;
// Nhúng thư viện ASP.NET MVC cung cấp Controller, ActionResult, HttpGet, HttpPost, Session, ViewBag...
using System.Web.Mvc;
// Nhúng không gian tên chứa DbContext và các Entity Model (User, FoodieHubDbContext) của dự án FoodieHub
using FoodieHub.Web.Models;

// Định nghĩa không gian tên thuộc Controller của dự án FoodieHub.Web
namespace FoodieHub.Web.Controllers
{
    // Khai báo lớp AccountController kế thừa từ lớp Controller để quản lý toàn bộ luồng Tài khoản
    public class AccountController : Controller
    {
        // Khởi tạo đối tượng DbContext để giao tiếp, kết nối và thực thi truy vấn với cơ sở dữ liệu SQL Server
        private FoodieHubDbContext db = new FoodieHubDbContext();

        // ======================================================================
        // 1. ACTION REGISTER: ĐĂNG KÝ TÀI KHOẢN NGƯỜI DÙNG
        // ======================================================================
        // Chỉ định Action Register tiếp nhận phương thức HTTP GET (Hiển thị form đăng ký)
        [HttpGet]
        // Phương thức trả về giao diện View đăng ký cho khách hàng
        public ActionResult Register()
        {
            // Trả về giao diện Views/Account/Register.cshtml
            return View();
        }

        // Chỉ định Action Register tiếp nhận phương thức HTTP POST (Submit form đăng ký)
        [HttpPost]
        // Tạo mã AntiForgeryToken bảo mật chống lại các cuộc tấn công giả mạo yêu cầu chéo trang (CSRF)
        [ValidateAntiForgeryToken]
        // Phương thức tiếp nhận đối tượng user được Model Binder đóng gói từ các ô nhập trên Form
        public ActionResult Register(User user)
        {
            // Kiểm tra xem dữ liệu người dùng nhập vào có thỏa mãn các ràng buộc Validation (Data Annotations) hay không
            if (ModelState.IsValid)
            {
                // Thực thi câu lệnh LINQ kiểm tra địa chỉ Email nhập vào đã bị tài khoản nào chiếm giữ trước đó chưa
                var checkUser = db.Users.FirstOrDefault(u => u.Email == user.Email);

                // Nếu không tìm thấy bản ghi nào trùng Email (checkUser bằng null), tiến hành thêm mới
                if (checkUser == null)
                {
                    // Đưa đối tượng user mới vào bộ đệm theo dõi của Entity Framework
                    db.Users.Add(user);
                    // Thực thi câu lệnh SQL INSERT INTO để lưu chính thức bản ghi tài khoản xuống SQL Server
                    db.SaveChanges();

                    // Đăng ký thành công, tự động chuyển hướng trình duyệt sang Action Login để người dùng đăng nhập
                    return RedirectToAction("Login");
                }
                // Trường hợp địa chỉ Email đã tồn tại trong hệ thống
                else
                {
                    // Đưa câu thông báo lỗi trùng Email vào ViewBag.Error để hiển thị lên View
                    ViewBag.Error = "Email này đã được đăng ký! Vui lòng sử dụng Email khác.";
                }
            }
            // Nếu dữ liệu bị lỗi Validation hoặc trùng Email, nạp lại trang View kèm đối tượng user vừa nhập để người dùng sửa
            return View(user);
        }

        // ======================================================================
        // 2. ACTION LOGIN: ĐĂNG NHẬP HỆ THỐNG
        // ======================================================================
        // Chỉ định Action Login tiếp nhận phương thức HTTP GET (Hiển thị form đăng nhập)
        [HttpGet]
        // Phương thức trả về giao diện View đăng nhập
        public ActionResult Login()
        {
            // Trả về giao diện Views/Account/Login.cshtml
            return View();
        }

        // Chỉ định Action Login tiếp nhận phương thức HTTP POST (Xác thực thông tin đăng nhập)
        [HttpPost]
        // Mã xác thực CSRF Token chống tấn công giả mạo yêu cầu chéo trang
        [ValidateAntiForgeryToken]
        // Phương thức tiếp nhận chuỗi email và password người dùng gửi lên từ Form
        public ActionResult Login(string email, string password)
        {
            // Kiểm tra an toàn: Nếu một trong hai trường email hoặc password bị bỏ trống
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                // Ghi thông báo nhắc nhở nhập đủ thông tin vào ViewBag.Error
                ViewBag.Error = "Vui lòng nhập đầy đủ Email và Mật khẩu!";
                // Trả về lại giao diện đăng nhập kèm thông báo lỗi
                return View();
            }

            // Truy vấn LINQ tìm bản ghi tài khoản có Email VÀ Mật khẩu trùng khớp 100% trong bảng Users
            var user = db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            // Nếu tìm thấy tài khoản hợp lệ trong cơ sở dữ liệu (user khác null)
            if (user != null)
            {
                // Lưu toàn bộ đối tượng user vào Session["User"] phục vụ kiểm tra đăng nhập toàn bộ ứng dụng
                Session["User"] = user;
                // Lưu Họ và tên người dùng vào Session["FullName"] để hiển thị lên thanh Header giao diện
                Session["FullName"] = user.FullName;
                // Lưu Mã định danh người dùng vào Session["UserID"] phục vụ gán đơn hàng khi Checkout
                Session["UserID"] = user.UserID;
                // Lưu Địa chỉ Email vào Session["UserEmail"] để đồng bộ hiển thị trên View Hồ sơ và Admin
                Session["UserEmail"] = user.Email;

                // Kiểm tra vai trò: Nếu địa chỉ Email có chứa chuỗi "admin" (không phân biệt hoa thường)
                if (!string.IsNullOrEmpty(user.Email) && user.Email.ToLower().Contains("admin"))
                {
                    // Đánh dấu vai trò tài khoản hiện tại là Admin trong Session
                    Session["UserRole"] = "Admin";
                    // Chuyển hướng trình duyệt sang trang quản lý hàng hóa của phân hệ Quản trị (Admin)
                    return RedirectToAction("Index", "AdminProduct");
                }
                // Nếu là tài khoản khách hàng thông thường
                else
                {
                    // Đánh dấu vai trò tài khoản hiện tại là Customer trong Session
                    Session["UserRole"] = "Customer";
                    // Chuyển hướng trình duyệt về lại trang chủ hệ thống
                    return RedirectToAction("Index", "Home");
                }
            }

            // Trường hợp nhập sai Email hoặc Mật khẩu không tồn tại trong CSDL
            ViewBag.Error = "Tài khoản hoặc mật khẩu không đúng!";
            // Nạp lại View đăng nhập để hiển thị thông báo lỗi
            return View();
        }

        // ======================================================================
        // 3. ACTION LOGOUT: ĐĂNG XUẤT HỆ THỐNG
        // ======================================================================
        // Phương thức thực thi thao tác Đăng xuất tài khoản
        public ActionResult Logout()
        {
            // Xóa bỏ toàn bộ các biến và dữ liệu đang lưu trong Session
            Session.Clear();
            // Hủy bỏ phiên làm việc Session hiện tại trên máy chủ Web
            Session.Abandon();

            // Chuyển hướng trình duyệt trở lại trang Đăng nhập
            return RedirectToAction("Login");
        }

        // ======================================================================
        // 4. ACTION PROFILE: XEM THÔNG TIN HỒ SƠ CÁ NHÂN
        // ======================================================================
        // Chỉ định Action Profile tiếp nhận phương thức HTTP GET
        [HttpGet]
        // Phương thức xử lý yêu cầu xem chi tiết hồ sơ người dùng
        public ActionResult Profile()
        {
            // Kiểm tra an toàn: Nếu người dùng chưa đăng nhập (Session["UserID"] bị null)
            if (Session["UserID"] == null)
            {
                // Tự động đá trình duyệt về lại trang Đăng nhập
                return RedirectToAction("Login", "Account");
            }

            // Ép kiểu giá trị UserID từ Session về kiểu số nguyên int
            int userId = Convert.ToInt32(Session["UserID"]);

            // Truy vấn tìm chính xác bản ghi người dùng trong bảng Users theo khóa chính userId
            var user = db.Users.Find(userId);
            // Kiểm tra an toàn: Nếu không tìm thấy bản ghi người dùng nào trong CSDL
            if (user == null)
            {
                // Trả về mã lỗi HttpNotFound (Mã lỗi 404) để ngắt luồng
                return HttpNotFound();
            }

            // Trả về View Profile.cshtml và nạp đối tượng user sang giao diện
            return View(user);
        }

        // ======================================================================
        // 5. GIẢI PHÓNG TÀI NGUYÊN KẾT NỐI CSDL (DISPOSE)
        // ======================================================================
        // Ghi đè phương thức Dispose để đóng kết nối CSDL khi Controller kết thúc vòng đời
        protected override void Dispose(bool disposing)
        {
            // Kiểm tra nếu biến disposing mang giá trị true (đang giải phóng tài nguyên)
            if (disposing)
            {
                // Ngắt kết nối SQL Server và giải phóng bộ nhớ RAM của đối tượng DbContext
                db.Dispose();
            }
            // Gọi phương thức Dispose của lớp cha (Controller) để hoàn tất
            base.Dispose(disposing);
        }
    }
}