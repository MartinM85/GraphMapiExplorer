namespace MapiExplorer.Services
{
    public interface IExcelService
    {
        Task WriteDataAsync(string driveId, string itemId);
    }
}