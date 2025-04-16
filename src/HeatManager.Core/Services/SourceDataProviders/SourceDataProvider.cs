using HeatManager.Core.Models.SourceData;

namespace HeatManager.Core.Services.SourceDataProviders;

internal class SourceDataProvider : ISourceDataProvider
{
    public SourceDataCollection? SourceDataCollection { get; set; }
}