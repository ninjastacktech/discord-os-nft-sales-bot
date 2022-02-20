# OpenSea NFT sales bot for Discord

Queries OpenSea APIs and posts to a specified channel in your Discord server.

Built with 💙 and:
- [NinjaWebSocket](https://github.com/ninjastacktech/ninja-websocket-net) - Lightweight, user-friendly WebSocket APIs
- [OpenSeaHttpClient](https://github.com/ninjastacktech/opensea-net) - SDK for the OpenSea marketplace API

Supports multiple marketplaces: https://github.com/ninjastacktech/discord-eth-nft-sales-bot (instead of polling OpenSea API, it subscribes to Ethereum websocket events)

## Usage
```C#
var bot = new NinjaBot(new NinjaBotOptions
{
    DiscordBotToken = "<discord_bot_token>",
    DiscordChannelId = "<discord_channel_id>",
    OpenSeaApiKey = "<opensea_api_key>",
    OpenSeaCollectionSlug = "<collection_slug>",
    PollingIntervalSeconds = 60000,
});

await bot.StartAsync();
```

## Options

- `OpenSeaApiKey [required]`. The OpenSea API Key. Request one here: https://docs.opensea.io/reference/request-an-api-key
- `OpenSeaCollectionSlug [required]`. The name of the collection on OpenSea.
- `DiscordBotToken [required]`. Create an application using the `Discord Developer Portal` and then create a bot within that application that has permissions to post messages. 
- `DiscordChannelId [required]`. The id of the channel you want the bot to post into. Get this by turning on `Developer Mode` in Discord, then click the channel settings icon.
- `PollingIntervalSeconds [optional]`. How often should the bot query OpenSea API (default: 60 seconds).

---

### MIT License
