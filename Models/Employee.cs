using System;
using System.Collections.Generic;

namespace BakeryApp.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Position { get; set; } = null!;

    public DateOnly HireDate { get; set; }

    public decimal Salary { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
