using FluentValidation;
using FluentValidation.Results;

namespace Darkside.LeasingCalc.Core.Validation;

public interface IValidationService
{
    ValidationResult IsValid<T, T1>(T validator, T1 request) where T : IValidator<T1>;
    string GetValidationErrors(ValidationResult validationResult);
}