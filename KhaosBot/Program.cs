using System;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace KhaosBot
{
    class Program
    {
        private DiscordSocketClient Client;
        private CommandService _Commands;

        static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug
            });

            _Commands = new CommandService(new CommandServiceConfig {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
                
            });

            Client.MessageReceived += Client_MessageReceived;
            await _Commands.AddModulesAsync(Assembly.GetEntryAssembly());
            Client.Ready += Client_Ready;
            Client.Log += Client_Log;

            string token = ""; //"NTI0NDA4NDE5Mzk4OTc1NDkw.Dvq0NA.GmTi60Xd4q5EJoMkZG9w3g67nPs";
            try
            {
                using (var stream = new FileStream((Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).Replace(@"bin\Debug\netcoreapp2.1", @"Data\Token.txt"), FileMode.Open, FileAccess.Read))
                using (var readToken = new StreamReader(stream))
                {
                    token = readToken.ReadToEnd();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            

            await Client.LoginAsync(TokenType.Bot, token);
            await Client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task Client_Log(LogMessage Message)
        {
            Console.WriteLine($"{DateTime.Now} at {Message.Source}] {Message.Message}");
        }

        private async Task Client_Ready()
        {
            await Client.SetGameAsync("Khaos Bot - Tutorial", "https://houseofkhaos.weebly.com/", StreamType.NotStreaming);
        }

        private async Task Client_MessageReceived(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            var context = new SocketCommandContext(Client, message);

            if (context.Message == null || context.Message.Content == "") return;
            if (context.User.IsBot) return;

            int ArgPos = 0;
            if (!(message.HasStringPrefix("!kb ", ref ArgPos) || message.HasMentionPrefix(Client.CurrentUser, ref ArgPos))) return;

            var result = await _Commands.ExecuteAsync(context, ArgPos);
            if (!result.IsSuccess)
                Console.WriteLine($"{DateTime.Now} Something went wrong while executing the command Text: {context.Message.Content} | Error: {result.ErrorReason}.");
        }
    }
}
