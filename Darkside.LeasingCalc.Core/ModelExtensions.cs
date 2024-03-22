using Darkside.LeasingCalc.Contracts.Requests;
using Darkside.LeasingCalc.Contracts.Response;
using Darkside.LeasingCalc.Data;
using Darkside.LeasingCalc.Data.Models;
using Microsoft.EntityFrameworkCore.Query;

namespace Darkside.LeasingCalc.Core;

public static class ModelExtensions
{
    public static DailyMilageDetail ToModel(this DailyMileageDetails details, Guid carLeaseId)
    {
        return new DailyMilageDetail()
        {
            MilageDate = details.MileageDate,
            ExpectedMileDriven = details.ExpectedMilesDriven,
            ExpectedMilage = details.ExpectedMileage,
            MilageDifference = details.MileageDifference,
            ActualMileage = details.ActualMilage,
            MilesDriven = details.MilesDriven,
            CarLeaseId = carLeaseId,
            IsDeleted = false,
            CreatedBy = "system",
            CreatedDate = DateTime.Now,
            UpdatedBy = "system",
            UpdatedDate = DateTime.Now,
        };
    }

    public static CarLease ToModel(this DailyMileageCalcRequest request)
    {
        return new CarLease()
        {
            CarNumber = request.CarNumber,
            CreatedBy = "System",
            CreatedDate = DateTime.Now,
            UpdatedBy = "System",
            UpdatedDate = DateTime.Now,
            CurrentMileage = request.CurrentMileage,
            StartingMileage = request.StartingMileage,
            YearlyMiles = request.YearlyMiles,
            CustomerName = request.CustomerName,
            IsDeleted = false,
            StartDate = request.StartDate,
            TotalYears = request.TotalYears,
            
            
        };
    }
}