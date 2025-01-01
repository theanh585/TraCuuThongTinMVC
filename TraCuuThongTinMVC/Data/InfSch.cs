using System;
using System.Collections.Generic;

namespace TraCuuThongTinMVC.Data;

public partial class InfSch
{
    public string ScId { get; set; } = null!;

    public string? ScNm { get; set; }

    public string? ScArea { get; set; }

    public string? ScAddress { get; set; }

    public string? ScAddress1 { get; set; }

    public string? ScTcd { get; set; }

    public string? Tel { get; set; }

    public string? Fax { get; set; }

    public int? Count { get; set; }

    public string? Mail { get; set; }

    public string? Website { get; set; }

    public string? HiuTruong { get; set; }

    public string? HiuPho { get; set; }

    public string? NguoiChiuTrachNhiemPhapLuat { get; set; }

    public byte[]? Image { get; set; }
}
