using HeatManager.Core.Models.SourceData;

namespace HeatManager.Core.Services.SourceDataProviders;

public interface ISourceDataProvider
{
    public SourceDataCollection? SourceDataCollection { get; set; }

}