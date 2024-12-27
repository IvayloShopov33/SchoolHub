namespace SchoolHub.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using SchoolHub.Data.Models;

    public class ClassesSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Classes.Any())
            {
                return;
            }

            await dbContext.Classes.AddRangeAsync(new List<Class>
            {
                new Class
                {
                    Name = "10A",
                    StartedOn = new DateTime(2024, 09, 16),
                    EndingOn = new DateTime(2025, 06, 30),
                    SchoolId = dbContext.Schools.First().Id,
                    HomeroomTeacherId = null,
                },
                new Class
                {
                    Name = "12A",
                    StartedOn = new DateTime(2025, 09, 15),
                    EndingOn = new DateTime(2026, 05, 15),
                    SchoolId = dbContext.Schools.First().Id,
                    HomeroomTeacherId = null,
                },
            });
        }
    }
}
