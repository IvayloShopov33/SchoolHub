namespace SchoolHub.Web.ViewModels.School
{
    using System.ComponentModel.DataAnnotations;

    using SchoolHub.Data.Models;
    using SchoolHub.Services.Mapping;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class SchoolFormModel : IMapTo<School>
    {
        [Required]
        [StringLength(SchoolNameMaxLength, MinimumLength = SchoolNameMinLength)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(SchoolAddressMaxLength, MinimumLength = SchoolAddressMinLength)]
        public string Address { get; set; } = null!;

        [Required]
        [Url]
        [StringLength(SchoolWebsiteMaxLength, MinimumLength = SchoolWebsiteMinLength)]
        public string WebsiteUrl { get; set; } = null!;
    }
}
