namespace SchoolHub.Web.ViewModels.Subject
{
    using System.Collections.Generic;

    using SchoolHub.Web.ViewModels.Grade;

    public class SubjectGradesViewModel
    {
        public string SubjectName { get; set; }

        public List<DetailsGradeViewModel> Grades { get; set; } = new List<DetailsGradeViewModel>();
    }
}
