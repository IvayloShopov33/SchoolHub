namespace SchoolHub.Web.ViewModels.Category
{
    using System.ComponentModel.DataAnnotations;

    using SchoolHub.Services.Mapping;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class CategoryFormModel : IMapFrom<Data.Models.Category>
    {
        public int Id { get; set; }

        [Required]
        [StringLength(CategoryNameMaxLength, MinimumLength = CategoryNameMinLength)]
        public string Name { get; set; } = null!;
    }
}
