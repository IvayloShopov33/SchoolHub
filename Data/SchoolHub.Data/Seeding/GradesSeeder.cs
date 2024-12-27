namespace SchoolHub.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using SchoolHub.Data.Models;

    public class GradesSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Grades.Any())
            {
                return;
            }

            await dbContext.Grades.AddRangeAsync(new List<Grade>
            {
                new Grade
                {
                    Score = 2,
                    CategoryId = 5,
                    Date = new DateTime(2024, 12, 12),
                    SubjectId = 10,
                    StudentId = dbContext.Students.First().Id,
                    TeacherId = dbContext.Teachers.First().Id,
                },
                new Grade
                {
                    Score = 3,
                    CategoryId = 4,
                    Date = new DateTime(2024, 12, 10),
                    SubjectId = 10,
                    StudentId = dbContext.Students.Skip(1).First().Id,
                    TeacherId = dbContext.Teachers.First().Id,
                },
                new Grade
                {
                    Score = 4,
                    CategoryId = 3,
                    Date = new DateTime(2024, 12, 01),
                    SubjectId = 10,
                    StudentId = dbContext.Students.Skip(2).First().Id,
                    TeacherId = dbContext.Teachers.First().Id,
                },
                new Grade
                {
                    Score = 5,
                    CategoryId = 5,
                    Date = new DateTime(2024, 12, 03),
                    SubjectId = 1,
                    StudentId = dbContext.Students.First().Id,
                    TeacherId = dbContext.Teachers.Skip(1).First().Id,
                },
                new Grade
                {
                    Score = 6,
                    CategoryId = 6,
                    Date = new DateTime(2024, 10, 10),
                    SubjectId = 1,
                    StudentId = dbContext.Students.Skip(1).First().Id,
                    TeacherId = dbContext.Teachers.Skip(1).First().Id,
                },
            });
        }
    }
}
