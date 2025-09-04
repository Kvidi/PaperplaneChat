using Azure.AI.OpenAI;
using OpenAI.Chat;
using OpenAI.Images;
using PaperplaneChat.Services.Interfaces;
using Markdig;
using Azure;

namespace PaperplaneChat.Services
{
    public class ChatService : IChatService
    {
        private readonly AzureOpenAIClient _azureClient;

        public ChatService(AzureOpenAIClient azureClient)
        {
            _azureClient = azureClient;
        }        

        public async Task<string> GetChatConversationAsync(List<(string, string)> userInput)
        {
            var conversation = "";
            foreach (var message in userInput)
            {
                conversation += $"{message.Item1}: {message.Item2}\n";
            }

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are an helpful expert assistant"),
                new UserChatMessage(conversation)
            };

            var options = new ChatCompletionOptions
            {
                Temperature = (float)0.7,
                MaxOutputTokenCount = 3000,
                TopP = (float)0.95,
                FrequencyPenalty = (float)0.0,
                PresencePenalty = (float)0.0
            };

            try
            {
                // Initialize the ChatClient with the specified deployment name
                ChatClient chatClient = _azureClient.GetChatClient("gpt-4o");
                // Create the chat completion request
                ChatCompletion completion = await chatClient.CompleteChatAsync(messages, options);
                // Print the response
                if (completion != null)
                {
                    var markdown = completion.Content[0].Text;
                    var html = Markdown.ToHtml(markdown);
                    return html;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> GenerateImageAsync(string text)
        {
            try
            {
                ImageClient imageClient = _azureClient.GetImageClient("dall-e-3");

                var imageGeneration = await imageClient.GenerateImageAsync(text,
                    new ImageGenerationOptions
                    {
                        Size = GeneratedImageSize.W1792xH1024
                    }
                );
                var url = imageGeneration.Value.ImageUri.AbsoluteUri;
                return url;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }


        }
    }
}
