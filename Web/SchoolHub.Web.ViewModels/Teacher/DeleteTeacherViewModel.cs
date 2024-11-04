namespace SchoolHub.Web.ViewModels.Teacher
{
    using AutoMapper;

    using SchoolHub.Services.Mapping;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class DeleteTeacherViewModel : IMapFrom<TeacherFormModel>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string BirthDate { get; set; }

        public string SchoolId { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<TeacherFormModel, DeleteTeacherViewModel>()
                .ForMember(x => x.BirthDate, mo => mo.MapFrom(y => y.BirthDate.ToString(DateTimeFormat)));
        }
    }
}
