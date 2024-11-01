namespace SchoolHub.Web.ViewModels.School
{
    using SchoolHub.Services.Mapping;

    public class DeleteSchoolViewModel : IMapFrom<DetailsSchoolViewModel>
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
