namespace MapiExplorer.Models
{
    public class EventDto
    {
        public string Id { get; set; }

        public string Organizer { get; set; }

        public string Subject { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public string Location { get; set; }

        public string BodyPreview { get; set; }
    }
}