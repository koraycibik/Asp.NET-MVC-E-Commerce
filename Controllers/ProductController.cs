using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCeCommerce.Models;

namespace MVCeCommerce.Controllers
{
    public class ProductController : Controller
    {
        MVCeCommerceDbDuyguEntities db = new MVCeCommerceDbDuyguEntities();

        public ActionResult Product(int id)
        {
            TempData["ProductList"] = db.Products.Where(x => x.SubCategoryID == id).ToList();

            return View();
        }

        public ActionResult ProductDetail(int id)
        {
            ViewData["Reviews"] = db.Reviews.Where(x => x.ProductID == id).ToList();
            TempData["ProductDetail"] = db.Products.Where(x => x.ProductID == id).FirstOrDefault();

            return View();
        }

        [HttpPost]
        public ActionResult AddReview(int id, FormCollection frm)
        {
            Review review = new Review();
            review.ProductID = id;
            review.CustomerID = TemporaryUserData.UserID;
            review.Name = frm["Name"];
            review.Review1 = frm["Review"];
            review.Rate = int.Parse(frm["Rate"]);
            review.DateTime = DateTime.Now;
            review.IsDeleted = false;

            db.Reviews.Add(review);
            db.SaveChanges();

            return RedirectToAction("ProductDetail", new { id = review.ProductID });
        }
    }
}