namespace SchoolHub.Web.ViewModels.Remark
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using SchoolHub.Services.Mapping;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class RemarkFormModel : IMapTo<SchoolHub.Data.Models.Remark>
    {
        [Required]
        [StringLength(RemarkDescriptionMaxLength, MinimumLength = RemarkDescriptionMinLength)]
        public string Comment { get; set; } = null!;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public bool IsPraise { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [Required]
        public string StudentId { get; set; }

        [Required]
        public string TeacherId { get; set; }
    }
}
