namespace SchoolHub.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using SchoolHub.Data.Common.Models;

    public class Grade : BaseDeletableModel<string>
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public int Score { get; set; }

        [Required]
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        [Required]
        public DateTime Date { get; set; }

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
