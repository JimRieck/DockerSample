namespace Darkside.LeasingCalc.Contracts.Helpers;

public static class TakeDaysHelper
{
    public static int CalculateTakeDays(DateTime leaseStartDate)
    {
        var firstDate = leaseStartDate;
        var today = DateTime.Now;
        var takeDays = today - firstDate;
        
        return takeDays.Days;
    }
}