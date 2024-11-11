namespace SchoolHub.Web.ViewModels.Student
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using SchoolHub.Services.Mapping;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class StudentFormModel : IMapTo<SchoolHub.Data.Models.Student>, IMapFrom<SchoolHub.Data.Models.Student>
    {
        [Required]
        [StringLength(SchoolMemberFullNameMaxLength, MinimumLength = SchoolMemberFullNameMinLength)]
        public string FullName { get; set; } = null!;

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public string ClassId { get; set; } = null!;

        public virtual ICollection<StudentClassFormModel> Classes { get; set; } = new HashSet<StudentClassFormModel>();

        [Required]
        public string SchoolId { get; set; } = null!;
    }
}
