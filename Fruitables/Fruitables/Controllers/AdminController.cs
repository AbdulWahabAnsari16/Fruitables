using Fruitables.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Fruitables.Controllers
{
    public class AdminController : Controller
    {
        private readonly MainDbContextFile db;
        private readonly IWebHostEnvironment webHostEnvironment;

        public AdminController(MainDbContextFile db, IWebHostEnvironment webHostEnvironment)
        {
            this.db = db;
            this.webHostEnvironment = webHostEnvironment;
        }

        // Category Session
        // Index page show
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("id") == null)
            {
                return RedirectToAction("adminLogin");
            }
            var list = db.categories.ToList();
            return View(list);
        }

        // Category Form
        public IActionResult addCategory()
        {
            if (HttpContext.Session.GetInt32("id") != null)
            {
                return View();
            }
            return RedirectToAction("adminLogin");
        }
        // Category insert into database
        [HttpPost]
        public IActionResult addCategory(category category)
        {
            db.categories.Add(category);
            db.SaveChanges();
            //ViewBag.cat = "category inserted";
            return RedirectToAction("viewCategory");
        }

        public IActionResult editCategory(int? catid)
        {
            var data = db.categories.Find(catid);
            return View(data);
        }
        [HttpPost]
        public IActionResult editCategory(category category)
        {
            db.categories.Update(category);
            db.SaveChanges();
            //ViewBag.cat = "category updated";
            return RedirectToAction("viewCategory");
        }

        public IActionResult deleteCategory(int? catid)
        {
            var data = db.categories.Find(catid);
            db.categories.Remove(data);
            db.SaveChanges();
            return RedirectToAction("viewCategory");
        }
        public IActionResult viewCategory()
        {
            var list = db.categories.ToList();
            return View(list);
        }


        // Product Session
        // Product Form
        public IActionResult addProduct()
        {
            if (HttpContext.Session.GetInt32("id") != null)
            {
                var categories = db.categories.ToList();
                ViewBag.cate = new SelectList(categories, "catId", "catName");
                return View();
            }
            return RedirectToAction("adminLogin");
        }

        [HttpPost]
        public IActionResult addProduct(product p)
        {
            // Ensure we have an uploaded file
            if (p.ProductImage != null)
            {
                // Generate the upload folder path (physical path inside wwwroot/Product/Images)
                string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Product", "Images");

                // Ensure directory exists
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                // Get the file extension and validate
                string extension = Path.GetExtension(p.ProductImage.FileName).ToLower();
                var allowedExtensions = new[] { ".jpg", ".png", ".jfif", ".webp", ".mp4" };

                if (!allowedExtensions.Contains(extension))
                {
                    TempData["extension_error"] = "File Extension Not Valid";
                    return View();
                }

                // Check file size (example: <= 1GB)
                if (p.ProductImage.Length > 1048576000)
                {
                    TempData["error"] = "File Size is too large. Max allowed is ~1GB.";
                    return View();
                }

                // Create unique filename
                string filename = Guid.NewGuid().ToString() + "_" + Path.GetFileName(p.ProductImage.FileName);
                string filePath = Path.Combine(uploadFolder, filename);

                // Save the file
                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    p.ProductImage.CopyTo(fs);
                }

                // Save relative path to DB (use forward slashes for URL compatibility)
                string relativePath = $"Product/Images/{filename}";

                // Now create the product object
                product prod = new product
                {
                    prodName = p.prodName,
                    prodPrice = p.prodPrice,
                    prodDesc = p.prodDesc,
                    prodQty = p.prodQty,
                    PImage = relativePath,
                    CategoryId = p.CategoryId
                };

                // Save to DB
                db.products.Add(prod);
                db.SaveChanges();

                // Redirect to list page
                return RedirectToAction("viewProduct");
            }

            // If no image uploaded, stay on form
            return View();
        }

        public IActionResult editProduct(int id)
        {
            var data = db.products.Find(id);
            var categories = db.categories.ToList();
            ViewBag.cate = new SelectList(categories, "catId", "catName");
            return View(data);
        }

        [HttpPost]
        public IActionResult editProduct(int id, product p)
        {
            var existingProduct = db.products.FirstOrDefault(pr => pr.prodId == id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            var categories = db.categories.ToList();
            ViewBag.cate = new SelectList(categories, "catId", "catName");

            // Handle image upload like addProduct
            if (p.ProductImage != null)
            {
                string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Product", "Images");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string extension = Path.GetExtension(p.ProductImage.FileName).ToLower();
                var allowedExtensions = new[] { ".jpg", ".png", ".jfif", ".webp", ".mp4" };

                if (!allowedExtensions.Contains(extension))
                {
                    TempData["extension_error"] = "File Extension Not Valid";
                    return View(existingProduct);
                }

                if (p.ProductImage.Length > 1048576000)
                {
                    TempData["error"] = "File Size is too large. Max allowed is ~1GB.";
                    return View(existingProduct);
                }

                // Delete old image if exists
                if (!string.IsNullOrEmpty(existingProduct.PImage))
                {
                    string oldFilePath = Path.Combine(webHostEnvironment.WebRootPath, existingProduct.PImage.Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Save new file
                string filename = Guid.NewGuid().ToString() + "_" + Path.GetFileName(p.ProductImage.FileName);
                string filePath = Path.Combine(uploadFolder, filename);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    p.ProductImage.CopyTo(fs);
                }

                existingProduct.PImage = $"Product/Images/{filename}";
            }

            // Update other fields
            existingProduct.prodName = p.prodName;
            existingProduct.prodPrice = p.prodPrice;
            existingProduct.prodDesc = p.prodDesc;
            existingProduct.prodQty = p.prodQty;
            existingProduct.CategoryId = p.CategoryId;

            // Use update
            db.products.Update(existingProduct);
            db.SaveChanges();

            return RedirectToAction("viewProduct");
        }

        public IActionResult deleteProduct(int? prodid)
        {
            var data = db.products.Find(prodid);
            db.products.Remove(data);
            db.SaveChanges();
            return RedirectToAction("viewProduct");
        }

        public IActionResult viewProduct()
        {
            var list = db.products.Include(cat => cat.category).ToList();
            return View(list);
        }      

        public IActionResult addCoupon()
        {
            if (HttpContext.Session.GetInt32("id") != null)
            {
                return View();
            }
            return RedirectToAction("adminLogin");
        }

        [HttpPost]
        public IActionResult addCoupon(coupon code)
        {
            db.coupons.Add(code);
            db.SaveChanges();
            ViewBag.coupon = "coupon inserted";
            return View();
        }


        public IActionResult adminLogin()
        {
            if (HttpContext.Session.GetInt32("id") != null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpPost]
        public IActionResult adminLogin(adminLogin adminLogin)
        {
            var admin = db.adminLogins.Where(db => db.adminEmail == adminLogin.adminEmail && db.adminPass == adminLogin.adminPass).FirstOrDefault();
            if(admin != null)
            {
                HttpContext.Session.SetInt32("id", admin.adminId);
                HttpContext.Session.SetString("name", admin.adminName);
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.err = "Login Failed";
            }
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("adminLogin");
        }
    }
}
