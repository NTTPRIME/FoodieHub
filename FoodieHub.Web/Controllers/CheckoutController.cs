using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FoodieHub.Web.Models;
using FoodieHub.Web.Models.ViewModels; // Đã sửa: Nạp đúng ViewModel CartItem

namespace FoodieHub.Web.Controllers
{
    public class CheckoutController : Controller
    {
        private FoodieHubDbContext db = new FoodieHubDbContext();

        [HttpGet]
        public ActionResult Index()
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            // Truyền danh sách giỏ hàng sang View để hiển thị tóm tắt đơn hàng
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProcessOrder(string CustomerName, string Address)
        {
            var cart = Session["Cart"] as List<CartItem>;

            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            // 1. Tạo đơn hàng tổng
            Order newOrder = new Order
            {
                CustomerName = CustomerName,
                Address = Address,
                OrderDate = DateTime.Now
            };

            var currentUser = Session["User"] as User;
            if (currentUser != null)
            {
                newOrder.UserID = currentUser.UserID;
            }

            newOrder.TotalAmount = cart.Sum(item => item.Price * item.Quantity);
            db.Orders.Add(newOrder);
            db.SaveChanges(); // Lưu để sinh OrderID tự động

            // 2. Tạo chi tiết đơn hàng & Trừ số lượng tồn kho (Barem 58-60)
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