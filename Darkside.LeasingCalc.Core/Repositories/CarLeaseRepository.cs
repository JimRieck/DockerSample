using Darkside.LeasingCalc.Data;
using Darkside.LeasingCalc.Data.Models;
using Microsoft.EntityFrameworkCore;
using RevverDigital.Common.Data.Models.Interfaces;
using RevverDigital.Common.Data.Repositories;
using RevverDigital.Common.Data.Repositories.Interfaces;

namespace Darkside.LeasingCalc.Core.Repositories;

public class CarLeaseRepository : EntityRepository<CarLease>, ICarLeaseRepository
{
    private readonly DbContext _dbContext;
    public CarLeaseRepository(DbContext db, IUser userInfo = null) : base(db, userInfo)
    {
        _dbContext = db;

    }

    public async Task<CarLease?> GetCarByCarNumber(string carNumber)
    {
        var cars = await SearchAsync(p => p.CarNumber == carNumber && !p.IsDeleted);

        return cars?.FirstOrDefault();
    }

   
}

