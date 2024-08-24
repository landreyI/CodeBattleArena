﻿using CodeBattleArena.Data;
using CodeBattleArena.Models.DBModels;
using Microsoft.EntityFrameworkCore;

namespace CodeBattleArena.Service.DBServices
{
    public class DBChatService
    {
        private readonly AppDBContext _dbContext;
        private readonly ILogger<DBService> _logger;
        public DBChatService(AppDBContext dbContext, ILogger<DBService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public int? AddChat(int? playerId1, int? playerId2)
        {
            if (playerId1 == null || playerId2 == null) return null;

            try
            {
                // Приводим ID игроков к определенному порядку
                if (playerId1 > playerId2)
                {
                    var temp = playerId1;
                    playerId1 = playerId2;
                    playerId2 = temp;
                }

                // Проверяем, существует ли уже такой чат
                var existingChat = _dbContext.Chats
                    .FirstOrDefault(c => c.IdPlayer1 == playerId1 && c.IdPlayer2 == playerId2);

                if (existingChat != null)
                {
                    // Возвращаем существующий ID чата
                    return existingChat.IdChat;
                }

                // Создаем новый чат
                var chat = new ChatModel
                {
                    IdPlayer1 = playerId1.Value,
                    IdPlayer2 = playerId2.Value
                };

                _dbContext.Chats.Add(chat);
                _dbContext.SaveChanges();
                return chat.IdChat; // Возвращаем ID созданного чата
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when adding Chat to table");
                return null;
            }
        }
        public async Task<int?> AddChatAsync(int? playerId1, int? playerId2)
        {
            if (playerId1 == null || playerId2 == null) return null;

            try
            {
                // Приводим ID игроков к определенному порядку
                if (playerId1 > playerId2)
                {
                    var temp = playerId1;
                    playerId1 = playerId2;
                    playerId2 = temp;
                }

                // Проверяем, существует ли уже такой чат
                var existingChat = await _dbContext.Chats
                    .FirstOrDefaultAsync(c => c.IdPlayer1 == playerId1 && c.IdPlayer2 == playerId2);

                if (existingChat != null)
                {
                    // Возвращаем существующий ID чата
                    return existingChat.IdChat;
                }

                // Создаем новый чат
                var chat = new ChatModel
                {
                    IdPlayer1 = playerId1.Value,
                    IdPlayer2 = playerId2.Value
                };

                await _dbContext.Chats.AddAsync(chat);
                await _dbContext.SaveChangesAsync();
                return chat.IdChat; // Возвращаем ID созданного чата
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when adding Chat to table");
                return null;
            }
        }


        public bool AddMessage(int chatId, int senderId, string messageText)
        {
            try
            {
                var message = new MessageModel
                {
                    IdChat = chatId,
                    IdSender = senderId,
                    MessageText = messageText,
                    SentDateTime = DateTime.Now
                };

                _dbContext.Messages.Add(message);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when adding Message to table");
                return false;
            }
        }
        public async Task<bool> AddMessageAsync(int chatId, int senderId, string messageText)
        {
            try
            {
                var message = new MessageModel
                {
                    IdChat = chatId,
                    IdSender = senderId,
                    MessageText = messageText,
                    SentDateTime = DateTime.Now
                };

                await _dbContext.Messages.AddAsync(message);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when adding Message to table");
                return false;
            }
        }


        public List<MessageModel> GetMessagesByChatId(int chatId)
        {
            try
            {
                return _dbContext.Messages
                                 .Where(m => m.IdChat == chatId)
                                 .OrderBy(m => m.SentDateTime)
                                 .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when accessing Messages by ChatId");
                return null;
            }
        }
        public async Task<List<MessageModel>> GetMessagesByChatIdAsync(int chatId)
        {
            try
            {
                return await _dbContext.Messages
                                 .Where(m => m.IdChat == chatId)
                                 .OrderBy(m => m.SentDateTime)
                                 .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when accessing Messages by ChatId");
                return null;
            }
        }


        public ChatModel GetChat(int idChat)
        {
            try
            {
                var chat = _dbContext.Chats
                                      .Include(c => c.Player1)
                                      .Include(c => c.Player2)
                                      .Include(c => c.Messages)
                                      .FirstOrDefault(c => c.IdChat == idChat);

                if (chat == null)
                {
                    _logger.LogWarning($"Chat with IdChat {idChat} not found.");
                }

                return chat;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when retrieving Chat with IdChat {idChat}");
                return null;
            }
        }
        public async Task<ChatModel> GetChatAsync(int idChat)
        {
            try
            {
                var chat = await _dbContext.Chats
                                      .Include(c => c.Player1)
                                      .Include(c => c.Player2)
                                      .Include(c => c.Messages)
                                      .FirstOrDefaultAsync(c => c.IdChat == idChat);

                if (chat == null)
                {
                    _logger.LogWarning($"Chat with IdChat {idChat} not found.");
                }

                return chat;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when retrieving Chat with IdChat {idChat}");
                return null;
            }
        }
        public async Task<ChatModel> GetChatByIdPlayerAsync(int? playerId1, int? playerId2)
        {
            if (playerId1 == null || playerId2 == null) return null;

            try
            {
                if (playerId1 > playerId2)
                {
                    var temp = playerId1;
                    playerId1 = playerId2;
                    playerId2 = temp;
                }

                var chat = await _dbContext.Chats
                                      .Include(c => c.Player1)
                                      .Include(c => c.Player2)
                                      .Include(c => c.Messages)
                                      .FirstOrDefaultAsync(c => c.IdPlayer1 == playerId1 && c.IdPlayer2 == playerId2);

                if (chat == null)
                {
                    _logger.LogWarning($"Chat with IdChat not found.");
                }

                return chat;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when retrieving Chat");
                return null;
            }
        }

        public List<ChatModel> GetChatsByPlayerId(int playerId)
        {
            try
            {
                return _dbContext.Chats
                     .Where(c => c.IdPlayer1 == playerId || c.IdPlayer2 == playerId)
                     .Include(c => c.Player1)
                     .Include(c => c.Player2)
                     .Include(c => c.Messages)
                     .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when accessing Chats by PlayerId");
                return null;
            }
        }
        public async Task<List<ChatModel>> GetChatsByPlayerIdAsync(int playerId)
        {
            try
            {
                return await _dbContext.Chats
                     .Where(c => c.IdPlayer1 == playerId || c.IdPlayer2 == playerId)
                     .Include(c => c.Player1)
                     .Include(c => c.Player2)
                     .Include(c => c.Messages)
                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when accessing Chats by PlayerId");
                return null;
            }
        }


        public bool DeleteChat(int? idChat)
        {
            if (idChat == null) return false;
            try
            {
                var messeges = _dbContext.Messages
                                        .Where(ch => ch.IdChat == idChat)
                                        .ToList();
                _dbContext.Messages.RemoveRange(messeges);

                // Удаление записи из Session
                var chat = _dbContext.Chats.FirstOrDefault(s => s.IdChat == idChat);
                if (chat != null)
                {
                    _dbContext.Chats.Remove(chat);
                    _dbContext.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when delete Chat");
                return false;
            }
        }
        public async Task<bool> DeleteChatAsync(int? idChat)
        {
            if (idChat == null) return false;
            try
            {
                var messeges = await _dbContext.Messages
                                        .Where(ch => ch.IdChat == idChat)
                                        .ToListAsync();
                _dbContext.Messages.RemoveRange(messeges);

                // Удаление записи из Session
                var chat = await _dbContext.Chats.FirstOrDefaultAsync(s => s.IdChat == idChat);
                if (chat != null)
                {
                    _dbContext.Chats.Remove(chat);
                    await _dbContext.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when delete Chat");
                return false;
            }
        }
        public async Task<bool> DeleteMessageAsync(int? idMessage)
        {
            if (idMessage == null) return false;
            try
            {
                var messeges = await _dbContext.Messages.FirstOrDefaultAsync(msg => msg.IdMessage == idMessage);
                if (messeges == null)
                {
                    return false;
                }
                _dbContext.Messages.Remove(messeges);

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error when delete Chat");
                return false;
            }
        }
    }
}
