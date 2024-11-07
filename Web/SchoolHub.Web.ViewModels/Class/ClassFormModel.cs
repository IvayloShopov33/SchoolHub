namespace SchoolHub.Web.ViewModels.Class
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using SchoolHub.Services.Mapping;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class ClassFormModel : IMapTo<SchoolHub.Data.Models.Class>, IMapFrom<SchoolHub.Data.Models.Class>
    {
        [Required]
        [StringLength(ClassNameMaxLength, MinimumLength = ClassNameMinLength)]
        public string Name { get; set; } = null!;

        [Required]
        public DateTime StartedOn { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime EndingOn { get; set; }

        public string SchoolId { get; set; } = null!;

        public string HomeroomTeacherId { get; set; } = null!;
    }
}
