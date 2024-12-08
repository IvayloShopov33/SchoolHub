namespace SchoolHub.Web.ViewModels.Class
{
    using System.Collections.Generic;

    public class AssignExistingClassFormModel
    {
        public string TeacherId { get; set; }

        public string SelectedClassId { get; set; }

        public List<ClassFormModel> Classes { get; set; } = new List<ClassFormModel>();
    }
}
