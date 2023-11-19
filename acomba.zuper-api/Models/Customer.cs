using System;
using System.Collections.Generic;

namespace acomba.zuper_api.Models;

public class Customer
{
    public long Id { get; set; }

    public string? CustomerId { get; set; }

    public string? CustomerFirstName { get; set; }

    public string? CustomerLastName { get; set; }

    public string? CustomerEmail { get; set; }

    public string? CompanyUid { get; set; }
}
