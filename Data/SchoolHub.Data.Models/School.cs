namespace SchoolHub.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using SchoolHub.Data.Common.Models;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class School : BaseDeletableModel<string>
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(SchoolNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(SchoolAddressMaxLength)]
        public string Address { get; set; } = null!;

        [Required]
        [MaxLength(SchoolWebsiteMaxLength)]
        public string WebsiteUrl { get; set; } = null!;

        public virtual ICollection<Class> Classes { get; set; } = new HashSet<Class>();

        public virtual ICollection<Teacher> Teachers { get; set; } = new HashSet<Teacher>();

        public virtual ICollection<Student> Students { get; set; } = new HashSet<Student>();
    }
}
