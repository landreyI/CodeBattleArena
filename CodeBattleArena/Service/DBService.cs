using CodeBattleArena.Data;
using CodeBattleArena.Models.DBModels;
using CodeBattleArena.Service.DBServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CodeBattleArena.Service
{
    public class DBService
    {
        public DBPlayerService PlayerService { get; set; }
        public DBSessionService SessionService { get; set; }
        public DBChatService ChatService { get; set; }
        public DBTaskService TaskService { get; set; }

        private readonly AppDBContext _dbContext;
        private readonly ILogger<DBService> _logger;
        public DBService(AppDBContext dbContext, ILogger<DBService> logger, DBPlayerService dBPlayerService, DBSessionService dBSessionService, DBChatService dBChatService, DBTaskService dBTaskService)
        {
            PlayerService = dBPlayerService;
            SessionService = dBSessionService;
            ChatService = dBChatService;
            TaskService = dBTaskService;

            _dbContext = dbContext;
            _logger = logger;
        }

        
        

        
    }
}
