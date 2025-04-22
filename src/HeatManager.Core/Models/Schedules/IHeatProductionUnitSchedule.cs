namespace HeatManager.Core.Models.Schedules;

public interface IHeatProductionUnitSchedule /*: IHeatProductionUnit*/
{
    public string Name { get; }
    List<IHeatProductionUnitResultDataPoint> DataPoints { get; }
    //Heat Production
    double[] HeatProduction { get;  }
    double TotalHeatProduction { get;  }
    double MaxHeatProduction { get;  }
    
    //Emissions
    public double[] Emissions { get;  }
    public double TotalEmissions { get;  }
    public double MaxEmissions { get;  }
    
    //Costs
    decimal[] Costs { get;  }
    decimal TotalCost { get;  }
    decimal MaxCost { get;  }
    
    //Resources
    double[] ResourceConsumption { get;  }
    double TotalResourceConsumption { get;  }
    double MaxResourceConsumption { get;  }
    
    //Utilization 
    double[] Utilization { get;  }
    double TotalUtilization { get;  }
    double MaxUtilization { get;  }
    
    void AddDataPoint(IHeatProductionUnitResultDataPoint dataPoint);
}