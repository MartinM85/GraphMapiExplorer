using MapiExplorer.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Drives.Item.Items.Item.Workbook.CreateSession;
using Microsoft.Graph.Models;

namespace MapiExplorer.Services
{
    /// <summary>
    /// Not used. Working with excel is not supported in Microsoft.Graph SDK v5.1
    /// </summary>
    public class ExcelService : IExcelService
    {
        private readonly GraphServiceClient _client;
        private readonly ILogger _logger;

        public ExcelService(GraphServiceClient client, ILogger<ExcelService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task WriteDataAsync(string driveId, string itemId)
        {
            var sessionBody = new CreateSessionPostRequestBody
            {
                PersistChanges = true
            };
            var session = await _client.Drives[driveId].Items[itemId].Workbook.CreateSession.PostAsync(sessionBody);

            var table = new WorkbookTable
            {
                Name = GitHubExtensionProperty.Id,
                ShowHeaders = true,
            };
            await _client.Drives[driveId].Items[itemId].Workbook.Tables.PostAsync(table);
            table = await _client.Drives[driveId].Items[itemId].Workbook.Worksheets["0"].Tables.PostAsync(table, x =>
            {
                x.Headers["Workbook-Session-Id"] = new[] { session.Id };
            });

        }
    }
}