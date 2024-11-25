namespace SchoolHub.Web.ViewModels.Student
{
    using System.Collections.Generic;

    using SchoolHub.Web.ViewModels.Absence;

    public class StudentGroupedAbsencesViewModel
    {
        public string StudentName { get; set; }

        public List<GroupedAbsencesViewModel> GroupedAbsences { get; set; } = new List<GroupedAbsencesViewModel>();
    }
}
