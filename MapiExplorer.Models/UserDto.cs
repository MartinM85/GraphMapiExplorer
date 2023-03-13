namespace MapiExplorer.Models
{
    public class UserDto
    {
        public string Id { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public string GitHubWorkAccount { get; set; }

        public string GitHubPersonalAccount { get; set; }

        public UserActivity Activity { get; set; }

        public UserAvailability Availability { get; set; }
    }
}