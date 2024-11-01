namespace SchoolHub.Web.ViewModels.School
{
    using AutoMapper;

    using SchoolHub.Services.Mapping;

    public class IndexSchoolViewModel : IMapFrom<SchoolHub.Data.Models.School>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Name { get; set; } = null!;

        public string WebsiteUrl { get; set; } = null!;

        public int TeachersCount { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<SchoolHub.Data.Models.School, IndexSchoolViewModel>()
                .ForMember(x => x.TeachersCount, mo => mo.MapFrom(y => y.Teachers.Count));
        }
    }
}
