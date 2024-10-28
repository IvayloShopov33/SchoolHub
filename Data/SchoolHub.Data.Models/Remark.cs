namespace SchoolHub.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using SchoolHub.Data.Common.Models;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class Remark : BaseDeletableModel<string>
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(RemarkDescriptionMaxLength)]
        public string Comment { get; set; } = null!;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public bool IsPraise { get; set; }

        [Required]
        [ForeignKey(nameof(Subject))]
        public int SubjectId { get; set; }

        public virtual Subject Subject { get; set; }

        [Required]
        [ForeignKey(nameof(Student))]
        public string StudentId { get; set; } = null!;

        public virtual Student Student { get; set; }

        [Required]
        [ForeignKey(nameof(Teacher))]
        public string TeacherId { get; set; } = null!;

        public virtual Teacher Teacher { get; set; }
    }
}
