namespace SchoolHub.Web.ViewModels.Student
{
    using AutoMapper;

    using SchoolHub.Services.Mapping;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class StudentAbsenceViewModel : IMapFrom<SchoolHub.Data.Models.Absence>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string SubjectName { get; set; }

        public string TeacherId { get; set; }

        public string TeacherName { get; set; }

        public string AbsenceType { get; set; }

        public string Date { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<SchoolHub.Data.Models.Absence, StudentAbsenceViewModel>()
                .ForMember(x => x.Id, mo => mo.MapFrom(y => y.Id))
                .ForMember(x => x.TeacherId, mo => mo.MapFrom(y => y.TeacherId))
                .ForMember(x => x.SubjectName, mo => mo.MapFrom(y => y.Subject.Name))
                .ForMember(x => x.AbsenceType, mo => mo.MapFrom(y => y.Category.Name))
                .ForMember(x => x.Date, mo => mo.MapFrom(y => y.Date.ToString(DateTimeFormat)))
                .ForMember(x => x.TeacherName, mo => mo.MapFrom(y => y.Teacher.FullName));
        }
    }
}
