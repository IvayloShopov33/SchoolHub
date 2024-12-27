namespace SchoolHub.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using SchoolHub.Data.Models;

    public class TeachersSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Teachers.Any())
            {
                return;
            }

            await dbContext.Teachers.AddRangeAsync(new List<Teacher>
            {
                new Teacher
                {
                    FullName = "Ivaylo Shopov",
                    BirthDate = new DateTime(1960, 09, 01),
                    UserId = null,
                    SchoolId = dbContext.Schools.First().Id,
                    ClassId = null,
                    SubjectId = dbContext.Subjects.OrderBy(x => x.Name).Last().Id,
                },
                new Teacher
                {
                    FullName = "Petar Petrov",
                    BirthDate = new DateTime(1954, 04, 08),
                    UserId = null,
                    SchoolId = dbContext.Schools.OrderBy(x => x.Name).Last().Id,
                    ClassId = null,
                    SubjectId = dbContext.Subjects.First().Id,
                },
            });
        }
    }
}
