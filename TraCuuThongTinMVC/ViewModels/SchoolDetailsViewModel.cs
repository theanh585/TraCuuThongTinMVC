using TraCuuThongTinMVC.Data;

namespace TraCuuThongTinMVC.ViewModels
{
    public class SchoolDetailsViewModel
    {
        public InfSch School { get; set; } // Thông tin trường học
        public IEnumerable<ProfessionDb> Professions { get; set; } // Danh sách ngành nghề
    }

}
