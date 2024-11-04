namespace SchoolHub.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using SchoolHub.Data.Models;

    public class SubjectsSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Subjects.Any())
            {
                return;
            }

            await dbContext.Subjects.AddRangeAsync(new List<Subject>
            {
                new Subject
                {
                    Name = "Math",
                    Description = "Mathematic teaches problem-solving and analytical skills through numbers, equations, and various mathematical concepts.",
                },
                new Subject
                {
                    Name = "English",
                    Description = "Focuses on reading, writing, and analyzing literature in English, as well as improving language and communication skills.",
                },
                new Subject
                {
                    Name = "Art",
                    Description = "Involves creative expression through drawing, painting, sculpture, and other visual media.",
                },
                new Subject
                {
                    Name = "History",
                    Description = "Studies past events, civilizations, and historical figures to understand human development and cultural heritage.",
                },
                new Subject
                {
                    Name = "Music",
                    Description = "Involves learning about rhythm, melody, musical theory, and sometimes playing instruments or singing.",
                },
                new Subject
                {
                    Name = "Geography",
                    Description = "Examines the Earth’s physical features, climate, population, and human-environment interactions.",
                },
                new Subject
                {
                    Name = "Physical Education",
                    Description = "Promotes physical fitness, teamwork, and healthy lifestyles through sports, exercise, and physical activities.",
                },
                new Subject
                {
                    Name = "Biology",
                    Description = "A branch of science focused on living organisms, including their structure, function, growth, and evolution.",
                },
                new Subject
                {
                    Name = "Chemistry",
                    Description = "Studies matter, its properties, and how substances interact and change to form new materials.",
                },
                new Subject
                {
                    Name = "Physics",
                    Description = "Explores the fundamental principles of the universe, such as motion, energy, and forces.",
                },
                new Subject
                {
                    Name = "Information Technology",
                    Description = "Teaches computer skills, digital literacy, and the basics of programming, as well as information management.",
                },
                new Subject
                {
                    Name = "Philosophy",
                    Description = "Examines fundamental questions about existence, knowledge, values, reason, and the nature of reality.",
                },
            });
        }
    }
}
