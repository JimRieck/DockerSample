namespace Darkside.LeasingCalc.Contracts.Response
{
    public class DailyMileageCalcResponse : IApiResponse
    {
        public string CarNumber { get; set; }
        public string CustomerName { get; set; }
        public int YearlyMiles { get; set; }
        public DateTime StartDateTime { get; set; }
        public int TotalYears { get; set; }
        public int StartingMileage { get; set; }
        public int CurrentMileage { get; set; }

        public List<DailyMileageDetails> DailyMilageDetails { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}