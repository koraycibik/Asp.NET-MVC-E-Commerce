using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCeCommerce.Models;

namespace MVCeCommerce.Controllers
{
    public class LoginController : Controller
    {
        MVCeCommerceDbDuyguEntities db = new MVCeCommerceDbDuyguEntities();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection frm)
        {
            string userName = frm["UserName"];
            string password = frm["Password"];

            Customer c = db.Customers.Where(x => x.UserName == userName && x.Password == password).FirstOrDefault();

            if(c != null)
            {
                Session["Kullanici"] = c.UserName;
                TemporaryUserData.UserID = c.CustomerID;

                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public ActionResult Logout()
        {
            Session["Kullanici"] = null;
            TemporaryUserData.UserID = 0;

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(FormCollection frm)
        {
            string uName = frm["UserName"];
            Customer c = db.Customers.Where(x => x.UserName == uName).FirstOrDefault();

            if (c == null)
            {
                Customer cust = new Customer();
                cust.FirstName = frm["FirstName"];
                cust.LastName = frm["LastName"];
                cust.UserName = frm["UserName"];
                cust.Password = frm["Password"];
                cust.Age = int.Parse(frm["Age"]);
                cust.Gender = frm["Gender"];
                cust.BirthDate = DateTime.Parse(frm["Birthdate"]);
                cust.Email = frm["Email"];
                cust.Mobile1 = frm["Mobile1"];
                cust.Address1 = frm["Address1"];

                db.Customers.Add(cust);
                db.SaveChanges();

                Session["Kullanici"] = cust.UserName;
                TemporaryUserData.UserID = cust.CustomerID;

                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}