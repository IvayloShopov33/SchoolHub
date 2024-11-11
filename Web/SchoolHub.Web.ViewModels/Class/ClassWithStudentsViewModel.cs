namespace SchoolHub.Web.ViewModels.Class
{
    using System.Collections.Generic;

    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.Student;

    public class ClassWithStudentsViewModel : IMapFrom<ClassFormModel>
    {
        public string Name { get; set; }

        public ICollection<IndexStudentViewModel> Students { get; set; }
    }
}
