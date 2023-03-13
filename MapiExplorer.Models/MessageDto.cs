namespace MapiExplorer.Models
{
    public class MessageDto
    {
        public string Id { get; set; }

        public string Subject { get; set; }

        public string Sender { get; set; }

        public string BodyPreview { get; set; }

        public DateTimeOffset ReceivedDateTime { get; set; }
    }
}