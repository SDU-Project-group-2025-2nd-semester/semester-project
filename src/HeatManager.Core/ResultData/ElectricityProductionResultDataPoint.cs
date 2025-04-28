namespace HeatManager.Core.ResultData;

public class ElectricityProductionResultDataPoint : IElectricityProductionResultDataPoint
{
    public DateTime TimeFrom { get; }
    public DateTime TimeTo { get; }
    public decimal ElectricityPrice { get; }
    public double ElectricityProduction { get; }
    
    public ElectricityProductionResultDataPoint(DateTime timeFrom, DateTime timeTo, decimal electricityPrice, double electricityProduction)
    {
        TimeFrom = timeFrom;
        TimeTo = timeTo;
        ElectricityPrice = electricityPrice;
        ElectricityProduction = electricityProduction;
    }
}