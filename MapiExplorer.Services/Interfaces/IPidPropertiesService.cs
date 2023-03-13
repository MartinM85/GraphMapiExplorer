using MapiExplorer.Models;
using MapiExplorer.Models.MAPI;

namespace MapiExplorer.Services
{
    public interface IPidPropertiesService
    {
        List<PidNameProperty> GetCustomNamedProperties(GraphResource resource);
        List<PidLidProperty> GetPidLidProperties(GraphResource resource);
        List<PidNameProperty> GetPidNamedProperties(GraphResource resource);
        List<PidTagProperty> GetPidTagProperties(GraphResource resource);
        Task LoadCustomNamedPropertiesAsync(Stream fileStream);
        Task LoadPidPropertiesMappingAsync(Stream fileStream);
    }
}