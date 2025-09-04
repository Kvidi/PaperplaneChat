namespace PaperplaneChat.Services.Interfaces
{
    public interface IChatService
    {        
        Task<string> GetChatConversationAsync(List<(string, string)> userInput);
        Task<string> GenerateImageAsync(string text);
    }
}
