using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodieHub.Web.Controllers
{
    // Controller quản lý quá trình Thanh toán & Đặt hàng
    public class CheckoutController : Controller
    {
        // GET: /Checkout/Index (Trang nhập thông tin giao hàng và xác nhận đặt món)
        public ActionResult Index()
        {
            return View(); // Trả về View Views/Checkout/Index.cshtml
        }
    }
}