namespace SchoolHub.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using SchoolHub.Data.Models;

    public class RemarksSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Remarks.Any())
            {
                return;
            }

            await dbContext.Remarks.AddRangeAsync(new List<Remark>
            {
                new Remark
                {
                    Comment = "Amazing student with excellent behavior!",
                    IsPraise = true,
                    Date = DateTime.UtcNow,
                    SubjectId = 1,
                    StudentId = dbContext.Students.First().Id,
                    TeacherId = dbContext.Teachers.Skip(1).First().Id,
                },
                new Remark
                {
                    Comment = "Student is not consistent in class and does not have homework.",
                    IsPraise = false,
                    Date = new DateTime(2024, 12, 12),
                    SubjectId = 10,
                    StudentId = dbContext.Students.Skip(1).First().Id,
                    TeacherId = dbContext.Teachers.First().Id,
                },
                new Remark
                {
                    Comment = "Displays a positive attitude towards learning.",
                    IsPraise = true,
                    Date = new DateTime(2024, 11, 11),
                    SubjectId = 1,
                    StudentId = dbContext.Students.Skip(2).First().Id,
                    TeacherId = dbContext.Teachers.Skip(1).First().Id,
                },
            });
        }
    }
}
