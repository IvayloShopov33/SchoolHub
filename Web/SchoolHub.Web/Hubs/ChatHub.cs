namespace SchoolHub.Web.Hubs
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.SignalR;
    using SchoolHub.Services;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class ChatHub : Hub
    {
        private readonly IChatService chatService;

        public ChatHub(IChatService chatService)
        {
            this.chatService = chatService;
        }

        public async Task SendMessage(string classId, string senderId, string senderName, string message)
        {
            await this.chatService.AddChatMessageAsync(classId, senderId, senderName, message);

            await this.Clients.Group(classId).SendAsync("ReceiveMessage", senderName, message, DateTime.UtcNow.ToString(DateTimeFormat), false);
        }

        public async Task JoinClass(string classId)
        {
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, classId);
        }

        public async Task LeaveClass(string classId)
        {
            await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, classId);
        }

        public async Task FetchMessageHistory(string classId)
        {
            var messages = await this.chatService.FetchMessageHistory(classId);

            // Mark messages as read
            await this.chatService.MarkMessagesAsRead(classId);

            // Notify all clients in the group about read status
            await this.Clients.Group(classId).SendAsync("MarkAsRead", classId);

            await this.Clients.Caller.SendAsync("FetchMessageHistory", messages);
        }

        public async Task Typing(string classId, string senderName)
        {
            await this.Clients.Group(classId).SendAsync("ShowTypingIndicator", senderName);
        }

        public async Task MarkAsRead(string classId)
        {
            await this.chatService.MarkMessagesAsRead(classId);

            await this.Clients.Group(classId).SendAsync("MarkAsRead", classId);
        }
    }
}
