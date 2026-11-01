using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FoodieHub.Web.Models;

namespace FoodieHub.Web.Controllers
{
    public class AdminWarehouseController : Controller
    {
        // Khởi tạo đối tượng kết nối CSDL
        private FoodieHubDbContext db = new FoodieHubDbContext();

        // ----------------------------------------------------------------------
        // ACTION INDEX: QUẢN LÝ KHO HÀNG (Barem Dòng 18)
        // Thống kê 2 phần: Danh sách hàng tồn (Stock > 0) và Hàng cần nhập (Stock == 0)
        // ----------------------------------------------------------------------
        public ActionResult Index()
        {
            // Truy xuất toàn bộ sản phẩm từ CSDL kèm thông tin Danh mục liên kết
            var allProducts = db.Products.Include(p => p.Category).ToList();

            // Phân loại danh sách kho hàng đưa vào ViewBag để truyền cho các Partial View
            ViewBag.InStockProducts = allProducts.Where(p => p.Stock > 0).ToList();
            ViewBag.NeedImportProducts = allProducts.Where(p => p.Stock == 0).ToList();

            // Trả về View chính cùng toàn bộ danh sách sản phẩm
            return View(allProducts);
        }

        // Giải phóng tài nguyên kết nối DbContext khi không sử dụng
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