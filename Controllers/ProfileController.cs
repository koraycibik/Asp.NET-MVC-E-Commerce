using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCeCommerce.Models;

namespace MVCeCommerce.Controllers
{
    public class ProfileController : Controller
    {
        MVCeCommerceDbDuyguEntities db = new MVCeCommerceDbDuyguEntities();

        public ActionResult Index()
        {
            return View(db.Customers.Find(TemporaryUserData.UserID));

        }

        [HttpPost]
        public ActionResult Index(FormCollection frm)
        {
            Customer c = db.Customers.Find(TemporaryUserData.UserID);

            c.FirstName = frm["FirstName"];
            c.LastName = frm["LastName"];
            c.Password = frm["Password"];
            c.Age = int.Parse(frm["Age"]);
            c.Gender = frm["Gender"];
            c.BirthDate = DateTime.Parse(frm["BirthDate"]);
            c.Organization = frm["Organization"];
            c.Country = frm["Country"];
            c.State = frm["State"];
            c.City = frm["City"];
            c.PostalCode = frm["PostalCode"];
            c.Email = frm["Email"];
            c.AltEmail = frm["AltEmail"];
            c.Phone1 = frm["Phone1"];
            c.Mobile1 = frm["Mobile1"];
            c.Address1 = frm["Address1"];
            c.Address2 = frm["Address2"];

            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}