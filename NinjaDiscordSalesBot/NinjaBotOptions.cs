#nullable disable
namespace NinjaDiscordSalesBot
{
    public class NinjaBotOptions
    {
        public string DiscordChannelId { get; set; }

        public string DiscordBotToken { get; set; }

        public string DiscordWebhookUrl { get; set; }

        public string OpenSeaApiKey { get; set; }

        public string OpenSeaCollectionSlug { get; set; }

        public long PollingIntervalSeconds { get; set; } = 60000;
    }
}
