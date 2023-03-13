using Microsoft.Extensions.Configuration;

namespace MapiExplorer.UI
{
    public class PidPropertiesSettings : IPidPropertiesSettings
    {
        public string PidPropertiesResourceFileName { get; internal set; }

        public string CustomNamedPropertiesResourceFileName { get; internal set; }

        public PidPropertiesSettings(IConfiguration configuration)
        {
            PidPropertiesResourceFileName = configuration["MapiProperties:PidPropertiesResourceFileName"];
            CustomNamedPropertiesResourceFileName = configuration["MapiProperties:CustomNamedPropertiesResourceFileName"];
        }
    }
}
