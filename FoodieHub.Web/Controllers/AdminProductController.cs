using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FoodieHub.Web.Models;

namespace FoodieHub.Web.Controllers
{
    public class AdminProductController : Controller
    {
        // Khởi tạo đối tượng kết nối CSDL
        private FoodieHubDbContext db = new FoodieHubDbContext();

        // ----------------------------------------------------------------------
        // 1. ACTION INDEX: HIỂN THỊ DANH SÁCH MÓN ĂN (Barem Dòng 13)
        // ----------------------------------------------------------------------
        public ActionResult Index()
        {
            // Lấy danh sách sản phẩm kèm thông tin Danh mục liên kết
            var products = db.Products.Include(p => p.Category).ToList();
            return View(products);
        }

        // ----------------------------------------------------------------------
        // 2. ACTION DETAILS: CHI TIẾT MÓN ĂN (Barem Dòng 14)
        // ----------------------------------------------------------------------
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // ----------------------------------------------------------------------
        // 3. ACTION CREATE: THÊM MỚI MÓN ĂN (Barem Dòng 15, 16)
        // ----------------------------------------------------------------------
        [HttpGet]
        public ActionResult Create()
        {
            // Nạp danh sách danh mục vào DropDownList
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Xử lý Upload Ảnh (Barem Dòng 16)
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                    string extension = Path.GetExtension(ImageFile.FileName);
                    // Đặt tên file kèm Timestamp để tránh trùng tên ảnh
                    fileName = fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;

                    string folderPath = Server.MapPath("~/Content/Images/");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string path = Path.Combine(folderPath, fileName);
                    ImageFile.SaveAs(path);
                    product.ImageUrl = "/Content/Images/" + fileName;
                }

                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Nếu dữ liệu không hợp lệ, giữ nguyên danh mục đã chọn
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // ----------------------------------------------------------------------
        // 4. ACTION EDIT: CHỈNH SỬA MÓN ĂN (Barem Dòng 15, 16)
        // ----------------------------------------------------------------------
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase ImageFile, string OldImageUrl)
        {
            if (ModelState.IsValid)
            {
                // Nếu người dùng chọn ảnh mới
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                    string extension = Path.GetExtension(ImageFile.FileName);
                    fileName = fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;

                    string folderPath = Server.MapPath("~/Content/Images/");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string path = Path.Combine(folderPath, fileName);
                    ImageFile.SaveAs(path);
                    product.ImageUrl = "/Content/Images/" + fileName;
                }
                else
                {
                    // Giữ lại đường dẫn ảnh cũ nếu không đổi ảnh
                    product.ImageUrl = OldImageUrl;
                }

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // ----------------------------------------------------------------------
        // 5. ACTION DELETE: XÓA MÓN ĂN (Barem Dòng 17 - Kiểm tra ràng buộc)
        // ----------------------------------------------------------------------
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            if (product != null)
            {
                // KIỂM TRA RÀNG BUỘC (Constraints): Kiểm tra xem món ăn đã từng nằm trong Chi tiết đơn hàng chưa
                int checkOrderDetails = db.OrderDetails.Count(od => od.ProductID == id);
                if (checkOrderDetails > 0)
                {
                    ViewBag.Error = "Không thể xóa món ăn này vì đã nằm trong " + checkOrderDetails + " chi tiết đơn hàng lịch sử!";
                    return View(product);
                }

                db.Products.Remove(product);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}