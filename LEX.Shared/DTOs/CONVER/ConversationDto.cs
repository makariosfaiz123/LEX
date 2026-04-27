namespace LEX.Application.DTOs.Chat
{
    public class ConversationDto
    {
        public int Id { get; set; }
        public string FirebaseChatId { get; set; }
        public int CaseRequestId { get; set; }
        public string CaseDescription { get; set; }

        // Information about the *other* person in the chat
        public int OtherUserId { get; set; }
        public string OtherUserName { get; set; }
        public string OtherUserImage { get; set; }
        public string OtherUserRole { get; set; } // "Lawyer" or "Customer"

        public DateTime CreatedAt { get; set; }
    }
}