using Azure.Core;
using KiemTra.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace KiemTra.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly KiemTraContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Data", "provinces.json");

        public HomeController(ILogger<HomeController> logger, KiemTraContext context, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        // Hiển thị danh sách nhân viên và hỗ trợ tìm kiếm, phân trang
        public async Task<IActionResult> Index(string keyword, int page = 1)
        {
            //kiem tra xem co ton tai username trong session khong
            var user = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
           // neu khong ton tai username trong session thi chuyen huong ve trang dang nhap
           if (string.IsNullOrEmpty(user))
            {
                return RedirectToAction("Index", "DangNhap");
            }
            int pageSize = 3; // Số nhân viên mỗi trang
            var query = _context.NhanViens.AsQueryable();

            // Kiểm tra nếu keyword không null và không rỗng
            if (!string.IsNullOrEmpty(keyword))
            {
                // Lọc theo từ khóa tìm kiếm trong tên nhân viên
                query = query.Where(nv => nv.HoTen.Contains(keyword));
            }

            // Tính tổng số nhân viên thỏa mãn điều kiện tìm kiếm
            int totalNhanVien = query.Count();
            int totalPages = (int)Math.Ceiling(totalNhanVien / (double)pageSize);

            // Lấy danh sách nhân viên theo phân trang
            var nhanViens = query
                .Skip((page - 1) * pageSize)  // Bỏ qua số lượng bản ghi trước đó
                .Take(pageSize)              // Lấy số lượng bản ghi theo pageSize
                .ToList();

            // Trả về các biến cho View
            ViewBag.TotalPages = totalPages;
            ViewBag.PageNumber = page;
            ViewBag.Keyword = keyword; // Trả lại keyword cho view để hiển thị khi tìm kiếm

            return View(nhanViens);
        }

        // Hiển thị form tạo mới nhân viên
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Lấy danh sách tỉnh thành từ API
            var provinces = GetProvincesFromFile();
            ViewBag.Provinces = provinces;
            return View();
        }

        // Lưu nhân viên mới vào cơ sở dữ liệu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(NhanVien employee)
        {
            if (ModelState.IsValid)
            {
                // Lưu nhân viên vào cơ sở dữ liệu
                _context.NhanViens.Add(employee);
                _context.SaveChanges();
                // Lưu thông tin thao tác vào bảng ThaoTacSua
                var thaoTacSua = new ThaoTacSua
                {
                    username = HttpContext.Session.GetString("Username"), // Tên tài khoản thực hiện thao tác
                    LoaiThaoTac = "Thêm nhân viên",
                    NgayThaoTac = DateTime.Now,
                    ChiTietThaoTac = $"Thêm nhân viên mới với mã {employee.MaNhanVien} bởi {HttpContext.Session.GetString("Username")}."
                };
                _context.ThaoTacSuas.Add(thaoTacSua);
                _context.SaveChanges();
                // Chuyển hướng về trang danh sách nhân viên
                return RedirectToAction("Index", "Home");
            }

            // Nếu có lỗi, hiển thị lại form
            return View("Create", employee);
        }
        public List<Province> GetProvincesFromFile()
        {
            if (!System.IO.File.Exists(_filePath))
            {
                // Log error or handle the case when file is not found
                return new List<Province>();
            }

            var json = System.IO.File.ReadAllText(_filePath);
            var provinces = JsonConvert.DeserializeObject<List<Province>>(json);
            return provinces;
        }
        [HttpGet("12345/{provinceName}")]
        public IActionResult GetDistrictsByName(string provinceName)
        {
            try
            {
                var districts = GetDistrictsFromFile(provinceName);
                if (districts == null || !districts.Any())
                {
                    return NotFound("Không tìm thấy quận cho tỉnh này.");
                }
                return Ok(districts);  // Trả về danh sách quận dưới dạng JSON
            }
            catch (FileNotFoundException)
            {
                return NotFound("Không tìm thấy tệp dữ liệu tỉnh.");
            }
            catch (JsonException)
            {
                return BadRequest("Dữ liệu tệp quận không hợp lệ.");
            }
        }

        // Phương thức đọc dữ liệu từ file JSON và lọc quận theo tên tỉnh
        private List<District> GetDistrictsFromFile(string provinceName)
        {
            if (!System.IO.File.Exists(_filePath))
            {
                throw new FileNotFoundException("Tệp không tồn tại.");
            }

            var jsonData = System.IO.File.ReadAllText(_filePath);
            var provinces = JsonConvert.DeserializeObject<List<Province>>(jsonData);

            // Lọc tỉnh theo tên
            var province = provinces.FirstOrDefault(p => p.Name.ToLower() == provinceName.ToLower());

            if (province == null)
            {
                return null; // Không tìm thấy tỉnh
            }

            return province.Districts; // Trả về danh sách quận thuộc tỉnh tìm được
        }
    // Controller method to get provinces
    [Route("1234")]
        public IActionResult GetProvinces()
        {
            var provinces =GetProvincesFromFile(); // Gọi phương thức của bạn để lấy dữ liệu từ file
            return Ok(provinces); // Trả về dữ liệu tỉnh thành dưới dạng JSON
        }

        // Controller method to get Cap Bac (Chuc Vu)
        [Route("api/capbac")]
        public IActionResult GetCapBac()
        {
            var capBacs = _context.DanhMucCapBacs.ToList(); // Lấy danh sách Chức Vụ từ bảng DanhMucCapBac
            return Ok(capBacs); // Trả về dữ liệu dưới dạng JSON
        }
        //Sua Nhan Vien
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var employee = _context.NhanViens.Find(id);
            if (employee == null)
            {
                return NotFound();
            }
            var provinces = employee.DiaChi;
            var districts = employee.QueQuan;
            ViewBag.Provinces = provinces;
            ViewBag.Districts = districts;
            return View(employee);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(NhanVien employee)
        {
            if (ModelState.IsValid)
            {
                // Tìm nhân viên trong cơ sở dữ liệu bằng MaNhanVien
                var existingEmployee = _context.NhanViens.Find(employee.MaNhanVien);

                if (existingEmployee == null)
                {
                    // Nếu không tìm thấy nhân viên trong cơ sở dữ liệu
                    ModelState.AddModelError("", "Employee not found.");
                    return View("Edit", employee);
                }

                // Lưu tên tài khoản người dùng vào Session
                var username = HttpContext.Session.GetString("Username");
                if (string.IsNullOrEmpty(username))
                {
                    // Nếu không có tên đăng nhập trong session, người dùng chưa đăng nhập
                    return RedirectToAction("Index", "Home");
                }

                // Cập nhật các thuộc tính của nhân viên
                existingEmployee.HoTen = employee.HoTen;
                existingEmployee.DiaChi = employee.DiaChi;
                existingEmployee.QueQuan = employee.QueQuan;
                existingEmployee.NgayBatDau = employee.NgayBatDau;
                existingEmployee.NgayKetThuc = employee.NgayKetThuc;
                existingEmployee.ChucVu = employee.ChucVu;
                existingEmployee.TrangThai = employee.TrangThai;

                // Lưu thay đổi vào cơ sở dữ liệu
                _context.SaveChanges();

                // Ghi lại thao tác vào bảng ThaoTacSua
                var thaoTacSua = new ThaoTacSua
                {
                    username = username, // Tên tài khoản thực hiện thao tác
                    LoaiThaoTac = "Sửa thông tin nhân viên",
                    NgayThaoTac = DateTime.Now,
                    ChiTietThaoTac = $"Cập nhật thông tin nhân viên với mã {employee.MaNhanVien} bởi {username}."
                };

                // Thêm lịch sử thao tác vào bảng
                _context.ThaoTacSuas.Add(thaoTacSua);
                _context.SaveChanges();

                // Chuyển hướng về trang danh sách nhân viên
                return RedirectToAction("Index", "Home");
            }

            // Nếu có lỗi, hiển thị lại form
            return View("Edit", employee);
        }

        // Xoa San Pham
        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                // Tìm nhân viên theo ID
                var employee = _context.NhanViens.Find(id);
                if (employee == null)
                {
                    return NotFound(); // Trả về lỗi nếu không tìm thấy nhân viên
                }

                // Xóa nhân viên khỏi cơ sở dữ liệu
                _context.NhanViens.Remove(employee);
                _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

                // Kiểm tra Session và lấy tên người dùng
                var username = HttpContext.Session.GetString("Username");
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(); // Trả về lỗi nếu không tìm thấy username trong session
                }
                //kiem tra xem co ton tai username trong session khong
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(); // Trả về lỗi nếu không tìm thấy username trong session
                }
                // Ghi lại thao tác xóa nhân viên vào bảng ThaoTacSua
                var thaoTacSua = new ThaoTacSua
                {
                    username = username, // Tên tài khoản thực hiện thao tác
                    LoaiThaoTac = "Xóa nhân viên",
                    NgayThaoTac = DateTime.Now,
                    ChiTietThaoTac = $"Xóa nhân viên với mã {employee.MaNhanVien} bởi {username}."
                };

                _context.ThaoTacSuas.Add(thaoTacSua);
                _context.SaveChanges(); // Lưu thao tác vào cơ sở dữ liệu

                return RedirectToAction("Index", "Home"); // Chuyển hướng về trang chính
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về một thông báo lỗi
                // Có thể ghi log lỗi ở đây nếu cần thiết
                ModelState.AddModelError("", $"Đã xảy ra lỗi: {ex.Message}");
                return View("Error"); // Có thể hiển thị view lỗi nếu muốn
            }
        }

        // Hien Thi Thao Tac Sua
        [HttpGet]
        public IActionResult ThaoTacSua()
        {
            var user = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(role))
            {
             // Nếu session chưa được thiết lập đúng
             return RedirectToAction("Index", "DangNhap");
            }
            if(role != "ADMIN")
            {
                // Nếu user không phải là Admin
                return RedirectToAction("Index", "Home");
            }

            // Nếu user và role đã có giá trị, tiếp tục với thao tác
            var thaoTacSuas = _context.ThaoTacSuas.ToList();
            return View(thaoTacSuas);
        }

        [HttpGet]
        public IActionResult VanBang()
        {
            var vanBangs = _context.VanBangs.Include(v => v.MaNhanVienNavigation).ToList();
            return View(vanBangs);

        }
        // POST: VanBang/Create4
        [HttpGet]
        public IActionResult SuaVanBang(int id)
        {
            var vanBang = _context.VanBangs.Include(v => v.MaNhanVienNavigation).FirstOrDefault(v => v.MaVanBang == id);

            if (vanBang == null)
            {
                return NotFound();
            }
            var NhanViens = _context.NhanViens.ToList();
            ViewBag.NhanViens = NhanViens;
            return View(vanBang);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaVanBang(int id, VanBang vanBang)
        {
            // Lấy bản ghi Văn Bằng từ cơ sở dữ liệu
            var vanBang1 = _context.VanBangs.Include(v => v.MaNhanVienNavigation).FirstOrDefault(v => v.MaVanBang == id);

            if (vanBang1 == null)
            {
                return NotFound();
            }

            // Cập nhật các thuộc tính của vanBang1 từ đối tượng vanBang
            vanBang1.MaNhanVien = vanBang.MaNhanVien;  // Cập nhật nhân viên
            vanBang1.TenVanBang = vanBang.TenVanBang;  // Cập nhật tên văn bằng
            vanBang1.NoiCap = vanBang.NoiCap;  // Cập nhật nơi cấp
            vanBang1.NamCap = vanBang.NamCap;  // Cập nhật năm cấp

            // Lưu lại các thay đổi
            _context.VanBangs.Update(vanBang1);
            _context.SaveChanges();

            // Chuyển hướng đến danh sách Văn Bằng
            return RedirectToAction("VanBang");
        }
        //tao moi van bang
       public IActionResult ThemVanBang()
        {
            var NhanViens = _context.NhanViens.ToList();
            ViewBag.NhanViens = NhanViens;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemVanBang(VanBang vanBang)
        {
            
           _context.VanBangs.Add(vanBang);
            _context.SaveChanges();
            return RedirectToAction("VanBang");
        }
       
        //xoa van bang

        public IActionResult XoaVanBang(int id)
        {
            var vanBang = _context.VanBangs.Find(id);
            if (vanBang == null)
            {
                return NotFound();
            }

            _context.VanBangs.Remove(vanBang);  // Xóa bản ghi
            _context.SaveChanges();  // Lưu thay đổi

            return RedirectToAction("VanBang");  // Quay lại danh s
        }




        // Trả về thông tin về quyền riêng tư
        public IActionResult Privacy()
        {
            return View();
        }

        // Xử lý lỗi
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    // Lớp đại diện cho Tỉnh Thành
    // Lớp đại diện cho Tỉnh Thành
    public class Ward
    {
        public string Name { get; set; }
        public int Code { get; set; }
        public string DivisionType { get; set; }
        public string Codename { get; set; }
    }

    public class District
    {
        public string Name { get; set; }
        public int Code { get; set; }
        public string DivisionType { get; set; }
        public string Codename { get; set; }
        public int ProvinceCode { get; set; }
        public List<Ward> Wards { get; set; }
    }

    public class Province
    {
        public string Name { get; set; }
        public int Code { get; set; }
        public string DivisionType { get; set; }
        public string Codename { get; set; }
        public int PhoneCode { get; set; }
        public List<District> Districts { get; set; }
    }


}
