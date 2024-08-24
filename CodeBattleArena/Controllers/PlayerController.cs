using CodeBattleArena.Models.DBModels;
using CodeBattleArena.Models.ViewModel;
using CodeBattleArena.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using System.Security.Claims;

namespace CodeBattleArena.Controllers
{
    [Authorize]
    public class PlayerController : Controller
    {
        private DBService dbService;

        private readonly Notification _notification;

        public PlayerController(DBService _dbService, Notification notification)
        {
            this.dbService = _dbService;
            _notification = notification;
        }

        [HttpGet]
        public async Task<IActionResult> InfoPlayer(int playerId)
        {

            PlayerModel player = await dbService.PlayerService.GetPlayerAsync(playerId);

            ViewBag.PlayerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            return View("InfoPlayer", player);
        }

        public async Task<IActionResult> FindFriends()
        {
            Dictionary<int, string> players = await dbService.PlayerService.GetListAllPlayersAsync();
            players.Remove(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));

            return View(players);
        }
        public async Task<IActionResult> GetFriends()
        {
            Dictionary<int, string> players = await dbService.PlayerService.GetAllFriendsAsync(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));

            return View(players);
        }

        [HttpGet]
        public async Task<IActionResult> ListMyGame()
        {
            int idPlayerAuth = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var listMyGame = await dbService.PlayerService.MyGamesListAsync(idPlayerAuth);

            return View(listMyGame);
        }

        public async Task<IActionResult> DeleteFriend(int ?idFriend)
        {
            if (idFriend == null)
            {
                return Json(new { success = false, errorMessage = "Error when deleting a friend!" });
            }

            if (!await dbService.PlayerService.DeleteFriendAsync(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), idFriend))
            {
                return Json(new { success = false, errorMessage = "Error when deleting a friend!" });
            }

            PlayerModel nickName = await dbService.PlayerService.GetPlayerAsync(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));

            await _notification.SendFriendNotification(nickName.NickName, idFriend.Value, "Removed you from friends!", true);

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> AddFriend(List<int> idPlayers)
        {
            if (idPlayers.Count == 0 || idPlayers == null)
            {
                return Json(new { success = false, errorMessage = "You haven't selected any players." });
            }
            int idPlayerAuth = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            foreach (var idPlayer in idPlayers)
            {
                int? idChat = await dbService.ChatService.AddChatAsync(idPlayerAuth, idPlayer);
                if(idChat != null)
                {
                    await dbService.ChatService.AddMessageAsync(idChat ?? default(int), idPlayerAuth, User.FindFirstValue(ClaimTypes.Name) + " wants to add you as a friend!");

                    PlayerModel nickName = await dbService.PlayerService.GetPlayerAsync(idPlayer);
                    await _notification.SendFriendNotification(nickName.NickName, idPlayer, "You have a new friend request!", true);
                }
                else
                {
                    return Json(new { success = false, errorMessage = "Error creating chat!" });
                }
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> AcceptInvitationFriends(int? idPlayer1, int? idPlayer2)
        {

            if (idPlayer1 == null || idPlayer2 == null)
            {
                return Json(new { success = false, errorMessage = "Error when adding as a friend!" });
            }

            if (!await dbService.PlayerService.AddFriendAsync(idPlayer1, idPlayer2))
            {
                return Json(new { success = false, errorMessage = "Error when adding as a friend!" });
            }

            PlayerModel playerNick = null;

            if(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) == idPlayer1)
            {
                playerNick = await dbService.PlayerService.GetPlayerAsync(idPlayer1);
                await _notification.SendFriendNotification(playerNick.NickName, idPlayer2.Value, "Accepted friend request!", true);
            }
            else
            {
                playerNick = await dbService.PlayerService.GetPlayerAsync(idPlayer2);
                await _notification.SendFriendNotification(playerNick.NickName, idPlayer1.Value, "Accepted friend request!", true);
            }

            return Json(new { success = true });
        }
        [HttpPost]
        public async Task<IActionResult> ApplyChanges(int? idPlayer, string NickName, string Name, string Info, string ImgAvatar)
        {
            if (idPlayer == null)
            {
                return Json(new { success = false, errorMessage = "Player ID is required!" });
            }

            if (string.IsNullOrEmpty(NickName) || string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Info))
            {
                return Json(new { success = false, errorMessage = "Fill in all the fields!" });
            }

            PlayerModel playerModel = await dbService.PlayerService.GetPlayerAsync(idPlayer);

            if (playerModel == null)
            {
                return Json(new { success = false, errorMessage = "Error when changing player data!" });
            }

            bool isModified = false;

            if (playerModel.NickName != NickName)
            {
                if (!await dbService.PlayerService.CheckNickNameAsync(NickName))
                {
                    playerModel.NickName = NickName;
                    isModified = true;
                }
                else
                {
                    return Json(new { success = false, errorMessage = "This nickname already exists!" });
                }
            }

            if (playerModel.Name != Name)
            {
                playerModel.Name = Name;
                isModified = true;
            }

            if (playerModel.AdditionalInformation != Info)
            {
                playerModel.AdditionalInformation = Info;
                isModified = true;
            }

            if(playerModel.Avatar != ImgAvatar)
            {
                playerModel.Avatar = ImgAvatar;
                isModified = true;
            }

            if (isModified)
            {
                if (!await dbService.PlayerService.SaveEditAsync(playerModel))
                {
                    return Json(new { success = false, errorMessage = "Error when changing player data!" });
                }
            }

            return Json(new { success = true });
        }
    }
}
