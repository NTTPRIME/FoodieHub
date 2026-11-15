using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using FoodieHub.Web.Models;

namespace FoodieHub.Web.Controllers
{
    public class AdminUserController : BaseAdminController
    {
        private FoodieHubDbContext db = new FoodieHubDbContext();

        [HttpGet]
        public ActionResult Index()
        {
            // Chỉ lấy danh sách tài khoản Admin
            var adminUsersList = db.Users
                                   .Where(u => u.Email != null && u.Email.ToLower().Contains("admin"))
                                   .ToList();
            return View(adminUsersList);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = db.Users.FirstOrDefault(u => u.Email == user.Email);
                if (existingUser == null)
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "Email này đã được sử dụng. Vui lòng nhập Email khác!";
                }
            }
            return View(user);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = db.Users.FirstOrDefault(u => u.Email == user.Email && u.UserID != user.UserID);
                if (existingUser != null)
                {
                    ViewBag.Error = "Email này đã được tài khoản khác sử dụng. Vui lòng chọn Email khác!";
                    return View(user);
                }

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var user = db.Users.Include(u => u.Orders)
                               .FirstOrDefault(u => u.UserID == id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var user = db.Users.Include(u => u.Orders)
                               .FirstOrDefault(u => u.UserID == id);
            if (user != null)
            {
                // Chặn tự xóa tài khoản đang đăng nhập
                if (Session["UserID"] != null && Convert.ToInt32(Session["UserID"]) == id)
                {
                    ViewBag.Error = "Bạn không thể tự xóa tài khoản Admin đang đăng nhập hiện tại!";
                    return View("Delete", user);
                }

                // Chặn xóa tài khoản đã có đơn hàng để tránh lỗi khóa ngoại SQL
                int orderCount = db.Orders.Count(o => o.UserID == id);
                if (orderCount > 0)
                {
                    ViewBag.Error = "Không thể xóa tài khoản này vì đang liên kết với " + orderCount + " đơn hàng trong lịch sử!";
                    return View("Delete", user);
                }

                db.Users.Remove(user);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
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