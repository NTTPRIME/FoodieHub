using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodieHub.Web.Controllers
{
    // Controller quản lý toàn bộ luồng trang Sản phẩm / Món ăn
    public class ProductController : Controller
    {
        // GET: /Product/Index (Trang Danh sách món ăn / Menu)
        public ActionResult Index()
        {
            return View(); // Trả về View Views/Product/Index.cshtml
        }

        // GET: /Product/Detail/1 (Trang Chi tiết món ăn cụ thể)
        // Parameter 'id' nhận mã món ăn từ trang danh sách gửi sang
        public ActionResult Detail(int? id)
        {
            return View(); // Trả về View Views/Product/Detail.cshtml
        }
    }
}