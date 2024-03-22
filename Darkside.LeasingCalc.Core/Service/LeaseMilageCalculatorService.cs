using Darkside.LeasingCalc.Contracts.Helpers;
using Darkside.LeasingCalc.Contracts.Requests;
using Darkside.LeasingCalc.Contracts.Response;
using Darkside.LeasingCalc.Core.Repositories;
using Darkside.LeasingCalc.Core.Validation;
using Darkside.LeasingCalc.Data.Models;

namespace Darkside.LeasingCalc.Core.Service;

public class LeaseMilageCalculatorService : ILeaseCalculatorService
{
    private readonly ILeaseCalculatorRepository _leaseCalculatorRepository;
    private readonly ICarLeaseRepository _carLeaseRepository;
    private readonly IValidationService _validationService;

    public LeaseMilageCalculatorService(ILeaseCalculatorRepository leaseCalculatorRepository, ICarLeaseRepository carLeaseRepository, IValidationService validationService)
    {
        _leaseCalculatorRepository = leaseCalculatorRepository;
        _carLeaseRepository = carLeaseRepository;
        _validationService = validationService;
    }

    public async Task<DailyMileageCalcResponse> CalculateDailyMilage(DailyMileageCalcRequest request)
    {
        var response = new DailyMileageCalcResponse() {Success = true, DailyMilageDetails = new List<DailyMileageDetails>() };
        var validationResult = _validationService.IsValid(new DailyMilageCalcRequestValidator(), request);
        if (!validationResult.IsValid)
        {
            response = new DailyMileageCalcResponse()
            {
                Success = false,
                Message = $"Validation Errors: {_validationService.GetValidationErrors(validationResult)}"
            };
        }
        else
        {
            var (foundCar, car) = await FindCarNumber(request);
            var carLeaseId = new Guid();
            if (!foundCar)
            {
                carLeaseId = car.Id;
                var mileageDetails = await CalculateDailyMiles(request);
                await SaveMilageEstimates(mileageDetails, carLeaseId);
            }

            var carLeaseDetails = await _carLeaseRepository.GetByIdAsync(carLeaseId);
            response.TotalYears = carLeaseDetails.TotalYears.Value;
            response.CarNumber = carLeaseDetails.CarNumber;
            response.CustomerName = carLeaseDetails.CustomerName;
            response.StartDateTime = carLeaseDetails.StartDate.Value;
            response.StartingMileage = carLeaseDetails.StartingMileage.Value;
            response.YearlyMiles = (int)carLeaseDetails.YearlyMiles.Value;

            var carCalcDetails = await _leaseCalculatorRepository.SearchAsync(p => p.CarLeaseId == carLeaseId);

            foreach (var calcDetail in carCalcDetails)
            {
                response.DailyMilageDetails.Add(new DailyMileageDetails()
                {
                    ExpectedMileage = calcDetail.ExpectedMilage.Value,
                    MileageDifference = calcDetail.MilageDifference.Value,
                    ActualMilage = calcDetail.ActualMileage.Value,
                    MilesDriven = calcDetail.MilesDriven.Value,
                    ExpectedMilesDriven = calcDetail.ExpectedMileDriven.Value,
                    IsDeleted = false,
                    CreatedBy = "system",
                    CarLeaseId = carLeaseId,
                    UpdatedBy = "system",
                    MileageDate = calcDetail.MilageDate.Value,
                    UpdatedDate = DateTime.Now
                });
            }
        }

        return response;
    }

    private async Task SaveMilageEstimates(DailyMileageCalcResponse mileageDetails, Guid carLeaseId)
    {
        try
        {
            foreach (var detail in mileageDetails.DailyMilageDetails)
            {
                var newModel = detail.ToModel(carLeaseId);
                await _leaseCalculatorRepository.InsertAsync(newModel);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task<DailyMileageCalcResponse> CalculateDailyMiles(DailyMileageCalcRequest request)
    {
        try
        {


            //Calculation Here
            var response = new DailyMileageCalcResponse() { DailyMilageDetails = new List<DailyMileageDetails>() };
            var leaseEndDate = request.StartDate.AddYears(request.TotalYears);
            var totalMilesForLease = request.YearlyMiles * request.TotalYears;
            var totalDaysForLease = request.TotalYears * 365;
            var milesPerDay = totalMilesForLease / totalDaysForLease;
            var currentDay = request.StartDate;
            var previousDayMiles = request.StartingMileage;
            var previousDaysMilageDifference = 0;
            var rnd = new Random();
            var takeDays = TakeDaysHelper.CalculateTakeDays(request.StartDate);
            for (var loopCounter = 0; loopCounter < takeDays; loopCounter++)
            {
                //Expected Miles - Previous Day's mileage + miles Per Day - done
                //Miles Driven - Random number from 1-100.
                //Total Miles on Car - Previous Day's mileage + miles driven
                //Mileage Difference Total Miles on Car - Expected miles.

                var details = new DailyMileageDetails();
                details.MileageDate = currentDay;
                details.ExpectedMilesDriven = milesPerDay;
                details.MilesDriven = rnd.Next(100);
                details.ExpectedMileage = previousDayMiles + milesPerDay;
                details.ActualMilage = previousDayMiles + details.MilesDriven;
                details.MileageDifference = previousDaysMilageDifference + (details.ActualMilage - details.ExpectedMileage); 

                previousDaysMilageDifference = details.MileageDifference;
                response.DailyMilageDetails.Add(details);
                currentDay = currentDay.AddDays(1);
                previousDayMiles += details.MilesDriven;
            }

            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task<(bool, CarLease)> FindCarNumber(DailyMileageCalcRequest carInfo)
    {
        var returnedCar = new CarLease();
        var carFound = false;

        //TODO: Save Data here.  be sure to get the Car Lease ID.
        var car = await _carLeaseRepository.GetCarByCarNumber(carInfo.CarNumber);
        var carLeaseId = Guid.Empty;
        if (car != null)
        {
            carLeaseId = car.Id;
            carFound = true;
            returnedCar = car;
        }
        else
        {
            var newCar = await _carLeaseRepository.InsertAsync(carInfo.ToModel());
            carLeaseId = newCar.Id;
            carFound = false;
            returnedCar = newCar;
        }

        return (carFound, returnedCar);
    }
}