// Khai báo thư viện LINQ hỗ trợ truy vấn dữ liệu từ cơ sở dữ liệu SQL Server
using System.Linq;
// Khai báo thư viện ASP.NET MVC cung cấp Controller, ActionResult, ViewBag, HttpNotFound...
using System.Web.Mvc;
// Khai báo thư viện Entity Framework hỗ trợ quản lý trạng thái thực thể (EntityState)
using System.Data.Entity;
// Nhúng namespace chứa các mô hình dữ liệu (User, FoodieHubDbContext) của ứng dụng FoodieHub
using FoodieHub.Web.Models;

// Định nghĩa không gian tên chứa các Controller phục vụ phân hệ Quản trị của FoodieHub
namespace FoodieHub.Web.Controllers
{
    // Khai báo lớp AdminUserController kế thừa từ Controller để quản lý tài khoản người dùng Admin
    public class AdminUserController : Controller
    {
        // Khởi tạo đối tượng db từ FoodieHubDbContext để thực hiện các thao tác CRUD với CSDL
        private FoodieHubDbContext db = new FoodieHubDbContext();

        // ----------------------------------------------------------------------
        // 1. ACTION INDEX: HIỂN THỊ DANH SÁCH NGƯỜI DÙNG ADMIN (Barem Dòng 19)
        // ----------------------------------------------------------------------
        // Phương thức Index nhận yêu cầu GET để lấy và hiển thị danh sách tất cả tài khoản Admin
        [HttpGet]
        public ActionResult Index()
        {
            // Truy vấn lấy toàn bộ danh sách bản ghi từ bảng Users trong CSDL và chuyển thành List
            var usersList = db.Users.ToList();
            // Trả về View Index.cshtml kèm theo danh sách usersList để hiển thị lên bảng
            return View(usersList);
        }

        // ----------------------------------------------------------------------
        // 2. ACTION CREATE: THÊM NGƯỜI DÙNG ADMIN MỚI (Barem Dòng 20)
        // ----------------------------------------------------------------------
        // Phương thức Create GET dùng để hiển thị giao diện Form thêm mới người dùng
        [HttpGet]
        public ActionResult Create()
        {
            // Trả về View Create.cshtml rỗng để người dùng nhập thông tin tài khoản mới
            return View();
        }

        // Phương thức Create POST nhận dữ liệu gửi lên từ Form khi người dùng nhấn "Lưu tài khoản"
        [HttpPost]
        // Bộ lọc ValidateAntiForgeryToken giúp bảo mật form, chống tấn công giả mạo CSRF
        [ValidateAntiForgeryToken]
        // Nhận đối tượng user chứa thông tin nhập vào từ Form
        public ActionResult Create(User user)
        {
            // Kiểm tra tính hợp lệ của dữ liệu theo các Data Annotation trong UserMetadata
            if (ModelState.IsValid)
            {
                // Truy vấn LINQ kiểm tra xem địa chỉ Email vừa nhập đã tồn tại trong CSDL chưa
                var existingUser = db.Users.FirstOrDefault(u => u.Email == user.Email);

                // Nếu chưa tồn tại tài khoản nào sử dụng Email này (existingUser == null)
                if (existingUser == null)
                {
                    // Thêm đối tượng user mới vào bộ đệm của Entity Framework
                    db.Users.Add(user);
                    // Thực thi câu lệnh SQL INSERT INTO để lưu chính thức tài khoản vào CSDL
                    db.SaveChanges();
                    // Chuyển hướng người dùng trở lại trang danh sách tài khoản (Index)
                    return RedirectToAction("Index");
                }
                // Trường hợp địa chỉ Email đã được tài khoản khác đăng ký sử dụng
                else
                {
                    // Ghi thông báo lỗi trùng Email vào ViewBag.Error để hiển thị trên View
                    ViewBag.Error = "Email này đã được sử dụng. Vui lòng nhập Email khác!";
                }
            }
            // Trả về lại View Create kèm đối tượng user chứa thông tin đã nhập để sửa lại
            return View(user);
        }

        // ----------------------------------------------------------------------
        // 3. ACTION EDIT: CHỈNH SỬA TÀI KHOẢN NGƯỜI DÙNG ADMIN
        // ----------------------------------------------------------------------
        // Phương thức Edit GET nhận id người dùng cần sửa để lấy dữ liệu cũ đưa lên Form
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            // Kiểm tra an toàn: Nếu tham số id truyền vào bị rỗng (null)
            if (id == null)
            {
                // Trả về trang lỗi HttpNotFound (404) để dừng xử lý
                return HttpNotFound();
            }
            // Truy vấn CSDL tìm tài khoản có UserID tương ứng với mã id
            var user = db.Users.Find(id);
            // Kiểm tra nếu không tìm thấy bản ghi tài khoản nào khớp với id
            if (user == null)
            {
                // Trả về mã lỗi HttpNotFound (404) báo không tìm thấy người dùng
                return HttpNotFound();
            }
            // Trả về View Edit.cshtml và nạp dữ liệu tài khoản user vừa tìm được
            return View(user);
        }

        // Phương thức Edit POST xử lý cập nhật thông tin người dùng sau khi chỉnh sửa trên Form
        [HttpPost]
        // Xác thực token chống tấn công CSRF để đảm bảo an toàn giao dịch
        [ValidateAntiForgeryToken]
        // Nhận đối tượng user chứa thông tin mới vừa chỉnh sửa gửi lên từ Form
        public ActionResult Edit(User user)
        {
            // Kiểm tra tính hợp lệ của các trường dữ liệu theo Data Annotation
            if (ModelState.IsValid)
            {
                // Kiểm tra xem Email mới có bị trùng với tài khoản của người dùng khác hay không
                var existingUser = db.Users.FirstOrDefault(u => u.Email == user.Email && u.UserID != user.UserID);
                // Nếu tìm thấy tài khoản khác đang chiếm giữ Email này
                if (existingUser != null)
                {
                    // Đưa thông báo lỗi trùng Email vào ViewBag.Error
                    ViewBag.Error = "Email này đã được tài khoản khác sử dụng. Vui lòng chọn Email khác!";
                    // Nạp lại View Edit kèm dữ liệu user vừa chỉnh sửa
                    return View(user);
                }

                // Đánh dấu thực thể user là đã thay đổi (Modified) trong Entity Framework
                db.Entry(user).State = EntityState.Modified;
                // Thực thi câu lệnh SQL UPDATE để ghi nhận dữ liệu mới xuống CSDL
                db.SaveChanges();
                // Chuyển hướng người dùng về trang danh sách tài khoản (Index)
                return RedirectToAction("Index");
            }
            // Nếu dữ liệu không hợp lệ, giữ nguyên giao diện View Edit để người dùng sửa lại
            return View(user);
        }

        // ----------------------------------------------------------------------
        // 4. ACTION DELETE: XÓA TÀI KHOẢN NGƯỜI DÙNG ADMIN (Barem Dòng 21)
        // ----------------------------------------------------------------------
        // Phương thức Delete GET hiển thị trang xác nhận xóa tài khoản người dùng
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            // Kiểm tra bảo mật: Nếu tham số id truyền vào bị rỗng (null)
            if (id == null)
            {
                // Trả về lỗi HttpNotFound (404) để dừng luồng xử lý
                return HttpNotFound();
            }
            // Truy vấn tìm bản ghi người dùng mang mã id trong cơ sở dữ liệu
            var user = db.Users.Find(id);
            // Kiểm tra an toàn: Nếu không tìm thấy người dùng trùng khớp với mã id
            if (user == null)
            {
                // Trả về lỗi HttpNotFound (404) báo không tìm thấy
                return HttpNotFound();
            }
            // Trả về View Delete.cshtml nạp thông tin người dùng để quản trị viên xác nhận xóa
            return View(user);
        }

        // Phương thức DeleteConfirmed POST thực thi thao tác xóa người dùng vĩnh viễn
        [HttpPost, ActionName("Delete")]
        // Xác thực token chống tấn công giả mạo yêu cầu chéo trang CSRF
        [ValidateAntiForgeryToken]
        // Nhận mã id định danh tài khoản người dùng cần xóa
        public ActionResult DeleteConfirmed(int id)
        {
            // Tìm chính xác bản ghi người dùng có UserID bằng id trong CSDL
            var user = db.Users.Find(id);
            // Đảm bảo đối tượng người dùng thực sự tồn tại (khác null)
            if (user != null)
            {
                // Đánh dấu đối tượng user vào danh sách xóa của Entity Framework
                db.Users.Remove(user);
                // Thực thi câu lệnh SQL DELETE để xóa bản ghi khỏi bảng Users trong CSDL
                db.SaveChanges();
            }
            // Chuyển hướng trình duyệt trở lại trang danh sách người dùng (Index)
            return RedirectToAction("Index");
        }

        // ----------------------------------------------------------------------
        // 5. GIẢI PHÓNG TÀI NGUYÊN KẾT NỐI CSDL
        // ----------------------------------------------------------------------
        // Ghi đè phương thức Dispose để đóng kết nối CSDL khi Controller kết thúc vòng đời
        protected override void Dispose(bool disposing)
        {
            // Kiểm tra nếu đang trong quá trình giải phóng tài nguyên
            if (disposing)
            {
                // Ngắt kết nối tới SQL Server để giải phóng tài nguyên bộ nhớ RAM
                db.Dispose();
            }
            // Gọi phương thức Dispose của lớp cha (Controller)
            base.Dispose(disposing);
        }
    }
}