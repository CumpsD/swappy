namespace SwappyBot.EntityFramework
{
    using Microsoft.EntityFrameworkCore;

    public class BotContext : DbContext
    {
        // ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
        public virtual DbSet<SwapState> SwapState { get; set; } = null!;
        // ReSharper restore AutoPropertyCanBeMadeGetOnly.Global
        
        public BotContext() { }

        // This needs to be DbContextOptions<T> for Autofac!
        public BotContext(
            DbContextOptions<BotContext> options)
            : base(options) { }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.AddEntityConfigurationsFromAssembly<BotContext>(typeof(BotContext).Assembly);
        }
    }
}