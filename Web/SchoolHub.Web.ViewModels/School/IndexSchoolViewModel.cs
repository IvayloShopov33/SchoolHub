namespace SchoolHub.Web.ViewModels.School
{
    public class IndexSchoolViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; } = null!;

        public string WebsiteUrl { get; set; } = null!;

        public int TeachersCount { get; set; }
    }
}
