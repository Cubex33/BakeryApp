using System;
using System.Collections.Generic;

namespace BakeryApp.Models;

public partial class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int EmployeeId { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
