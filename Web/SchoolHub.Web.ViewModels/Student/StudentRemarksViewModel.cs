namespace SchoolHub.Web.ViewModels.Student
{
    using System.Collections.Generic;

    using SchoolHub.Web.ViewModels.Remark;

    public class StudentRemarksViewModel
    {
        public string Id { get; set; }

        public string StudentName { get; set; }

        public string ClassId { get; set; }

        public List<IndexRemarkViewModel> Remarks { get; set; } = new List<IndexRemarkViewModel>();
    }
}
