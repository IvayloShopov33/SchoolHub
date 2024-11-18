namespace SchoolHub.Web.ViewModels.Grade
{
    using System.ComponentModel.DataAnnotations;

    using SchoolHub.Services.Mapping;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class GradeCategoryFormModel : IMapFrom<SchoolHub.Data.Models.Category>
    {
        public string Id { get; set; }

        [Required]
        [StringLength(CategoryNameMaxLength, MinimumLength = CategoryNameMinLength)]
        public string Name { get; set; } = null!;
    }
}
