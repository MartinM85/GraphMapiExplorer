namespace MapiExplorer.Models.MAPI
{
    public class PidLidProperty
    {
        public PidLidProperties Name { get; set; }

        public string Id => $"0x{(int)Name:X8}";

        public string DisplayName => $"{Name} ({Id})";

        public string Type { get; set; }

        public string PropertySet { get; set; }

        public string GraphValue { get; set; }

        public string GraphId => $"{Type} {PropertySet} Id {Id}";

        public string GraphExpandQuery => Type.Contains("Array") ?
                $"$expand=multiValueExtendedProperties($filter=id eq '{GraphId}')"
              : $"$expand=singleValueExtendedProperties($filter=id eq '{GraphId}')";
    }
}