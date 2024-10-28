namespace SchoolHub.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using SchoolHub.Data.Common.Models;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class Category : BaseDeletableModel<int>
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(CategoryNameMaxLength)]
        public string Name { get; set; } = null!;

        public virtual ICollection<Grade> Grades { get; set; } = new HashSet<Grade>();

        public virtual ICollection<Absence> Absences { get; set; } = new HashSet<Absence>();
    }
}
