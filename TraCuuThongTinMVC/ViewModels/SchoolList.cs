namespace TraCuuThongTinMVC.ViewModels
{
    public class SchoolList
    {
        public string scId { get; set; }
        public string scNm { get; set; }
        public string ScArea { get; set; }
        public int View { get; set; }
        public string Image { get; set; }
    }

    public class SchoolDetail
    {
        public string ScId { get; set; }
        public string ScNm { get; set; }
        public string ScArea { get; set; }
        public string ScAddress { get; set; }
        public string ScAddress1 { get; set; }
        public string ScTcd { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string Mail { get; set; }
        public string Website { get; set; }
        public int View { get; set; }
        public string HieuT { get; set; }
        public string HieuP { get; set; }
        public string NgChTrN { get; set; }
        public byte[] Image { get; set; }
        public List<Profession> profList { get; set; }
    }
}
