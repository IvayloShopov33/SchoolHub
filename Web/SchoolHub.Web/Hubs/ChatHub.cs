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

            // Notify all users in the group
            await this.Clients.Group(classId).SendAsync("ReceiveMessage", senderName, message, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
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
            await this.Clients.Caller.SendAsync("FetchMessageHistory", messages);
        }
    }
}
