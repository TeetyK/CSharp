using Microsoft.AspNetCore.Mvc;
using ShopMusicProject.Models;
using System.Diagnostics;
using ShopMusicProject.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using System.Dynamic;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace ShopMusicProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ShopCombusContext _db;

        public HomeController(ShopCombusContext db)
        {
            _db = db;
        }      

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string userName, string userPass)
        {
            /*HttpContext.Session.SetString("s_userName", userName);
            return RedirectToAction("Index","Product");*/
            var cus = from c in _db.Customers
                      where c.Username.Equals(userName)
                      && c.Password.Equals(userPass)
                      select c;
            if (cus.ToList().Count() == 0)
            {
                TempData["ErrorMessage"] = "ผู้ใช้หรือรหัสผ่านไม่ถูกต้อง";
                return RedirectToAction("Index");
            }
            string CusId;
            string CusName;
            string CusEmail;
            foreach (var item in cus)
            {
                CusId = item.CusId;
                CusName = item.Fname + " " + item.Lname;
                CusEmail = item.Email;

                HttpContext.Session.SetString("CusId", CusId);
                HttpContext.Session.SetString("CusName", CusName);
                HttpContext.Session.SetString("CusEmail", CusEmail);

                var theRecord = _db.Customers.Find(CusId);

                theRecord.LoginAt = DateOnly.FromDateTime(DateTime.Now);

                _db.Entry(theRecord).State = EntityState.Modified;
            }
            _db.SaveChanges();
            //return RedirectToAction("Check", "Cart");
            return RedirectToAction("Index");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Customer obj)
        {
            
            try
            {
               
                if (ModelState.IsValid)
                {
                    var check_user = from p in _db.Customers
                                     where p.Username.Equals(obj.Username)
                                     select p.Username;
                    var check_row = from p in _db.Customers
                                    select p;
                    
                    if (check_user.Count() == 0)
                    {

                        if (check_row.Count() ==0)
                        {
                            obj.CusId = "C001";

                        }
                        else
                        {
                            //generate id
                            var pull_value = _db.Customers.Select(p => p.CusId).Max();
                            string lex = pull_value.ToString();
                            string test1 = lex.Substring(1, 3);
                            int change1 = int.Parse(test1);
                            change1 += 1001;
                            lex = change1.ToString();
                            string gen = 'C' + lex.Substring(1, 3);
                            //
                            obj.CusId = gen;
                        }
                        obj.CreateAt = DateOnly.FromDateTime(DateTime.Now);
                        _db.Customers.Add(obj);
                        _db.SaveChanges();
                        ViewBag.ErrorMessage = "";
                        // auto
                        var cus = from c in _db.Customers
                                  where c.Username.Equals(obj.Username)
                                  && c.Password.Equals(obj.Password)
                                  select c;
                        if (cus.ToList().Count() == 0)
                        {
                            TempData["ErrorMessage"] = "ผู้ใช้หรือรหัสผ่านไม่ถูกต้อง";
                            return RedirectToAction("Index");
                        }
                        string CusId;
                        string CusName;
                        string CusEmail;
                        foreach (var item in cus)
                        {
                            CusId = item.CusId;
                            CusName = item.Fname + " " + item.Lname;
                            CusEmail = item.Email;

                            HttpContext.Session.SetString("CusId", CusId);
                            HttpContext.Session.SetString("CusName", CusName);
                            HttpContext.Session.SetString("CusEmail", CusEmail);

                            var theRecord = _db.Customers.Find(CusId);

                            theRecord.LoginAt = DateOnly.FromDateTime(DateTime.Now);

                            _db.Entry(theRecord).State = EntityState.Modified;
                        }
                        _db.SaveChanges();
                        return RedirectToAction("Index", "home");
                    }
                    else
                    {
                        ViewBag.check_user = 1;
                        ViewBag.ErrorMessage = "User login มีอยู่แล้ว";
                        return View();
                    }

                }
                else
                {
                    ViewBag.ErrorMessage = "กรุณากรอกให้ครบ";
                }


            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
            ViewBag.ErrorMessage = "การสร้างผิดพลาด";

            return View(obj);
        }
        public IActionResult Shop(int page =1)
        {
            int pageSize = 8;
            /*var pd = from p in _db.Products
                     select p;*/
            var pd = from p in _db.Products
                     join p_t in _db.Categorys on p.Catid equals p_t.Id into join_p_t
                     from p_pt in join_p_t.DefaultIfEmpty()
                     join b in _db.Brands on p.Brandid equals b.BrandId into join_p_b
                     from p_b in join_p_b.DefaultIfEmpty()
                     select new PdVM
                     {
                         Id = p.Id,
                         Name = p.Name,
                         Price = p.Price,
                         Stock = p.Stock,
                         Cost = p.Cost,
                         BrandName = p_b.BrandName,
                         CategoryName = p_pt.Ncategory
                     };
            var pageShow = pd.Skip((page - 1) * pageSize).Take(pageSize);
            ViewBag.CurrentPageShop = page;
            ViewBag.TotalPagesShop = (int)Math.Ceiling((double)pd.Count() / pageSize);
            
            var catgory = _db.Categorys.Distinct().ToList();
            dynamic DyModel = new ExpandoObject();
            DyModel.Product = pageShow;
            DyModel.Category = catgory;
            if (pd == null)
            {
                return NotFound();
            }
            if (catgory == null)
            {
                return NotFound();
            }
         
            return View(DyModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Shop(string? stext,string? categoryText)
        {

            var pd = (stext !=null && categoryText !=null )?(
                 from p in _db.Products
                 join p_t in _db.Categorys on p.Catid equals p_t.Id into join_p_t
                 from p_pt in join_p_t.DefaultIfEmpty()
                 join b in _db.Brands on p.Brandid equals b.BrandId into join_p_b
                 from p_b in join_p_b.DefaultIfEmpty()
                 where (p.Name.Contains(stext) && p_pt.Ncategory.Contains(categoryText))
                      /*  (p_pt.Ncategory.Contains(categoryText)) ||
                        (p_b.BrandName.Contains(stext) && p_pt.Ncategory.Contains(categoryText))*/
                 select new PdVM
                 {
                     Id = p.Id,
                     Name = p.Name,
                     Price = p.Price,
                     Stock = p.Stock,
                     Cost = p.Cost,
                     BrandName = p_b.BrandName,
                     CategoryName = p_pt.Ncategory
                 }
                
                ) :((categoryText != null && stext == null)?(
                from p in _db.Products
                join p_t in _db.Categorys on p.Catid equals p_t.Id into join_p_t
                from p_pt in join_p_t.DefaultIfEmpty()
                join b in _db.Brands on p.Brandid equals b.BrandId into join_p_b
                from p_b in join_p_b.DefaultIfEmpty()
                where (p_pt.Ncategory.Contains(categoryText))
                select new PdVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Stock = p.Stock,
                    Cost = p.Cost,
                    BrandName = p_b.BrandName,
                    CategoryName = p_pt.Ncategory
                }
                ):((categoryText == null && stext != null)?
                (
                from p in _db.Products
                join p_t in _db.Categorys on p.Catid equals p_t.Id into join_p_t
                from p_pt in join_p_t.DefaultIfEmpty()
                join b in _db.Brands on p.Brandid equals b.BrandId into join_p_b
                from p_b in join_p_b.DefaultIfEmpty()
                where (p.Name.Contains(stext) || p_b.BrandName.Contains(stext))
                select new PdVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Stock = p.Stock,
                    Cost = p.Cost,
                    BrandName = p_b.BrandName,
                    CategoryName = p_pt.Ncategory
                }
                ):(
                from p in _db.Products
                join p_t in _db.Categorys on p.Catid equals p_t.Id into join_p_t
                from p_pt in join_p_t.DefaultIfEmpty()
                join b in _db.Brands on p.Brandid equals b.BrandId into join_p_b
                from p_b in join_p_b.DefaultIfEmpty()
                select new PdVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Stock = p.Stock,
                    Cost = p.Cost,
                    BrandName = p_b.BrandName,
                    CategoryName = p_pt.Ncategory
                }
                )
                
                )
                      
        );
              
                int pageSize = 8;
                int page = 1;
                var pageShow = pd.Skip((page - 1) * pageSize).Take(pageSize);
                ViewBag.CurrentPageShop = page;
                ViewBag.TotalPagesShop = (int)Math.Ceiling((double)pd.Count() / pageSize);

                var catgory = _db.Categorys.Distinct().ToList();
                dynamic DyModel = new ExpandoObject();
                DyModel.Product = pageShow;
                DyModel.Category = catgory;
                if (pd == null)
                {
                    return NotFound();
                }
                if (catgory == null)
                {
                    return NotFound();
                }

                return View(DyModel);
        }
       

        public IActionResult ShopDetail(string id)
        {

             var pd = from p in _db.Products
                      join p_t in _db.Categorys on p.Catid equals p_t.Id into join_p_t
                      from p_pt in join_p_t.DefaultIfEmpty()
                      join b in _db.Brands on p.Brandid equals b.BrandId into join_p_b
                      from p_b in join_p_b.DefaultIfEmpty()
                      where p.Id.Contains(id)
                      select new PdVM
                      {
                          Id = p.Id,
                          Name = p.Name,
                          Price = p.Price,
                          Stock = p.Stock,
                          Cost = p.Cost,
                          BrandName = p_b.BrandName,
                          CategoryName = p_pt.Ncategory
                      };
            dynamic DyModel = new ExpandoObject();
            DyModel.ShopDetail = pd;
            if (pd==null)
            {
                return NotFound();
            }
            if (DyModel == null)
            {
                return NotFound();
            }
            return View(DyModel);
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
            var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\imgcus");
            var filePath = Path.Combine(imgPath, fileName);

            if (System.IO.File.Exists(filePath))
            {
                ViewBag.ImgFile = "/imgcus/" + id + ".jpg";
            }
            else
            {
                ViewBag.ImgFile = "/image/login.png";
            }

            return View(obj);
        }
     
        public IActionResult ImgUpload(IFormFile imgfiles, string theid)
        {
            var FileName = theid; 
            var FileExtension = Path.GetExtension(imgfiles.FileName);
            var SaveFileName = FileName + FileExtension;

            var SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\imgcus");
            var SaveFilePath = Path.Combine(SavePath, SaveFileName);
            using (FileStream fs = System.IO.File.Create(SaveFilePath))
            {
                imgfiles.CopyTo(fs);
                fs.Flush();
            }
            return RedirectToAction("Show", new { id = theid });
        }
        public IActionResult ImgDelete(string id)
        {
            var fileName = id.ToString() + ".jpg";
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\imgcus");
            var filePath = Path.Combine(imagePath, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            return RedirectToAction("Show", new { id = id });
        }
    }
}
