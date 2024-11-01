namespace SchoolHub.Web.ViewModels.School
{
    using System.ComponentModel;

    using AutoMapper;

    using SchoolHub.Services.Mapping;

    public class DetailsSchoolViewModel : IMapFrom<SchoolHub.Data.Models.School>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Name { get; set; } = null!;

        public string Address { get; set; } = null!;

        [DisplayName("Website")]
        public string WebsiteUrl { get; set; } = null!;

        [DisplayName("Count of Classes")]
        public int ClassesCount { get; set; }

        [DisplayName("Count of Teachers")]
        public int TeachersCount { get; set; }

        [DisplayName("Count of Students")]
        public int StudentsCount { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<SchoolHub.Data.Models.School, DetailsSchoolViewModel>()
                .ForMember(x => x.ClassesCount, mo => mo.MapFrom(y => y.Classes.Count))
                .ForMember(x => x.TeachersCount, mo => mo.MapFrom(y => y.Teachers.Count))
                .ForMember(x => x.StudentsCount, mo => mo.MapFrom(y => y.Students.Count));
        }
    }
}
