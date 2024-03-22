
using Darkside.LeasingCalc.Contracts.Requests;
using Darkside.LeasingCalc.Contracts.Response;
using Darkside.LeasingCalc.Data;
using RevverDigital.Common.Data.Repositories.Interfaces;

namespace Darkside.LeasingCalc.Core.Service;

public interface ILeaseCalculatorService
{
    Task<DailyMileageCalcResponse> CalculateDailyMilage(DailyMileageCalcRequest request);
}