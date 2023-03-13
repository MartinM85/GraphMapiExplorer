namespace MapiExplorer.Models.MAPI
{
    public class PidNameProperty
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string DisplayName => Id == Name ?
            Id : $"{Id} ({Name})";

        public string Type { get; set; }

        public string PropertySet { get; set; }

        public string Hint { get; set; }

        public string GraphValue { get; set; }

        public string GraphId => $"{Type} {PropertySet} Name {Name}";

        public string GraphExpandQuery => Type.Contains("Array") ?
                $"$expand=multiValueExtendedProperties($filter=id eq '{GraphId}')"
              : $"$expand=singleValueExtendedProperties($filter=id eq '{GraphId}')";
    }
}