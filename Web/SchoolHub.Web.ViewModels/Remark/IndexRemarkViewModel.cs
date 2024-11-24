namespace SchoolHub.Web.ViewModels.Remark
{
    using AutoMapper;
    using SchoolHub.Services.Mapping;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class IndexRemarkViewModel : IMapFrom<SchoolHub.Data.Models.Remark>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string SubjectName { get; set; }

        public string TeacherId { get; set; }

        public string TeacherName { get; set; }

        public string Comment { get; set; }

        public bool IsPraise { get; set; }

        public string Date { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<SchoolHub.Data.Models.Remark, IndexRemarkViewModel>()
                .ForMember(x => x.SubjectName, mo => mo.MapFrom(y => y.Subject.Name))
                .ForMember(x => x.TeacherName, mo => mo.MapFrom(y => y.Teacher.FullName))
                .ForMember(x => x.Date, mo => mo.MapFrom(y => y.Date.ToString(DateTimeFormat)));
        }
    }
}
