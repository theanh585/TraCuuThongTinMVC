namespace TraCuuThongTinMVC.ViewModels
{
    public class MostSearchedSchool
    {
        public string SchoolId { get; set; }
        public string SchoolNm { get; set; }
        public int SearchCount { get; set; }
    }

    public class MostSearchedProfession
    {
        public string ProfessionId { get; set; }
        public string ProfessionNm { get; set; }
        public int SearchCount { get; set; }
    }

    public class SearchAndSelectRate
    {
        public string SchoolId { get; set; }
        public string ProfessionId { get; set; }
        public string SchoolNm { get; set; }
        public string ProfessionNm { get; set; }
        public double SelectRate { get; set; }
    }
    public class ReportViewModel
    {
        public List<MostSearchedSchool> MostSearchedSchools { get; set; }
        public List<MostSearchedProfession> MostSearchedProfessions { get; set; }
        public List<SearchAndSelectRate> SearchAndSelectRates { get; set; }
    }
}
