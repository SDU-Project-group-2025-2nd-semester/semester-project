using HeatManager.Core.Models.SourceData;

namespace HeatManager.Core.Services;

public class SourceDataProvider : ISourceDataProvider
{
    public ISourceDataCollection SourceDataCollection { get; set; }
}