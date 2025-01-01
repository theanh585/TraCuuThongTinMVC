using System;
using System.Collections.Generic;

namespace TraCuuThongTinMVC.Data;

public partial class ProfessionDb
{
    public string Cd { get; set; } = null!;

    public string? Nm { get; set; }

    public string? ScId { get; set; }

    public int? Count { get; set; }

    public string? MoTa { get; set; }

    public string? GvCn { get; set; }
}
