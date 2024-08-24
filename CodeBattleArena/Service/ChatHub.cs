using Microsoft.AspNetCore.SignalR;

namespace CodeBattleArena.Service
{
    public class ChatHub : Hub
    {
        private readonly ConnectionMapping _connectionMapping;

        public ChatHub(ConnectionMapping connectionMapping)
        {
            _connectionMapping = connectionMapping;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (userId != null)
            {
                _connectionMapping.AddConnection(userId, Context.ConnectionId);
            }

            var httpContext = Context.GetHttpContext();

            if (httpContext.Request.Cookies["idSession"] != null)
            {
                var sessionId = httpContext.Request.Cookies["idSession"];
                _connectionMapping.AddGroupSession(sessionId, userId);

                var connectionId = _connectionMapping.GetConnection(userId.ToString());

                if (connectionId != null)
                {
                    await Groups.AddToGroupAsync(connectionId, "Session-" + sessionId);
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;
            if (userId != null)
            {
                var httpContext = Context.GetHttpContext();

                if (httpContext.Request.Cookies["idSession"] != null)
                {
                    var sessionId = httpContext.Request.Cookies["idSession"];
                    _connectionMapping.AddGroupSession(sessionId, userId);

                    var connectionId = _connectionMapping.GetConnection(userId.ToString());

                    if (connectionId != null)
                    {
                        await Groups.RemoveFromGroupAsync(connectionId, "Session-" + sessionId);
                    }
                }

                _connectionMapping.RemoveConnection(userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public async Task SendMessageToGroupSession(string sessionId, string nickname, string message)
        {
            await Clients.Group("Session-" + sessionId).SendAsync("ReceiveMessageGroupSession", nickname ,message);
        }
    }
}
