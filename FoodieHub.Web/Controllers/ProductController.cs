using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodieHub.Web.Controllers
{
    // Tạo một Class mô hình dữ liệu tạm thời để chạy không cần Database
    public class ProductItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int CategoryID { get; set; } // 1: Cơm, 2: Trà Sữa, 3: Bún Phở, 4: Ăn Vặt
        public int Stock { get; set; }
        public string ImageUrl { get; set; }
    }
    // Controller quản lý toàn bộ luồng trang Sản phẩm / Món ăn
    public class ProductController : Controller
    {
        private List<ProductItem> GetSampleProducts()
        {
            return new List<ProductItem>
            {
                new ProductItem {
                    ProductID = 1,
                    ProductName = "Cơm Tấm Sườn Bì Chả",
                    Price = 55000,
                    CategoryID = 1,
                    Stock = 10,
                    Description = "Sườn nướng mật ong thơm lừng, bì giòn rụm, chả trứng béo ngậy kèm nước mắm đậm đà.",
                    ImageUrl = "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&w=600&q=80"
                },
                new ProductItem {
                    ProductID = 2,
                    ProductName = "Phở Bò Đặc Biệt",
                    Price = 65000,
                    CategoryID = 3,
                    Stock = 15,
                    Description = "Nước dùng hầm xương đậm đà 24h, tái nạm gầu giòn ngon kèm húng xế, giá sống tươi tắn.",
                    ImageUrl = "https://images.unsplash.com/photo-1569718212165-3a8278d5f624?auto=format&fit=crop&w=600&q=80"
                },
                new ProductItem {
                    ProductID = 3,
                    ProductName = "Salad Hạt & Ức Gà",
                    Price = 49000,
                    CategoryID = 4,
                    Stock = 8,
                    Description = "Món ăn Healthy dinh dưỡng cao, rau củ hữu cơ tươi ngon kết hợp sốt Mè rang thơm ngon.",
                    ImageUrl = "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=600&q=80"
                }
            };
        }

        // Đảm bảo chỉ duy nhất 1 hàm Index xử lý cả lọc danh mục và tìm kiếm từ khóa
        public ActionResult Index(int? categoryId, string searchString)
        {
            var products = GetSampleProducts().Where(p => p.Stock > 0).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.ProductName.ToLower().Contains(searchString.ToLower()));
                ViewBag.CategoryName = "Kết quả tìm kiếm: \"" + searchString + "\"";
            }
            else if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryID == categoryId.Value);
                if (categoryId.Value == 1) ViewBag.CategoryName = "Cơm & Món Chính";
                else if (categoryId.Value == 2) ViewBag.CategoryName = "Trà Sữa & Đồ Uống";
                else if (categoryId.Value == 3) ViewBag.CategoryName = "Bún / Phở / Mì";
                else if (categoryId.Value == 4) ViewBag.CategoryName = "Ăn Vặt & Ăn Nhẹ";
            }
            else
            {
                ViewBag.CategoryName = "Thực Đơn Món Ăn";
            }

            return View(products.ToList());
        }

        // GET: /Product/Index (Trang Danh sách món ăn / Menu)



        // GET: /Product/Detail/1 (Trang Chi tiết món ăn cụ thể)
        // Parameter 'id' nhận mã món ăn từ trang danh sách gửi sang
        public ActionResult Detail(int? id)
        {
            return View(); // Trả về View Views/Product/Detail.cshtml
        }
    }
}