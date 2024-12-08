namespace SchoolHub.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using SchoolHub.Data.Common.Repositories;
    using SchoolHub.Data.Models;
    using SchoolHub.Services.Mapping;
    using SchoolHub.Web.ViewModels.Chat;

    public class ChatService : IChatService
    {
        private readonly IRepository<ChatMessage> chatRepository;

        public ChatService(IRepository<ChatMessage> chatRepository)
        {
            this.chatRepository = chatRepository;
        }

        public async Task AddChatMessageAsync(string classId, string senderId, string senderName, string message)
        {
            var chatMessage = new ChatMessage
            {
                ClassId = classId,
                SenderId = senderId,
                SenderName = senderName,
                Message = message,
                Timestamp = DateTime.UtcNow,
            };

            await this.chatRepository.AddAsync(chatMessage);
            await this.chatRepository.SaveChangesAsync();
        }

        public async Task<List<ChatMessagesFetchViewModel>> FetchMessageHistory(string classId)
            => await this.chatRepository
                .All()
                .Where(m => m.ClassId == classId)
                .OrderBy(m => m.Timestamp)
                .To<ChatMessagesFetchViewModel>()
                .ToListAsync();

        public async Task MarkMessagesAsRead(string classId)
        {
            var messages = this.chatRepository.All()
                .Where(m => m.ClassId == classId && !m.IsRead);

            foreach (var message in messages)
            {
                message.IsRead = true;
            }

            await this.chatRepository.SaveChangesAsync();
        }
    }
}
