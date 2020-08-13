using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCeCommerce.Models;

namespace MVCeCommerce.Controllers
{
    public class HomeController : Controller
    {
        MVCeCommerceDbDuyguEntities db = new MVCeCommerceDbDuyguEntities();

        public ActionResult Index()
        {
            TempData["RandomProductKlasik"] = db.Products.Where(x => x.SubCategoryID == 1).OrderBy(r => Guid.NewGuid()).Take(4).ToList();
            TempData["RandomProductElektro"] = db.Products.Where(x => x.SubCategoryID == 2).OrderBy(r => Guid.NewGuid()).Take(4).ToList();
            TempData["RandomProductBas"] = db.Products.Where(x => x.SubCategoryID == 3).OrderBy(r => Guid.NewGuid()).Take(4).ToList();


            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}