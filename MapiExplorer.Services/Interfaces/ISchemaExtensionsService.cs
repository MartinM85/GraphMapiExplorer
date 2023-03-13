using MapiExplorer.Models;
using Microsoft.Graph.Models;

namespace MapiExplorer.Services
{
    public interface ISchemaExtensionsService
    {
        Task<SchemaExtension> CreateSchemaExtensionAsync(string appId, SchemaExtensionDto schema);
        Task<List<SchemaExtension>> GetExtensionsAsync(string filter);
        Task MakeSchemaExtensionAvailableAsync(string schemaExtensionId);
        Task<SchemaExtension> GetSchemaExtensionAsync(string schemaExtensionId);
    }
}