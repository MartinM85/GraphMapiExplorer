namespace MapiExplorer.Models.MAPI
{
    public class PidPropertiesMapping
    {
        public Dictionary<GraphResource, List<PidTagProperties>> ResourceTagProperties { get; set; }

        public Dictionary<GraphResource, List<PidLidProperties>> ResourceLidProperties { get; set; }

        public Dictionary<GraphResource, List<PidNameProperties>> ResourceNamedProperties { get; set; }

        public Dictionary<GraphResource, List<string>> ResourceCustomNamedProperties { get; set; }
    }
}