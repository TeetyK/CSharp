using Microsoft.AspNetCore.Mvc;
using ShopMusicProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
namespace ShopMusicProject.Controllers
{
    public class ProductController : Controller
    {
        public readonly ShopCombusContext _db;
        public ProductController(ShopCombusContext db)
        {
            _db = db;
        }
        public IActionResult Index(int page = 1)
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index","Home");
            }
            int pageSize = 8;
            var pd = from p in _db.Products
                     select p;
            if (pd == null)
            {
                return NotFound();
            }
            var pageShow = pd.Skip((page -1) *pageSize ).Take(pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)pd.Count() / pageSize);
            return View(pageShow);
        }
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            string theid;
            int rowCount;
            int i = 0;
            CultureInfo us = new CultureInfo("en-US");
            do
            {
                i++;
                theid = string.Concat("PD", i.ToString("000"));
                var pdida = from b in _db.Products
                             where b.Id.Equals(theid)
                             select b;
                rowCount = pdida.ToList().Count;
            } while (rowCount != 0);
            ViewBag.Pd = theid;
            
            ViewData["Category"] = new SelectList(_db.Categorys, "Id", "Ncategory");
            ViewData["Brand"] = new SelectList(_db.Brands, "BrandId", "BrandName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product obj)
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Products.Add(obj);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
            ViewBag.ErrorMessage = "การบันทึกผิดพลาด";
            return View(obj);
        }
        public IActionResult Edit(string pdid)
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (pdid == null)
            {
                ViewBag.ErrorMessage = "ระบุ id";
                return RedirectToAction("Index");
            }
            var obj = _db.Products.Find(pdid);
            if (obj == null)
            {
                ViewBag.ErrorMessage = "ไม่พบข้อมูล";
                return RedirectToAction("Index");
            }
           
            var fileName = pdid.ToString() + ".jpg";
            var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\image");
            var filePath = Path.Combine(imgPath, fileName);

            if (System.IO.File.Exists(filePath))
            {
                ViewBag.ImgFile = "/image/" + pdid + ".jpg";
            }
            else
            {
                ViewBag.ImgFile = "/image/empty.jpg";
            }
            ViewData["Category"] = new SelectList(_db.Categorys, "Id", "Ncategory");
            ViewData["Brand"] = new SelectList(_db.Brands, "BrandId", "BrandName");
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product obj)
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Products.Update(obj);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(obj);
            }
            ViewBag.ErrorMessage = "การแก้ไขผิดพลาด";
            ViewData["Category"] = new SelectList(_db.Categorys, "Id", "Ncategory",obj.Catid);
            ViewData["Brand"] = new SelectList(_db.Brands, "BrandId", "BrandName",obj.Brandid);

            return View(obj);
        }
        public IActionResult Delete(string pdid)
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (pdid == null)
            {
                ViewBag.ErrorMessage = "ระบุ id";
                return RedirectToAction("Index");
            }
            var obj = _db.Products.Find(pdid);
            if (obj == null)
            {
                ViewBag.ErrorMessage = "ไม่พบข้อมูล";
                return RedirectToAction("Index");
            }
            ViewBag.categoryName = _db.Categorys.FirstOrDefault(pt => pt.Id == obj.Catid).Ncategory;
            ViewBag.brandName = _db.Brands.FirstOrDefault(br => br.BrandId == obj.Brandid).BrandName;

            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(string id)
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                var obj = _db.Products.Find(id);
                if (obj == null)
                {
                    ViewBag.ErrorMessage = "ไม่พบข้อมูล";
                    return RedirectToAction("Index");
                }
                _db.Products.Remove(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction("Index");
            }

        }
        public IActionResult Show(string id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ต้องระบุค่า ID";
                return RedirectToAction("Index");
            }
            var obj = _db.Customers.Find(id);
            if (obj == null)
            {
                TempData["ErrorMessage"] = "ไม่พบข้อมูลที่ระบุ";
                return RedirectToAction("Index");
            }
            var fileName = id.ToString() + ".jpg";
            var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\image");
            var filePath = Path.Combine(imgPath, fileName);

            if (System.IO.File.Exists(filePath))
            {
                ViewBag.ImgFile = "/image/" + id + ".jpg";
            }
            else
            {
                ViewBag.ImgFile = "/image/login.png";
            }

            return View(obj);
        }
       
        public IActionResult ImgUploadC(IFormFile imgfiles, string theid)
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            var FileName = theid;
            var FileExtension = Path.GetExtension(imgfiles.FileName);
            var SaveFileName = FileName + FileExtension;

            var SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\image");
            var SaveFilePath = Path.Combine(SavePath, SaveFileName);
            using (FileStream fs = System.IO.File.Create(SaveFilePath))
            {
                imgfiles.CopyTo(fs);
                fs.Flush();
            }
            return RedirectToAction("Create", new { pdid = theid });
        }
        public IActionResult ImgDeleteC(string id)
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            var fileName = id.ToString() + ".jpg";
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\image");
            var filePath = Path.Combine(imagePath, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            return RedirectToAction("Create", new { pdid = id });
        }
      
        public IActionResult ImgUploadE(IFormFile imgfiles, string theid)
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            var FileName = theid;
                var FileExtension = Path.GetExtension(imgfiles.FileName);
                var SaveFileName = FileName + FileExtension;

                var SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\image");
                var SaveFilePath = Path.Combine(SavePath, SaveFileName);
                using (FileStream fs = System.IO.File.Create(SaveFilePath))
                {
                    ViewBag.ImageUrl = "wwwroot\\image\\ " + FileName;
                    imgfiles.CopyTo(fs);
                    fs.Flush();
                }
            
            return RedirectToAction("Edit", new { pdid = theid });
        }
        public IActionResult ImgDeleteE(string id)
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            var fileName = id.ToString() + ".jpg";
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\image");
            var filePath = Path.Combine(imagePath, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            ViewBag.ImageUrl = null;
            return RedirectToAction("Edit", new { pdid = id });
        }
    }
}
