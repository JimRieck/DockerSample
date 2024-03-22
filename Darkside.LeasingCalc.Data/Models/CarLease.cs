﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Darkside.LeasingCalc.Data.Models;

public partial class CarLease
{
    public Guid Id { get; set; }

    public string CarNumber { get; set; }

    public string CustomerName { get; set; }

    public decimal? YearlyMiles { get; set; }

    public DateTime? StartDate { get; set; }

    public int? TotalYears { get; set; }

    public int? StartingMileage { get; set; }

    public int? CurrentMileage { get; set; }

    public bool IsDeleted { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; }

    public DateTime UpdatedDate { get; set; }
}