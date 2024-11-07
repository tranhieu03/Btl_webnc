using G09.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace G09.Controllers
{
    // [Route("api/[controller]")]
    // [ApiController]
    public class TrangChuController : Controller
    {
        private readonly ILogger<TrangChuController> _logger;
        private readonly DbG09foodContext _context;
        private NguoiDung us;
        public TrangChuController(DbG09foodContext context, ILogger<TrangChuController> logger)
        {
            _logger = logger;
            _context = context;


        }

        [HttpGet]
        public IActionResult ListBaiViet()
        {
            var currentUserEmail = HttpContext.Session.GetString("Email");
            us = _context.NguoiDungs
               .FirstOrDefault(t => t.Email == currentUserEmail);

            var baiViets = _context.BaiViets
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

            var cmts = _context.BinhLuans.Select(b => new BinhLuan
            {
                MaBinhLuan = b.MaBinhLuan,
                MaBaiViet = b.MaBaiViet,
                MaNguoiDung = b.MaNguoiDung,
                TenNguoiDung = b.MaNguoiDungNavigation.TenNguoiDung,
                NoiDung = b.NoiDung,
                NgayTao = b.NgayTao



            }).ToList();

            ViewBag.us = us; // Pass the current user ID to the view
            ViewBag.cmts = cmts;
            return View(baiViets);
        }

        //  [HttpPost]
        //[Route("LikeEvent")]
        [HttpPost()]
        public IActionResult LikeEvent(int mabaiviet, int tennguoidung)
        {
            if (mabaiviet == null || tennguoidung == null)
            {
                _logger.LogWarning("mabaiviet hoặc tennguoidung là null");
                return Content("mabaiviet hoặc tennguoidung là null");
            }

            // Hiển thị giá trị của các tham số
            _logger.LogInformation($"mabaiviet: {mabaiviet}, tennguoidung: {tennguoidung}");
            var currentUserEmail = HttpContext.Session.GetString("Email");
            var uss = _context.NguoiDungs
               .FirstOrDefault(t => t.Email == currentUserEmail);
            var existingLike = _context.Thiches
               .FirstOrDefault(t => t.MaBaiViet == mabaiviet && t.MaNguoiDung == uss.MaNguoiDung);
            var baiviet = _context.BaiViets
               .FirstOrDefault(t => t.MaBaiViet == mabaiviet);
            if (existingLike != null)
            {

                baiviet.SoLuongLike--;
                _context.Thiches.Remove(existingLike);
                _context.SaveChanges();
                 return Json(new { success = true   , newLikeCount = baiviet.SoLuongLike });
            }
            else
            {

                var thich = new Thich
                {
                    MaBaiViet = mabaiviet,
                    MaNguoiDung = uss.MaNguoiDung,
                };
                baiviet.SoLuongLike++;
                _context.Thiches.Add(thich);
                _context.SaveChanges();
                return Json(new { success = true, newLikeCount = baiviet.SoLuongLike });
            }

        }


        [HttpPost]
        public IActionResult AddComment(string comment, int mabaiviet, int tennguoidung)
        {
            var currentUserEmail = HttpContext.Session.GetString("Email");
            var uss = _context.NguoiDungs
               .FirstOrDefault(t => t.Email == currentUserEmail);
            var cmt = new BinhLuan
            {
                MaBaiViet = mabaiviet,
                MaNguoiDung = uss.MaNguoiDung,
                NoiDung = comment
            };

            _context.BinhLuans.Add(cmt);
            _context.SaveChanges();


            return Json(new { success = true, tennguoidung = uss.TenNguoiDung });


        }
        //[HttpPost]
        //public IActionResult FollowPost(int mabaiviet)
        //{
        //    var currentUserEmail = HttpContext.Session.GetString("Email");
        //    var uss = _context.NguoiDungs.FirstOrDefault(t => t.Email == currentUserEmail);

        //    if (uss == null)
        //    {
        //        return Json(new { success = false, message = "Người dùng không tồn tại" });
        //    }

        //    // Lấy thông tin của bài viết và người dùng được theo dõi
        //    var baiViet = _context.BaiViets.FirstOrDefault(b => b.MaBaiViet == mabaiviet);

        //    if (baiViet == null)
        //    {
        //        return Json(new { success = false, message = "Bài viết không tồn tại" });
        //    }

        //    // Kiểm tra nếu người dùng đã theo dõi người dùng được liên kết với bài viết này
        //    var existingFollow = _context.TheoDois
        //        .FirstOrDefault(t => t.MaNguoiTheoDoi == uss.MaNguoiDung && t.MaNguoiDuocTheoDoi == baiViet.MaNguoiDung);

        //    if (existingFollow != null)
        //    {
        //        // Nếu đã theo dõi, hủy theo dõi
        //        _context.TheoDois.Remove(existingFollow);
        //        _context.SaveChanges();
        //        return Json(new { success = true, isFollowed = false });
        //    }
        //    else
        //    {
        //        // Nếu chưa theo dõi, thêm vào bảng TheoDoi
        //        var follow = new TheoDoi
        //        {
        //            MaNguoiTheoDoi = uss.MaNguoiDung,
        //            MaNguoiDuocTheoDoi = baiViet.MaNguoiDung,
        //            NgayTao = DateTime.Now
        //        };
        //        _context.TheoDois.Add(follow);
        //        _context.SaveChanges();
        //        return Json(new { success = true, isFollowed = true });
        //    }
        //}

    }
}

       


