namespace CodeBattleArena.Service
{
    public class ConnectionMapping
    {
        private readonly Dictionary<string, string> _connections = new Dictionary<string, string>();

        private readonly Dictionary<string, List<string>> _groupSession = new Dictionary<string, List<string>>();

        public void AddConnection(string userId, string connectionId)
        {
            lock (_connections)
            {
                _connections[userId] = connectionId;
            }
        }

        public void RemoveConnection(string userId)
        {
            lock (_connections)
            {
                _connections.Remove(userId);
            }
        }

        public string GetConnection(string userId)
        {
            lock (_connections)
            {
                _connections.TryGetValue(userId, out var connectionId);
                return connectionId;
            }
        }
        public void AddGroupSession(string sessionId, string userId)
        {
            lock (_groupSession)
            {
                if (!_groupSession.ContainsKey(sessionId))
                {
                    _groupSession[sessionId] = new List<string>();
                }

                if (!_groupSession[sessionId].Contains(userId))
                {
                    _groupSession[sessionId].Add(userId);
                }
            }
        }
        public void RemoveGroupSession(string sessionId, string userId)
        {
            lock (_groupSession)
            {
                if (_groupSession.ContainsKey(sessionId))
                {
                    _groupSession[sessionId].Remove(userId);

                    if (_groupSession[sessionId].Count == 0)
                    {
                        _groupSession.Remove(sessionId);
                    }
                }
            }
        }

        // Получение списка пользователей по сессии
        public List<string> GetUsersInSession(string sessionId)
        {
            lock (_groupSession)
            {
                var connections = new List<string>();

                if (_groupSession.ContainsKey(sessionId))
                {
                    foreach (var userId in _groupSession[sessionId])
                    {
                        var connectionId = GetConnection(userId);
                        if (connectionId != null)
                        {
                            connections.Add(connectionId);
                        }
                    }
                }

                return connections;
            }
        }
    }
}
