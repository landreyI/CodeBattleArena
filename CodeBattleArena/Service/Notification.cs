using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CodeBattleArena.Service
{
    public class Notification
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ConnectionMapping _connectionMapping;

        public Notification(IHubContext<ChatHub> hubContext, ConnectionMapping connectionMapping)
        {
            _hubContext = hubContext;
            _connectionMapping = connectionMapping;
        }

        public async Task SendFriendNotification(string nickname, int userId, string message, bool isReload)
        {
            var connectionId = _connectionMapping.GetConnection(userId.ToString());
            if (connectionId != null)
            {
                await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessageFromUser", nickname, message, isReload);
            }
        }
        public async Task SendNotificationJoinSession(string nickname, int recipientId, int sessionId, string message)
        {
            var connectionId = _connectionMapping.GetConnection(recipientId.ToString());
            if (connectionId != null)
            {
                await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessageFromUserJoinSession", nickname, sessionId, message);
            }
        }
        public async Task SendNotificationSession( int sessionId, string message, bool isReload, bool isBlackout)
        {
            await _hubContext.Clients.Group("Session-" + sessionId).SendAsync("ReceiveMessageSession", message, isReload, isBlackout);
        }
        public async Task SendNotificationSessionStartGame(int sessionId)
        {
            await _hubContext.Clients.Group("Session-" + sessionId).SendAsync("ReceiveSessionStarGame");
        }

        public async Task SendNotificationSessionCreatorExit(int playerId, int sessionId, string message)
        {
            var connectionId = _connectionMapping.GetConnection(playerId.ToString());
            if (connectionId != null)
            {
                await _hubContext.Clients.Group("Session-" + sessionId).SendAsync("ReceiveMessageSessionCreatorExit", message);
            }
        }
        public async Task SendNotificationSessionCreatorExclusion(int playerId, string message)
        {
            var connectionId = _connectionMapping.GetConnection(playerId.ToString());
            if (connectionId != null)
            {
                await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessageSessionCreatorExit", message);
            }
        }

        public async Task SendMessagePersonalChat(string userSenderId, string userRecipientId, string userSenderNick, string message, string fontSender, string fontRecipient)
        {
            var connectionSenderId = _connectionMapping.GetConnection(userSenderId);
            var connectionRecipientId = _connectionMapping.GetConnection(userRecipientId);

            if (connectionSenderId != null)
            {
                await _hubContext.Clients.Client(connectionSenderId).SendAsync("ReceiveMessageFromUserChat", userSenderNick, message, fontSender);
            }

            if (connectionRecipientId != null)
            {
                await _hubContext.Clients.Client(connectionRecipientId).SendAsync("ReceiveMessageFromUserChat", userSenderNick, message, fontRecipient);
            }
        }

        public async Task DeletePersonalChat(string idPlayer1, string idPlayer2)
        {
            var connectionIdPlayer1 = _connectionMapping.GetConnection(idPlayer1);
            var connectionIdPlayer2 = _connectionMapping.GetConnection(idPlayer2);

            if (connectionIdPlayer1 != null)
            {
                await _hubContext.Clients.Client(connectionIdPlayer1).SendAsync("DeletePersonalChat");
            }

            if (connectionIdPlayer2 != null)
            {
                await _hubContext.Clients.Client(connectionIdPlayer2).SendAsync("DeletePersonalChat");
            }
        }

    }
}
