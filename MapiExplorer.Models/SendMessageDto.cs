namespace MapiExplorer.Models
{
    public class SendMessageDto
    {
        public string Subject { get; set; }

        public string Sender { get; set; }

        public List<string> ToRecipients { get; set; }

        public string Content { get; set; }
    }
}