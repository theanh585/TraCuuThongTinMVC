using Microsoft.AspNetCore.Mvc;
using TraCuuThongTinMVC.Data;
using TraCuuThongTinMVC.ViewModels;

public class ReportController : Controller
{
    private readonly SchoolContext _context;

    public ReportController(SchoolContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        // Báo cáo thống kê các trường được tìm kiếm nhiều nhất
        var mostSearchedSchools = _context.InfSches
            .GroupBy(s => s.ScNm) 
            .Select(group => new MostSearchedSchool
            {
                SchoolNm = group.Key,      
                SearchCount = (int)group.Sum(s => s.Count)
            })
            .OrderByDescending(s => s.SearchCount) 
            .Take(10)                             
            .ToList();

        // Báo cáo thống kê các chuyên ngành được tìm kiếm nhiều nhất
        var mostSearchedProfessions = _context.ProfessionDbs
            .GroupBy(p => p.Nm) // Nhóm theo Nm (tên chuyên ngành)
            .Select(group => new MostSearchedProfession
            {
                ProfessionNm = group.Key,          // Tên chuyên ngành
                SearchCount = (int)group.Sum(p => p.Count) // Tổng Count cho tất cả các bản ghi cùng Nm
            })
            .OrderByDescending(p => p.SearchCount) // Sắp xếp giảm dần theo tổng Count
            .Take(10)                              // Giới hạn 10 kết quả
            .ToList();


                // Trả về tất cả dữ liệu cho view
        var viewModel = new ReportViewModel
        {
            MostSearchedSchools = mostSearchedSchools,
            MostSearchedProfessions = mostSearchedProfessions,
        };

        return View(viewModel);
    }

    [HttpGet]
    public JsonResult GetSchoolsByProfession(string professionNm)
    {
        var schoolsWithProfession = _context.ProfessionDbs
            .Where(p => p.Nm == professionNm)
            .GroupBy(p => p.ScId)
            .Select(group => new
            {
                SchoolId = group.Key,
                SchoolNm = _context.InfSches
                            .Where(s => s.ScId == group.Key)
                            .Select(s => s.ScNm)
                            .FirstOrDefault(),
                SearchCount = group.Sum(p => p.Count) // Tổng số Count từ bảng ProfessionDbs
            })
            .OrderByDescending(s => s.SearchCount) // Sắp xếp giảm dần theo Count
            .ToList();

        return Json(schoolsWithProfession);
    }

}
