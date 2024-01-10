using Project_PRN211.Models;
namespace Project_PRN211.Repository
{
    public interface ILoaiSpRepository
    {
        Category Add(Category category);
        Category Update(Category category);
        Category Delete(String cid);
        Category GetLoaiSp(String cid);
        IEnumerable<Category> GetAllLoaiSp();

    }
}
