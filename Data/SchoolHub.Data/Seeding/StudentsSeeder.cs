namespace SchoolHub.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using SchoolHub.Data.Models;

    public class StudentsSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Students.Any())
            {
                return;
            }

            await dbContext.Students.AddRangeAsync(new List<Student>
            {
                new Student
                {
                    FullName = "Teodor Iliev",
                    BirthDate = new DateTime(2004, 11, 12),
                    UserId = null,
                    ClassId = dbContext.Classes.First().Id,
                    SchoolId = dbContext.Schools.First().Id,
                },
                new Student
                {
                    FullName = "Dimitar Dimitrov",
                    BirthDate = new DateTime(2004, 10, 10),
                    UserId = null,
                    ClassId = dbContext.Classes.First().Id,
                    SchoolId = dbContext.Schools.First().Id,
                },
                new Student
                {
                    FullName = "Plamen Hristov",
                    BirthDate = new DateTime(2004, 06, 16),
                    UserId = null,
                    ClassId = dbContext.Classes.First().Id,
                    SchoolId = dbContext.Schools.First().Id,
                },
            });
        }
    }
}
