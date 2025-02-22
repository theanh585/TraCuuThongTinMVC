using System.Text;
using DinkToPdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using TraCuuThongTinMVC.Data;
using TraCuuThongTinMVC.ViewModels;


namespace TraCuuThongTinMVC.Controllers
{
    public class SchoolListController : Controller
    {
        private readonly SchoolContext db;
        private readonly IViewEngine _viewEngine;

        public SchoolListController(SchoolContext conetxt, IViewEngine viewEngine)
        {
            db = conetxt;
            _viewEngine = viewEngine;
        }
        public IActionResult Index(string ScNm, string Nm, string ScTcd, string ScArea, int? page)
        {
            var schoolList = db.InfSches.AsQueryable();

            if (!string.IsNullOrEmpty(ScNm))
            {
                schoolList = schoolList.Where(p => p.ScNm.Contains(ScNm));
            }

            if (!string.IsNullOrEmpty(ScArea))
            {
                schoolList = schoolList.Where(p => p.ScArea == ScArea);
            }

            if (!string.IsNullOrEmpty(ScTcd))
            {
                schoolList = schoolList.Where(p => p.ScTcd == ScTcd);
            }

            if (!string.IsNullOrEmpty(Nm))
            {
                var professionList = db.ProfessionDbs
                    .Where(p => p.Nm == Nm)
                    .Select(p => p.ScId)
                    .Distinct();

                schoolList = schoolList.Where(p => professionList.Contains(p.ScId));
            }

            int pageSize = 6;
            int pageNumber = page ?? 1;
            var pagedResult = schoolList
            .Skip((pageNumber - 1) * pageSize) // Bỏ qua số lượng item đã được phân trang
            .Take(pageSize) // Lấy số lượng item theo pageSize
            .Select(p => new SchoolList
            {
                scId = p.ScId,
                scNm = p.ScNm,
                View = p.Count ?? 0,
                ScArea = CityHelper.GetCityName(p.ScArea),
                Image = p.Image != null ? Convert.ToBase64String(p.Image) : null,
            }).ToList();

            int totalRecords = schoolList.Count();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            ViewData["ScNm"] = ScNm;
            ViewData["Nm"] = Nm;
            ViewData["ScTcd"] = ScTcd;
            ViewData["ScArea"] = ScArea;
            ViewData["TotalPages"] = totalPages;
            ViewData["CurrentPage"] = pageNumber;

            return View(pagedResult);
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
                ScArea = CityHelper.GetCityName(data.ScArea),
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
                profList = profList ?? new List<Profession>(),
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
                GvCn = data.GvCn,
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
            csvBuilder.AppendLine("**Tiêu đề**,**Thông Tin**");

            // Thêm thông tin về trường vào CSV
            csvBuilder.AppendLine($"Mã trường,{school.ScId, -20}");
            csvBuilder.AppendLine($"Địa chỉ chính,{school.ScAddress,-20}");
            csvBuilder.AppendLine($"Địa chỉ các cơ sở,{school.ScAddress1,-20}");
            csvBuilder.AppendLine($"Tel,{school.Tel,-20}");
            csvBuilder.AppendLine($"Fax,{school.Fax,-20}");
            csvBuilder.AppendLine($"Mail,{school.Mail,-20}");
            csvBuilder.AppendLine($"Website,{school.Website,-20}");
            csvBuilder.AppendLine($"Hiệu Trưởng,{school.HiuTruong,-20}");
            csvBuilder.AppendLine($"Hiệu Phó,{school.HiuPho,-20}");
            csvBuilder.AppendLine($"Đại diện pháp luật,{school.NguoiChiuTrachNhiemPhapLuat,-20}");
            csvBuilder.AppendLine("");
            // Thêm các chuyên ngành vào CSV
            csvBuilder.AppendLine("**Chuyên Ngành**");
            foreach (var profession in professions)
            {
                csvBuilder.AppendLine(profession.Nm);
            }

            // Trả về file CSV với BOM (UTF-8)
            var fileName = $"{school.ScNm}_Information.csv";
            return File(Encoding.UTF8.GetBytes(csvBuilder.ToString()), "text/csv", fileName);
        }

        public async Task<string> RenderViewToString(string viewName, object model)
        {
            var actionContext = new ActionContext(HttpContext, RouteData, ControllerContext.ActionDescriptor);
            var viewResult = _viewEngine.FindView(actionContext, viewName, isMainPage: true);

            if (!viewResult.Success)
            {
                var searchedLocations = string.Join(", ", viewResult.SearchedLocations ?? Enumerable.Empty<string>());
                throw new InvalidOperationException($"Không thể tìm thấy view '{viewName}'. Đã tìm trong: {searchedLocations}");
            }


            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), ModelState)
            {
                Model = model
            };

            using (var stringWriter = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewData,
                    TempData,
                    stringWriter,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return stringWriter.ToString();
            }
        }

        public async Task<IActionResult> DownloadReport(string schoolId)
        {   
            var profList = await db.ProfessionDbs
                       .Where(p => p.ScId == schoolId)
                       .Select(p => new Profession
                       {
                           Nm = p.Nm,
                       }).ToListAsync();

            var schoolDetail = await db.InfSches
                                       .Where(s => s.ScId == schoolId)
                                       .Select(s => new SchoolDetail
                                       {
                                           ScId = s.ScId,
                                           ScNm = s.ScNm,
                                           ScArea = CityHelper.GetCityName(s.ScArea),
                                           ScAddress = s.ScAddress,
                                           ScTcd = s.ScTcd,
                                           Tel = s.Tel,
                                           Fax = s.Fax,
                                           Mail = s.Mail,
                                           Website = s.Website,
                                           HieuT = s.HiuTruong,
                                           HieuP = s.HiuPho,
                                           NgChTrN = s.NguoiChiuTrachNhiemPhapLuat,
                                           Image = s.Image,
                                           profList = profList ?? new List<Profession>(),
                                       }).FirstOrDefaultAsync();

            if (schoolDetail == null)
            {
                return NotFound();
            }

            // Render view thành HTML
            string fullHtmlContent = await RenderViewToString("SchoolDetailReport", schoolDetail);
            if (!fullHtmlContent.Contains("<div id=\"reportBody\">"))
            {
                throw new InvalidOperationException("The rendered view does not contain the expected <div id=\"reportBody\"> tag.");
            }
            string bodyContent = ExtractBodyContent(fullHtmlContent);

            // Tạo đối tượng HtmlToPdfDocument
            var converter = new BasicConverter(new PdfTools());
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Portrait,
            PaperSize = PaperKind.A4,
        },
                Objects = {
            new ObjectSettings() {
                HtmlContent = bodyContent,
                WebSettings = { DefaultEncoding = "utf-8" }
            }
        }
            };

            // Chuyển đổi HTML thành PDF
            byte[] pdf = converter.Convert(doc);

            // Trả về file PDF
            return File(pdf, "application/pdf", "SchoolDetailReport.pdf");
        }

        private string ExtractBodyContent(string fullHtmlContent)
        {
            var startTag = "<div id=\"reportBody\">";
            var endTag = "</div>";

            // Kiểm tra sự tồn tại của startTag và endTag
            int startIndex = fullHtmlContent.IndexOf(startTag);
            if (startIndex == -1)
            {
                throw new ArgumentException($"Start tag '{startTag}' not found in the provided HTML content.");
            }

            int endIndex = fullHtmlContent.IndexOf(endTag, startIndex);
            if (endIndex == -1)
            {
                throw new ArgumentException($"End tag '{endTag}' not found in the provided HTML content.");
            }

            // Lấy nội dung giữa startTag và endTag
            startIndex += startTag.Length;
            return fullHtmlContent.Substring(startIndex, endIndex - startIndex).Trim();
        }

    }
}
