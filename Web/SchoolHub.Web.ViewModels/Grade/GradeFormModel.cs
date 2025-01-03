﻿namespace SchoolHub.Web.ViewModels.Grade
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.Category;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class GradeFormModel : IMapTo<SchoolHub.Data.Models.Grade>
    {
        [Required]
        [Range(GradeScoreMinValue, GradeScoreMaxValue)]
        public int Score { get; set; }

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
