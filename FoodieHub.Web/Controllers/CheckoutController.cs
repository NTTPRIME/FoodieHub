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

            // 1. Tạo đơn hàng tổng (Đã bổ sung Phone và Email)
            Order newOrder = new Order
            {
                CustomerName = CustomerName,
                Phone = Phone,       // <-- Bổ sung gán số điện thoại
                Email = Email,       // <-- Bổ sung gán email
                Address = Address,
                OrderDate = DateTime.Now,
                UserID = currentUser.UserID
            };

            newOrder.TotalAmount = cart.Sum(item => item.Price * item.Quantity);
            db.Orders.Add(newOrder);
            db.SaveChanges(); // Lưu để sinh OrderID tự động

            // 2. Tạo chi tiết đơn hàng & Trừ số lượng tồn kho
            foreach (var item in cart)
            {
                OrderDetail detail = new OrderDetail
                {
                    OrderID = newOrder.OrderID,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price
                };
                db.OrderDetails.Add(detail);

                var productInDb = db.Products.Find(item.ProductID);
                if (productInDb != null)
                {
                    productInDb.Stock = productInDb.Stock - item.Quantity; // Cập nhật giảm kho
                }
            }

            db.SaveChanges();
            Session["Cart"] = null; // Xóa giỏ hàng sau khi đặt thành công

            return RedirectToAction("Index", "Home");
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