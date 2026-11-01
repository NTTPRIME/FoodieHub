using System;
using System.Linq;
using System.Web.Mvc;
// Nhúng namespace chứa FoodieHubDbContext và User Model
using FoodieHub.Web.Models;

namespace FoodieHub.Web.Controllers
{
    // Controller quản lý toàn bộ luồng Đăng ký, Đăng nhập và Đăng xuất của người dùng
    public class AccountController : Controller
    {
        // Khởi tạo đối tượng db từ FoodieHubDbContext để giao tiếp dữ liệu với SQL Server
        private FoodieHubDbContext db = new FoodieHubDbContext();

        // ----------------------------------------------------------------------
        // 1. ACTION REGISTER: ĐĂNG KÝ TÀI KHOẢN
        // ----------------------------------------------------------------------
        // GET: Hiển thị giao diện Form Đăng ký
        [HttpGet]
        public ActionResult Register()
        {
            return View(); // Trả về Views/Account/Register.cshtml
        }

        // POST: Xử lý lưu tài khoản mới vào CSDL khi người dùng submit form
        [HttpPost]
        [ValidateAntiForgeryToken] // Token bảo mật chống tấn công giả mạo yêu cầu chéo trang (CSRF)
        public ActionResult Register(User user) // Tự động gom thông tin các ô input thành đối tượng User
        {
            // ModelState.IsValid: Kiểm tra dữ liệu có vi phạm Data Annotations ([Required], [EmailAddress]...) ở UserMetadata không
            if (ModelState.IsValid)
            {
                // Truy vấn LINQ kiểm tra xem Email vừa nhập đã bị ai đăng ký trước đó chưa
                var checkUser = db.Users.FirstOrDefault(u => u.Email == user.Email);

                if (checkUser == null) // Nếu Email hoàn toàn mới (chưa ai dùng)
                {
                    db.Users.Add(user); // Đưa đối tượng user vào bộ đệm của Entity Framework
                    db.SaveChanges();   // Thực thi lệnh SQL INSERT INTO để lưu chính thức xuống SQL Server

                    // Đăng ký thành công, chuyển hướng ngay về trang Đăng nhập
                    return RedirectToAction("Login");
                }
                else
                {
                    // Email đã tồn tại, truyền thông báo lỗi sang View qua ViewBag
                    ViewBag.Error = "Email này đã được đăng ký! Vui lòng sử dụng Email khác.";
                }
            }
            // Dữ liệu lỗi hoặc bị trùng Email -> Nạp lại View kèm dữ liệu khách vừa nhập
            return View(user);
        }

        // ----------------------------------------------------------------------
        // 2. ACTION LOGIN: ĐĂNG NHẬP HỆ THỐNG
        // ----------------------------------------------------------------------
        // GET: Hiển thị giao diện Form Đăng nhập
        [HttpGet]
        public ActionResult Login()
        {
            return View(); // Trả về Views/Account/Login.cshtml
        }

        // POST: Xác thực tài khoản người dùng
        [HttpPost]
        [ValidateAntiForgeryToken] // Đảm bảo an toàn form đăng nhập chống CSRF
        public ActionResult Login(string email, string password)
        {
            // Kiểm tra sơ bộ không để trống ô nhập
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ Email và Mật khẩu!";
                return View();
            }

            // Truy vấn LINQ tìm tài khoản có Email VÀ Password trùng khớp 100% trong CSDL
            var user = db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null) // Đăng nhập thành công
            {
                // Khởi tạo Session lưu phiên làm việc của người dùng trên Server
                Session["User"] = user;
                Session["FullName"] = user.FullName; // Lưu tên hiển thị lên Header giao diện
                Session["UserID"] = user.UserID;     // Lưu ID phục vụ gán đơn hàng khi Checkout

                // Đá trình duyệt trở về trang chủ hệ thống
                return RedirectToAction("Index", "Home");
            }

            // Trường hợp sai Email hoặc Mật khẩu
            ViewBag.Error = "Tài khoản hoặc mật khẩu không đúng!";
            return View();
        }

        // ----------------------------------------------------------------------
        // 3. ACTION LOGOUT: ĐĂNG XUẤT HỆ THỐNG
        // ----------------------------------------------------------------------
        public ActionResult Logout()
        {
            // Xóa sạch toàn bộ Session người dùng đang lưu
            Session.Clear();
            Session.Abandon();

            // Chuyển hướng về lại trang Đăng nhập
            return RedirectToAction("Login");
        }

        // ----------------------------------------------------------------------
        // GIẢI PHÓNG TÀI NGUYÊN KẾT NỐI CSDL
        // ----------------------------------------------------------------------
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose(); // Đóng kết nối SQL Server để giải phóng bộ nhớ RAM
            }
            base.Dispose(disposing);
        }
    }
}