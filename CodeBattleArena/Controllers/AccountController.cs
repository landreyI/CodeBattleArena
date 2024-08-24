using CodeBattleArena.Models.DBModels;
using CodeBattleArena.Models.ViewModel;
using CodeBattleArena.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using System.Security.Claims;

namespace CodeBattleArena.Controllers
{
    public class AccountController : Controller
    {
        private readonly DBService dbService;
        public AccountController(DBService _bdService)
        {
            dbService = _bdService;
        }

        [HttpPost]
        public async Task<IActionResult> Registration(CreatePlayerModel player)
        {
            string errorMessage = "";

            if (!ModelState.IsValid)
            {
                errorMessage = ModelState.Values.SelectMany(v => v.Errors)
                                  .FirstOrDefault()?.ErrorMessage;
                
            }
            else
            {
                if(!await dbService.PlayerService.CheckEmailAsync(player.Email) && !await dbService.PlayerService.CheckNickNameAsync(player.NickName))
                {
                    PlayerModel playerModel = new PlayerModel
                    {
                        Name = player.Name,
                        NickName = player.NickName,
                        Email = player.Email,
                        Password = player.Password,
                        Role = "Player",
                        Avatar = "/img/avatar/avatar1.png"
                    };
                    if (await dbService.PlayerService.AddPlayerAsync(playerModel))
                    {
                        await HttpContext.SignInAsync("Cookies", UserVerification.AuthorizationUser(playerModel), UserVerification.AuthenProperties());

                        return Json(new { success = true });
                    }
                    errorMessage = "Error when adding Player to table DB";
                }
                else errorMessage = "A player with this nickname or email is already taken";
            }

            return Json(new { success = false, errorMessage = errorMessage });
        }

        [HttpPost]
        public async Task<IActionResult> Authorization(string Email, string Password)
        {
            string errorMessage = "";

            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                errorMessage = "Please fill in all required fields.";
            }
            else
            {
                if (await dbService.PlayerService.CheckEmailAsync(Email))
                {
                    var player = await dbService.PlayerService.GetPlayerAsync(Email);
                    if (player != null && Password == player.Password)
                    {
                        await HttpContext.SignInAsync("Cookies", UserVerification.AuthorizationUser(player), UserVerification.AuthenProperties());

                        return Json(new { success = true });
                    }
                    errorMessage = "Incorrect password";
                }
                else errorMessage = "There is no player with this email";
            }

            return Json(new { success = false, errorMessage = errorMessage });
        }
        public async Task<IActionResult> Clouse()
        {
            if(HttpContext.Request.Cookies["IdSession"] != null)
            {
                int? idPlayerAuth = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                int? IdSession = int.Parse(HttpContext.Request.Cookies["IdSession"]);

                if (IdSession != null || idPlayerAuth != null)
                {
                    if (await dbService.SessionService.DelPlayerSessionAsync(IdSession, idPlayerAuth))
                    {
                        PlayerModel nickName = await dbService.PlayerService.GetPlayerAsync(idPlayerAuth);
                    }
                    if (await dbService.SessionService.GetPlayerCountInSessionAsync(IdSession) == 0)
                    {
                        if (!await dbService.SessionService.GetVictorySessionAsync(IdSession))
                        {
                            await dbService.SessionService.DelSessionAsync(IdSession);
                        }
                    }
                }


                Response.Cookies.Delete("IdSession");
                Response.Cookies.Delete("KeySession");
                Response.Cookies.Delete("NameSession");
                Response.Cookies.Delete("StateSassion");
                Response.Cookies.Delete("AmountPiople");
                Response.Cookies.Delete("Difficulty");
                Response.Cookies.Delete("LangProgramming");
            }

            await HttpContext.SignOutAsync("Cookies");

            return RedirectToAction("HomePage", "Home");
        }

    }
}
