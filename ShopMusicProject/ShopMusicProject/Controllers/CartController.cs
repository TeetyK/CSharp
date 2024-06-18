using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopMusicProject.Models;
using ShopMusicProject.ViewModels;
using System.Dynamic;
using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace ShopMusicProject.Controllers
{
    public class CartController : Controller
    {
        private readonly ShopCombusContext _db;
        public CartController(ShopCombusContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AddDtl(string id)
        {
            if (HttpContext.Session.GetString("CusId") == null)
            {
                TempData["ErrorMessage"] = "Login ก่อนซื้อสินค้า";
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                TempData["ErrorMessage"] = "ต้องระบุรหัสสินค้า";
                return RedirectToAction("Index", "Home");
            }
            if (HttpContext.Session.GetString("CartId") == null)
            {
                return RedirectToAction("Add", new { id = id });
            }
           
            var pd = _db.Products.Find(id);
            string CartId = HttpContext.Session.GetString("CartId");
            var cdtl = from cd in _db.CartDtls
                       where cd.CartId.Equals(CartId)
                       && cd.PdId.Equals(id)
                       select cd;
            if (cdtl.Count() == 0)
            {
                CartDtl obj = new CartDtl();
                obj.CartId = CartId;
                obj.PdId = id;
               // obj.CdtlQty = 1 * (change_qty);
                obj.CdtlQty = 1;
                obj.CdtlPrice = pd.Price;
                obj.CdtlMoney = pd.Price * 1;
                _db.Entry(obj).State = EntityState.Added;
            }
            else
            {
                foreach (var obj in cdtl)
                {
                    // obj.CdtlQty = obj.CdtlQty + (change_qty);
                    obj.CdtlQty = obj.CdtlQty + 1;
                    obj.CdtlMoney = pd.Price * (obj.CdtlQty);
                    _db.Entry(obj).State = EntityState.Modified;
                }
            }
           // ViewBag.Test = quantity;
            _db.SaveChanges();
            var CartMoney = _db.CartDtls.Where(a => a.CartId == CartId).Sum(a => a.CdtlMoney);
            var CartQty = _db.CartDtls.Where(a => a.CartId == CartId).Sum(a => a.CdtlQty);
            var cart = _db.Carts.Find(CartId);
            _db.SaveChanges();
            HttpContext.Session.SetString("CartQty", CartQty.ToString());
            HttpContext.Session.SetString("CartMoney", CartMoney.ToString());
            return RedirectToAction("Shop", "Home");
            //return View(pdid);
        }
        public IActionResult Add(string id)
        {
            string theId;
            int rowCount = 0;
            int i = 0;
            string today;
            string CusId = HttpContext.Session.GetString("CusId");

            CultureInfo us = new CultureInfo("en-US");
            do
            {
                i++;
                today = DateTime.Now.ToString("'CT'yyyyMMdd");
                theId = string.Concat(today, i.ToString("0000"));
                var cart = from ct in _db.Carts
                           where ct.CartId.Equals(theId)
                           select ct;
                rowCount = cart.Count();
            } while (rowCount != 0);

            try
            {
                Cart obj = new Cart();

                obj.CartId = theId;
                obj.CusId = CusId;
                obj.CdateAt = DateOnly.FromDateTime(DateTime.Now.Date);
                obj.Cqty = 0;
                obj.Cmoney = 0;

                _db.Entry(obj).State = EntityState.Added;
                _db.SaveChanges();

                HttpContext.Session.SetString("CartId", theId);
                HttpContext.Session.SetString("CartQty", "0");
                HttpContext.Session.SetString("CartMoney", "0");

                return RedirectToAction("AddDtl", new { id = id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "การบันทึกผิดพลาด";
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult Show(string cartid)
        {
            string cusid = HttpContext.Session.GetString("CusId");
            var cart = from ct in _db.Carts
                       where ct.CartId == cartid &&
                           ct.CusId == cusid
                       select ct;
            if (cart == null)
            {
                TempData["ErrorMessage"] = "ไม่พบตะกร้าที่ระบุ";
                return RedirectToAction("Index", "Home");
            }
            //Detail เลือกข้อมูลของตะกร้า+สร้าง ViewModel CtdVM แสดงชื่อสินค้า
            var cartdtl = from ctd in _db.CartDtls
                          join p in _db.Products on ctd.PdId equals p.Id
                              into join_ctd_p
                          from ctd_p in join_ctd_p.DefaultIfEmpty()
                          where ctd.CartId == cartid
                          select new CartVM
                          {
                              CartId = ctd.CartId,
                              PdId = ctd.PdId,
                              PdName = ctd_p.Name,
                              CdtlMoney = ctd.CdtlMoney,
                              CdtlPrice = ctd.CdtlPrice,
                              CdtlQty = ctd.CdtlQty
                          };
            //สร้าง Dynamic model เพื่อส่งข้อมูลให้ View เป็นสองตารางพร้อมกัน
            dynamic DyModel = new ExpandoObject();
            //ระบุส่วน Master รับข้อมูลจาก Obj cart
            DyModel.Master = cart;
            //ระบุส่วน Detail รับข้อมูลจาก Obj cartdtl
            //set ใหม่
            var cartmoney = _db.CartDtls.Where(a => a.CartId == cartid).Sum(a => a.CdtlMoney);
            var cartqty = _db.CartDtls.Where(a => a.CartId == cartid).Sum(a => a.CdtlQty);
            var cart1 = _db.Carts.Find(cartid);
            cart1.Cqty = cartqty;
            cart1.Cmoney = cartmoney;
            _db.SaveChanges();
            HttpContext.Session.SetString("CartMoney", cartmoney.ToString());
            HttpContext.Session.SetString("CartQty", cartqty.ToString());
            DyModel.Detail = cartdtl;
            //ส่ง Dynamic Model ไปที่ View
            return View(DyModel);
        }
        public IActionResult Check()
        {
            // ตรวจสอบตะกร้า ของลูกค้าปัจจุบีน ที่ยังไม่ได้ทำการ CF - ถ้ามีค้างให้ใช้ CartId นั้น
            string cusid = HttpContext.Session.GetString("CusId");
            var cart = from ct in _db.Carts.Take(1)
                       where ct.CusId.Equals(cusid) && ct.Cf != "Y"
                       select ct;
            int rowCount = cart.Count();
            // ถ้ามีตะกร้าค้าง
            if (rowCount > 0)
            {
                Cart obj = new Cart();
                foreach (var item in cart)
                {
                    obj = item;
                }
                //กำหนด Session ต่างๆของตะกร้า
                HttpContext.Session.SetString("CartId", obj.CartId);
                HttpContext.Session.SetString("CartQty", obj.Cqty.ToString());
                HttpContext.Session.SetString("CartMoney", obj.Cmoney.ToString());
            }
            return RedirectToAction("Shop", "Home");
        }
        public IActionResult Delete(string cartid)
        {
            //การลบตะกร้า คือลบทั้งเอกสาร ดั้งนั้นต้องลบตัวMaster และ Detail ด้วย
            //ลบส่วน Detail
            //เลือกรายการที่อยู่ในตะกร้า
            var detail = from ctd in _db.CartDtls
                         where ctd.CartId.Equals(cartid)
                         select ctd;
            //วน Loop ไล่ลบที่ละรายการ
            foreach (var item in detail)
            {
                _db.CartDtls.Remove(item);
            }
            _db.SaveChanges();
            //ลบส่วน Master
            //หาเอกสารที่ระบุ
            var master = _db.Carts.Find(cartid);
            if (master == null)
            {
                TempData["ErrorMessage"] = "ไม่พบตะกร้า";
                return RedirectToAction("Show", "Cart", new { cartid = cartid });
            }
            _db.Carts.Remove(master);
            _db.SaveChanges();

            //ลบตะกร้าแล้ว ลบ Session ด้วย
            HttpContext.Session.Remove("CartId");
            HttpContext.Session.Remove("CartQty");
            HttpContext.Session.Remove("CartMoney");

            TempData["SuccessMessage"] = "ยกเลิกคำสั่งซื้อแล้ว";
            return RedirectToAction("Shop", "Home");
        }
        public IActionResult DeleteDtl(string pdid, string cartid)
        {
            //ลบ Detail แต่ไม่ต้องวน Loop เพราะเลือกสินค้ามารายการเดียว
            var obj = _db.CartDtls.Find(cartid, pdid);
            if (obj == null)
            {
                TempData["ErrorMessage"] = "ไม่พบข้อมูล";
                return RedirectToAction("Show", "Cart", new { cartid = cartid });
            }
            _db.CartDtls.Remove(obj);
            _db.SaveChanges();

            //เมื่อ Detail เปลี่ยน ทำการปรับยอดของ Master
            //เหมือนกับ AddDtl
            var cartmoney = _db.CartDtls.Where(a => a.CartId == cartid).Sum(a => a.CdtlMoney);
            var cartqty = _db.CartDtls.Where(a => a.CartId == cartid).Sum(a => a.CdtlQty);

            //ถ้าจำนวนสินค้าเป็น 0 ก็ลบตะกร้าทิ้ง
            if (cartqty == 0)
            {
                //ลบ Master
                var master = _db.Carts.Find(cartid);
                _db.Carts.Remove(master);
                _db.SaveChanges();

                //ลบตะกร้าแล้ว ลบSession ด้วย
                HttpContext.Session.Remove("CartId");
                HttpContext.Session.Remove("CartQty");
                HttpContext.Session.Remove("CartMoney");

                TempData["SuccessMessage"] = "ยกเลิกคำสั่งซื้อแล้ว";
                return RedirectToAction("Shop", "Home");
            }
            else
            {
                //Update Cart
                var cart = _db.Carts.Find(cartid);
                cart.Cqty = cartqty;
                cart.Cmoney = cartmoney;
                _db.SaveChanges();

                //Update Session
                HttpContext.Session.SetString("CartMoney", cartmoney.ToString());
                HttpContext.Session.SetString("CartQty", cartqty.ToString());

                return RedirectToAction("Show", "Cart", new { cartid = cartid });
            }


        }
        public IActionResult Confirm(string cartid)
        {
            //หาสินค้าที่อยู่ใน Detail เพื่อไปตัด Stock สินค้า
            var cartdtl = from ctd in _db.CartDtls
                          where ctd.CartId.Equals(cartid)
                          select ctd;
            int rowCount = cartdtl.Count();
            if (rowCount == 0)
            {
                TempData["ErrorMessage"] = "การยืนยันผิดพลาด";
                return RedirectToAction("Show", "Cart", new { cartid = cartid });
            }
            //วน Loop แต่ละสินค้า
            foreach (var detail in cartdtl)
            {
                //โดยปกติ ASP.NET Core จะไม่ให้ Connect DBContext ซ้อนกันต้องปิด Connection ก่อน
                //แก้ไขโดย Set Connection String ใน appsetting.json
                //เพิ่ม MultipleActiveResultSets=True ต่อท้าย
                Product pd = _db.Products.Find(detail.PdId);
                //Update Stock และวันที่ขายล่าสุด
                pd.Stock = pd.Stock - detail.CdtlQty;
                pd.LastsaleAt = DateOnly.FromDateTime(DateTime.Now.Date);
                _db.Entry(pd).State = EntityState.Modified;
            }
            _db.SaveChanges();
            //Update ตะกร้าว่าทำการยืนยันแล้ว
            var master = _db.Carts.Find(cartid);
            master.Cf = "Y";
            _db.Entry(master).State = EntityState.Modified;
            _db.SaveChanges();

            //ลบ Session ตะกร้า
            HttpContext.Session.Remove("CartId");
            HttpContext.Session.Remove("CartQty");
            HttpContext.Session.Remove("CartMoney");

            TempData["SuccessMessage"] = "ยืนยันคำสั่งซื้อแล้ว";
            return RedirectToAction("Shop", "Home");
        }
        public IActionResult List(string cusid)
        {
            if (HttpContext.Session.GetString("CusId") == null)
            {
                TempData["ErrorMessage"] = "Login ก่อนซื้อสินค้า";
                return RedirectToAction("Login", "Home");
            }
            var cart = from c in _db.Carts
                       where c.CusId.Equals(cusid)
                       orderby c.CartId descending
                       select c;
            return View(cart);
        }
    }
}
