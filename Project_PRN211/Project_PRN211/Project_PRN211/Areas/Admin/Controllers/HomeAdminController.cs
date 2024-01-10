using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Project_PRN211.Models;
using Project_PRN211.Models.Authorization;
using X.PagedList;

namespace Project_PRN211.Areas.Admin.Controllers
{
    [AdminAuthorization]
    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
    public class HomeAdminController : Controller
    {
        private readonly WishContext _db;
        public HomeAdminController(WishContext db)
        {
            _db = db;
        }

        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }
        [Route("danhmucsanpham")]
        public IActionResult DanhMucSanPham(int? page)
        {
            int userId = HttpContext.Session.GetInt32("UId") ?? 0;
            int pageSize = 10;
            int pageNumber = page == null || page <0 ? 1 : page.Value;
            //lst san pham theo uId
           /* var lstSanPham = db.Products.AsNoTracking().Where(u=>u.SellId==userId).OrderBy(x => x.Name);*/
            var lstSanPham = _db.Product.AsNoTracking().OrderBy(x => x.Name);
            PagedList<Product> lst = new PagedList<Product>(lstSanPham, pageNumber, pageSize);
            return View(lst);
        }

        

        [Route("SuaSanPham")]
        [HttpGet]
        public IActionResult SuaSanPham( int pId)
        {
            ViewBag.CateId = new SelectList(_db.Categories.ToList(), "Cid", "Cname");
            var sanPham = _db.Product.Find(pId);
            return View(sanPham);
        }
        [Route("SuaSanPham")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaSanPham(Product sanpham)
        {
            if (ModelState.IsValid)
            {
                //db.Update(sanpham);
                _db.Entry(sanpham).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("DanhMucSanPham", "HomeAdmin");
            }
            return View(sanpham);
        }
        [Route("XoaSanPham")]
        [HttpGet]
        public IActionResult XoaSanPham(int pId)
        {
            var chiTietSanPham = _db.Carts.Where(x=>x.ProductId==pId).ToList();
            if (chiTietSanPham.Count()==0)
            {
                var sanPham = _db.Product.Find(pId);
                if(sanPham !=null)
                {
                    _db.Product.Remove(sanPham);
                    _db.SaveChanges() ;
                    TempData["Message"] = "The product has been successfully deleted";
                    return RedirectToAction("DanhMucSanPham", "HomeAdmin");
                }
                TempData["Message"] = "The product is not exits";
                return RedirectToAction("DanhMucSanPham", "HomeAdmin");

            }
            else
            {
                TempData["Message"] = "Can't remove this product because it is for sale";
                return RedirectToAction("DanhMucSanPham", "HomeAdmin");
            }
            
        }
        [HttpGet]
        [Route("SanPhamChiTiet")]
        public IActionResult SanPhamChiTiet(int pId)
        {
            var chiTietSanPham = _db.Product.Where(x => x.Id == pId).ToList();

            // Trích xuất Id của danh mục (CateId) từ chi tiết sản phẩm đã lấy được
            var cateId = chiTietSanPham.FirstOrDefault()?.CateId;
            //Lấy CateName
            var cateName = _db.Categories.Where(c => c.Cid == cateId)
                .Select(c => c.Cname)
                .FirstOrDefault();
            ViewBag.CateName = cateName;
            var sellId = chiTietSanPham.FirstOrDefault()?.SellId;
            var seller = _db.Accounts.Where(s=>s.UId == sellId).Select(s=>s.User).FirstOrDefault();
            ViewBag.Seller = seller;
            return View(chiTietSanPham);
        }

        [Route("ThemSanPhamMoi")]
        [HttpGet]
        public IActionResult ThemSanPhamMoi()
        {
            ViewBag.CateId = new SelectList(_db.Categories.ToList(), "Cid", "Cname");
            return View();
        }
        [Route("ThemSanPhamMoi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemSanPhamMoi(Product sanpham)
        {
            if (ModelState.IsValid)
            {
                int userId = HttpContext.Session.GetInt32("UId") ?? 0;
                if (userId != 0)
                {
                    sanpham.SellId = userId;
                    _db.Product.Add(sanpham);
                    _db.SaveChanges();
                    return RedirectToAction("DanhMucSanPham");
                }

            }
            return View(sanpham);
        }

        [Route("ThemLoaiSanPham")]
        [HttpGet]
        public IActionResult ThemLoaiSanPham()
        {
            ViewBag.CateId = new SelectList(_db.Categories.ToList(), "Cid", "Cname");
            return View();
        }
        [Route("ThemLoaiSanPham")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemLoaiSanPham(Category category)
        {
            if (ModelState.IsValid)
            {

                _db.Categories.Add(category);
               _db.SaveChanges();
                    return RedirectToAction("DanhMucSanPham");
                

            }
            return View(category);
        }



    }
}
