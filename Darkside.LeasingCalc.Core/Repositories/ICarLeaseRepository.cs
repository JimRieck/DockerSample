using Darkside.LeasingCalc.Data;
using Darkside.LeasingCalc.Data.Models;
using RevverDigital.Common.Data.Repositories.Interfaces;

namespace Darkside.LeasingCalc.Core.Repositories;

public interface ICarLeaseRepository : IEntityRepository<CarLease>
{
    Task<CarLease?> GetCarByCarNumber(string carNumber);
}