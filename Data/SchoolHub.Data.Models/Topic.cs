namespace SchoolHub.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using SchoolHub.Data.Common.Models;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class Topic : BaseDeletableModel<int>
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(TopicNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [ForeignKey(nameof(Subject))]
        public int SubjectId { get; set; }

        public virtual Subject Subject { get; set; }
    }
}
