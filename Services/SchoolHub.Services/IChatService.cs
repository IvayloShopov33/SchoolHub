namespace SchoolHub.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SchoolHub.Web.ViewModels.Chat;

    public interface IChatService
    {
        Task AddChatMessageAsync(string classId, string senderId, string senderName, string message);

        Task<List<ChatMessagesFetchViewModel>> FetchMessageHistory(string classId);

        Task MarkMessagesAsRead(string classId);
    }
}
