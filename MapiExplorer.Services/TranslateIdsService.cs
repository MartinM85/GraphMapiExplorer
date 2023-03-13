using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Me.TranslateExchangeIds;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;

namespace MapiExplorer.Services
{
    public class TranslateIdsService : ITranslateIdsService
    {
        private readonly GraphServiceClient _client;
        private readonly ILogger _logger;

        public TranslateIdsService(GraphServiceClient client, ILogger<TranslateIdsService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<ConvertIdResult> TranslateRestIdToEntryIdAsync(string restId)
        {
            try
            {
                var body = new TranslateExchangeIdsPostRequestBody
                {
                    InputIds = new List<string> { restId },
                    SourceIdType = ExchangeIdFormat.RestId,
                    TargetIdType = ExchangeIdFormat.EntryId
                };
                var response = await _client.Me.TranslateExchangeIds.PostAsync(body);
                return response.Value.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to convert rest id to entry id.");
                return null;
            }
        }
    }
}
