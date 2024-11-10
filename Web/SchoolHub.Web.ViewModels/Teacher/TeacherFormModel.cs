namespace SchoolHub.Web.ViewModels.Teacher
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;

    using SchoolHub.Services.Mapping;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class TeacherFormModel : IMapTo<SchoolHub.Data.Models.Teacher>, IMapFrom<SchoolHub.Data.Models.Teacher>, IHaveCustomMappings
    {
        public string Id { get; set; }

        [Required]
        [StringLength(SchoolMemberFullNameMaxLength, MinimumLength = SchoolMemberFullNameMinLength)]
        public string FullName { get; set; } = null!;

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public string SchoolId { get; set; } = null!;

        public string ClassId { get; set; }

        public virtual ICollection<TeacherClassFormModel> Classes { get; set; } = new HashSet<TeacherClassFormModel>();

        [Required]
        public int SubjectId { get; set; }

        public virtual ICollection<TeacherSubjectFormModel> Subjects { get; set; } = new HashSet<TeacherSubjectFormModel>();

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<TeacherFormModel, SchoolHub.Data.Models.Teacher>()
                .ForMember(x => x.Id, mo => mo.Ignore());
        }
    }
}
