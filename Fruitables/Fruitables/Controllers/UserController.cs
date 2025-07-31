using Fruitables.Models;
using Fruitables.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fruitables.Controllers
{
    public class UserController : Controller
    {
        private readonly MainDbContextFile db;
        private readonly EmailService email;
        public UserController(MainDbContextFile db, EmailService e)
        {
            email = e;
            this.db = db;
        }

        public IActionResult Index()
        {
            var list = db.products.Include(cat => cat.category).ToList();

            // Show cart count in icon
            var userId = Convert.ToInt32(HttpContext.Session.GetInt32("uId"));
            var cartTot = db.addtocarts
                    .Where(c => c.userId == userId)
                    .Count();
            ViewBag.cartCount = cartTot;

            var wishCount = db.wishlists
                    .Include(c => c.product)
                    .Where(c => c.userId == userId)
                    .Count();
            ViewBag.wishCount = wishCount;


            return View(list);
        }

        // View Page
        public IActionResult AddToCart()
        {
            if (HttpContext.Session.GetInt32("uId") != null)
            {
                var userId = Convert.ToInt32(HttpContext.Session.GetInt32("uId"));


                var cartItems = db.addtocarts
                    .Include(c => c.product)
                    .Where(c => c.userId == userId)
                    .ToList();

                decimal subtotal = cartItems.Sum(item => item.product.prodPrice * item.Quantity);

                //var abc = new Cart
                //{
                //    addtocarts = cartItems,
                //    Subtotal = subtotal,
                //    Discount = subtotal * 10 / 100 // Default discount
                //};
                //return View(abc);
                return View(new Cart
                {
                    addtocarts = cartItems,
                    Subtotal = subtotal,
                    Discount = 0// Default discount
                });

            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // Add into db
        [HttpPost]
        public IActionResult AddToCart(int? id)
        {
            if (HttpContext.Session.GetInt32("uId") != null)
            {
                var userId = Convert.ToInt32(HttpContext.Session.GetInt32("uId"));
                var prod = db.products.Find(id);
                addtocart addtocart = new addtocart()
                {
                    prodId = prod.prodId,
                    userId = userId,
                };
                db.addtocarts.Add(addtocart);
                db.SaveChanges();
                return RedirectToAction("AddToCart");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public IActionResult checkCoupon(Cart cart)
        {
            var cou = db.coupons.FirstOrDefault(c => c.cCode == cart.coupons);
            if (cou != null)
            {
                // Re-fetch cart data and apply discount
                var userId = Convert.ToInt32(HttpContext.Session.GetInt32("id"));
                var cartItems = db.addtocarts
                    .Include(a => a.product)
                    .Where(a => a.userId == userId)
                    .ToList();

                // Calculate subtotal
                decimal subtotal = cartItems.Sum(item => item.product.prodPrice * item.Quantity);

                // Return Cart model to view
                return View("AddToCart", new Cart
                {
                    addtocarts = cartItems,
                    coupons = cart.coupons,
                    Subtotal = subtotal,
                    Discount = subtotal * (cou.discount / 100)
                });
            }
            else
            {
                TempData["Error"] = "Invalid coupon code!";
                return RedirectToAction("AddToCart");
            }
        }

        public IActionResult DeleteCart(int? id)
        {
            var data = db.addtocarts.Find(id);
            db.addtocarts.Remove(data);
            db.SaveChanges();
            return RedirectToAction("AddToCart");
        }

        public IActionResult WishList()
        {
            if (HttpContext.Session.GetInt32("uId") != null)
            {
                var userId = Convert.ToInt32(HttpContext.Session.GetInt32("uId"));

                var wishCount = db.wishlists
                    .Where(c => c.userId == userId)
                    .Count();
                ViewBag.wishCount = wishCount;

                var Items = db.wishlists
                    .Include(c => c.product)
                    .Include(c => c.user)
                    .Where(c => c.userId == userId)
                    .ToList();            
                return View(Items);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public IActionResult WishList(int? id)
        {
            if (HttpContext.Session.GetInt32("uId") != null)
            {
                var userId = Convert.ToInt32(HttpContext.Session.GetInt32("uId"));
                var prod = db.products.Find(id);
                wishlist wishlist = new wishlist()
                {
                    prodId = prod.prodId,
                    userId = userId,
                };
                db.wishlists.Add(wishlist);
                db.SaveChanges();
                return RedirectToAction("WishList");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public IActionResult DeleteWish(int? id)
        {
            var data = db.wishlists.Find(id);
            db.wishlists.Remove(data);
            db.SaveChanges();
            return RedirectToAction("WishList");
        }
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(contact contact)
        {
            db.contacts.Add(contact);
            db.SaveChanges();
            return View();
        }


        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(user users)
        {
            var user = db.users.Where(db => db.userEmail == users.userEmail).FirstOrDefault();
            if (user != null)
            {
                ViewBag.err = "This email is already exist";
                return View();
            }
            else
            {
                string randomString = GenerateRandomString();
                email.SendEmail(users.userEmail, "Verification Code", $"Your Verification Code is {randomString}");
                verificationCode newCode = new verificationCode
                {
                    vCode = randomString
                };

                TempData["userEmail"] = users.userEmail;
                db.vCodes.Add(newCode);
                db.SaveChanges();
                db.users.Add(users);
                db.SaveChanges();
                return RedirectToAction("Verify");
            }
        }

        public static string GenerateRandomString(int length = 6)
        {
            const string characters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random random = new Random();
            char[] randomString = new char[length];

            for (int i = 0; i < length; i++)
            {
                randomString[i] = characters[random.Next(characters.Length)];
            }

            return new string(randomString);
        }

        public IActionResult Verify()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Verify(verificationCode v)
        {
            var check = db.vCodes.Where(a => a.vCode == v.vCode).FirstOrDefault();
            if (check != null)
            {
                return RedirectToAction("Login");
            }
            ViewBag.err = "Invalid Code!";
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(user users)
        {
            var user = db.users.Where(db => db.userEmail == users.userEmail && db.userPass == users.userPass).FirstOrDefault();
            if (user != null)
            {
                HttpContext.Session.SetInt32("uId", user.userId);
                HttpContext.Session.SetString("uName", user.userName);
                HttpContext.Session.SetString("uEmail", user.userEmail);
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.err = "Login Failed";
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        
    }
}
