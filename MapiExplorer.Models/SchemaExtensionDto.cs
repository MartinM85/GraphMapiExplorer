namespace MapiExplorer.Models
{
    public class SchemaExtensionDto
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public List<string> TargetTypes { get; set; }

        public List<SchemaExtensionPropertyDto> Properties { get; set; }
    }
}