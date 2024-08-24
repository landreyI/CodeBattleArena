using CodeBattleArena.Data;
using CodeBattleArena.Models.DBModels;
using Microsoft.EntityFrameworkCore;

namespace CodeBattleArena.Service.DBServices
{
    public class DBPlayerService
    {
        private readonly AppDBContext _dbContext;
        private readonly ILogger<DBService> _logger;
        public DBPlayerService(AppDBContext dbContext, ILogger<DBService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public bool AddPlayer(PlayerModel player)
        {
            if (player == null) return false;
            try
            {
                _dbContext.Player.Add(player);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when adding Player to table");
                return false;
            }
        }
        public async Task<bool> AddPlayerAsync(PlayerModel player)
        {
            if (player == null) return false;
            try
            {
                await _dbContext.Player.AddAsync(player);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when adding Player to table");
                return false;
            }
        }


        public bool CheckEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            try
            {
                var existingUser = _dbContext.Player.FirstOrDefault(u => u.Email == email);
                if (existingUser == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when accessing User");
                return false;
            }
        }
        public async Task<bool> CheckEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            try
            {
                var existingUser = await _dbContext.Player.FirstOrDefaultAsync(u => u.Email == email);
                if (existingUser == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when accessing User");
                return false;
            }
        }


        public bool CheckNickName(string nickName)
        {
            if (string.IsNullOrEmpty(nickName)) return false;

            try
            {
                var existingUser = _dbContext.Player.FirstOrDefault(u => u.NickName == nickName);
                if (existingUser == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when accessing User");
                return false;
            }
        }
        public async Task<bool> CheckNickNameAsync(string nickName)
        {
            if (string.IsNullOrEmpty(nickName)) return false;

            try
            {
                var existingUser = await _dbContext.Player.FirstOrDefaultAsync(u => u.NickName == nickName);
                if (existingUser == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when accessing User");
                return false;
            }
        }


        public PlayerModel GetPlayer(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;

            try
            {
                return _dbContext.Player.FirstOrDefault(p => p.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when accessing User");
                return null;
            }
        }
        public async Task<PlayerModel> GetPlayerAsync(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;

            try
            {
                return await _dbContext.Player.FirstOrDefaultAsync(p => p.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when accessing User");
                return null;
            }
        }


        public PlayerModel GetPlayer(int? idPlayer)
        {
            if (idPlayer == null) return null;

            try
            {
                return _dbContext.Player.FirstOrDefault(u => u.IdPlayer == idPlayer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when accessing User");
                return null;
            }
        }
        public async Task<PlayerModel> GetPlayerAsync(int? idPlayer)
        {
            if (idPlayer == null) return null;

            try
            {
                return await _dbContext.Player.FirstOrDefaultAsync(u => u.IdPlayer == idPlayer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when accessing Player");
                return null;
            }
        }


        public PlayerModel GetPlayerNick(string nickName)
        {
            if (string.IsNullOrEmpty(nickName)) return null;

            try
            {
                return _dbContext.Player.FirstOrDefault(p => p.NickName == nickName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when accessing User");
                return null;
            }
        }
        public async Task<PlayerModel> GetPlayerNickAsync(string nickName)
        {
            if (string.IsNullOrEmpty(nickName)) return null;

            try
            {
                return await _dbContext.Player.FirstOrDefaultAsync(p => p.NickName == nickName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when accessing User");
                return null;
            }
        }


        public Dictionary<int, string> GetListAllPlayers()
        {
            try
            {
                var players = _dbContext.Player
                    .ToDictionary(p => p.IdPlayer, p => p.NickName);

                return players;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when accessing Players");
                return new Dictionary<int, string>();
            }
        }
        public async Task<Dictionary<int, string>> GetListAllPlayersAsync()
        {
            try
            {
                return await _dbContext.Player
                    .ToDictionaryAsync(p => p.IdPlayer, p => p.NickName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when accessing Players");
                return new Dictionary<int, string>();
            }
        }


        public bool AddFriend(int? playerId1, int? playerId2)
        {
            if (playerId1 == null || playerId2 == null) return false;

            try
            {
                if (playerId1 > playerId2)
                {
                    var temp = playerId1;
                    playerId1 = playerId2;
                    playerId2 = temp;
                }

                var friend = new FriendModel
                {
                    IdPlayer1 = playerId1.Value,
                    IdPlayer2 = playerId2.Value
                };

                _dbContext.Friend.Add(friend);
                _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when addition Friend");
                return false;
            }
        }
        public async Task<bool> AddFriendAsync(int? playerId1, int? playerId2)
        {
            if (playerId1 == null || playerId2 == null) return false;

            try
            {
                if (playerId1 > playerId2)
                {
                    var temp = playerId1;
                    playerId1 = playerId2;
                    playerId2 = temp;
                }

                var friend = new FriendModel
                {
                    IdPlayer1 = playerId1.Value,
                    IdPlayer2 = playerId2.Value
                };

                await _dbContext.Friend.AddAsync(friend);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when addition Friend");
                return false;
            }
        }


        public bool DeleteFriend(int? playerId1, int? playerId2)
        {
            if (playerId1 == null || playerId2 == null)
                return false;

            try
            {
                var friend = _dbContext.Friend
                    .FirstOrDefault(f => (f.IdPlayer1 == playerId1.Value && f.IdPlayer2 == playerId2.Value) ||
                                          (f.IdPlayer1 == playerId2.Value && f.IdPlayer2 == playerId1.Value));

                if (friend == null)
                    return false;

                _dbContext.Friend.Remove(friend);
                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when deleting Friend");
                return false;
            }
        }
        public async Task<bool> DeleteFriendAsync(int? playerId1, int? playerId2)
        {
            if (playerId1 == null || playerId2 == null)
                return false;

            try
            {
                var friend = await _dbContext.Friend
                    .FirstOrDefaultAsync(f => (f.IdPlayer1 == playerId1.Value && f.IdPlayer2 == playerId2.Value) ||
                                          (f.IdPlayer1 == playerId2.Value && f.IdPlayer2 == playerId1.Value));

                if (friend == null)
                    return false;

                _dbContext.Friend.Remove(friend);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when deleting Friend");
                return false;
            }
        }


        public Dictionary<int, string> GetAllFriends(int? playerId)
        {
            if (playerId == null) return null;
            try
            {
                var friends = _dbContext.Friend
                    .Where(f => f.IdPlayer1 == playerId || f.IdPlayer2 == playerId)
                    .Select(f => f.IdPlayer1 == playerId ? f.Player2 : f.Player1)
                    .ToDictionary(p => p.IdPlayer, p => p.Name);

                return friends;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when accessing Friend");
                return null;
            }
        }
        public async Task<Dictionary<int, string>> GetAllFriendsAsync(int? playerId)
        {
            if (playerId == null) return null;
            try
            {
                var friends = await _dbContext.Friend
                    .Where(f => f.IdPlayer1 == playerId || f.IdPlayer2 == playerId)
                    .Select(f => f.IdPlayer1 == playerId ? f.Player2 : f.Player1)
                    .ToDictionaryAsync(p => p.IdPlayer, p => p.Name);

                return friends;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when accessing Friend");
                return null;
            }
        }

        public async Task<bool> SaveEditAsync(PlayerModel playerModel)
        {
            if (playerModel == null)
                return false;

            try
            {
                _dbContext.Update(playerModel);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when saving changes");
                return false;
            }
        }

        public async Task<bool> AddVictoryPlayerAsync(int? idPlayer)
        {
            if (idPlayer == null) return false;

            try
            {
                PlayerModel player = await _dbContext.Player.FirstOrDefaultAsync(u => u.IdPlayer == idPlayer);

                if (player == null) return false;

                player.Victories++;

                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when edit User");
                return false;
            }
        }

        public async Task<List<SessionModel>> MyGamesListAsync(int? idPlayer)
        {
            if (idPlayer == null) return null;

            try
            {
                return await _dbContext.PlayerSession
                    .Where(u => u.IdPlayer == idPlayer)
                    .Select(ps => ps.Session)
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when edit User");
                return null;
            }
        }
    }
}
