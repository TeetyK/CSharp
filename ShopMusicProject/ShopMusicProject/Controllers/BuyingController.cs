using ShopMusicProject.Models;
using Microsoft.AspNetCore.Mvc;
using ShopMusicProject.ViewModels;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Windows.Markup;
using System.Dynamic;
using System;
using System.Runtime.Intrinsics.Arm;
using System.Collections.Generic;
using System.Xml.Linq;
namespace ShopMusicProject.Controllers
{
    public class BuyingController : Controller
    {
        public readonly ShopCombusContext _db;
        public BuyingController(ShopCombusContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (HttpContext.Session.GetString("StfId")==null)
            {
                TempData["ErrorMessage"] = "คุณไม่มีสิทธิใช้งาน";
                return RedirectToAction("Index","Home");
            }
            var Sfname = HttpContext.Session.GetString("StfName");
            DateOnly thedate = DateOnly.FromDateTime(DateTime.Now.Date);
            var pd = from b in _db.Buyings
                     join s in _db.Suppliers on b.SupId equals s.Sid into join_b_s
                     from b_s in join_b_s.DefaultIfEmpty()
                     where b.BuyDate == thedate
                     select new BuyVM
                     {
                         BuyId = b.BuyId,
                         SupId = b.SupId,
                         SupName = b_s.SupName,
                         BuyDate = b.BuyDate,
                         StfId = b.StfId,
                         BuyDocId = b.BuyDocId,
                         BuyQty = b.BuyQty,
                         BuyMoney = b.BuyMoney,
                         BuyRemark = b.BuyRemark,
                         Saleman = Sfname
                     };

            return View(pd);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(DateOnly thedate)
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (HttpContext.Session.GetString("StfId") == null)
            {
                TempData["ErrorMessage"] = "คุณไม่มีสิทธิใช้งาน";
                return RedirectToAction("Index", "Home");
            }
            var Sfname = HttpContext.Session.GetString("StfName");
            var pd = from b in _db.Buyings
                     join s in _db.Suppliers on b.SupId equals s.Sid into join_b_s
                     from b_s in join_b_s.DefaultIfEmpty()

                     where b.BuyDate == thedate
                     select new BuyVM
                     {
                         BuyId = b.BuyId,
                         SupId = b.SupId,
                         SupName = b_s.SupName,
                         BuyDate = b.BuyDate,
                         StfId = b.StfId,
                         BuyDocId = b.BuyDocId,
                         BuyQty = b.BuyQty,
                         BuyMoney = b.BuyMoney,
                         BuyRemark = b.BuyRemark,
                         Saleman = Sfname
                     };

            return View(pd);
        }
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("StfId") == null)
            {
                TempData["ErrorMessage"] = "คุณไม่มีสิทธิใช้งาน";
                return RedirectToAction("Index", "Home");
            }
            string theid;
            int rowCount;
            int i = 0;
            CultureInfo us = new CultureInfo("en-US");
            do
            {
                i++;
                theid = string.Concat(DateTime.Now.ToString("'Buy'yyyyMMdd", us), i.ToString("00"));
                var buying = from b in _db.Buyings
                             where b.BuyId.Equals(theid)
                             select b;
                rowCount = buying.ToList().Count;
            } while (rowCount != 0);
            ViewBag.BuyId =theid;
            ViewBag.SupName = new SelectList(_db.Suppliers,"Sid","SupName");
            ViewBag.EM = HttpContext.Session.GetString("StfId");
            ViewBag.Salesman = HttpContext.Session.GetString("StfName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Buying obj)
        {
            if (HttpContext.Session.GetString("StfId") == null)
            {
                TempData["ErrorMessage"] = "คุณไม่มีสิทธิใช้งาน";
                return RedirectToAction("Index", "Home");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Buyings.Add(obj);
                    _db.SaveChanges();
                    return RedirectToAction("Show", "Buying", new { buyid = obj.BuyId });
                }

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "การบันทึกผิดพลาด";
                return View(obj);
            }
            TempData["ErrorMessage"] = "การบันทึกผิดพลาด";
            return View(obj);
        }
        public IActionResult Show(string buyid)
        {
            if (HttpContext.Session.GetString("StfId") == null)
            {
                TempData["ErrorMessage"] = "คุณไม่มีสิทธิใช้งาน";
                return RedirectToAction("Index", "Home");
            }
            if (buyid == null)
            {
                TempData["ErrorMessage"] = "ต้องระบุเลขที่";
                return RedirectToAction("Index");
            }
            var buying = from b in _db.Buyings
                         where b.BuyId == buyid
                         select b;
            if(buying.ToList().Count == 0)
            {
                TempData["ErrorMessage"] = "ไม่พบเอกสาร";
                return RedirectToAction("Index");
            }
            foreach(var s in buying)
            {
                    ViewBag.SupName = _db.Suppliers.FirstOrDefault(sp => (sp.Sid == s.SupId)).SupName;
            }

            var buydtl = from bd in _db.BuyDtls
                         join p in _db.Products on bd.PdId equals p.Id into join_bd_p
                         from bd_p in join_bd_p.DefaultIfEmpty()
                         where bd.BuyId == buyid
                         select new BdVM
                         {
                             BuyId = bd.BuyId,
                             PdId = bd.PdId,
                             PdName = bd_p.Name,
                             BdtlMoney = bd.BdtlMoney,
                             BdtlPrice = bd.BdtlPrice,
                             BdtlQty = bd.BdtlQty,
                         };
            ViewBag.theid = buyid;

            dynamic DyModel = new ExpandoObject();
            DyModel.Master = buying;
            DyModel.Detail = buydtl;
            return View(DyModel);
        }
        public IActionResult Edit(string buyid)
        {
            if (HttpContext.Session.GetString("StfId") == null)
            {
                TempData["ErrorMessage"] = "คุณไม่มีสิทธิใช้งาน";
                return RedirectToAction("Index", "Home");
            }
            if (buyid == null)
            {
                TempData["ErrorMessage"] = "ต้องระบุเลขที่";
                return RedirectToAction("Index");
            }

            var obj = _db.Buyings.Find(buyid);
            if (obj == null)
            {
                TempData["ErrorMessage"] = "ไม่พบเอกสาร";
                return RedirectToAction("Index");
            }
            ViewBag.SupId = new SelectList(_db.Suppliers, "Sid", "SupName", obj.SupId);
            ViewBag.Salesman = HttpContext.Session.GetString("StfName");
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Buying obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Buyings.Update(obj);
                    _db.SaveChanges();
                    return RedirectToAction("Show", "Buying", new { buyid = obj.BuyId });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "การแก้ไขผิดพลาด";
                return View(obj);
            }
            TempData["ErrorMessage"] = "การแก้ไขผิดพลาด";
            return View(obj);
        }

        public IActionResult Delete(string buyid)
        {
            if (HttpContext.Session.GetString("StfId") == null)
            {
                TempData["ErrorMessage"] = "คุณไม่มีสิทธิใช้งาน";
                return RedirectToAction("Index", "Home");
            }
            ////ลบ Master
            var master = _db.Buyings.Find(buyid);
            _db.Buyings.Remove(master);
            _db.SaveChanges();

            var detail = from bd in _db.BuyDtls
                         where bd.BuyId == buyid
                         select bd;
            foreach (var item in detail)
            {
                _db.BuyDtls.Remove(item);
                var pd = _db.Products.Find(item.PdId);
                pd.Stock = pd.Stock - item.BdtlQty;

            }
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult CreateDtl(string buyid)
        {
            if (buyid == null)
            {
                TempData["ErrorMessage"] = "ต้องระบุเลขที่";
                return RedirectToAction("Show", new { buyid = buyid });
            }
            ViewBag.pdid = new SelectList(_db.Products, "Id", "Name");
            ViewBag.buyid = buyid;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateDtl(BuyDtl obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.BuyDtls.Add(obj);
                    _db.SaveChanges();
                    ////
                    var buymoney = _db.BuyDtls.Where(a => a.BuyId == obj.BuyId).Sum(b => b.BdtlMoney);
                    var buyqty = _db.BuyDtls.Where(a => a.BuyId == obj.BuyId).Sum(b => b.BdtlQty);

                    var buy = _db.Buyings.Find(obj.BuyId);
                    buy.BuyQty = buyqty;
                    buy.BuyMoney = buymoney;
                    _db.SaveChanges();

                    var pd = _db.Products.Find(obj.PdId);
                    pd.Stock = pd.Stock + obj.BdtlQty;
                    pd.LastbuyAt = buy.BuyDate;
                    pd.Cost = obj.BdtlPrice;

                    _db.SaveChanges();
                    return RedirectToAction("Show", "Buying", new { buyid = obj.BuyId });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "การบันทึกผิดพลาด";
                return RedirectToAction("Show", "Buying", new { buyid = obj.BuyId });

            }
            TempData["ErrorMessage"] = "การบันทึกผิดพลาด";
            return RedirectToAction("Show", "Buying", new { buyid = obj.BuyId });
        }
        public IActionResult DeleteDtl(string pdid, string buyid)
        {
            var obj = _db.BuyDtls.Find(buyid, pdid);

            var pd = _db.Products.Find(obj.PdId);
            pd.Stock = pd.Stock - obj.BdtlQty;

            _db.SaveChanges();

            _db.BuyDtls.Remove(obj);
            _db.SaveChanges();

            var buymoney = _db.BuyDtls.Where(a => a.BuyId == buyid).Sum(b => b.BdtlMoney);
            var buyqty = _db.BuyDtls.Where(a => a.BuyId == buyid).Sum(b => b.BdtlQty);

            var buy = _db.Buyings.Find(buyid);
            buy.BuyQty = buyqty;
            buy.BuyMoney = buymoney;
            _db.SaveChanges();

            return RedirectToAction("Show", "Buying", new { buyid = buyid });
        }
    }
}

