namespace SchoolHub.Web.ViewModels.Teacher
{
    using AutoMapper;

    using SchoolHub.Services.Mapping;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class IndexTeacherViewModel : IMapFrom<SchoolHub.Data.Models.Teacher>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string BirthDate { get; set; }

        public string Username { get; set; }

        public string School { get; set; }

        public string Class { get; set; }

        public string Subject { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<SchoolHub.Data.Models.Teacher, IndexTeacherViewModel>()
                .ForMember(x => x.BirthDate, mo => mo.MapFrom(y => y.BirthDate.ToString(DateTimeFormat)))
                .ForMember(x => x.Username, mo => mo.MapFrom(y => y.User.UserName))
                .ForMember(x => x.School, mo => mo.MapFrom(y => y.School.Name))
                .ForMember(x => x.Class, mo => mo.MapFrom(y => y.Class.Name))
                .ForMember(x => x.Subject, mo => mo.MapFrom(y => y.Subject.Name));
        }
    }
}
