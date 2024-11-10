namespace SchoolHub.Web.ViewModels.Teacher
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class BecomeTeacherFormModel
    {
        [Required]
        [StringLength(SchoolMemberFullNameMaxLength, MinimumLength = SchoolMemberFullNameMinLength)]
        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        [Required]
        public DateTime BirthDate { get; set; }
    }
}
