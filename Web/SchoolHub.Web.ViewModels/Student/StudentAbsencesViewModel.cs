namespace SchoolHub.Web.ViewModels.Student
{
    using System.Collections.Generic;

    public class StudentAbsencesViewModel
    {
        public string Id { get; set; }

        public string StudentName { get; set; }

        public string ClassId { get; set; }

        public List<StudentAbsenceViewModel> Absences { get; set; } = new List<StudentAbsenceViewModel>();

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int ItemsPerPage { get; set; }
    }
}
