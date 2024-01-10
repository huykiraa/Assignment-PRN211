using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_PRN211.Models;


namespace Project_PRN211.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        WishContext db = new WishContext();
        [HttpGet]
        public IEnumerable<Product> GetAllProduct()
        {
            var sanPham = (from p in db.Product
                           select new Product
                           {
                               Id = p.Id,
                               Name = p.Name,
                               Image = p.Image,
                               Price = p.Price,
                               Title = p.Title,
                               Description = p.Description,

                           }).ToList();
            return sanPham;
        }
        [HttpGet("{Cid}")]
        public IEnumerable<Product> getProductsByCategory(int Cid)
        {
            var sanPham = (from p in db.Product
                           where p.CateId == Cid
                           select new Product
                           {
                               Id = p.Id,
                               Name = p.Name,
                               Image = p.Image,
                               Price = p.Price,
                               Title = p.Title,
                               Description = p.Description,

                           }).ToList();
            return sanPham;
        }
        [HttpGet("search/{name}")]
        public IEnumerable<Product> GetProductsByName(string name)
        {
            var products = db.Product
                .Where(p => p.Name.Contains(name))
                .Select(p => new Product
                {
                    Id = p.Id,
                    Name = p.Name,
                    Image = p.Image,
                    Price = p.Price,
                    Title = p.Title,
                    Description = p.Description,
                })
                .ToList();

            return products;
        }
    }
}
