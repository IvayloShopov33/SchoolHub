namespace SchoolHub.Web.ViewModels.School
{
    using System;
    using System.Collections.Generic;

    public class PaginatedIndexSchoolViewModel
    {
        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public string? SearchQuery { get; set; }

        public IEnumerable<IndexSchoolViewModel> Schools { get; set; } = new List<IndexSchoolViewModel>();

        public int TotalPages => (int)Math.Ceiling((double)this.TotalCount / this.PageSize);
    }
}
