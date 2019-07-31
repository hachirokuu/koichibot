﻿using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
//discord stuff
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Discord;

namespace koichibot
{
    public class Program : ModuleBase<SocketCommandContext>
    {
        public DiscordSocketClient Client;
        private CommandService Commands;
        private IServiceProvider Services;

        static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = Discord.LogSeverity.Debug,
            });

            Commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                LogLevel = Discord.LogSeverity.Debug,
            });

            Services = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(Commands)
                .BuildServiceProvider();

            Client.MessageReceived += HandleCommandAsync;
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            Client.Ready += Client_Ready;
            Client.Log += Client_Log;

            string Token = "";
            using (var Stream = new FileStream((Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).Replace(@"bin\Debug\netcoreapp2.0", @"data\token.txt"), FileMode.Open, FileAccess.Read))
            using (var ReadToken = new StreamReader(Stream))
            {
                Token = ReadToken.ReadToEnd();
            }
            await Client.LoginAsync(Discord.TokenType.Bot, Token);
            await Client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;
            string compare = msg.ToString().ToLower();

            if (msg is null || msg.Author.IsBot) return;

            if (compare.Contains("aga") || compare.Contains("aqa") || compare.Contains("a g a") || compare.Contains("a q a"))
            {
                //Emoji[] aga = { new Emoji("🅰️"), new Emoji("🇬"), new Emoji("🇦") };
                //IEmote[] baga = new IEmote[] { new Emoji("🅰️"), new Emoji("🇬"), new Emoji("🇦") };

                await msg.Author.SendMessageAsync("aga deme lan");
            }

            int argPos = 0;
            if (msg.HasStringPrefix("b!", ref argPos) || msg.HasMentionPrefix(Client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(Client, msg);

                await Commands.ExecuteAsync(context, argPos, Services);
            }
        }

        private async Task Client_Log(Discord.LogMessage Message)
        {
            await Task.Run(() => Console.WriteLine($"[{DateTime.Now} at {Message.Source}] {Message.Message}"));
        }

        private async Task Client_Ready()
        {
            await Client.SetGameAsync("aga demeyin", null, Discord.ActivityType.Playing);
        }

        //private async Task Client_MessageReceived(SocketMessage arg)
        //{
        //    // komutları ayarla
        //    if (!arg.Author.IsBot && (arg.ToString().ToLower().Contains("aga") || arg.ToString().ToLower().Contains("aqa") || arg.ToString().ToLower().Contains("a q a") || arg.ToString().ToLower().Contains("a g a")))
        //    {
        //        await arg.Channel.SendMessageAsync($"lan { arg.Author.Mention }, kerhane mi burası");
        //    }
        //    else if (!arg.Author.IsBot && (arg.ToString().ToLower().Equals("rin") || arg.ToString().ToLower().Contains("rin ") || arg.ToString().ToLower().Contains("rini") || arg.ToString().ToLower().Contains("rin tohsaka") || arg.ToString().ToLower().Contains("rine") || arg.ToString().ToLower().Contains("rinde")))
        //    {
        //        await arg.Channel.SendMessageAsync("rin demeyin ahmet summonlanıyo");
        //    }
        //    //else if (arg.ToString().Contains("b!setgame "))
        //    //{
        //    //    var msg = arg.ToString();
        //    //    var bruhStr = msg.Split(msg, 10);
        //    //    string gameStr = bruhStr[2];
        //    //    await Client.SetGameAsync(gameStr, null, Discord.ActivityType.Playing);
        //    //}
        //}
    }
}
