namespace SchoolHub.Web.ViewModels.Student
{
    using System.Collections.Generic;

    using SchoolHub.Web.ViewModels.Subject;

    public class StudentGradesViewModel
    {
        public string StudentId { get; set; }

        public string StudentName { get; set; }

        public string ClassId { get; set; }

        public List<SubjectGradesViewModel> SubjectGrades { get; set; } = new List<SubjectGradesViewModel>();
    }
}
