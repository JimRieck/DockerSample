namespace Darkside.LeasingCalc.Contracts.Response;

public class QueueStatusResponse
{
    public string id { get; set; }
    public string purgeHistoryDeleteUri { get; set; }
    public string sendEventPostUri { get; set; }
    public string statusQueryGetUri { get; set; }
    public string terminatePostUri { get; set; }
}