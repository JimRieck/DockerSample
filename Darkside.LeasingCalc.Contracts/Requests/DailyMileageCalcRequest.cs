namespace Darkside.LeasingCalc.Contracts.Requests
{
    public class DailyMileageCalcRequest
    {
        public string CarNumber { get; set; }
        public string CustomerName { get; set; }
        public int YearlyMiles { get; set; }
        public DateTime StartDate { get; set; }
        public int TotalYears { get; set; }
        public int StartingMileage { get; set; }
        public int CurrentMileage { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

    }
}