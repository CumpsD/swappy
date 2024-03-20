namespace SwappyBot.Configuration
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    
    public class BotConfiguration
    {
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Convention for configuration is .Section")]
        public const string Section = "Bot";
        
        [Required]
        [NotNull]
        public string? Token
        {
            get; init;
        }
        
        [Required]
        [NotNull]
        public string? Prefix
        {
            get; init;
        }
        
        [Required]
        [NotNull]
        public string? DebugUser
        {
            get; init;
        }
        
        [Required]
        [NotNull]
        public string? BrokerApiKey
        {
            get; init;
        }
        
        [Required]
        [NotNull]
        public string? BrokerUrl
        {
            get; init;
        }
        
        [Required]
        [NotNull]
        public int? QuoteValidityInSeconds
        {
            get; init;
        }
        
        [Required]
        [NotNull]
        public ulong[]? NotificationChannelIds
        {
            get; init;
        }
        
        [Required]
        [NotNull]
        public int? StatusCheckIntervalInSeconds
        {
            get; init;
        }
    }
}