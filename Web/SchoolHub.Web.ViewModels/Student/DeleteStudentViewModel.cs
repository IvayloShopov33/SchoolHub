namespace SchoolHub.Web.ViewModels.Student
{
    using AutoMapper;

    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.Teacher;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class DeleteStudentViewModel : IMapFrom<TeacherFormModel>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string BirthDate { get; set; }

        public string ClassId { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<StudentFormModel, DeleteStudentViewModel>()
                .ForMember(x => x.BirthDate, mo => mo.MapFrom(y => y.BirthDate.ToString(DateTimeFormat)));
        }
    }
}
