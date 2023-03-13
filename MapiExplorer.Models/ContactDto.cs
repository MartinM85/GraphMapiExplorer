namespace MapiExplorer.Models
{
    public class ContactDto
    {
        public string Id { get; set; }

        public string DisplayName { get; set; }

        public List<string> EmailAddresses { get; set; }

        public string CompanyName { get; set; }

        public string MobilePhone { get; set; }
    }
}