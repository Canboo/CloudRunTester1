using System;
using System.Collections.Generic;

namespace api.Models;

public partial class TaiwanCity
{
    public int Id { get; set; }

    public string CityName { get; set; } = null!;

    public string Region { get; set; } = null!;
}
