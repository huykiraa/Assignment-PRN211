using Project_PRN211.Models;

namespace Project_PRN211.Repository
{
    public class LoaiSpRepository : ILoaiSpRepository
    {
        private readonly WishContext _context;
        public LoaiSpRepository(WishContext context)
        {
            _context = context;
        }

        public Category Add(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return category;
        }

        public Category Delete(string cid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Category> GetAllLoaiSp()
        {
            return _context.Categories;
        }

        public Category GetLoaiSp(string cid)
        {
            return _context.Categories.Find(cid);
        }

        public Category Update(Category category)
        {
            _context.Update(category);
            _context.SaveChanges();
            return category;
        }
    }
}
