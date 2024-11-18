namespace SchoolHub.Web.ViewModels.Grade
{
    using AutoMapper;

    using SchoolHub.Services.Mapping;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class DetailsGradeViewModel : IMapFrom<SchoolHub.Data.Models.Grade>, IHaveCustomMappings
    {
        public int Score { get; set; }

        public string Category { get; set; }

        public string Date { get; set; }

        public string Teacher { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<SchoolHub.Data.Models.Grade, DetailsGradeViewModel>()
                .ForMember(x => x.Category, mo => mo.MapFrom(y => y.Category.Name))
                .ForMember(x => x.Date, mo => mo.MapFrom(y => y.Date.ToString(DateTimeFormat)))
                .ForMember(x => x.Teacher, mo => mo.MapFrom(y => y.Teacher.FullName));
        }
    }
}
