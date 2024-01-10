using Project_PRN211.Models;
using Microsoft.AspNetCore.Mvc;
using Project_PRN211.Repository;

namespace Project_PRN211.ViewComponents
{
    public class LoaiSpMenuViewComponent : ViewComponent
    {
        private readonly ILoaiSpRepository _loaiSp;
        public LoaiSpMenuViewComponent(ILoaiSpRepository loaiSpRepository)
        {
            _loaiSp = loaiSpRepository;
        }
        public IViewComponentResult Invoke()
        {
            var loaisp = _loaiSp.GetAllLoaiSp().OrderBy(x => x.Cname);
            return View(loaisp);
        }
    }
}
