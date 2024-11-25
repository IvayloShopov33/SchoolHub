namespace SchoolHub.Web.ViewModels.Student
{
    using System.Collections.Generic;

    using SchoolHub.Web.ViewModels.Remark;

    public class StudentGroupedRemarksViewModel
    {
        public string StudentName { get; set; }

        public List<GroupedRemarksViewModel> GroupedRemarks { get; set; } = new List<GroupedRemarksViewModel>();
    }
}
