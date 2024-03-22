using Darkside.LeasingCalc.Core.Validation;
using FluentValidation;
using FluentValidation.Results;

namespace Darkside.LeasingCalc.Core.Service;

public class ValidationService : IValidationService
{
    public ValidationResult IsValid<T, T1>(T validator, T1 request) where T : IValidator<T1>
    {
        return validator.Validate(request);
    }

    public string GetValidationErrors(ValidationResult validationResult)
    {
        return validationResult.Errors.Aggregate(string.Empty, (current, error) => current + $"{error.ErrorMessage}.");
    }
}