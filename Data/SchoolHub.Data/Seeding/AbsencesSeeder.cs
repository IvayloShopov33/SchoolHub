namespace SchoolHub.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using SchoolHub.Data.Models;

    public class AbsencesSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Absences.Any())
            {
                return;
            }

            await dbContext.Absences.AddRangeAsync(new List<Absence>
            {
                new Absence
                {
                    CategoryId = 1,
                    SubjectId = 1,
                    StudentId = dbContext.Students.First().Id,
                    TeacherId = dbContext.Teachers.Skip(1).First().Id,
                    Date = new DateTime(2024, 11, 11),
                },
                new Absence
                {
                    CategoryId = 2,
                    SubjectId = 10,
                    StudentId = dbContext.Students.Skip(1).First().Id,
                    TeacherId = dbContext.Teachers.First().Id,
                    Date = new DateTime(2024, 12, 12),
                },
                new Absence
                {
                    CategoryId = 1,
                    SubjectId = 1,
                    StudentId = dbContext.Students.Skip(2).First().Id,
                    TeacherId = dbContext.Teachers.Skip(1).First().Id,
                    Date = new DateTime(2024, 10, 10),
                },
                new Absence
                {
                    CategoryId = 2,
                    SubjectId = 10,
                    StudentId = dbContext.Students.Skip(1).First().Id,
                    TeacherId = dbContext.Teachers.First().Id,
                    Date = new DateTime(2024, 09, 17),
                },
            });
        }
    }
}
