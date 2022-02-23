using NinjaDiscordSalesBot;

var bot = new NinjaBot(new NinjaBotOptions
{
    DiscordBotToken = "OTM4NzUxMDI0ODkyNjI5MDIy.Yfu2BQ.O3P-a9a8I5-XCc3vcFLCPkxLKVw",
    DiscordChannelId = "935635694490116156",
    OpenSeaApiKey = "0057a183991745f6bbdb385493edbf10",
    OpenSeaCollectionSlug = "planetdaos",
    PollingIntervalSeconds = 60000,
});

await bot.StartAsync();

Thread.Sleep(-1);
