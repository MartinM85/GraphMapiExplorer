namespace MapiExplorer.Models
{
    public class DriveItemDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public FileDto File { get; set; }

        public bool IsFile => File != null;

        public FolderDto Folder { get; set; }

        public string DriveId { get; set; }

        public string ParentDriveItemId { get; set; }
    }
}