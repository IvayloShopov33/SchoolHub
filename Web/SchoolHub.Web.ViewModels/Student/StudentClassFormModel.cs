namespace SchoolHub.Web.ViewModels.Student
{
    using System.ComponentModel.DataAnnotations;

    using SchoolHub.Services.Mapping;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class StudentClassFormModel : IMapFrom<SchoolHub.Data.Models.Class>
    {
        public string Id { get; set; }

        [Required]
        [StringLength(ClassNameMaxLength, MinimumLength = ClassNameMinLength)]
        public string Name { get; set; } = null!;
    }
}
