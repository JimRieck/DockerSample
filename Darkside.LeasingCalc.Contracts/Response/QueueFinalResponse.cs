namespace Darkside.LeasingCalc.Contracts.Response;

public class QueueFinalResponse
{
    public string name { get; set; }
    public string instanceId { get; set; }
    public string runtimeStatus { get; set; }
    public string customStatus { get; set; }
    public input input { get; set; }
    public List<DailyMileageDetails> output { get; set; }
}