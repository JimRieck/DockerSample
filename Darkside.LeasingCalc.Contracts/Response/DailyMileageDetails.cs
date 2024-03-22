namespace Darkside.LeasingCalc.Contracts.Response;

public class DailyMileageDetails 
{
    public DateTime MileageDate { get; set; }
    public int ExpectedMilesDriven { get; set; }
    public int MilesDriven { get; set; }
    public int ExpectedMileage { get; set; }
    public int ActualMilage { get; set; }
    public int MileageDifference { get; set; }
    public Guid CarLeaseId { get; set; }
    public bool IsDeleted { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public string UpdatedBy  {  get; set; }
    public DateTime UpdatedDate { get; set; }
}