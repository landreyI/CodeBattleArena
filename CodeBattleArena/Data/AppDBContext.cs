using CodeBattleArena.Models.DBModels;
using Microsoft.EntityFrameworkCore;

namespace CodeBattleArena.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> option) : base(option) { }
        public DbSet<PlayerModel> Player { get; set; }
        public DbSet<SessionModel> Session { get; set; }
        public DbSet<PlayerSessionModel> PlayerSession { get; set; }
        public DbSet<TaskProgrammingModel> TaskProgramming { get; set; }
        public DbSet<InputDataModel> InputData { get; set; }
        public DbSet<TaskInputData> TaskInputData { get; set; }
        public DbSet<FriendModel> Friend { get; set; }
        public DbSet<ChatModel> Chats { get; set; }
        public DbSet<MessageModel> Messages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerSessionModel>()
                .HasKey(b => new { b.IdPlayer, b.IdSession });

            modelBuilder.Entity<PlayerSessionModel>()
                .HasOne(b => b.Player)
                .WithMany(u => u.PlayerSessions)
                .HasForeignKey(b => b.IdPlayer);

            modelBuilder.Entity<PlayerSessionModel>()
                .HasOne(b => b.Session)
                .WithMany(p => p.PlayerSessions)
                .HasForeignKey(b => b.IdSession);


            modelBuilder.Entity<SessionModel>()
                .HasOne(s => s.TaskProgramming)
                .WithMany(t => t.Sessions)
                .HasForeignKey(s => s.TaskId);


            modelBuilder.Entity<TaskInputData>()
                .HasKey(ti => new { ti.TaskProgrammingId, ti.InputDataTaskId });

            modelBuilder.Entity<TaskInputData>()
                .HasOne(ti => ti.TaskProgramming)
                .WithMany(t => t.TaskInputData)
                .HasForeignKey(ti => ti.TaskProgrammingId);

            modelBuilder.Entity<TaskInputData>()
                .HasOne(ti => ti.InputData)
                .WithMany(i => i.TaskInputData)
                .HasForeignKey(ti => ti.InputDataTaskId);


            modelBuilder.Entity<FriendModel>()
            .HasKey(f => new { f.IdPlayer1, f.IdPlayer2 });

            modelBuilder.Entity<FriendModel>()
                .HasOne(f => f.Player1)
                .WithMany(p => p.Friends1)
                .HasForeignKey(f => f.IdPlayer1)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FriendModel>()
                .HasOne(f => f.Player2)
                .WithMany(p => p.Friends2)
                .HasForeignKey(f => f.IdPlayer2)
                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<ChatModel>()
                .HasOne(c => c.Player1)
                .WithMany(p => p.Chats1)
                .HasForeignKey(c => c.IdPlayer1)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatModel>()
                .HasOne(c => c.Player2)
                .WithMany(p => p.Chats2)
                .HasForeignKey(c => c.IdPlayer2)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MessageModel>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.IdChat)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MessageModel>()
                .HasOne(m => m.Sender)
                .WithMany(p => p.Messages)
                .HasForeignKey(m => m.IdSender)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatModel>()
                .HasIndex(c => new { c.IdPlayer1, c.IdPlayer2 })
                .IsUnique();
        }
    }
}
