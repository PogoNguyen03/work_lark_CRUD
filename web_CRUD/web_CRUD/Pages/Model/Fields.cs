namespace web_CRUD.Pages.Model
{
    public class Fields
    {
        public bool? CongViec { get; set; }
        public string HoanThanhCongViec { get; set; }
        public string MoTaCongViec { get; set; }
        public List<Person> NguoiHoTroGiaoViec { get; set; }
        public List<Person> NguoiLam { get; set; }
        public string PhongBan { get; set; }
        public int SoNgay { get; set; }
        public long ThoiGianBatDau { get; set; }
        public long ThoiGianKetThuc { get; set; }
        public string TenTask { get; set; }
        public string TinhTrang { get; set; }
        public string DoUuTien { get; set; }
    }
}
