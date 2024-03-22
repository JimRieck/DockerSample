using Darkside.LeasingCalc.Contracts.Requests;
using FluentValidation;

namespace Darkside.LeasingCalc.Core.Validation;
public class DailyMilageCalcRequestValidator : AbstractValidator<DailyMileageCalcRequest>
{
    public DailyMilageCalcRequestValidator()
    {
        RuleFor(x => x.CarNumber).NotEmpty().WithMessage("The car number value cannot be empty.");
        RuleFor(x => x.TotalYears).GreaterThan(0);
        RuleFor(x => x.StartDate).LessThanOrEqualTo(DateTime.Now);
        RuleFor(x => x.CustomerName).NotEmpty();
        RuleFor(x => x.CurrentMileage).GreaterThan(0);
        RuleFor(x => x.StartingMileage).GreaterThan(0);
        RuleFor(x => x.YearlyMiles).GreaterThan(0);
    }
}