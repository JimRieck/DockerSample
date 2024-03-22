namespace Darkside.LeasingCalc.Contracts.Response;

public interface IApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
}