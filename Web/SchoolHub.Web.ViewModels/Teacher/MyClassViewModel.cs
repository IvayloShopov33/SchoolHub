namespace SchoolHub.Web.ViewModels.Teacher
{
    using AutoMapper;

    using SchoolHub.Services.Mapping;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class MyClassViewModel : IMapFrom<SchoolHub.Data.Models.Class>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Name { get; set; } = null!;

        public string StartedOn { get; set; } = null!;

        public string EndingOn { get; set; } = null!;

        public string School { get; set; } = null!;

        public string HomeroomTeacher { get; set; } = null!;

        public int StudentsCount { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<SchoolHub.Data.Models.Class, MyClassViewModel>()
                .ForMember(x => x.Id, mo => mo.MapFrom(y => y.Id))
                .ForMember(x => x.StartedOn, mo => mo.MapFrom(y => y.StartedOn.ToString(DateTimeFormat)))
                .ForMember(x => x.EndingOn, mo => mo.MapFrom(y => y.EndingOn.ToString(DateTimeFormat)))
                .ForMember(x => x.School, mo => mo.MapFrom(y => y.School.Name))
                .ForMember(x => x.HomeroomTeacher, mo => mo.MapFrom(y => y.HomeroomTeacher.FullName))
                .ForMember(x => x.StudentsCount, mo => mo.MapFrom(y => y.Students.Count));
        }
    }
}
