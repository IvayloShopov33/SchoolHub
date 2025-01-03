﻿namespace SchoolHub.Web.ViewModels.Class
{
    using SchoolHub.Services.Mapping;

    public class DeleteClassViewModel : IMapFrom<ClassFormModel>
    {
        public string Id { get; set; }

        public string Name { get; set; } = null!;

        public string SchoolId { get; set; } = null!;
    }
}
