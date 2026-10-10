using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodieHub.Web.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        public ActionResult TransactionHistory()
        {
            return View();
        }
    }
}