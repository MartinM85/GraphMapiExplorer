namespace MapiExplorer.Models.MAPI
{
    public class PidTagProperty
    {
        public PidTagProperties Name { get; set; }

        public string Id => $"0x{(int)Name:X4}";

        public string DisplayName => $"{Name} ({Id})";

        public string Type { get; set; }

        public string GraphValue { get; set; }

        public string GraphId => $"{Type} {Id}";

        public string GraphExpandQuery => Type.Contains("Array") ?
                $"$expand=multiValueExtendedProperties($filter=id eq '{GraphId}')"
              : $"$expand=singleValueExtendedProperties($filter=id eq '{GraphId}')";
    }
}