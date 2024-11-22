namespace SchoolHub.Web.ViewModels.Absence
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.Category;

    public class AbsenceFormModel : IMapTo<SchoolHub.Data.Models.Absence>
    {
        [Required]
        public int CategoryId { get; set; }

        public virtual ICollection<CategoryFormModel> Categories { get; set; } = new HashSet<CategoryFormModel>();

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [Required]
        public string StudentId { get; set; }

        [Required]
        public string TeacherId { get; set; }
    }
}
