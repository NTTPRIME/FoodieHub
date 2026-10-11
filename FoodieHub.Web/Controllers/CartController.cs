using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodieHub.Web.Controllers
{
    // Controller quản lý Giỏ hàng
    public class CartController : Controller
    {
        // GET: /Cart/Index (Trang xem danh sách các món trong giỏ hàng)
        public ActionResult Index()
        {
            return View(); // Trả về View Views/Cart/Index.cshtml
        }
    }
}