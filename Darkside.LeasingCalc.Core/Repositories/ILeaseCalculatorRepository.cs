using RevverDigital.Common.Data.Repositories.Interfaces;
using Darkside.LeasingCalc.Data;
using Darkside.LeasingCalc.Data.Models;

namespace Darkside.LeasingCalc.Core.Repositories;

public interface ILeaseCalculatorRepository : IEntityRepository<DailyMilageDetail>
{
}