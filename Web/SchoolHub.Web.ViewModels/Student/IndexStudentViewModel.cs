namespace SchoolHub.Web.ViewModels.Student
{
    using AutoMapper;

    using SchoolHub.Services.Mapping;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class IndexStudentViewModel : IMapFrom<SchoolHub.Data.Models.Student>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string BirthDate { get; set; }

        public string Username { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<SchoolHub.Data.Models.Student, IndexStudentViewModel>()
                .ForMember(x => x.BirthDate, mo => mo.MapFrom(y => y.BirthDate.ToString(DateTimeFormat)))
                .ForMember(x => x.Username, mo => mo.MapFrom(y => y.User.UserName));
        }
    }
}
