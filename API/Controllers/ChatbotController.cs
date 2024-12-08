using API.ML.BO;
using API.ML.BOBase;
using API.ML.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI;
using Newtonsoft.Json;

namespace API.Controllers
{
    public class ChatbotController : MLBaseController<ChatbotConversation>
    {
        private HttpClient _httpClient;

        public ChatbotController(ApplicationDBContext context, HttpClient client) : base(context)
        {
            _httpClient = client;
        }

        [HttpGet("GetChatbotConversation")]
        public MLActionResult GetChatbotConversation(Guid conversationID)
        {
            MLActionResult result = new()
            {
                Success = true,
            };

            try
            {
                ChatbotConversation? conversation = _context.ChatbotConversation.AsNoTracking().FirstOrDefault(cc => cc.ConversationID == conversationID);
                if (conversation != null)
                {
                    var details = _context.ChatbotConversationDetail.AsNoTracking().Where(ccd => ccd.ConversationID == conversationID).OrderBy(ccd => ccd.Timestamp).ToList();
                    conversation.ChatbotConversationDetails = details;
                    result.Data = conversation;
                }
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        [HttpGet("GetNewChatbotResponse")]
        public async Task<MLActionResult<ChatbotConversationDetail>> GetNewChatbotResponse(Guid conversationID, string message)
        {
            MLActionResult<ChatbotConversationDetail> result = new();

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"http://127.0.0.1:9000/GetNewChatbotResponse?conversationID={conversationID}&message={message}");
                response.EnsureSuccessStatusCode();

                return JsonConvert.DeserializeObject<MLActionResult<ChatbotConversationDetail>>(await response.Content.ReadAsStringAsync()) ?? result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMsg = ex.Message;
            }

            return result;
        }

        [HttpPost("CreateNewConversation")]
        public MLActionResult CreateNewConversation()
        {
            MLActionResult result = new();

            try
            {
                ChatbotConversation chatbotConversation = new ChatbotConversation();
                _context.ChatbotConversation.Add(chatbotConversation);

                result.Success = _context.SaveChanges() > 0;
                if (result.Success)
                {
                    result.Data = chatbotConversation;
                }
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        [HttpPost("SendChatbotMessage")]
        public MLActionResult SendChatbotMessage(ChatbotConversationDetail detail)
        {
            MLActionResult result = new();

            try
            {
                detail.Timestamp = DateTime.UtcNow;

                _context.ChatbotConversationDetail.Add(detail);
                result.Success = _context.SaveChanges() > 0;

                if (result.Success)
                {
                    result.Data = detail;
                }
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }
    }
}
