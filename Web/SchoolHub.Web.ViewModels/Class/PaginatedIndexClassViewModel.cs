namespace SchoolHub.Web.ViewModels.Class
{
    using System.Collections.Generic;

    public class PaginatedIndexClassViewModel
    {
        public IEnumerable<IndexClassViewModel> Classes { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public string SchoolId { get; set; }

        public string SearchTerm { get; set; }
    }
}
