namespace SchoolHub.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

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

        public virtual ICollection<Teacher> Teachers { get; set; } = new HashSet<Teacher>();

        public virtual ICollection<Grade> Grades { get; set; } = new HashSet<Grade>();

        public virtual ICollection<Absence> Absences { get; set; } = new HashSet<Absence>();

        public virtual ICollection<Remark> Remarks { get; set; } = new HashSet<Remark>();
    }
}
