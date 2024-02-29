namespace SwappyBot.EntityFramework
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    #pragma warning disable CS8618
    public class SwapState
    {
        public string StateId { get; set; }
        
        public DateTimeOffset SwapStarted { get; set; }
        
        public string? AssetFrom { get; set;  }
        public string? AssetTo { get; set;  }
        public decimal? Amount { get; set;  }
        public string? DestinationAddress { get; set; }
        
        public DateTimeOffset? QuoteTime { get; set; }
        
        public decimal? QuoteDeposit { get; set; }
        public decimal? QuoteReceive { get; set; }
        public string? QuoteRate { get; set; }
        public decimal? QuotePlatformFee { get; set; }
        public decimal? QuoteChainflipFee { get; set; }
        
        public DateTimeOffset? SwapAccepted { get; set; }
        public DateTimeOffset? SwapCancelled { get; set; }
        
        public DateTimeOffset? DepositGenerated { get; set; }
        public string? DepositAddress { get; set; }
        public string? DepositChannel { get; set; }
        
        public string? SwapStatus { get; set; }
        
    }
    #pragma warning restore CS8618

    public class SwapStateConfiguration : EntityTypeConfiguration<BotContext, SwapState>
    {
        private const string TableName = "swap_state";

        public override void Configure(EntityTypeBuilder<SwapState> builder)
        {
            builder
                .ToTable(TableName)
                .HasKey(x => x.StateId);

            builder.Property(x => x.StateId).HasMaxLength(50);  
            
            builder.Property(x => x.SwapStarted).IsRequired();
            builder.HasIndex(x => x.SwapStarted);
            
            builder.Property(x => x.AssetFrom).IsRequired(false).HasMaxLength(5);
            builder.Property(x => x.AssetTo).IsRequired(false).HasMaxLength(5);
            builder.Property(x => x.Amount).IsRequired(false);
            builder.Property(x => x.DestinationAddress).IsRequired(false).HasMaxLength(100);

            builder.Property(x => x.QuoteTime).IsRequired(false);
            builder.HasIndex(x => x.QuoteTime);
            
            builder.Property(x => x.QuoteDeposit).IsRequired(false);
            builder.Property(x => x.QuoteReceive).IsRequired(false);
            builder.Property(x => x.QuoteRate).IsRequired(false).HasMaxLength(200);
            builder.Property(x => x.QuotePlatformFee).IsRequired(false);
            builder.Property(x => x.QuoteChainflipFee).IsRequired(false);
            
            builder.Property(x => x.SwapAccepted).IsRequired(false);
            builder.HasIndex(x => x.SwapAccepted);
            
            builder.Property(x => x.SwapCancelled).IsRequired(false);
            builder.HasIndex(x => x.SwapCancelled);
            
            builder.Property(x => x.DepositGenerated).IsRequired(false);
            builder.HasIndex(x => x.DepositGenerated);
            
            builder.Property(x => x.DepositAddress).IsRequired(false).HasMaxLength(100);
            builder.Property(x => x.DepositChannel).IsRequired(false).HasMaxLength(50);
            
            builder.Property(x => x.SwapStatus).IsRequired(false).HasMaxLength(10000);
        }
    }
}