using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using ShopMusicProject.Models;
using ShopMusicProject.ViewModels;
using System;
using System.Globalization;

namespace ShopMusicProject.Controllers
{
    public class EmployeeController : Controller
    {
        public readonly ShopCombusContext _db;
        public EmployeeController(ShopCombusContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("StfId") == null)
            {
                return RedirectToAction("Login");
            }
            return RedirectToAction("DashBoard");
        }
      /*  public IActionResult DashBoard()
        {
            //ยอดสินค้า ขายดี จำนวน , ยอดสินค้า ขายดี ราคา
            // ยอดขาย
            var rep1 = _db.Carts.Select(s => new { Date = s.CdateAt, Amount = s.Cqty }).ToList();
            //ยอดซื้อ
            var rep2 = _db.Buyings.Select(s => new { Date = s.BuyDate, Amount = s.BuyQty }).ToList();
            //ยอดขายดีด้านราคา
            var rep3 = from cd in _db.CartDtls

                       join p in _db.Products on cd.PdId equals p.Id into join_cd_p
                       from cd_p in join_cd_p.DefaultIfEmpty()

                       join c in _db.Carts on cd.CartId equals c.CartId into join_cd
                       from c_cd in join_cd.DefaultIfEmpty()
                    //   join cu in _db.Customers on c_cd.CusId equals cu.CusId into join_cu_c
                      // from cu_c in join_cu_c.DefaultIfEmpty()
                       where c_cd.Cf.Equals("Y")
                       orderby c_cd.Cmoney
                       select cd_p.Name;
            //ยอดขายดีด้านจำนวน
            var rep4 = from cd in _db.CartDtls

                       join p in _db.Products on cd.PdId equals p.Id into join_cd_p
                       from cd_p in join_cd_p.DefaultIfEmpty()

                       join c in _db.Carts on cd.CartId equals c.CartId into join_cd
                       from c_cd in join_cd.DefaultIfEmpty()
                      // join cu in _db.Customers on c_cd.CusId equals cu.CusId into join_cu_c
                       //from cu_c in join_cu_c.DefaultIfEmpty()
                       where c_cd.Cf.Equals("Y")
                       orderby c_cd.Cqty
                       select cd_p.Name;
            //
            ViewBag.TopQuantitySales = rep4.Take(5);
            ViewBag.TopPriceSales = rep3.Take(5);
            // ViewBag.ChartData = ;
            //var chartData = sales.Select(s => new { Date = s.Date.ToShortDateString(), Amount = s.Amount }).ToList();
            return View();
        }*/
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string userName, string userPass , Work work_add)
        {
            //Query หาว่ามี Login Password ที่ระบุหรือไม่
            var stf = from s in _db.Employees
                      where s.Username.Equals(userName)
                          && s.Password.Equals(userPass)
                      select s;
            //ถ้าข้อมูลเท่ากับ 0 ให้บอกว่าหาข้อมูลไม่พบ
            if (stf.Count() == 0)
            {
                TempData["ErrorMessage"] = "ระบุผู้ใช้หรือรหัสผ่านไม่ถูกต้อง";
                return RedirectToAction("Login","Employee");
            }
            else
            {

                foreach (var item in stf)
                {
                    var check_new = from c in _db.Works
                                    where c.WorkDate.Equals(DateOnly.FromDateTime(DateTime.Now)) && c.EmId.Equals(item.EmId)
                                    select c;
                    if (check_new.Count() == 0)
                    {
                        work_add.EmId = item.EmId;
                        work_add.WorkDate = DateOnly.FromDateTime(DateTime.Now);
                        work_add.WorkIn = TimeOnly.FromDateTime(DateTime.Now);
                        _db.Works.Add(work_add);
                        _db.SaveChanges();
                    }
                    
                }
                 
            }
            //ถ้าหาข้อมูลพบ ให้เก็บค่าเข้า Session
            string StfId;
            string StfName;
            string DutyId;
            foreach (var item in stf)
            {
                //อ่านค่าจาก Object เข้าตัวแปร
                StfId = item.EmId;
                StfName = item.Fname + " " +item.Lname;
                DutyId = item.Type;
                //เอาค่าจากตัวแปรเข้า Session
                HttpContext.Session.SetString("StfId", StfId);
                HttpContext.Session.SetString("StfName", StfName);
                HttpContext.Session.SetString("DutyId", DutyId);
            }

            //ทำการย้ายไปหน้าที่ต้องการ
            return RedirectToAction("WorkForEmployee","report");

        }
        
        public IActionResult List() {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            var staff = from p in _db.Employees
                        where p.Type == "Manager" || p.Type == "User"
                        select p;
            if (staff.Count() == 0)
            {
                return NotFound();
            }
            return View(staff);
         }
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (HttpContext.Session.GetString("DutyId") != "Admin")
             {
                 TempData["ErrorMessage"] = "ไม่มีสิทธิใช้งาน";
                 return RedirectToAction("List", "Employee");
             }
            var em = from p in _db.Dutys
                     where p.DutyId != "Admin"
                     select p;
            string theid;
            int rowCount;
            int i = 0;
            CultureInfo us = new CultureInfo("en-US");
            do
            {
                i++;
                theid = string.Concat("E", i.ToString("000"));
                var pdida = from b in _db.Employees
                            where b.EmId.Equals(theid)
                            select b;
                rowCount = pdida.ToList().Count;
            } while (rowCount != 0);
            ViewBag.EM = theid;
            //ViewData["EmType"] = new SelectList(_db.Dutys.Where(a => a.DutyId != "Admin").ToList(), "DutyId", "DutyName");
            ViewData["EmType"] = new SelectList(em, "DutyId", "DutyName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Employee obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Employees.Add(obj);
                    _db.SaveChanges();
                    return RedirectToAction("List", "Employee");
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
        public IActionResult Edit(string emid)
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (emid == null)
            {
                ViewBag.ErrorMessage = "ระบุ id";
                return RedirectToAction("Index");
            }
            var obj = _db.Employees.Find(emid);
            if (obj == null)
            {
                ViewBag.ErrorMessage = "ไม่พบข้อมูล";
                return RedirectToAction("List", "Employee");
            }
            var em = from p in _db.Dutys
                     where p.DutyId != "Admin"
                     select p;
            //ViewData["EmType"] = new SelectList(_db.Dutys.Where(a => a.DutyId != "Admin").ToList(), "DutyId", "DutyName");
            ViewData["EmType"] = new SelectList(em, "DutyId", "DutyName");

            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Employee obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Employees.Update(obj);
                    _db.SaveChanges();
                    return RedirectToAction("List", "Employee");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(obj);
            }
            ViewBag.ErrorMessage = "การแก้ไขผิดพลาด";
            var em = from p in _db.Dutys
                     where p.DutyId != "Admin"
                     select p;
            //ViewData["EmType"] = new SelectList(_db.Dutys.Where(a => a.DutyId != "Admin").ToList(), "DutyId", "DutyName");
            ViewData["EmType"] = new SelectList(em, "DutyId", "DutyName",obj.Type);

            return View(obj);
        }
        public IActionResult Delete(string emid)
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (emid == null)
            {
                ViewBag.ErrorMessage = "ระบุ id";
                return RedirectToAction("List", "Employee");
            }
            var obj = _db.Employees.Find(emid);
            if (obj == null)
            {
                ViewBag.ErrorMessage = "ไม่พบข้อมูล";
                return RedirectToAction("List", "Employee");
            }
         
            ViewBag.EmType = _db.Dutys.FirstOrDefault(et => et.DutyId == obj.Type).DutyName;
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(string Emid)
        {
            try
            {
                var obj = _db.Employees.Find(Emid);
                if (obj == null)
                {
                    ViewBag.ErrorMessage = "ไม่พบข้อมูล";
                    return RedirectToAction("List", "Employee");
                }
                _db.Employees.Remove(obj);
                _db.SaveChanges();
                return RedirectToAction("List", "Employee");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction("List", "Employee");
            }
        }
        public IActionResult Logout()
        {
            string work_out = HttpContext.Session.GetString("StfId");
            string checkType = HttpContext.Session.GetString("Duty");
            DateOnly checkDate = DateOnly.FromDateTime(DateTime.Now);
            /*var check_new = from c in _db.Works
                            where c.WorkDate.Equals(DateOnly.FromDateTime(DateTime.Now)) && c.EmId.Equals(work_out)
                            select c;*/
            if (checkType != "Admin")
            {
                var theRecord = _db.Works.FirstOrDefault(w => w.WorkDate == checkDate  && w.EmId == work_out);
                //var theRecord = _db.Works.Find(check_new);

                //theRecord.WorkOut = TimeOnly.FromDateTime(DateTime.Now);
                if (theRecord != null)
                {
                    // ตรวจสอบ WorkOut ก่อนที่จะกำหนดค่า
                    if (theRecord.WorkOut == null)
                    {
                        // สร้าง TimeOnly และกำหนดค่า WorkOut
                        theRecord.WorkOut = TimeOnly.FromDateTime(DateTime.Now);
                        _db.Entry(theRecord).State = EntityState.Modified;
                    }
                }
            }
               
            _db.SaveChanges();
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
      
    }
}
