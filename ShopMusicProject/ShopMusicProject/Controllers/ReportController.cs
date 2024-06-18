using ShopMusicProject.Models;
using ShopMusicProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Threading.Tasks.Dataflow;

namespace ShopMusicProject.Controllers
{
    public class ReportController : Controller
    {
        public readonly ShopCombusContext _db;
        public ReportController(ShopCombusContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        public IActionResult SaleDaily()
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            DateOnly thedate = DateOnly.FromDateTime(DateTime.Now.Date);
            var rep = from cd in _db.CartDtls

                      join p in _db.Products on cd.PdId equals p.Id into join_cd_p
                      from cd_p in join_cd_p.DefaultIfEmpty()

                      join c in _db.Carts on cd.CartId equals c.CartId into join_cd
                      from c_cd in join_cd
                      where c_cd.CdateAt == thedate
                      group cd by new { cd.PdId, cd_p.Name } into g
                      select new RepSale
                      {
                          PdId = g.Key.PdId,
                          PdName = g.Key.Name,
                          CdtlMoney = g.Sum(x => x.CdtlMoney),
                          CdtlQty = g.Sum(x => x.CdtlQty)
                      };
            ViewBag.theDate = thedate.ToString("yyyy-MM-dd");
            
            return View(rep);
            //return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaleDaily(DateOnly thedate)
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            var rep = from cd in _db.CartDtls

                      join p in _db.Products on cd.PdId equals p.Id into join_cd_p
                      from cd_p in join_cd_p.DefaultIfEmpty()
                      join c in _db.Carts on cd.CartId equals c.CartId into join_cd
                      from c_cd in join_cd
                      where c_cd.CdateAt == thedate

                      group cd by new { cd.PdId, cd_p.Name } into g

                      select new RepSale
                      {
                          PdId = g.Key.PdId,
                          PdName = g.Key.Name,
                          CdtlMoney = g.Sum(x => x.CdtlMoney),
                          CdtlQty = g.Sum(x => x.CdtlQty)
                      };
            ViewBag.theDate = thedate.ToString("yyyy-MM-dd");
            return View(rep);
        }
        public IActionResult SaleMonthly()
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            //กำหนดวันแรก และคำนวณหาวันสุดท้ายของเดือนปัจจุบัน
            var theMonth = DateTime.Now.Month;
            var theYear = DateTime.Now.Year;
            //วันแรกคือวันที่ 1 ของเดือน
            DateOnly sDate = new DateOnly(theYear, theMonth, 1);
            //วันสุดท้ายคือวันที 1 ของเดือนหน้า ลบไป1วัน
            DateOnly eDate = new DateOnly(theYear, theMonth, 1).AddMonths(1).AddDays(-1);

            var rep = from cd in _db.CartDtls

                      join p in _db.Products on cd.PdId equals p.Id into join_cd_p
                      from cd_p in join_cd_p.DefaultIfEmpty()

                      join c in _db.Carts on cd.CartId equals c.CartId into join_cd
                      from c_cd in join_cd
                      where c_cd.CdateAt >= sDate && c_cd.CdateAt <= eDate
                      group cd by new { cd.PdId, cd_p.Name } into g
                      select new RepSale
                      {
                          PdId = g.Key.PdId,
                          PdName = g.Key.Name,
                          CdtlMoney = g.Sum(x => x.CdtlMoney),
                          CdtlQty = g.Sum(x => x.CdtlQty)
                      };
            ViewBag.sDate = sDate.ToString("yyyy-MM-dd");
            ViewBag.eDate = eDate.ToString("yyyy-MM-dd");

            return View(rep);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaleMonthly(DateOnly sDate, DateOnly eDate)
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            var rep = from cd in _db.CartDtls

                      join p in _db.Products on cd.PdId equals p.Id into join_cd_p
                      from cd_p in join_cd_p.DefaultIfEmpty()

                      join c in _db.Carts on cd.CartId equals c.CartId into join_cd
                      from c_cd in join_cd
                      where c_cd.CdateAt >= sDate && c_cd.CdateAt <= eDate
                      group cd by new { cd.PdId, cd_p.Name } into g
                      select new RepSale
                      {
                          PdId = g.Key.PdId,
                          PdName = g.Key.Name,
                          CdtlMoney = g.Sum(x => x.CdtlMoney),
                          CdtlQty = g.Sum(x => x.CdtlQty)
                      };
            ViewBag.sDate = sDate.ToString("yyyy-MM-dd");
            ViewBag.eDate = eDate.ToString("yyyy-MM-dd");
            return View(rep);

        }
        public IActionResult SumPurchaseDaily()
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            DateOnly thedate = DateOnly.FromDateTime(DateTime.Now.Date);
            var rep = from cd in _db.CartDtls

                      join p in _db.Products on cd.PdId equals p.Id into join_cd_p
                      from cd_p in join_cd_p.DefaultIfEmpty()

                      join c in _db.Carts on cd.CartId equals c.CartId into join_cd
                      from c_cd in join_cd
                      join cu in _db.Customers on c_cd.CusId equals cu.CusId into join_cu_c
                      from cu_c in join_cu_c.DefaultIfEmpty()
                      where c_cd.CdateAt == thedate
                      group cd by new { cu_c.CusId, cu_c.Fname, cu_c.Lname } into g
                      select new RepSaleC
                      {
                          CusId = g.Key.CusId,
                          CusName = g.Key.Fname + " " + g.Key.Lname,
                          CdtlMoney = g.Sum(x => x.CdtlMoney),
                          CdtlQty = g.Sum(x => x.CdtlQty)
                      };
            ViewBag.theDate = thedate.ToString("yyyy-MM-dd");

            return View(rep);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SumPurchaseDaily(DateOnly thedate)
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            //DateOnly thedate = DateOnly.FromDateTime(DateTime.Now.Date);
            var rep = from cd in _db.CartDtls

                      join p in _db.Products on cd.PdId equals p.Id into join_cd_p
                      from cd_p in join_cd_p.DefaultIfEmpty()

                      join c in _db.Carts on cd.CartId equals c.CartId into join_cd
                      from c_cd in join_cd
                      join cu in _db.Customers on c_cd.CusId equals cu.CusId into join_cu_c
                      from cu_c in join_cu_c.DefaultIfEmpty()
                      where c_cd.CdateAt == thedate
                      group cd by new { cu_c.CusId, cu_c.Fname, cu_c.Lname } into g
                      select new RepSaleC
                      {
                          CusId = g.Key.CusId,
                          CusName = g.Key.Fname + " " + g.Key.Lname,
                          CdtlMoney = g.Sum(x => x.CdtlMoney),
                          CdtlQty = g.Sum(x => x.CdtlQty)
                      };
            ViewBag.theDate = thedate.ToString("yyyy-MM-dd");

            return View(rep);
        }
        public IActionResult SumPurchaseMonthly()
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            //กำหนดวันแรก และคำนวณหาวันสุดท้ายของเดือนปัจจุบัน
            var theMonth = DateTime.Now.Month;
            var theYear = DateTime.Now.Year;
            //วันแรกคือวันที่ 1 ของเดือน
            DateOnly sDate = new DateOnly(theYear, theMonth, 1);
            //วันสุดท้ายคือวันที 1 ของเดือนหน้า ลบไป1วัน
            DateOnly eDate = new DateOnly(theYear, theMonth, 1).AddMonths(1).AddDays(-1);

            var rep = from cd in _db.CartDtls

                      join p in _db.Products on cd.PdId equals p.Id into join_cd_p
                      from cd_p in join_cd_p.DefaultIfEmpty()
                      
                      join c in _db.Carts on cd.CartId equals c.CartId into join_cd
                      from c_cd in join_cd
                      join cu in _db.Customers on c_cd.CusId equals cu.CusId into join_cu_c
                      from cu_c in join_cu_c.DefaultIfEmpty()
                      where c_cd.CdateAt >= sDate && c_cd.CdateAt <= eDate
                      group cd by new { cu_c.CusId , cu_c.Fname , cu_c.Lname } into g
                      select new RepSaleC
                      {
                          CusId = g.Key.CusId,
                          CusName = g.Key.Fname + " " + g.Key.Lname,
                          CdtlMoney = g.Sum(x => x.CdtlMoney),
                          CdtlQty = g.Sum(x => x.CdtlQty)
                      };
            ViewBag.sDate = sDate.ToString("yyyy-MM-dd");
            ViewBag.eDate = eDate.ToString("yyyy-MM-dd");

            return View(rep);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SumPurchaseMonthly(DateOnly sDate , DateOnly eDate)
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            //กำหนดวันแรก และคำนวณหาวันสุดท้ายของเดือนปัจจุบัน
            //var theMonth = DateTime.Now.Month;
            //var theYear = DateTime.Now.Year;
            //วันแรกคือวันที่ 1 ของเดือน
            //DateOnly sDate = new DateOnly(theYear, theMonth, 1);
            //วันสุดท้ายคือวันที 1 ของเดือนหน้า ลบไป1วัน
            //DateOnly eDate = new DateOnly(theYear, theMonth, 1).AddMonths(1).AddDays(-1);

            var rep = from cd in _db.CartDtls

                      join p in _db.Products on cd.PdId equals p.Id into join_cd_p
                      from cd_p in join_cd_p.DefaultIfEmpty()

                      join c in _db.Carts on cd.CartId equals c.CartId into join_cd
                      from c_cd in join_cd
                      join cu in _db.Customers on c_cd.CusId equals cu.CusId into join_cu_c
                      from cu_c in join_cu_c.DefaultIfEmpty()
                      where c_cd.CdateAt >= sDate && c_cd.CdateAt <= eDate
                      group cd by new { cu_c.CusId, cu_c.Fname, cu_c.Lname } into g
                      select new RepSaleC
                      {
                          CusId = g.Key.CusId,
                          CusName = g.Key.Fname + " " + g.Key.Lname,
                          CdtlMoney = g.Sum(x => x.CdtlMoney),
                          CdtlQty = g.Sum(x => x.CdtlQty)
                      };
            ViewBag.sDate = sDate.ToString("yyyy-MM-dd");
            ViewBag.eDate = eDate.ToString("yyyy-MM-dd");

            return View(rep);
        }
        public IActionResult WorkForEmployee(int page =1)
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            var work = from em in _db.Employees
                       join work1 in _db.Works on em.EmId equals work1.EmId into join_em_w
                       from em_w in join_em_w.DefaultIfEmpty()
                       where (em.Type.Equals("Manager") || em.Type.Equals("User")) && em_w.WorkDate != null
                       select new WorkEm{
                           WorkDate = em_w.WorkDate,
                           WorkIn = em_w.WorkIn,
                           WorkOut = em_w.WorkOut,
                           EmName = em.Fname + " " + em.Lname,
                       };
            int pageSize = 10;

            if (work == null)
            {
                return NotFound();
            }
            var pageShow = work.Skip((page - 1) * pageSize).Take(pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)work.Count() / pageSize);

            return View(pageShow);
        }
        public IActionResult JournalPurchase(int page=1)
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            var rep = from cd in _db.BuyDtls

                      join cu in _db.Buyings on cd.BuyId equals cu.BuyId into join_cu_c
                      from cu_c in join_cu_c.DefaultIfEmpty()

                      join p in _db.Products on cd.PdId equals p.Id into join_cd_p
                      from cd_p in join_cd_p.DefaultIfEmpty()

                      join s in _db.Suppliers on cu_c.SupId equals s.Sid into join_s_b
                      from s_b in join_s_b.DefaultIfEmpty()

                      group cd by new { cu_c.BuyDate , cd_p.Name , s_b.SupName , cu_c.Saleman , cd.BdtlQty , cd.BdtlPrice } into g
                      select new PurchaseJournal
                      {
                          bdate = g.Key.BuyDate,
                          PdName = g.Key.Name,
                          SupName = g.Key.SupName,
                          Saleman = g.Key.Saleman,
                          Bqty = g.Key.BdtlQty,
                          Bmoney = g.Key.BdtlPrice
                      };
            int pageSize = 10;

            if (rep == null)
            {
                return NotFound();
            }
            var pageShow = rep.Skip((page - 1) * pageSize).Take(pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)rep.Count() / pageSize);
            return View(pageShow);
        }
        public IActionResult JournalSale(int page=1)
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            var rep = from cd in _db.CartDtls

                      join p in _db.Products on cd.PdId equals p.Id into join_cd_p
                      from cd_p in join_cd_p.DefaultIfEmpty()

                      join c in _db.Carts on cd.CartId equals c.CartId into join_cd
                      from c_cd in join_cd.DefaultIfEmpty()
                      join cu in _db.Customers on c_cd.CusId equals cu.CusId into join_cu_c
                      from cu_c in join_cu_c.DefaultIfEmpty()
                      where c_cd.Cf.Equals("Y")
                      group cd by new {c_cd.CdateAt, cd_p.Name, cu_c.Fname, cu_c.Lname ,c_cd.Cmoney , c_cd.Cqty} into g
                      select new SaleJournal
                      {
                          date = g.Key.CdateAt,
                          PdName = g.Key.Name,
                          CusName = g.Key.Fname + " " + g.Key.Lname,
                          Cmoney = g.Key.Cmoney,
                          Cqty = g.Key.Cqty
                      };
            int pageSize = 10;

            if (rep == null)
            {
                return NotFound();
            }
            var pageShow = rep.Skip((page - 1) * pageSize).Take(pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)rep.Count() / pageSize);
            return View(pageShow);
        }
        public IActionResult JournalStock(int page =1)
        {
            if (HttpContext.Session.GetString("CusId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            /* var rep = from cd in _db.CartDtls

                       join p in _db.Products on cd.PdId equals p.Id into join_cd_p
                       from cd_p in join_cd_p.DefaultIfEmpty()

                       join c in _db.Carts on cd.CartId equals c.CartId into join_cd
                       from c_cd in join_cd
                       join cu in _db.Customers on c_cd.CusId equals cu.CusId into join_cu_c
                       from cu_c in join_cu_c.DefaultIfEmpty()
                       where c_cd.Cf.Equals("Y")
                       group cd by new { c_cd.CdateAt, cd_p.Name, cu_c.Fname, cu_c.Lname, c_cd.Cmoney, c_cd.Cqty } into g
                       select new StockJournal
                       {
                           status = g.Key.CdateAt,
                           PdName = ,
                           CremainW = ,
                           Cmoney = g.Key.Cmoney,
                           Cqty = g.Key.Cqty
                       };*/
            var pd = from p in _db.Products
                     join p_t in _db.Categorys on p.Catid equals p_t.Id into join_p_t
                     from p_pt in join_p_t.DefaultIfEmpty()
                     join b in _db.Brands on p.Brandid equals b.BrandId into join_p_b
                     from p_b in join_p_b.DefaultIfEmpty()
                     select new StockJournal
                     {
                         
                         PdName = p.Name,
                         Cprice = p.Price,
                         StockLimitLess = 5,
                         StockLimitMore = 600,
                         CremainW = p.Stock,
                         BrandName = p_b.BrandName,
                         CategoryName = p_pt.Ncategory,
                     };
            int pageSize = 13;
            
            if (pd == null)
            {
                return NotFound();
            }
            var pageShow = pd.Skip((page - 1) * pageSize).Take(pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)pd.Count() / pageSize);
            return View(pageShow);
        }
      
    }
}
