using CodeBattleArena.Models.DBModels;
using CodeBattleArena.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CodeBattleArena.Controllers
{
    public class ChatController : Controller
    {
        private DBService dbService;

        private readonly Notification _notification;

        public ChatController(DBService _dbService, Notification notification)
        {
            this.dbService = _dbService;
            _notification = notification;
        }

        public async Task<IActionResult> GetPersonalChats()
        {
            int idPlayerAuth = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            List<ChatModel> playerChats = await dbService.ChatService.GetChatsByPlayerIdAsync(idPlayerAuth);

            ViewBag.PlayerId = idPlayerAuth;

            return View(playerChats);
        }
        public async Task<IActionResult> PersonalChat(int? idChat, int? recipientId)
        {
            ChatModel chatModel = null;

            if (idChat != null)
            {
                chatModel = await dbService.ChatService.GetChatAsync(idChat.Value);
            }

            else if(recipientId != null)
            {
                int? idChatFind = await dbService.ChatService.AddChatAsync(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), recipientId);

                chatModel = await dbService.ChatService.GetChatAsync(idChatFind.Value);
            }

            ViewBag.PlayerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewBag.NickName = User.FindFirstValue(ClaimTypes.Name);

            return View(chatModel);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int chatId, string senderNick, string senderId, string recipientId, string message)
        {
            if(await dbService.ChatService.AddMessageAsync(chatId, int.Parse(senderId), message))
            {
                string fontColor;
                if (User.FindFirstValue(ClaimTypes.NameIdentifier) == senderId) fontColor = "primary";
                else fontColor = "success";

                await _notification.SendMessagePersonalChat(senderId, recipientId, senderNick, message, "primary", "success");

                PlayerModel nickName = await dbService.PlayerService.GetPlayerAsync(int.Parse(senderId));
                await _notification.SendFriendNotification(nickName.NickName, int.Parse(recipientId), "Sent you a new message!", false);
            }
            else
            {
                return Json(new { success = false, errorMessage = "Error sending message" });
            }
            return Json(new { success = true });
        }
        public async Task<IActionResult> DeleteChat(int idChat)
        {
            ChatModel chatModel = dbService.ChatService.GetChat(idChat);

            if (chatModel == null || !await dbService.ChatService.DeleteChatAsync(idChat))
            {
                return Json(new { success = false, errorMessage = "There was an error deleting the chat" });
            }

            await _notification.DeletePersonalChat(chatModel.IdPlayer1.ToString(), chatModel.IdPlayer2.ToString());


            return Json(new { success = true });
        }
    }
}
