namespace MapiExplorer.Models
{
    public class MailFolderDto
    {
        public string Id { get; set; }

        public string DisplayName { get; set; }

        public int ChildFolderCount { get; set; }

        public int TotalItemCount { get; set; }

        public int UnreadItemCount { get; set; }
    }
}