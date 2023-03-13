using Microsoft.Graph.Models;

namespace MapiExplorer.Services
{
    public interface ITranslateIdsService
    {
        Task<ConvertIdResult> TranslateRestIdToEntryIdAsync(string restId);
    }
}