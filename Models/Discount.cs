using System;
using System.Collections.Generic;

namespace BakeryApp.Models;

public partial class Discount
{
    public int Id { get; set; }

    public int? CustomerId { get; set; }

    public int? Discount1 { get; set; }

    public int? Activation { get; set; }
}
