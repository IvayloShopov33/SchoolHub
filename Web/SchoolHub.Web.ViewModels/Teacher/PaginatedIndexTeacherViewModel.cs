namespace SchoolHub.Web.ViewModels.Teacher
{
    using System.Collections.Generic;

    public class PaginatedIndexTeacherViewModel
    {
        public IEnumerable<IndexTeacherViewModel> Teachers { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public string SchoolId { get; set; }

        public string SearchTerm { get; set; }
    }
}
