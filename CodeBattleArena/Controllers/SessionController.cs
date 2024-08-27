using CodeBattleArena.Filters;
using CodeBattleArena.Models;
using CodeBattleArena.Models.DBModels;
using CodeBattleArena.Models.ViewModel;
using CodeBattleArena.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CodeBattleArena.Controllers
{
    [Authorize]
    public class SessionController : Controller
    {
        private DBService dbService;

        private readonly Notification _notification;

        public SessionController(DBService _dbService, Notification notification)
        {
            this.dbService = _dbService;
            _notification = notification;
        }

        [HttpGet]
        public IActionResult CreateSession()
        {
            return View(new CreateSessionModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateSession(CreateSessionModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int playerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            string KeySession = Guid.NewGuid().ToString();

            SessionModel sessionModel = new SessionModel
            { 
                Key = KeySession,
                Name = model.Name,
                AmountPiople = model.AmountPiople,
                LangProgramming = model.LangProgramming,
                State = model.State,
                Difficulty = model.Difficulty,
                CreatorId = playerId,
                DateCreating = DateTime.Now
            };


            if(!await dbService.SessionService.AddSessionAsync(sessionModel))
            {
                ViewBag.ErrorMessage = "Looks like there was an error when adding your session to the database!";
                return View(model);
            }

            sessionModel = await dbService.SessionService.GetSessionAsync(KeySession);

            if(sessionModel != null)
            {
                PlayerSessionModel playerSessionModel = new PlayerSessionModel
                {
                    IdPlayer = playerId,
                    IdSession = sessionModel.IdSession
                };

                if(!await dbService.SessionService.AddPlayerSessionAsync(playerSessionModel))
                {
                    ViewBag.ErrorMessage = "Error when adding a player to the database";
                    return View(model);
                }

                CookieOptions options = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1)
                };

                Response.Cookies.Append("IdSession", sessionModel.IdSession.ToString(), options);
                Response.Cookies.Append("KeySession", sessionModel.Key, options);
                Response.Cookies.Append("NameSession", model.Name, options);
                Response.Cookies.Append("StateSassion", model.State.ToString(), options);
                Response.Cookies.Append("AmountPiople", model.AmountPiople.ToString(), options);
                Response.Cookies.Append("Difficulty", model.Difficulty, options);
                Response.Cookies.Append("LangProgramming", model.LangProgramming, options);

                return RedirectToAction("InfoSession", new { sessionId = sessionModel.IdSession });
            }
            else
            {
                ViewBag.ErrorMessage = "Looks like there was an error when adding your session to the database!";
                return View(model);
            }
        }

        [CheckSessionCookie]
        public async Task<IActionResult> ProcessCode()
        {
            int? IdSession = int.Parse(HttpContext.Request.Cookies["IdSession"]);
            int? idPlayerAuth = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            SessionModel sessionModel = await dbService.SessionService.GetSessionAsync(IdSession);

            if(sessionModel == null || sessionModel.TaskProgramming == null) return RedirectToAction("HomePage", "Home");

            PlayerSessionModel playerSession = await dbService.SessionService.GetPlayerSessionAsync(IdSession, idPlayerAuth);
            if (playerSession == null)
            {
                return RedirectToAction("Home", "ErrorMessage", new { message = "Error, you could not be found in the session database!" });
            }
            if (playerSession.State == true)
            {
                return RedirectToAction("Home", "ErrorMessage", new { message = "You have already completed the task." });
            }

            string? codePlayer = null;

            CodeModel obgCode = new CodeModel
            {
                Preparation = sessionModel.TaskProgramming.Preparation,
                Lang = sessionModel.LangProgramming,
            };

            
            codePlayer = playerSession.CodeText;
            obgCode.Code = codePlayer;

            if (playerSession.Time != null && playerSession.Memory != null)
            {
                ExecutionResult executionResult = new ExecutionResult
                {
                    Time = playerSession.Time,
                    Memory = playerSession.Memory.ToString()
                };
                obgCode.ExecutionResult = executionResult;
            }
            

            ViewBag.TaskSession = sessionModel.TaskProgramming;

            ViewBag.TaskListInputData = await dbService.TaskService.GetTaskInputDataByIdTaskAsync(sessionModel.TaskId);

            //Site theme
            if (HttpContext.Request.Cookies["theme"] != null)
            {
                string theme = HttpContext.Request.Cookies["theme"].ToString();

                if (theme == "Darkly")
                {
                    ViewBag.ThemeSite = "vs-dark";
                }
                if (theme == "Sketch")
                {
                    ViewBag.ThemeSite = "vs-light";
                }
                if (theme == "Minty")
                {
                    ViewBag.ThemeSite = "vs-light";
                }
            }

            return View(obgCode);
        }

        [HttpPost]
        [CheckSessionCookie]
        public async Task<IActionResult> ProcessCode(CodeModel obgCode)
        {
            int? IdSession = int.Parse(HttpContext.Request.Cookies["IdSession"]);
            SessionModel sessionModel = await dbService.SessionService.GetSessionAsync(IdSession);

            var taskListInputData = await dbService.TaskService.GetTaskInputDataByIdTaskAsync(sessionModel.TaskId);

            List<string> stdinList = new List<string>();
            List<string> expectedOutputList = new List<string>();

            foreach (var taskInputData in taskListInputData)
            {
                stdinList.Add(taskInputData.InputData.Data);
                expectedOutputList.Add(taskInputData.Answer);
            }

            // Объединяем элементы списка в строки с пробелами
            string code = obgCode.Code + $"\n{sessionModel.TaskProgramming.VerificationCode}";
            string idLang = "";

            if (obgCode.Lang == "csharp") idLang = "51";
            else if (obgCode.Lang == "cpp") idLang = "54";
            else if (obgCode.Lang == "java") idLang = "62";
            else if (obgCode.Lang == "javascript") idLang = "63";
            else if (obgCode.Lang == "python") idLang = "71";

            string stdin = string.Join(" ", stdinList);
            string expectedOutput = string.Join(" ", expectedOutputList);

            ExecutionResult result = await Judge0Client.Check(code, idLang, stdin, expectedOutput);

            obgCode.ExecutionResult = result;

            ViewBag.TaskSession = sessionModel.TaskProgramming;

            ViewBag.TaskListInputData = await dbService.TaskService.GetTaskInputDataByIdTaskAsync(sessionModel.TaskId);

            return View(obgCode);
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> SaveCode([FromForm] string code)
        {
            int? IdSession = int.Parse(HttpContext.Request.Cookies["IdSession"]);

            int? idPlayerAuth = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if(string.IsNullOrEmpty(code) || !await dbService.SessionService.SaveCodePlayerAsync(IdSession, idPlayerAuth, code))
            {
                return RedirectToAction("Home", "ErrorMessage", new { message = "Failed to save code." });
            }

            return Ok();
        }

        [CheckSessionCookie]
        public async Task<IActionResult> LeaveSession()
        {
            int? idPlayerAuth = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            int? IdSession = int.Parse(HttpContext.Request.Cookies["IdSession"]);
            SessionModel sessionModel = await dbService.SessionService.GetSessionAsync(IdSession);

            if (idPlayerAuth != null && sessionModel != null)
            {
                if (sessionModel.WinnerId == null && sessionModel.IsFinish == false)
                {
                    await dbService.SessionService.DelPlayerSessionAsync(IdSession, idPlayerAuth);
                }
                
                PlayerModel nickName = await dbService.PlayerService.GetPlayerAsync(idPlayerAuth);

                if(sessionModel.State == "Private" && sessionModel.CreatorId == nickName.IdPlayer)
                {
                    if (sessionModel.WinnerId == null && sessionModel.IsFinish == false)
                    {
                        Dictionary<int, string> listPlayerSession = await dbService.SessionService.GetListPlayerFromSessionAsync(IdSession);

                        foreach (var item in listPlayerSession)
                        {
                            await dbService.SessionService.DelPlayerSessionAsync(IdSession, item.Key);
                        }
                    }
                    await _notification.SendNotificationSessionCreatorExit(idPlayerAuth.Value, IdSession.Value, $"Session creator - {nickName.NickName} left the session!");
                }
                else
                {
                    await _notification.SendNotificationSession(IdSession.Value, $"Player - {nickName.NickName} left the session!", true, false);
                }
                
                if (await dbService.SessionService.GetPlayerCountInSessionAsync(IdSession) == 0)
                {
                    if(!await dbService.SessionService.GetVictorySessionAsync(IdSession))
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

            return RedirectToAction("HomePage", "Home");
        }

        public async Task<IActionResult> InfoSession(int? sessionId)
        {
            int cookiesSessionId = -1;
            if (HttpContext.Request.Cookies["IdSession"] != null)
            {
                cookiesSessionId = int.Parse(HttpContext.Request.Cookies["IdSession"]);
            }

            if (sessionId == null)
            {
                sessionId = cookiesSessionId;
            }

            int idPlayerAuth = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            PlayerSessionModel playerSession = await dbService.SessionService.GetPlayerSessionAsync(sessionId, idPlayerAuth);

            SessionModel sessionModel = await dbService.SessionService.GetSessionAsync(sessionId);

            if (playerSession != null && playerSession.IdSession == cookiesSessionId)
            {
                ViewBag.LeaveSession = true;
                if((sessionModel.State != "Private" || sessionModel.CreatorId == idPlayerAuth) && sessionModel.IsFinish == false)
                {
                    ViewBag.EditSession = true;

                    Dictionary<int, string> friends = await dbService.PlayerService.GetAllFriendsAsync(idPlayerAuth);
                    ViewBag.FriendsPlayer = friends;
                }
            }

            return View(sessionModel);
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> AddPlayer(List<int> idPlayers)
        {
            if (idPlayers.Count == 0 || idPlayers == null)
            {
                return Json(new { success = false, errorMessage = "You haven't selected any players." });
            }

            int? idSession = int.Parse(HttpContext.Request.Cookies["IdSession"]);

            if (idSession == null)
            {
                return Json(new { success = false, errorMessage = "Error when sending a message" });
            }

            foreach (var idPlayer in idPlayers)
            {
                PlayerModel nickNmae = await dbService.PlayerService.GetPlayerAsync(idPlayer);
                await _notification.SendNotificationJoinSession(nickNmae.NickName, idPlayer, idSession.Value, "You have been invited to join the game!");
            }
            return Json(new { success = true });
        }

        [HttpPost]
        [CheckSessionCookie]
        public async Task<IActionResult> DeletePlayer(int? playerId)
        {
            if (playerId == null)
            {
                return Json(new { success = false, errorMessage = "You haven't selected any player." });
            }

            int? IdSession = int.Parse(HttpContext.Request.Cookies["IdSession"]);

            SessionModel session = dbService.SessionService.GetSession(IdSession);

            PlayerModel nickName = dbService.PlayerService.GetPlayer(playerId);

            await _notification.SendNotificationSessionCreatorExclusion(playerId.Value, $"You have been excluded from the session");

            if(session.WinnerId == null && session.IsFinish == false)
            {
                dbService.SessionService.DelPlayerSession(IdSession, playerId);
            }

            await _notification.SendNotificationSession(IdSession.Value, $"Player - {nickName.NickName} was excluded from the session.", true, false);

            return Json(new { success = true });
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(int? idSession, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return Json(new { success = false, errorMessage = "You haven't selected any password." });
            }

            if(idSession == null || !await dbService.SessionService.ChangePasswordSessionAsync(idSession, password))
            {
                return Json(new { success = false, errorMessage = "Error when changing password!" });
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> JoinSession(int sessionId, bool isInvite, string? password)
        {
            SessionModel model = await dbService.SessionService.GetSessionAsync(sessionId);
            if(model == null)
            {
                return Json(new { success = false, errorMessage = "Failed to join the session!" });
            }

            if(isInvite == false && model.State == "Private")
            {
                if(password == null || model.Password != password)
                {
                    return Json(new { success = false, errorMessage = "Failed to join the session!" });
                }
            }

            Dictionary<int, string> players = await dbService.SessionService.GetListPlayerFromSessionAsync(sessionId);
            if(players == null || model.AmountPiople <= players.Count)
            {
                return Json(new { success = false, errorMessage = "This session already has the maximum number of players!" });
            }


            int playerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            PlayerSessionModel playerSessionModel = new PlayerSessionModel
            {
                IdSession = sessionId,
                IdPlayer = playerId
            };
            if(!await dbService.SessionService.AddPlayerSessionAsync(playerSessionModel))
            {
                return Json(new { success = false, errorMessage = "Failed to join the session!" });
            }

            SessionModel sessionModel = await dbService.SessionService.GetSessionAsync(sessionId);

            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1)
            };

            Response.Cookies.Append("IdSession", sessionModel.IdSession.ToString(), options);
            Response.Cookies.Append("KeySession", sessionModel.Key, options);
            Response.Cookies.Append("NameSession", sessionModel.Name, options);
            Response.Cookies.Append("StateSassion", sessionModel.State.ToString(), options);
            Response.Cookies.Append("AmountPiople", sessionModel.AmountPiople.ToString(), options);
            Response.Cookies.Append("Difficulty", sessionModel.Difficulty, options);
            Response.Cookies.Append("LangProgramming", sessionModel.LangProgramming, options);

            PlayerModel nickName = await dbService.PlayerService.GetPlayerAsync(playerId);
            await _notification.SendNotificationSession(sessionId, $"Player - {nickName.NickName} joined the session!", true, false);

            return Json(new { success = true });
        }

        public async Task<IActionResult> ListSession(string state)
        {
            return View(await dbService.SessionService.GetListSssionAsync(state));
        }

        public async Task<IActionResult> ListTask(bool isLang)
        {
            ViewBag.LangProgrammings = new LangProgramming().LangProgrammings;

            if (!isLang)
                return View(await dbService.TaskService.GetTaskListAsync());
            else
            {
                ViewBag.IsLang = true;
                string language = HttpContext.Request.Cookies["LangProgramming"];
                return View(await dbService.TaskService.GetTaskListAsync(language));
            }
        }


        [HttpGet]
        public async Task<IActionResult> InfoTask(int? idTask)
        {
            TaskProgrammingModel model = await dbService.TaskService.GetTaskByIdAsync(idTask);

            return View(model);
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> AddTaskToSession(int? IdTaskProgramming)
        {
            int? idSession = int.Parse(HttpContext.Request.Cookies["IdSession"]);

            if (idSession == null || IdTaskProgramming == null)
            {
                return Json(new { success = false, errorMessage = "Error when adding a task to a session!" });
            }

            SessionModel sessionModel = dbService.SessionService.GetSession(idSession);

            if (sessionModel == null)
            {
                return Json(new { success = false, errorMessage = "Error when adding a task to a session!" });
            }

            if (sessionModel.DateStart != null)
            {
                return Json(new { success = false, errorMessage = "Error, the game has already started, so the task cannot be changed!" });
            }

            if (!await dbService.SessionService.AddTaskToSession(idSession, IdTaskProgramming))
            {
                return Json(new { success = false, errorMessage = "Error when adding a task to a session!" });
            }

            return Json(new { success = true });
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> StartGame()
        {
            int? idSession = int.Parse(HttpContext.Request.Cookies["IdSession"]);
            int? idPlayer = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (idSession == null || idPlayer == null)
            {
                return Json(new { success = false, errorMessage = "Error when starting the game!" });
            }

            SessionModel session = dbService.SessionService.GetSession(idSession);
            if (session == null)
            {
                return Json(new { success = false, errorMessage = "Error, there is no such session in the database!" });
            }

            if(session.CreatorId != idPlayer)
            {
                return Json(new { success = false, errorMessage = "Error, you are not the creator of this session, so you cannot start it!" });
            }

            if(session.TaskProgramming == null)
            {
                return Json(new { success = false, errorMessage = "Error, the task for the game has not yet been selected!" });
            }

            await _notification.SendNotificationSession(idSession.Value, $"Game starts in:", false, true);
            await Task.Delay(1000);

            for (int i = 10; i > 0; i--)
            {
                await _notification.SendNotificationSession(idSession.Value, $"{i}!", false, true);
                await Task.Delay(1000);
            }

            await _notification.SendNotificationSessionStartGame(idSession.Value);

            if(!await dbService.SessionService.AddStartTimeAsync(idSession, DateTime.Now))
            {
                return Json(new { success = false, errorMessage = "Error when updating the start time for session!" });
            }

            return Json(new { success = true });
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> SaveResult(string Time, int Memory)
        {
            int? idPlayerAuth = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            int? IdSession = int.Parse(HttpContext.Request.Cookies["IdSession"]);

            if (!await dbService.SessionService.SaveResultAsync(IdSession, idPlayerAuth, Time, Memory))
            {
                return Json(new { success = false, errorMessage = "Error when saving!" });
            }

            return Json(new { success = true });
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> FinishTask(string Time, int Memory)
        {
            int? idPlayerAuth = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            int? idSession = int.Parse(HttpContext.Request.Cookies["IdSession"]);

            await dbService.SessionService.SaveResultAsync(idSession, idPlayerAuth, Time, Memory);
            await dbService.SessionService.FinishTaskAsync(idSession, idPlayerAuth);

            return RedirectToAction("InfoSession", "Session", idSession);
        }

        [CheckSessionCookie]
        [HttpGet]
        public async Task<IActionResult> FinishSession()
        {
            int sessionId = int.Parse(HttpContext.Request.Cookies["IdSession"]);

            SessionModel sessionModel = await dbService.SessionService.GetSessionAsync(sessionId);

            PlayerSessionModel winner = null;
            double bestTime = double.MaxValue;
            int bestMemory = int.MaxValue;


            foreach (var playerSession in sessionModel.PlayerSessions)
            {

                double currentTime;
                double.TryParse(playerSession.Time, NumberStyles.Any, CultureInfo.InvariantCulture, out currentTime);

                int currentMemory = playerSession.Memory.Value;

                if (currentTime < bestTime)
                {
                    bestTime = currentTime;
                    bestMemory = currentMemory;
                    winner = playerSession;
                }

                else if (currentTime == bestTime && currentMemory < bestMemory)
                {
                    bestMemory = currentMemory;
                    winner = playerSession;
                }
            }

            if (winner != null)
            {
                sessionModel.WinnerId = winner.IdPlayer;
                sessionModel.IsFinish = true;

                await dbService.SessionService.UpdateSessionAsync(sessionModel);
                await dbService.PlayerService.AddVictoryPlayerAsync(winner.IdPlayer);

                await _notification.SendNotificationSession(sessionId, $"The results of the game are already known, go to the session information to watch!", true, false);

                return View(sessionModel);
            }

            return RedirectToAction("Home", "ErrorMessage", new { message = "Failed to determine winner." });
        }
    }
}
