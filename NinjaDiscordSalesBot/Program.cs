using NinjaDiscordSalesBot;

var bot = new NinjaBot(new NinjaBotOptions
{
    DiscordBotToken = "<discord_bot_token>",
    DiscordChannelId = "<discord_channel_id>",
    DiscordWebhookUrl = "<discord_webhook_url>",
    OpenSeaApiKey = "<opensea_api_key>",
    OpenSeaCollectionSlug = "<collection_slug>",
    PollingIntervalSeconds = 60000,
});

await bot.StartAsync();

Thread.Sleep(-1);
