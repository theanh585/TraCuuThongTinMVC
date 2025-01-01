using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TraCuuThongTinMVC.Data;
using TraCuuThongTinMVC.ViewModels;

namespace TraCuuThongTinMVC.Controllers
{
    public class SchoolListController : Controller
    {
        private readonly SchoolContext db;

        public SchoolListController(SchoolContext conetxt)
        {
            db = conetxt;
        }
        public IActionResult Index(string ScNm, string Nm, string ScTcd, string ScArea)
        {
            var schoolList = db.InfSches.AsQueryable();

            if (ScNm != null)
            {
                schoolList = schoolList.Where(p => p.ScNm.Contains(ScNm));
            }

            if (ScArea != null)
            {
                schoolList = schoolList.Where(p => p.ScArea == ScArea);
            }

            if (ScTcd != null)
            {
                schoolList = schoolList.Where(p => p.ScTcd == ScTcd);
            }

            if (Nm != null)
            {
                var professionList = db.ProfessionDbs.AsQueryable().Where(p => p.Nm == Nm).Select(p => p.ScId).ToList();
                if (professionList.Any())
                {
                    schoolList = schoolList.Where(p => professionList.Contains(p.ScId));
                }
            }
           

            var result = schoolList.Select(p => new SchoolList
            {
                scId = p.ScId,
                scNm = p.ScNm,
            });

            ViewData["ScNm"] = ScNm;
            ViewData["Nm"] = Nm;
            ViewData["ScTcd"] = ScTcd;
            ViewData["ScArea"] = ScArea;

            return View(result);
        }

        public async Task<IActionResult> Detail(string id)
        {
            var data = await db.InfSches.FindAsync(id);

            if (data == null)
            {
                return NotFound();
            }

            if (data.Count.HasValue)
            {
                data.Count = data.Count.Value + 1;  // Tăng Count nếu nó có giá trị
            }
            else
            {
                data.Count = 1;  // Nếu Count là null, khởi tạo giá trị ban đầu là 1
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            db.InfSches.Update(data);
            await db.SaveChangesAsync();
            // Lọc các chuyên ngành và xử lý nullable Count
            var profList = await db.ProfessionDbs
                                   .Where(p => p.ScId == id)
                                   .Select(p => new Profession
                                   {
                                       Cd = p.Cd,
                                       Nm = p.Nm,
                                       View = p.Count.HasValue ? p.Count.Value : 0,  // Kiểm tra và sử dụng GetValueOrDefault nếu là nullable
                                   }).ToListAsync();

            var result = new SchoolDetail
            {
                ScId = data.ScId,
                ScNm = data.ScNm,
                ScArea = data.ScArea,
                ScAddress = data.ScAddress,
                ScAddress1 = data.ScAddress,
                ScTcd = data.ScTcd,
                Tel = data.Tel,
                Fax = data.Fax,
                Mail = data.Mail,
                Website = data.Website,
                View = data.Count.HasValue ? data.Count.Value : 0,  // Kiểm tra và sử dụng GetValueOrDefault nếu là nullable
                HieuT = data.HiuTruong,
                HieuP = data.HiuPho,
                NgChTrN = data.NguoiChiuTrachNhiemPhapLuat,
                Image = data.Image,
                profList = profList,
            };

            return View(result);
        }


        public async Task<IActionResult> ProfessionDetail(string id)
        {
            var data = await db.ProfessionDbs.FindAsync(id);

            if (data == null)
            {
                return NotFound();
            }

            if (data.Count.HasValue)
            {
                data.Count = data.Count.Value + 1;  // Tăng Count nếu nó có giá trị
            }
            else
            {
                data.Count = 1;  // Nếu Count là null, khởi tạo giá trị ban đầu là 1
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            db.ProfessionDbs.Update(data);
            await db.SaveChangesAsync();
           

            var result = new ProfessionDetail
            {
               
                Nm = data.Nm,
                Mota = data.MoTa,
            };

            return View(result);
        }

        public async Task<IActionResult> GetImage(string id)
        {
            var infSch = await db.InfSches.FindAsync(id);
            if (infSch == null || infSch.Image == null)
            {
                return NotFound();
            }

            return File(infSch.Image, "image/jpeg");
        }

        [HttpPost]
        public IActionResult DownloadInformation(string schoolId)
        {
            var school = db.InfSches.FirstOrDefault(s => s.ScId == schoolId);
            var professions = db.ProfessionDbs.Where(p => p.ScId == schoolId).ToList();

            if (school == null)
            {
                return NotFound();
            }

            // Tạo một CSV file với BOM (Byte Order Mark) cho UTF-8
            var csvBuilder = new StringBuilder();

            // Thêm BOM UTF-8 vào đầu file
            csvBuilder.Append("\uFEFF");

            // Tiêu đề của file CSV
            csvBuilder.AppendLine("Tiêu đề,Thông Tin");

            // Thêm thông tin về trường vào CSV
            csvBuilder.AppendLine($"Mã trường,{school.ScId}");
            csvBuilder.AppendLine($"Địa chỉ chính,{school.ScAddress}");
            csvBuilder.AppendLine($"Địa chỉ các cơ sở,{school.ScAddress1}");
            csvBuilder.AppendLine($"Tel,{school.Tel}");
            csvBuilder.AppendLine($"Fax,{school.Fax}");
            csvBuilder.AppendLine($"Mail,{school.Mail}");
            csvBuilder.AppendLine($"Website,{school.Website}");
            csvBuilder.AppendLine($"Hiệu Trưởng,{school.HiuTruong}");
            csvBuilder.AppendLine($"Hiệu Phó,{school.HiuPho}");
            csvBuilder.AppendLine($"Đại diện pháp luật,{school.NguoiChiuTrachNhiemPhapLuat}");

            // Thêm các chuyên ngành vào CSV
            csvBuilder.AppendLine("Chuyên Ngành");
            foreach (var profession in professions)
            {
                csvBuilder.AppendLine(profession.Nm);
            }

            // Trả về file CSV với BOM (UTF-8)
            var fileName = $"{school.ScNm}_Information.csv";
            return File(Encoding.UTF8.GetBytes(csvBuilder.ToString()), "text/csv", fileName);
        }


    }
}
