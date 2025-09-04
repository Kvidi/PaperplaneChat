namespace PaperplaneChat.Models.VM
{
    public class ChatVM
    {
        public string UserMessage { get; set; } = string.Empty;      
        public List<(string Role, string Content)> ChatHistory { get; set; } = new();
        public bool FirstMessageSent { get; set; } = false;
    }
}
