namespace SchoolHub.Web.ViewModels.Chat
{
    using AutoMapper;

    using SchoolHub.Services.Mapping;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class ChatMessagesFetchViewModel : IMapFrom<SchoolHub.Data.Models.ChatMessage>, IHaveCustomMappings
    {
        public string SenderName { get; set; }

        public string Message { get; set; }

        public string Timestamp { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<SchoolHub.Data.Models.ChatMessage,  ChatMessagesFetchViewModel>()
                .ForMember(x => x.Timestamp, mo => mo.MapFrom(y => y.Timestamp.ToString(DateTimeFormat)));
        }
    }
}
