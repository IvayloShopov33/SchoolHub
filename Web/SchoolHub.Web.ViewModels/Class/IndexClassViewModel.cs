namespace SchoolHub.Web.ViewModels.Class
{
    using AutoMapper;

    using SchoolHub.Services.Mapping;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class IndexClassViewModel : IMapFrom<SchoolHub.Data.Models.Class>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Name { get; set; } = null!;

        public string StartedOn { get; set; }

        public string EndingOn { get; set; }

        public string SchoolId { get; set; } = null!;

        public string School { get; set; } = null!;

        public string HomeroomTeacherId { get; set; } = null!;

        public string HomeroomTeacher { get; set; } = null!;

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<SchoolHub.Data.Models.Class, IndexClassViewModel>()
                .ForMember(x => x.StartedOn, mo => mo.MapFrom(y => y.StartedOn.ToString(DateTimeFormat)))
                .ForMember(x => x.EndingOn, mo => mo.MapFrom(y => y.EndingOn.ToString(DateTimeFormat)))
                .ForMember(x => x.School, mo => mo.MapFrom(y => y.School.Name))
                .ForMember(x => x.HomeroomTeacher, mo => mo.MapFrom(y => y.HomeroomTeacher.FullName));
        }
    }
}
