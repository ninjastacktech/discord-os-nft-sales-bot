using OpenSeaClient;

namespace NinjaDiscordSalesBot
{
    public class NinjaBot
    {
        private readonly NinjaBotOptions _options;
        private readonly DiscordHttpClient _discordHttpClient;
        private readonly DiscordWebSocketClient _discordWebSocketClient;
        private readonly OpenSeaHttpClient _openSeaHttpClient;
        private Timer? _heartbeatTimer;
        private long _heartbeatRefTimestamp = new DateTimeOffset(DateTime.UtcNow.AddHours(-10)).ToUnixTimeSeconds();

        public NinjaBot(NinjaBotOptions options)
        {
            _options = options;
            _discordHttpClient = new DiscordHttpClient(botToken: _options.DiscordBotToken);
            _discordWebSocketClient = new DiscordWebSocketClient(botToken: _options.DiscordBotToken);
            _openSeaHttpClient = new OpenSeaHttpClient(apiKey: _options.OpenSeaApiKey);
        }

        public async Task StartAsync()
        {
            await _discordWebSocketClient.StartAsync();

            _heartbeatTimer = new Timer((objState) =>
            {
                _ = Heartbeat();

            }, null, TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(_options.PollingIntervalSeconds));
        }

        public async Task StopAsync()
        {
            await _discordWebSocketClient.StopAsync();

            _heartbeatTimer?.Dispose();
        }

        private async Task Heartbeat()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            try
            {
                var events = await _openSeaHttpClient.GetEventsAsync(new GetEventsQueryParams
                {
                    CollectionSlug = _options.OpenSeaCollectionSlug,
                    OccuredAfter = _heartbeatRefTimestamp,
                    EventType = "successful",
                });

                Console.WriteLine($"OpenSea heartbeat for {_options.OpenSeaCollectionSlug} found {events!.Count} new events.");

                _heartbeatRefTimestamp = now;

                events!.Reverse();

                foreach (var ev in events)
                {
                    try
                    {
                        await _discordHttpClient.SendMessageAsync(BuildDiscordMessage(ev), _options.DiscordChannelId);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Discord Bot send message exception: {ex.Message}");
                    }

                    await Task.Delay(2000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OpenSea exception: {ex.Message}");
            }
        }

        private static DiscordMessage BuildDiscordMessage(Event ev) => new DiscordMessageBuilder()
            .SetTitle($"{ev.Asset!.Name} sold for {ev.TotalPriceEth} ETH!")
            .SetDescription($"Collection: {ev.Asset!.Collection?.Name}")
            .SetUrl($"https://opensea.io/assets/{ev.Asset!.AssetContract?.Address}/{ev.Asset?.TokenId}")
            .SetImageUrl(ev.Asset!.ImagePreviewUrl)
            .SetTimestamp(ev.CreatedDate)
            .Build();

    }
}
