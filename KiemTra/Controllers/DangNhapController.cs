using KiemTra.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace KiemTra.Controllers
{
    public class DangNhapController : Controller
    {
        private List<User> users = new List<User>();

        public DangNhapController()
        {
            // Thêm các tài khoản vào danh sách người dùng
            users.Add(new User { Username = "CongHp", Password = "1234",Role="ADMIN" });
            users.Add(new User { Username = "ThaiND", Password = "12345",Role="User"});
            users.Add(new User { Username = "DanNT", Password = "123456" });
            users.Add(new User { Username = "TungLam", Password = "2345678" });
            users.Add(new User { Username = "bruce_wayne", Password = "B@tman2023" });
            users.Add(new User { Username = "tony_stark", Password = "IronMan456" });
            users.Add(new User { Username = "natasha_romanoff", Password = "BlackWidow!22" });
            users.Add(new User { Username = "steve_rogers", Password = "CapAmerica!99" });
            users.Add(new User { Username = "wanda_maximoff", Password = "ScarletW1tch" });
            users.Add(new User { Username = "sam_wilson", Password = "Falcon2024" });
            users.Add(new User { Username = "nick_fury", Password = "Shield!Fury" });
            users.Add(new User { Username = "thor_odinson", Password = "Mjolnir!2024" });
            users.Add(new User { Username = "hulk_bruce", Password = "HulkSmash99" });
            users.Add(new User { Username = "rocket_raccoon", Password = "Groot1234" });
            users.Add(new User { Username = "black_panther", Password = "WakandaForever!" });
            users.Add(new User { Username = "hawkeye_clint", Password = "Archery4Life" });
            users.Add(new User { Username = "doctor_strange", Password = "Strange@2023" });
            users.Add(new User { Username = "vision_wanda", Password = "MindStone987" });
            users.Add(new User { Username = "spider_man", Password = "WebSlinger2024" });
            users.Add(new User { Username = "loki_odinson", Password = "TricksterLoki!" });
        }

        // GET: DangNhap/Index
        public IActionResult Index()
        {
            return View();
        }

        // POST: DangNhap/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(User model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra tài khoản và mật khẩu từ danh sách người dùng
                var user = users.Find(u => u.Username == model.Username && u.Password == model.Password);

                if (user != null)
                {
                    // Đăng nhập thành công, lưu tên người dùng và role vào Session
                    HttpContext.Session.SetString("Username", model.Username);
                    HttpContext.Session.SetString("Role", user.Role); // Đảm bảo Role được lưu đúng

                    // Chuyển hướng đến trang chính sau khi đăng nhập thành công
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Nếu thông tin đăng nhập không đúng
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }

            // Quay lại view đăng nhập nếu không thành công
            return View(model);
        }


        //dang xuat
        public IActionResult Logout()
        {
            // Xóa thông tin đăng
            HttpContext.Session.Remove("Username");
            HttpContext.Session.Remove("Role");
            // Chuyển hướng đến trang đăng nhập
            return RedirectToAction("Index","DangNhap");
        }
    }
}
