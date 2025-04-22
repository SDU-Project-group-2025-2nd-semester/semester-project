using HeatManager.Core.Models.SourceData;

namespace HeatManager.Core.Services.SourceDataProviders;

public class SourceDataProvider : ISourceDataProvider
{
    public SourceDataCollection? SourceDataCollection { get; set; }
}