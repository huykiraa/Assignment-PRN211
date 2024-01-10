using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_PRN211.Models;
using Project_PRN211.Models.Authentication;
using System.Diagnostics;
using System.Security.Cryptography;
using X.PagedList;

namespace Project_PRN211.Controllers
{
    [Authentication]

    public class HomeController : Controller
    {

        private readonly WishContext _db;

        public HomeController(WishContext db)
        {
            _db = db;
        }
        
        public IActionResult Index(int? page)
        {
            int userId = HttpContext.Session.GetInt32("UId") ?? 0;
            //var lstSapPham = _db.Products.ToList();
            int pageSize = 8;
            int pageNumber = page==null||page<0?1:page.Value;
            var lstSapPham = _db.Product.AsNoTracking().OrderBy(x => x.Name);
            var cartCount = _db.Carts.Count(c => c.AccountId == userId);
            ViewBag.CartCount = cartCount;
            PagedList<Product> lst = new PagedList<Product>(lstSapPham, pageNumber, pageSize);
            return View(lst);
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

        public IActionResult SamPhamTheoLoai(int cid, int? page)
        {

            int pageSize = 8;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lstSapPham = _db.Product.AsNoTracking().Where(x=>x.CateId==cid).OrderBy(x => x.Name);
            PagedList<Product> lst = new PagedList<Product>(lstSapPham, pageNumber, pageSize);
           
            ViewBag.CateId = cid;
            return View(lst);
        }

        public IActionResult ChiTietSanPham(int sid)
        {
            var sanPham = _db.Product.SingleOrDefault(x => x.Id == sid);
            return View(sanPham);
        }



        public IActionResult ThemVaoGiohang(int pid, Cart cart, int amount, int address)
        {
            if (ModelState.IsValid)
            {
                int userId = HttpContext.Session.GetInt32("UId") ?? 0;
                if (userId != 0)
                {
                    // Kiểm tra xem đã có Cart nào của User có cùng ProductId chưa
                    var existingCart = _db.Carts
                        .Where(c => c.AccountId == userId && c.ProductId == pid)
                        .FirstOrDefault();

                    if (existingCart != null)
                    {
                        // Nếu đã có, cộng thêm vào Amount và lưu vào cơ sở dữ liệu
                        existingCart.Amount += amount;
                        _db.SaveChanges();
                    }
                    else
                    {
                        // Nếu chưa có, tạo mới một Cart và lưu vào cơ sở dữ liệu
                        var newCart = new Cart
                        {
                            AccountId = userId,
                            ProductId = pid,
                            Amount = amount,
                            Receiving_AddressId = address,
                        };

                        _db.Carts.Add(newCart);
                        _db.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }
            }

            return View(cart);
        }



        public IActionResult SearchSp(int? page, string search = "")
        {
            int pageSize = 8;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lstSanPham = _db.Product.Where(x=>x.Name.Contains(search)).OrderBy(x => x.Name);
            PagedList<Product> lst = new PagedList<Product>(lstSanPham, pageNumber, pageSize);
            ViewBag.TenSp = search;
            return View(lst);
        }

        public class CartProductViewModel
        {
            public Cart? Cart { get; set; }
            public Product? Product { get; set; }
            public decimal? TotalPrice => (decimal?)(Cart?.Amount * Product?.Price);

            public ProductSold? ProductSolds { get; internal set; }
            public Receiving_Address? ReceivingAddress { get; set; }
        }
        public IActionResult ListCart(int cid, int? page)
        {
            int userId = HttpContext.Session.GetInt32("UId") ?? 0;
            int pageSize = 6;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            

            
            // Sử dụng truy vấn LINQ để kết hợp thông tin từ bảng Cart và ProductSold
            var query = from cart in _db.Carts
                        join product in _db.Product on cart.ProductId equals product.Id
                        join productSold in _db.ProductSold on cart.ProductId equals productSold.ProductId into productSoldGroup
                        from productSold in productSoldGroup.DefaultIfEmpty()
                        where cart.AccountId == userId
                        select new CartProductViewModel
                        {
                            Cart = cart,
                            Product = product,
                            ProductSolds = productSold
                        };
            ViewBag.ad = new SelectList(_db.Receiving_Address.Where(ra => ra.UId == userId).ToList(), "AddressId", "Address");
            var cartCount = _db.Carts.Count(c => c.AccountId == userId);
            ViewBag.CartCount = cartCount;

            // Chuyển đổi kết quả thành danh sách các đối tượng CartProductViewModel
            var result = query.AsNoTracking().ToList();

            // Tạo danh sách PagedList từ kết quả
            PagedList<CartProductViewModel> lstCart = new PagedList<CartProductViewModel>(result, pageNumber, pageSize);

            return View(lstCart);
        }


        public IActionResult DeleteCart(int cId)
        {
                var sanPham = _db.Carts.Find(cId);
            var sanphamExits = _db.ProductSold.Where(x=>x.CartId == cId).ToList();
            if (sanphamExits.Count() == 0)
            {
                if (sanPham != null)
                {
                    _db.Carts.Remove(sanPham);
                    _db.SaveChanges();
                    TempData["Message"] = "The product has been successfully deleted from the cart";
                    return RedirectToAction("ListCart", "Home");
                }
                TempData["Message"] = "The product is not exits";
                return RedirectToAction("ListCart", "Home");
            }
            else
            {
                TempData["Message"] = "This product cannot remove because Its sold.";
                return RedirectToAction("ListCart", "Home");
            }
        }
        public IActionResult EditCard(int pid, Cart cart, int amount)
        {
            if (ModelState.IsValid)
            {
                int userId = HttpContext.Session.GetInt32("UId") ?? 0;
                if (userId != 0)
                {
                    // Kiểm tra xem đã có Cart nào của User có cùng ProductId chưa
                    var existingCart = _db.Carts
                        .Where(c => c.AccountId == userId && c.ProductId == pid)
                        .FirstOrDefault();

                    if (existingCart != null)
                    {
                        // Nếu đã có, cộng thêm vào Amount và lưu vào cơ sở dữ liệu
                        existingCart.Amount = amount;
                        _db.SaveChanges();
                    }
                    else
                    {
                        // Nếu chưa có, tạo mới một Cart và lưu vào cơ sở dữ liệu
                        var newCart = new Cart
                        {
                            AccountId = userId,
                            ProductId = pid,
                            Amount = amount
                        };

                        _db.Carts.Add(newCart);
                        _db.SaveChanges();
                    }

                    return RedirectToAction("ListCart");
                }
            }

            return View(cart);
        }

        public IActionResult BuyProduct(int cId,int uId, ProductSold productSold, int pId, int address)
        {
            var sanPham = _db.Carts.Find(cId);
            

            
            if (ModelState.IsValid)
            {
                int userId = HttpContext.Session.GetInt32("UId") ?? 0;
                
                if (userId != 0 )
                {
                    // Set properties using correct names
                    productSold.UId = userId;
                    productSold.CartId = cId;
                    productSold.ProductId = pId;
                    productSold.Receiving_Address = address;

                    // Optionally, you can set other properties of productSold here

                    _db.ProductSold.Add(productSold);
                    
                    _db.SaveChanges();
                    

                    return RedirectToAction("ListCart");
                    
                }
            }
            
            return View(productSold);
        }
        public IActionResult EditAdress(Cart cart, int address, int pid)
        {
            int userId = HttpContext.Session.GetInt32("UId") ?? 0;
            if (ModelState.IsValid)
            {
                var existingCart = _db.Carts
                        .Where(c => c.AccountId == userId && c.ProductId == pid)
                        .FirstOrDefault();

                if (existingCart != null)
                {
                    // Nếu đã có, cộng thêm vào Amount và lưu vào cơ sở dữ liệu
                    existingCart.Receiving_AddressId = address;
                    _db.SaveChanges();
                }

                return RedirectToAction("ListCart");
                
            }

            return View(cart);
        }
        [Route("AddAddress")]
        [HttpGet]
        public IActionResult AddAddress()
        {
           
            return View();
        }
        [Route("AddAddress")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAddress(Receiving_Address receiving, string address )
        {
            int userId = HttpContext.Session.GetInt32("UId") ?? 0;
            if (ModelState.IsValid)
            {
                var exitstingAddress = _db.Receiving_Address
                    .Where(a=>a.Address == address).FirstOrDefault();
                if (exitstingAddress != null)
                {
                    exitstingAddress.Address = address;
                    exitstingAddress.UId = userId;
                    _db.SaveChanges();
                }
                else
                {
                    var newAddress = new Receiving_Address
                    {
                        Address = address,
                        UId = userId,
                    };

                    _db.Receiving_Address.Add(newAddress);
                    _db.SaveChanges();
                }
                return RedirectToAction("ListCart");

            }
            return View(receiving);
        }

        public IActionResult DeleteBill(int Id)
        {
            var sanPham = _db.ProductSold.Find(Id);
            if (sanPham != null)
            {
                _db.ProductSold.Remove(sanPham);
                _db.SaveChanges();
                TempData["Message"] = "The product has been successfully deleted from the cart";
                return RedirectToAction("ListBill", "Home");
            }
            TempData["Message"] = "The product is not exits";
            return RedirectToAction("ListBill", "Home");
        }
        
        public IActionResult ListBill(int? page)
        {
            int userId = HttpContext.Session.GetInt32("UId") ?? 0;
            int pageSize = 8;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            /* var query = from cart in _db.Carts
                         join product in _db.Product_Sold on cart.ProductId equals product.Id
                         where cart.AccountId == userId
                         select new CartProductViewModel { Cart = cart, Product = product };*/
            var query = from cart in _db.Carts
                        join product in _db.Product on cart.ProductId equals product.Id
                        join productS in _db.ProductSold on cart.CartId equals productS.CartId
                        join address in _db.Receiving_Address on cart.Receiving_AddressId equals address.AddressId
                        where productS.UId == userId
                        select new CartProductViewModel { Cart = cart, Product = product, ProductSolds = productS, ReceivingAddress = address };
            var result = query.AsNoTracking().ToList();
            ViewBag.adress = _db.Receiving_Address.Where(ra => ra.UId == userId).ToList();
            PagedList<CartProductViewModel> lstBill = new PagedList<CartProductViewModel>(result, pageNumber, pageSize);
            var cartCount = _db.Carts.Count(c => c.AccountId == userId);
            ViewBag.CartCount = cartCount;
            return View(lstBill);
        }
       
        public IActionResult ChangePassword(Account account,int uId, string pass, string Repass)
        {
            int userId = HttpContext.Session.GetInt32("UId") ?? 0;
            var passwordOfUser = _db.Accounts
                .Where(x => x.UId==userId).FirstOrDefault();
            ViewBag.passwordOfU = passwordOfUser;
            return View(account);
        }



    }
}