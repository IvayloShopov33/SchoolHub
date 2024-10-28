namespace SchoolHub.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using SchoolHub.Data.Common.Models;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class Class : BaseDeletableModel<string>
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(ClassNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        public DateTime StartedOn { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime EndingOn { get; set; }

        [Required]
        [ForeignKey(nameof(School))]
        public string SchoolId { get; set; } = null!;

        public virtual School School { get; set; }

        [Required]
        [ForeignKey(nameof(HomeroomTeacher))]
        public string HomeroomTeacherId { get; set; } = null!;

        public virtual Teacher HomeroomTeacher { get; set; }

        public virtual ICollection<Subject> Subjects { get; set; } = new HashSet<Subject>();

        public virtual ICollection<Student> Students { get; set; } = new HashSet<Student>();
    }
}
