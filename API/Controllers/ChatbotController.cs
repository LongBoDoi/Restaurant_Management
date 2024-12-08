using API.ML.BO;
using API.ML.BOBase;
using API.ML.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API.Controllers
{
    public class ChatbotController : MLBaseController<ChatbotConversation>
    {
        public ChatbotController(ApplicationDBContext context) : base(context)
        {
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
                    var details = _context.ChatbotConversationDetail.AsNoTracking().Where(ccd => ccd.ConversationID == conversationID).ToList();
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
