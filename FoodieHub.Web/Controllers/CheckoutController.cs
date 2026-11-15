using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FoodieHub.Web.Models;
using FoodieHub.Web.Models.ViewModels;

namespace FoodieHub.Web.Controllers
{
    public class CheckoutController : Controller
    {
        private FoodieHubDbContext db = new FoodieHubDbContext();

        [HttpGet]
        public ActionResult Index()
        {
            var currentUser = Session["User"] as User;
            if (currentUser == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập trước khi tiến hành thanh toán!";
                return RedirectToAction("Login", "Account");
            }

            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProcessOrder(string CustomerName, string Phone, string Email, string Address)
        {
            var currentUser = Session["User"] as User;
            if (currentUser == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập trước khi đặt đơn hàng!";
                return RedirectToAction("Login", "Account");
            }

            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            // BƯỚC 1: Kiểm tra tồn kho lần cuối trước khi ghi nhận đơn hàng
            foreach (var item in cart)
            {
                var productInDb = db.Products.Find(item.ProductID);
                if (productInDb == null || productInDb.Stock < item.Quantity)
                {
                    string name = productInDb != null ? productInDb.ProductName : item.ProductName;
                    int remain = productInDb != null ? productInDb.Stock : 0;
                    TempData["Error"] = $"Món \"{name}\" chỉ còn {remain} phần trong kho, không đủ số lượng bạn đặt ({item.Quantity} phần). Vui lòng cập nhật lại giỏ hàng!";
                    return RedirectToAction("Index");
                }
            }

            // BƯỚC 2: Khởi tạo đơn hàng tổng kèm trạng thái mặc định ban đầu
            Order newOrder = new Order
            {
                CustomerName = CustomerName,
                Phone = Phone,
                Email = Email,
                Address = Address,
                OrderDate = DateTime.Now,
                UserID = currentUser.UserID,
                Status = "Chờ xác nhận", // Gán trạng thái khởi tạo chuẩn cho Admin quản lý
                TotalAmount = cart.Sum(item => item.Price * item.Quantity)
            };
            db.Orders.Add(newOrder);

            // BƯỚC 3: Tạo chi tiết đơn hàng & Trừ tồn kho đồng thời
            foreach (var item in cart)
            {
                OrderDetail detail = new OrderDetail
                {
                    Order = newOrder,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price
                };
                db.OrderDetails.Add(detail);

                var productInDb = db.Products.Find(item.ProductID);
                if (productInDb != null)
                {
                    productInDb.Stock -= item.Quantity; // Cập nhật giảm kho thực tế
                }
            }

            // BƯỚC 4: Lưu toàn bộ dữ liệu vào CSDL chỉ với 1 lần SaveChanges() duy nhất (An toàn tuyệt đối, không sợ phát sinh hóa đơn rỗng)
            db.SaveChanges();

            // Xóa giỏ hàng và thông báo thành công
            Session["Cart"] = null;
            TempData["Success"] = "Đặt hàng thành công! Bạn có thể theo dõi trạng thái đơn hàng tại đây.";

            // Điều hướng sang trang Lịch sử giao dịch của khách hàng
            return RedirectToAction("TransactionHistory", "Customer");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}