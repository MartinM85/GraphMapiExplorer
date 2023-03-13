using MapiExplorer.Models;
using MapiExplorer.Models.MAPI;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MapiExplorer.Services
{
    public class PidPropertiesService : IPidPropertiesService
    {
        private readonly ILogger _logger;

        private Dictionary<string, PidNameProperty> _customNamedProperties;
        private PidPropertiesMapping _pidPropertiesMapping;

        public PidPropertiesService(ILogger<PidPropertiesService> logger)
        {
            _logger = logger;
        }

        public async Task LoadCustomNamedPropertiesAsync(Stream fileStream)
        {
            if (_customNamedProperties == null)
            {
                try
                {
                    var customNamedProperties = await JsonSerializer.DeserializeAsync<List<PidNameProperty>>(fileStream);
                    _customNamedProperties = customNamedProperties.ToDictionary(x => x.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to load custom named properties from assets.");
                    _customNamedProperties = new Dictionary<string, PidNameProperty>();
                }
            }
        }

        public async Task LoadPidPropertiesMappingAsync(Stream fileStream)
        {
            if (_pidPropertiesMapping == null)
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        Converters =
                        {
                            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                        }
                    };
                    _pidPropertiesMapping = await JsonSerializer.DeserializeAsync<PidPropertiesMapping>(fileStream, options);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to load pid properties from assets.");
                    _pidPropertiesMapping = new PidPropertiesMapping();
                }
            }
        }

        public List<PidLidProperty> GetPidLidProperties(GraphResource resource)
        {
            var result = new List<PidLidProperty>();
            if (_pidPropertiesMapping.ResourceLidProperties != null && _pidPropertiesMapping.ResourceLidProperties.TryGetValue(resource, out var values))
            {
                foreach (var property in values)
                {
                    try
                    {
                        result.Add(PidPropertyFactory.Create(property));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to add pid lid {property}.");
                    }
                }
            }
            return result;
        }

        public List<PidTagProperty> GetPidTagProperties(GraphResource resource)
        {
            var result = new List<PidTagProperty>();
            if (_pidPropertiesMapping.ResourceTagProperties != null && _pidPropertiesMapping.ResourceTagProperties.TryGetValue(resource, out var values))
            {
                foreach (var property in values)
                {
                    try
                    {
                        result.Add(PidPropertyFactory.Create(property));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to add pid tag {property}.");
                    }
                }
            }
            return result;
        }

        public List<PidNameProperty> GetPidNamedProperties(GraphResource resource)
        {
            var result = new List<PidNameProperty>();
            if (_pidPropertiesMapping.ResourceNamedProperties != null && _pidPropertiesMapping.ResourceNamedProperties.TryGetValue(resource, out var values))
            {
                foreach (var property in values)
                {
                    try
                    {
                        result.Add(PidPropertyFactory.Create(property));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to add pid named {property}.");
                    }
                }
            }
            return result;
        }

        public List<PidNameProperty> GetCustomNamedProperties(GraphResource resource)
        {
            var result = new List<PidNameProperty>();
            if (_pidPropertiesMapping.ResourceCustomNamedProperties != null && _pidPropertiesMapping.ResourceCustomNamedProperties.TryGetValue(resource, out var values))
            {
                foreach (var property in values)
                {
                    try
                    {
                        result.Add(new PidNameProperty
                        {
                            Id = property,
                            Name = property,
                            Type = _customNamedProperties[property].Type,
                            PropertySet = _customNamedProperties[property].PropertySet,
                            Hint = CreateHint(_customNamedProperties[property].Type)
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to add custom named {property}.");
                    }
                }
            }
            return result;

            string CreateHint(string type)
            {
                var hint = string.Empty;
                if (type.Contains("Array"))
                {
                    hint += "Values separated by semicolon. ";
                    type = type.Replace("Array", string.Empty);
                }
                hint += type switch
                {
                    "String" => "Add some text",
                    "Integer" => "Add integer",
                    "Short" => "Add integer",
                    "Double" => "Add number with decimal value, ex: 2.5",
                    "Binary" => "Add base 64 encoded value",
                    "Boolean" => "Possible values: true, false, 0 or 1",
                    "SystemTime" => "Add date time in ISO-8601 format: yyyy-MM-ddTHH:mm:ssZ",
                    _ => ""
                };
                return hint;
            }
        }
    }
}
