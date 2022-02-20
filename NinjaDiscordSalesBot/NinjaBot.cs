using OpenSeaClient;

namespace NinjaDiscordSalesBot
{
    public class NinjaBot
    {
        private readonly NinjaBotOptions _options;
        private readonly DiscordHttpClient _discordHttpClient;
        private readonly DiscordWebSocketClient? _discordWebSocketClient;
        private readonly OpenSeaHttpClient _openSeaHttpClient;
        private Timer? _heartbeatTimer;
        private long _heartbeatRefTimestamp = new DateTimeOffset(DateTime.UtcNow.AddHours(-10)).ToUnixTimeSeconds();

        public NinjaBot(NinjaBotOptions options)
        {
            _options = options;

            _openSeaHttpClient = new OpenSeaHttpClient(apiKey: _options.OpenSeaApiKey);

            _discordHttpClient = new DiscordHttpClient(options: _options);

            if (string.IsNullOrEmpty(_options.DiscordWebhookUrl))
            {
                _discordWebSocketClient = new DiscordWebSocketClient(botToken: _options.DiscordBotToken);
            }
        }

        public async Task StartAsync()
        {
            await (_discordWebSocketClient?.StartAsync() ?? Task.CompletedTask);

            _heartbeatTimer = new Timer((objState) =>
            {
                _ = Heartbeat();

            }, null, TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(_options.PollingIntervalSeconds));
        }

        public async Task StopAsync()
        {
            await (_discordWebSocketClient?.StopAsync() ?? Task.CompletedTask);

            if (_heartbeatTimer != null)
            {
                await _heartbeatTimer.DisposeAsync();
            }
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
            .SetTitle(ev.Asset!.Name ?? "Unknown")
            .SetDescription($"Collection: [{ev.Asset!.Collection?.Name}](https://opensea.io/collection/{ev.Asset!.Collection?.Slug})")
            .SetUrl($"https://opensea.io/assets/{ev.Asset!.AssetContract?.Address}/{ev.Asset?.TokenId}")
            .SetImageUrl(ev.Asset!.ImageUrl)
            .SetTimestamp(DateTime.UtcNow)
            .AddField("Sale Price", $"{ev.TotalPriceEth}Ξ", inline: true)
            .AddField("Seller", $"[{TrimString(ev.Seller?.User?.Username) ?? TrimString(ev.Seller?.Address) ?? "Unknown"}](https://opensea.io/{ev.Seller?.Address})", inline: true)
            .AddField("Buyer", $"[{TrimString(ev.WinnerAccount?.User?.Username) ?? TrimString(ev.WinnerAccount?.Address) ?? "Unknown"}](https://opensea.io/{ev.WinnerAccount?.Address})", inline: true)
            .Build();

        private static string? TrimString(string? str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            return new string(str.Take(10).ToArray());
        }

    }
}