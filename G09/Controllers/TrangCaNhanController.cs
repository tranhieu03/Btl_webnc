using G09.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace G09.Controllers
{

    public class TrangCaNhanController : Controller
    {

        private readonly DbG09foodContext _context;
        private NguoiDung us;
        public TrangCaNhanController(DbG09foodContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("TrangCaNhan/K_TrangCaNhan/{id}")]
        public IActionResult K_TrangCaNhan(int id)
        {
            /*id = 2;*/
            // Lấy thông tin người dùng 
            NguoiDung nguoiDung = _context.NguoiDungs.Find(id);

            // Lọc danh sách bài viết của người dùng 

            var baiViets_nd = _context.BaiViets
                                        .Where(b => b.MaNguoiDung == id)
                                        .Select(b => new BaiViet
                                        {
                                            MaBaiViet = b.MaBaiViet,
                                            MaNguoiDung = b.MaNguoiDung,
                                            TenNguoiDung = b.MaNguoiDungNavigation.TenNguoiDung,
                                            AnhDaiDien = b.MaNguoiDungNavigation.AnhDaiDien,
                                            TenLoaiMonAn = b.MaLoaiMonAnNavigation.TenLoaiMonAn,
                                            NoiDung = b.NoiDung,
                                            AnhBaiViet = b.AnhBaiViet,
                                            NgayTao = b.NgayTao ?? DateTime.Now,
                                            Thiches = b.Thiches,
                                            IsLiked = _context.Thiches.Any(t => t.MaBaiViet == b.MaBaiViet && t.MaNguoiDung == id),
                                            SoLuongLike = b.SoLuongLike
                                        }).ToList();
            // Lọc danh sách người được theo doi của người dùng 
            List<TheoDoi> dctheodoi_nd = _context.TheoDois
                                                .Where(theodoi => theodoi.MaNguoiTheoDoi == id)

                                                .ToList();
            // Lọc danh sách người theo doi của người dùng 
            List<TheoDoi> theodoi_nd = _context.TheoDois
                                                .Where(theodoi => theodoi.MaNguoiDuocTheoDoi == id)
                                                .ToList();
            //Lấy danh sách bình luận
            var cmts = _context.BinhLuans.Select(b => new BinhLuan
            {
                MaBinhLuan = b.MaBinhLuan,
                MaBaiViet = b.MaBaiViet,
                MaNguoiDung = b.MaNguoiDung,
                TenNguoiDung = b.MaNguoiDungNavigation.TenNguoiDung,
                NoiDung = b.NoiDung,
                NgayTao = b.NgayTao



            }).ToList();

            // Gán dữ liệu vào ViewBag
            ViewBag.nguoiD = nguoiDung;
            ViewBag.baiV = baiViets_nd;
            ViewBag.SoNguoi_DcTheoDoi = dctheodoi_nd.Count;
            ViewBag.SoNguoi_TheoDoi = theodoi_nd.Count;
            ViewBag.cmts = cmts;

            return View("TrangCaNhan");
        }
        public IActionResult TrangCaNhan()
        {
            var currentUserEmail = HttpContext.Session.GetString("Email");
            us = _context.NguoiDungs.FirstOrDefault(t => t.Email == currentUserEmail);
            // Lấy thông tin người dùng 
            NguoiDung nguoiDung = _context.NguoiDungs.Find(us.MaNguoiDung);

            // Lọc danh sách bài viết của người dùng 

            var baiViets_nd = _context.BaiViets
                                        .Where(b => b.MaNguoiDung == us.MaNguoiDung)
                                        .Select(b => new BaiViet
                                        {
                                            MaBaiViet = b.MaBaiViet,
                                            MaNguoiDung = b.MaNguoiDung,
                                            TenNguoiDung = b.MaNguoiDungNavigation.TenNguoiDung,
                                            AnhDaiDien = b.MaNguoiDungNavigation.AnhDaiDien,
                                            TenLoaiMonAn = b.MaLoaiMonAnNavigation.TenLoaiMonAn,
                                            NoiDung = b.NoiDung,
                                            AnhBaiViet = b.AnhBaiViet,
                                            NgayTao = b.NgayTao ?? DateTime.Now,
                                            Thiches = b.Thiches,
                                            IsLiked = _context.Thiches.Any(t => t.MaBaiViet == b.MaBaiViet && t.MaNguoiDung == us.MaNguoiDung),
                                            SoLuongLike = b.SoLuongLike
                                        }).ToList();
            // Lọc danh sách người được theo doi của người dùng 
            List<TheoDoi> dctheodoi_nd = _context.TheoDois
                                                .Where(theodoi => theodoi.MaNguoiTheoDoi == us.MaNguoiDung)

                                                .ToList();
            // Lọc danh sách người theo doi của người dùng 
            List<TheoDoi> theodoi_nd = _context.TheoDois
                                                .Where(theodoi => theodoi.MaNguoiDuocTheoDoi == us.MaNguoiDung)
                                                .ToList();
            var cmts = _context.BinhLuans.Select(b => new BinhLuan
            {
                MaBinhLuan = b.MaBinhLuan,
                MaBaiViet = b.MaBaiViet,
                MaNguoiDung = b.MaNguoiDung,
                TenNguoiDung = b.MaNguoiDungNavigation.TenNguoiDung,
                NoiDung = b.NoiDung,
                NgayTao = b.NgayTao



            }).ToList();

            // Gán dữ liệu vào ViewBag
            ViewBag.nguoiD = nguoiDung;
            ViewBag.baiV = baiViets_nd;
            ViewBag.SoNguoi_DcTheoDoi = dctheodoi_nd.Count;
            ViewBag.SoNguoi_TheoDoi = theodoi_nd.Count;
            ViewBag.cmts = cmts;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(IFormFile image = null, string tenND = "", string TieuSu = "")
        {
            var currentUserEmail = HttpContext.Session.GetString("Email");
            var nguoiDung = _context.NguoiDungs.FirstOrDefault(t => t.Email == currentUserEmail);

            if (nguoiDung == null)
            {
                return NotFound();
            }

            if (image != null)
            {
                var filePath = Path.Combine("wwwroot/User/img", image.FileName);
                if (!System.IO.File.Exists(filePath))
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                }

                string imageUrl = "/User/img/" + image.FileName;
                nguoiDung.AnhDaiDien = imageUrl;
            }

            if (!string.IsNullOrWhiteSpace(tenND))
            {
                nguoiDung.TenNguoiDung = tenND;
            }

            if (!string.IsNullOrWhiteSpace(TieuSu))
            {
                nguoiDung.TieuSu = TieuSu;
            }

            _context.Update(nguoiDung);
            await _context.SaveChangesAsync();

            return RedirectToAction("TrangCaNhan");
        }

    }
}
