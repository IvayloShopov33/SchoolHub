namespace SchoolHub.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using SchoolHub.Data.Common.Models;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class Student : BaseDeletableModel<string>
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(SchoolMemberFullNameMaxLength)]
        public string FullName { get; set; } = null!;

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;

        public virtual ApplicationUser User { get; set; }

        [Required]
        [ForeignKey(nameof(Class))]
        public string ClassId { get; set; } = null!;

        public virtual Class Class { get; set; }

        [Required]
        [ForeignKey(nameof(School))]
        public string SchoolId { get; set; } = null!;

        public virtual School School { get; set; }

        public virtual ICollection<Grade> Grades { get; set; } = new HashSet<Grade>();

        public virtual ICollection<Absence> Absences { get; set; } = new HashSet<Absence>();

        public virtual ICollection<Remark> Remarks { get; set; } = new HashSet<Remark>();
    }
}
