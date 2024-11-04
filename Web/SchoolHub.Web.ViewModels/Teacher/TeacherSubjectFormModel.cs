namespace SchoolHub.Web.ViewModels.Teacher
{
    using System.ComponentModel.DataAnnotations;

    using SchoolHub.Services.Mapping;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class TeacherSubjectFormModel : IMapFrom<SchoolHub.Data.Models.Subject>
    {
        public int Id { get; set; }

        [Required]
        [StringLength(SubjectNameMaxLength, MinimumLength = SubjectNameMinLength)]
        public string Name { get; set; } = null!;
    }
}
