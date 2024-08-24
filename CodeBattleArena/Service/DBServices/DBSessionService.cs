using CodeBattleArena.Data;
using CodeBattleArena.Models.DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;

namespace CodeBattleArena.Service.DBServices
{
    public class DBSessionService
    {
        private readonly AppDBContext _dbContext;
        private readonly ILogger<DBService> _logger;
        public DBSessionService(AppDBContext dbContext, ILogger<DBService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public bool AddSession(SessionModel session)
        {
            if (session == null) return false;

            try
            {
                _dbContext.Session.Add(session);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when adding Session to table");
                return false;
            }
        }
        public async Task<bool> AddSessionAsync(SessionModel session)
        {
            if (session == null) return false;

            try
            {
                await _dbContext.Session.AddAsync(session);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when adding Session to table");
                return false;
            }
        }


        public bool AddPlayerSession(PlayerSessionModel playerSession)
        {
            if (playerSession == null) return false;

            try
            {
                _dbContext.PlayerSession.Add(playerSession);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when adding PlayerSession to table");
                return false;
            }
        }
        public async Task<bool> AddPlayerSessionAsync(PlayerSessionModel playerSession)
        {
            if (playerSession == null) return false;

            try
            {
                await _dbContext.PlayerSession.AddAsync(playerSession);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when adding PlayerSession to table");
                return false;
            }
        }

        public async Task<bool> SaveCodePlayerAsync(int? idSession, int? idPlayer, string code)
        {
            if (idPlayer == null || idSession == null || string.IsNullOrEmpty(code)) return false;

            try
            {
                PlayerSessionModel playerSession = await _dbContext.PlayerSession.FirstOrDefaultAsync(p => p.IdPlayer == idPlayer && p.IdSession == idSession);

                if (playerSession == null) return false;

                playerSession.CodeText = code;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when edit PlayerSession to table");
                return false;
            }
        }

        public SessionModel GetSession(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;

            try
            {
                return _dbContext.Session.Include(s => s.TaskProgramming).FirstOrDefault(u => u.Key == key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when accessing User");
                return null;
            }
        }
        public async Task<SessionModel> GetSessionAsync(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;

            try
            {
                return await _dbContext.Session.Include(s => s.TaskProgramming).FirstOrDefaultAsync(u => u.Key == key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when accessing Session");
                return null;
            }
        }


        public SessionModel GetSession(int? idSession)
        {
            if (idSession == null) return null;

            try
            {
                return _dbContext.Session.Include(s => s.TaskProgramming).FirstOrDefault(u => u.IdSession == idSession);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when accessing User");
                return null;
            }
        }
        public async Task<SessionModel> GetSessionAsync(int? idSession)
        {
            if (idSession == null) return null;

            try
            {
                return await _dbContext.Session.Include(s => s.TaskProgramming).Include(s => s.PlayerSessions).ThenInclude(s => s.Player)
                                                        .FirstOrDefaultAsync(u => u.IdSession == idSession);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when accessing Session");
                return null;
            }
        }

        public async Task<PlayerSessionModel> GetPlayerSessionAsync(int? idSession, int? idPlayer)
        {
            if (idPlayer == null || idSession == null) return null;

            try
            {
                return await _dbContext.PlayerSession.FirstOrDefaultAsync(p => p.IdPlayer == idPlayer && p.IdSession == idSession);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when edit PlayerSession to table");
                return null;
            }
        }

        public bool ChangePasswordSession(int? idSession, string password)
        {
            if (string.IsNullOrEmpty(password) || idSession == null) return false;

            try
            {
                SessionModel sessionModel = _dbContext.Session.FirstOrDefault(u => u.IdSession == idSession);
                if (sessionModel == null) return false;
                sessionModel.Password = password;
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when changing password");
                return false;
            }
        }
        public async Task<bool> ChangePasswordSessionAsync(int? idSession, string password)
        {
            if (string.IsNullOrEmpty(password) || idSession == null) return false;

            try
            {
                var sessionModel = await _dbContext.Session.FirstOrDefaultAsync(u => u.IdSession == idSession);
                if (sessionModel == null) return false;

                sessionModel.Password = password;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when changing password");
                return false;
            }
        }


        public bool DelPlayerSession(int? IdSession, int? IdPlayer)
        {
            if (IdSession == null || IdPlayer == null) return false;
            try
            {
                var playerSession = _dbContext.PlayerSession
                                           .FirstOrDefault(ps => ps.IdSession == IdSession && ps.IdPlayer == IdPlayer);
                if (playerSession != null)
                {
                    _dbContext.PlayerSession.Remove(playerSession);
                    _dbContext.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when delete PlayerSession");
                return false;
            }
        }
        public async Task<bool> DelPlayerSessionAsync(int? IdSession, int? IdPlayer)
        {
            if (IdSession == null || IdPlayer == null) return false;

            try
            {
                var playerSession = await _dbContext.PlayerSession
                                            .FirstOrDefaultAsync(ps => ps.IdSession == IdSession && ps.IdPlayer == IdPlayer);
                if (playerSession != null)
                {
                    _dbContext.PlayerSession.Remove(playerSession);
                    await _dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when deleting PlayerSession");
                return false;
            }
        }


        public bool DelSession(int? IdSession)
        {
            if (IdSession == null) return false;
            try
            {
                var playerSessions = _dbContext.PlayerSession
                                        .Where(ps => ps.IdSession == IdSession)
                                        .ToList();
                _dbContext.PlayerSession.RemoveRange(playerSessions);

                // Удаление записи из Session
                var session = _dbContext.Session.FirstOrDefault(s => s.IdSession == IdSession);
                if (session != null)
                {
                    _dbContext.Session.Remove(session);
                    _dbContext.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when delete Session");
                return false;
            }
        }
        public async Task<bool> DelSessionAsync(int? IdSession)
        {
            if (IdSession == null) return false;

            try
            {
                var playerSessions = await _dbContext.PlayerSession
                                            .Where(ps => ps.IdSession == IdSession)
                                            .ToListAsync();
                _dbContext.PlayerSession.RemoveRange(playerSessions);

                var session = await _dbContext.Session.FirstOrDefaultAsync(s => s.IdSession == IdSession);
                if (session != null)
                {
                    _dbContext.Session.Remove(session);
                    await _dbContext.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when deleting Session");
                return false;
            }
        }


        public Dictionary<int, string> GetListPlayerFromSession(int? IdSession)
        {
            if (IdSession == null) return null;
            var playerList = new Dictionary<int, string>();
            try
            {
                var playersInSession = _dbContext.PlayerSession
                                              .Where(ps => ps.IdSession == IdSession)
                                              .Select(ps => new { ps.IdPlayer, ps.Player.Name })
                                              .ToList();

                foreach (var player in playersInSession)
                {
                    playerList.Add(player.IdPlayer, player.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when retrieving a list of players from a session");
                return null;
            }

            return playerList;
        }
        public async Task<Dictionary<int, string>> GetListPlayerFromSessionAsync(int? IdSession)
        {
            if (IdSession == null) return null;

            var playerList = new Dictionary<int, string>();
            try
            {
                var playersInSession = await _dbContext.PlayerSession
                                              .Where(ps => ps.IdSession == IdSession)
                                              .Select(ps => new { ps.IdPlayer, ps.Player.Name })
                                              .ToListAsync();

                foreach (var player in playersInSession)
                {
                    playerList.Add(player.IdPlayer, player.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when retrieving a list of players from a session");
                return null;
            }

            return playerList;
        }


        public int GetPlayerCountInSession(int? IdSession)
        {
            if (IdSession == null) return -1;

            try
            {
                return _dbContext.PlayerSession.Count(ps => ps.IdSession == IdSession);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when accessing PlayerSession");
                return -1;
            }
        }
        public async Task<int> GetPlayerCountInSessionAsync(int? IdSession)
        {
            if (IdSession == null) return -1;

            try
            {
                return await _dbContext.PlayerSession.CountAsync(ps => ps.IdSession == IdSession);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when accessing PlayerSession");
                return -1;
            }
        }


        public bool GetVictorySession(int? IdSession)
        {
            if (IdSession == null) return false;

            try
            {
                var session = _dbContext.Session.FirstOrDefault(s => s.IdSession == IdSession);
                return session != null && session.WinnerId.HasValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when accessing Session");
                return false;
            }
        }
        public async Task<bool> GetVictorySessionAsync(int? IdSession)
        {
            if (IdSession == null) return false;

            try
            {
                var session = await _dbContext.Session.FirstOrDefaultAsync(s => s.IdSession == IdSession);
                return session != null && session.WinnerId.HasValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when accessing Session");
                return false;
            }
        }


        public List<SessionModel> GetListSssion(string state)
        {
            if (string.IsNullOrEmpty(state)) return null;

            try
            {
                if(state != "Public" && state != "Private") return null;
                return _dbContext.Session.Where(s => s.State == state).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when retrieving the list of sessions");
                return null;
            }
        }
        public async Task<List<SessionModel>> GetListSssionAsync(string state)
        {
            if (string.IsNullOrEmpty(state)) return null;

            try
            {
                if (state != "Public" && state != "Private") return null;
                return await _dbContext.Session.Where(s => s.State == state && s.IsFinish == false).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when retrieving the list of sessions");
                return null;
            }
        }
        public async Task<bool> AddTaskToSession(int? idSession, int? idTask)
        {
            if (idSession == null || idTask == null) return false;
            try
            {
                SessionModel session = await _dbContext.Session.FirstOrDefaultAsync(s => s.IdSession == idSession);

                TaskProgrammingModel taskProgramming = await _dbContext.TaskProgramming.FirstOrDefaultAsync(t => t.IdTaskProgramming == idTask);

                if(taskProgramming == null || session == null) return false;

                session.TaskId = idTask;

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when adding a task to a session");
                return false;
            }
        }

        public bool DeleteExpiredSessions(DateTime? dateTime)
        {
            if (dateTime == null) return false;

            try
            {
                // Выборка сессий, которые существуют более одного дня
                var listSessionsExpired = _dbContext.Session
                    .Where(s => s.WinnerId == null && dateTime > s.DateCreating.AddDays(1))
                    .Select(s => s.IdSession)
                    .ToList();

                if (listSessionsExpired.Any())
                {
                    var playerSessions = _dbContext.PlayerSession
                        .Where(ps => listSessionsExpired.Contains(ps.IdSession));

                    _dbContext.PlayerSession.RemoveRange(playerSessions);

                    var sessionsToDelete = _dbContext.Session
                        .Where(s => listSessionsExpired.Contains(s.IdSession));

                    _dbContext.Session.RemoveRange(sessionsToDelete);

                    _dbContext.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when deleting expired sessions");
                return false;
            }
            
        }

        public async Task<bool> AddStartTimeAsync(int? idSession, DateTime? dateStart)
        {
            if (idSession == null || dateStart == null) return false;
            try
            {
                SessionModel session = await _dbContext.Session.FirstOrDefaultAsync(s => s.IdSession == idSession);
                if (session == null) return false;

                session.DateStart = dateStart;

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when updating the start time for session!");
                return false;
            }
        }
        public async Task<bool> SaveResultAsync(int? idSession, int? idPlayer, string? time, int? memory)
        {
            if (idSession == null || idPlayer == null || time == null || memory == null) return false;
            try
            {
                PlayerSessionModel playerSession = await _dbContext.PlayerSession.FirstOrDefaultAsync(s => s.IdSession == idSession && s.IdPlayer == idPlayer);
                if (playerSession == null) return false;

                playerSession.Time = time;
                playerSession.Memory = memory;

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when updating for session!");
                return false;
            }
        }
        public async Task<bool> FinishTaskAsync(int? idSession, int? idPlayer)
        {
            if (idSession == null || idPlayer == null) return false;
            try
            {
                PlayerSessionModel playerSession = await _dbContext.PlayerSession.FirstOrDefaultAsync(s => s.IdSession == idSession && s.IdPlayer == idPlayer);
                if (playerSession == null) return false;

                playerSession.State = true;

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when updating for session!");
                return false;
            }
        }

        public async Task<bool> UpdateSessionAsync(SessionModel session)
        {
            if (session == null)
                return false;

            try
            {
                _dbContext.Update(session);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when saving changes");
                return false;
            }
        }
    }
}
