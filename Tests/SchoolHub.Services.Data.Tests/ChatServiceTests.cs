namespace SchoolHub.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Moq;

    using SchoolHub.Data;
    using SchoolHub.Data.Common.Repositories;
    using SchoolHub.Data.Models;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.Chat;

    using Xunit;

    public class ChatServiceTests
    {
        private readonly string schoolId = Guid.NewGuid().ToString();
        private readonly string classId = Guid.NewGuid().ToString();

        public ChatServiceTests()
        {
            AutoMapperConfig.RegisterMappings(
                typeof(ChatMessage).Assembly,
                typeof(ChatMessagesFetchViewModel).Assembly);
        }

        [Fact]
        public async Task AddChatMessageAsync_ShouldAddMessageToRepository()
        {
            // Arrange
            var mockRepo = await this.GetMockChatMessageRepositoryAsync("TestDb_AddChatMessage");
            var chatService = new ChatService(mockRepo);

            var classId = "class123";
            var senderId = "user123";
            var senderName = "John Doe";
            var message = "Hello, world!";

            // Act
            await chatService.AddChatMessageAsync(classId, senderId, senderName, message);

            // Assert
            var addedMessage = mockRepo.All().FirstOrDefault(m => m.Message == message);

            Assert.NotNull(addedMessage);
            Assert.Equal(classId, addedMessage.ClassId);
            Assert.Equal(senderId, addedMessage.SenderId);
            Assert.Equal(senderName, addedMessage.SenderName);
            Assert.Equal(message, addedMessage.Message);
        }

        [Fact]
        public async Task FetchMessageHistory_ShouldReturnMessagesOrderedByTimestamp()
        {
            // Arrange
            var mockRepo = await this.GetMockChatMessageRepositoryAsync("TestDb_FetchMessageHistory");
            var chatService = new ChatService(mockRepo);

            // Act
            var messages = await chatService.FetchMessageHistory(this.classId);

            // Assert
            Assert.NotNull(messages);
            Assert.Equal(2, messages.Count);
        }

        [Fact]
        public async Task MarkMessagesAsRead_ShouldMarkAllUnreadMessagesAsRead()
        {
            // Arrange
            var mockRepo = await this.GetMockChatMessageRepositoryAsync("TestDb_MarkMessagesAsRead");
            var chatService = new ChatService(mockRepo);

            // Act
            await chatService.MarkMessagesAsRead(this.classId);

            // Assert
            var unreadMessages = mockRepo.All().Where(m => m.ClassId == this.classId && !m.IsRead).ToList();
            Assert.Empty(unreadMessages);

            var allMessages = mockRepo.All().Where(m => m.ClassId == this.classId).ToList();
            Assert.All(allMessages, m => Assert.True(m.IsRead));
            Assert.Equal(2, allMessages.Count);
        }

        private async Task<IRepository<ChatMessage>> GetMockChatMessageRepositoryAsync(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            var context = new ApplicationDbContext(options);

            // Seed data
            this.SeedData(context);
            await context.SaveChangesAsync();

            var mockRepo = new Mock<IRepository<ChatMessage>>();
            mockRepo.Setup(r => r.All())
            .Returns(context.ChatMessages.AsQueryable);

            mockRepo.Setup(r => r.AllAsNoTracking())
                    .Returns(context.ChatMessages.AsQueryable);

            mockRepo.Setup(r => r.AddAsync(It.IsAny<ChatMessage>()))
                    .Callback((ChatMessage chatMessage) =>
                    {
                        context.ChatMessages.Add(chatMessage);
                    });

            mockRepo.Setup(r => r.SaveChangesAsync())
                    .Callback(async () => await context.SaveChangesAsync());

            return mockRepo.Object;
        }

        private void SeedData(ApplicationDbContext context)
        {
            context.Schools.Add(new School { Id = this.schoolId, Name = "Alpha School", Address = "Alpha St", WebsiteUrl = "https://alpha.edu" });
            context.Classes.Add(new Class
            {
                Id = this.classId,
                Name = "Class A",
                SchoolId = this.schoolId,
                EndingOn = DateTime.Now.AddDays(50),
            });

            context.ChatMessages.AddRange(new ChatMessage[]
            {
                new ChatMessage
                {
                    SenderId = Guid.NewGuid().ToString(),
                    SenderName = "Ivan Ivanov",
                    ClassId = this.classId,
                    Message = "Very good!",
                    Timestamp = DateTime.UtcNow,
                },
                new ChatMessage
                {
                    SenderId = Guid.NewGuid().ToString(),
                    SenderName = "Galin Petrov",
                    ClassId = this.classId,
                    Message = "Okay, thanks!",
                    Timestamp = DateTime.UtcNow,
                    IsRead = true,
                },
            });
        }
    }
}
