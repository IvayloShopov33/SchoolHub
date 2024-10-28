namespace SchoolHub.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using SchoolHub.Data.Common.Models;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class Subject : BaseDeletableModel<int>
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(SubjectNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(SubjectDescriptionMaxLength)]
        public string Description { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Class))]
        public string ClassId { get; set; } = null!;

        public virtual Class Class { get; set; }

        [Required]
        [ForeignKey(nameof(Teacher))]
        public string TeacherId { get; set; } = null!;

        public virtual Teacher Teacher { get; set; }

        public virtual ICollection<Topic> Topics { get; set; } = new HashSet<Topic>();

        public virtual ICollection<Grade> Grades { get; set; } = new HashSet<Grade>();

        public virtual ICollection<Absence> Absences { get; set; } = new HashSet<Absence>();

        public virtual ICollection<Remark> Remarks { get; set; } = new HashSet<Remark>();
    }
}
