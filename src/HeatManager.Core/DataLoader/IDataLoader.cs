using Avalonia.Platform.Storage;

namespace HeatManager.Core.DataLoader;

public interface IDataLoader
{
    public void LoadData(string csvFilePath);
    public Task LoadData(IStorageFile csvFilePath);
}