using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCeCommerce.Models;

namespace MVCeCommerce.Controllers
{
    public class ShoppingController : Controller
    {
        MVCeCommerceDbDuyguEntities db = new MVCeCommerceDbDuyguEntities();

        public ActionResult AddToCart(int id)
        {
            if (Session["Kullanici"] != null)
            {
                bool addedBefore = false;
                foreach (OrderDetail item in db.OrderDetails.ToList())
                {
                    if (item.ProductID == id)
                    {
                        addedBefore = true;
                        break;
                    }
                }

                if (!addedBefore)
                {
                    OrderDetail od = new OrderDetail();
                    od.ProductID = id;
                    od.CustomerID = TemporaryUserData.UserID;
                    od.UnitPrice = db.Products.Find(id).UnitPrice;
                    od.Quantity = 1;
                    od.Discount = db.Products.Find(id).Discount;
                    od.TotalAmount = od.UnitPrice * od.Quantity * (1 - od.Discount);
                    od.OrderDate = DateTime.Now;
                    od.isActive = true;
                    db.OrderDetails.Add(od);
                }
                else
                {
                    OrderDetail od = db.OrderDetails.Where(x => x.ProductID == id).FirstOrDefault();
                    od.Quantity++;
                    od.TotalAmount = od.UnitPrice * od.Quantity * (1 - od.Discount);
                    od.OrderDate = DateTime.Now;
                }

                db.SaveChanges();

                return RedirectToAction("ProductDetail", "Product", new { id = id });
            }
            else
                return RedirectToAction("Login", "Login");
        }

        public ActionResult AddToWishlist(int id)
        {
            if (Session["Kullanici"] != null)
            {
                bool addedBefore = false;
                foreach (Wishlist item in db.Wishlists.ToList())
                {
                    if (item.ProductID == id)
                    {
                        addedBefore = true;
                        break;
                    }
                }

                if (!addedBefore)
                {
                    Wishlist wishlist = new Wishlist();
                    wishlist.CustomerID = TemporaryUserData.UserID;
                    wishlist.ProductID = id;
                    wishlist.IsActive = true;

                    wishlist.Product = db.Products.Find(id);

                    db.Wishlists.Add(wishlist);
                    db.SaveChanges();
                }

                return RedirectToAction("ProductDetail", "Product", new { id = id });
            }
            else
                return RedirectToAction("Login", "Login");
        }

        public ActionResult Cart()
        {
            TempData["OrderDetail"] = db.OrderDetails.Where(x => x.CustomerID == TemporaryUserData.UserID && x.isActive == true).ToList();
            return View();
        }

        public ActionResult Wishlist()
        {
            TempData["Wishlist"] = db.Wishlists.Where(x => x.CustomerID == TemporaryUserData.UserID).ToList();
            return View();
        }

        public ActionResult RemoveFromWishlist(int id)
        {
            Wishlist wish = db.Wishlists.Where(x => x.ProductID == id).FirstOrDefault();

            db.Wishlists.Remove(wish);
            db.SaveChanges();

            TempData["Wishlist"] = db.Wishlists.ToList();
            return RedirectToAction("Wishlist");
        }

        public ActionResult AddToCartFromWishlist(int id)
        {
            Wishlist wishlist = db.Wishlists.Where(x => x.ProductID == id).FirstOrDefault();

            db.Wishlists.Remove(wishlist);

            OrderDetail od = db.OrderDetails.Where(x => x.ProductID == id).FirstOrDefault();

            if(od != null)
            {
                od.Quantity++;
                od.TotalAmount += od.UnitPrice * (1 - od.Discount);
                od.OrderDate = DateTime.Now;
            }
            else
            {
                od = new OrderDetail();

                od.ProductID = id;
                od.CustomerID = TemporaryUserData.UserID;
                od.UnitPrice = db.Products.Find(id).UnitPrice;
                od.Discount = db.Products.Find(id).Discount;
                od.Quantity = 1;
                od.TotalAmount = od.UnitPrice * od.Quantity * (1 - od.Discount);
                od.OrderDate = DateTime.Now;
                od.isActive = true;

                db.OrderDetails.Add(od);
            }

            db.SaveChanges();

            TempData["Wishlist"] = db.Wishlists.ToList();
            return RedirectToAction("Wishlist");
        }

        public ActionResult RemoveFromCart(int id)
        {
            OrderDetail od = db.OrderDetails.Where(x => x.ProductID == id).FirstOrDefault();

            db.OrderDetails.Remove(od);
            db.SaveChanges();

            TempData["OrderDetail"] = db.OrderDetails.ToList();
            return RedirectToAction("Cart");
        }

        public ActionResult AddToWishlistFromCart(int id)
        {
            OrderDetail od = db.OrderDetails.Where(x => x.ProductID == id).FirstOrDefault();
            db.OrderDetails.Remove(od);

            Wishlist w = db.Wishlists.Where(x => x.ProductID == id).FirstOrDefault();

            if(w == null)
            {
                w = new Wishlist();

                w.ProductID = id;
                w.CustomerID = TemporaryUserData.UserID;
                w.IsActive = true;

                db.Wishlists.Add(w);
            }
            db.SaveChanges();

            TempData["OrderDetail"] = db.OrderDetails.Where(x=> x.isActive == true).ToList();
            return RedirectToAction("Cart");
        }

        public ActionResult CompleteOrder()
        {

            List<OrderDetail> siparisler = db.OrderDetails.Where(x => x.CustomerID == TemporaryUserData.UserID && x.isActive == true).ToList();

            foreach(var urun in siparisler)
            {
                Order o = new Order();
                o.CustomerID = TemporaryUserData.UserID;
                o.OrderDetailsID = urun.OrderDetailsID;
                o.Discount = Convert.ToInt32(urun.Discount);
                o.Taxes = 18;
                o.TotalAmount = Convert.ToInt32(urun.TotalAmount);
                o.IsCompleted = false;
                o.OrderDate = DateTime.Now;
                db.Orders.Add(o);
                urun.isActive = false;

                
            }

            db.SaveChanges();

            TempData["PaymentTypes"] = db.PaymentTypes.ToList();
            TempData["Customer"] = db.Customers.Where(x => x.CustomerID == TemporaryUserData.UserID).FirstOrDefault();

            return View();
        }

        [HttpPost]
        public ActionResult CompleteOrder(FormCollection frm)
        {
            List<Order> siparisler = db.Orders.Where(x => x.CustomerID == TemporaryUserData.UserID && x.IsCompleted == false).ToList();

            Payment odeme = new Payment();
            odeme.Type = int.Parse(frm["PaymentType"]);
            odeme.PaymentDateTime = DateTime.Now;
            db.Payments.Add(odeme);

            ShippingDetail spd = new ShippingDetail();
            spd.Address = frm["Adres1"];
            spd.Mobile = frm["Mobile1"];
            spd.FirstName = db.Customers.Find(TemporaryUserData.UserID).FirstName;
            spd.LastName = db.Customers.Find(TemporaryUserData.UserID).LastName;
            db.ShippingDetails.Add(spd);

            db.SaveChanges();
            
            foreach (var urun in siparisler)
            {
                urun.IsCompleted = true;
                urun.ShippingID = spd.ShippingID;
                urun.Shipped = true;
                urun.ShippingDate = DateTime.Now;
                urun.PaymentID = odeme.PaymentID;
            }

            db.SaveChanges();
            //TODO: bütün ürünlerden satın alınan miktar kadar UnitInStock değerinden düşme yapılacak
            

            return RedirectToAction("Index", "Home");
        }


    }
}