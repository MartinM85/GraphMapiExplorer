using MapiExplorer.Models;
using Microsoft.Graph.Models;

namespace MapiExplorer.Services
{
    public interface IApplicationService
    {
        Task AddExtensionPropertyValue(string appId, string name, string extensionValue);
        Task<ExtensionProperty> CreateExtensionAsync(string appId, string name, List<string> targetObjects, GraphPropertyTypes dataType);
        Task<ExtensionProperty> GetExtensionAsync(string appId, string name);
        Task<T> GetExtensionPropertyValueAsync<T>(string appId, string name);
    }
}