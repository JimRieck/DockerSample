using RevverDigital.Common.Data.Repositories;
using Darkside.LeasingCalc.Data.Models;
using Microsoft.EntityFrameworkCore;
using IUser = RevverDigital.Common.Data.Models.Interfaces.IUser;

namespace Darkside.LeasingCalc.Core.Repositories;

public class LeaseCalculatorRepository : EntityRepository<DailyMilageDetail>, ILeaseCalculatorRepository
{
    private readonly DbContext _dbContext;
    public LeaseCalculatorRepository(DbContext db, IUser userInfo = null) : base(db, userInfo)
    {
        _dbContext = db;
        
    }
}