using Microsoft.Graph.Models;

namespace MapiExplorer.Services
{
    public interface ISitesService
    {
        Task<Site> FindRootSiteAsync();
    }
}