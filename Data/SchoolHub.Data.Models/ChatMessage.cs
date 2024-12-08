namespace SchoolHub.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using static SchoolHub.Data.Common.ModelsValidationConstraints;

    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SenderId { get; set; }

        [Required]
        public string SenderName { get; set; }

        [Required]
        public string ClassId { get; set; }

        [Required]
        [MaxLength(ChatMessageMaxLength)]
        public string Message { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public bool IsRead { get; set; } = false;
    }
}
