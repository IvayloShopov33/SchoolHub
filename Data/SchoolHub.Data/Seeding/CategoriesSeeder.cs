namespace SchoolHub.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using SchoolHub.Data.Models;

    public class CategoriesSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Categories.Any())
            {
                return;
            }

            await dbContext.Categories.AddRangeAsync(new List<Category>
            {
                new Category { Name = "Absent" },
                new Category { Name = "Late" },
                new Category { Name = "Exam" },
                new Category { Name = "Homework" },
                new Category { Name = "Term test" },
                new Category { Name = "Entry test" },
                new Category { Name = "Writing test" },
                new Category { Name = "Speaking test" },
                new Category { Name = "Practical test" },
                new Category { Name = "Individual test" },
            });
        }
    }
}
