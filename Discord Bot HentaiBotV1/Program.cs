using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Discord_Bot_HentaiBotV1.Handler;
using System.Data.SqlClient;

namespace Discord_Bot_HentaiBotV1
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        public static bool debug = true;

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();
            List<string> tokens = new List<string>()
            {
                ""
            };
            

            _client.Log += _client_Log;
            _client.ButtonExecuted += _button_handler;
            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, tokens[0]);

            await _client.StartAsync();

            await Task.Delay(-1);

        }

        private Task _client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        private async Task _button_handler(SocketMessageComponent component)
        {
            switch (component.Data.CustomId)
            {
                case "backButton":

                    // Creating hanime Object and some vars
                    Hanime.Hanime hanime = new Hanime.Hanime();
                    int currentPage;
                    int actualPage;
                    string jsonString = "";
                    int currentIndex;
                    int actualIndex;

                    // Database Connection + select and check
                    SQLConnectionManager connMan = new SQLConnectionManager();
                    var connStr = @"Server=KOTLIN\SQLEXPRESS;Database=hentaibot;Trusted_Connection=True;MultipleActiveResultSets=true;";
                    var comStr = $"SELECT * FROM hanime_requests WHERE CAST(message_id as varchar) = '{component.Message.Id}'";
                    bool cc = connMan.CreateConnection(connStr);
                    bool open = connMan.Open();
                    bool execute = connMan.GetHanimeSqlCommandExecute(comStr);
                    if (!cc) break;
                    if (!open) break;
                    if (!execute) break;

                    if (connMan.currentPage != -1)
                    {
                        currentPage = connMan.currentPage;
                        actualPage = currentPage + 1;
                    }
                    else
                    {
                        currentPage = 0;
                        actualPage = 0;
                    }
                    if (connMan.searchRes != -1)
                    {
                        currentIndex = connMan.searchRes;
                        actualIndex = currentIndex + 1;

                        if (currentPage > 0)
                        {
                            actualIndex = actualIndex + (currentPage * 48);
                        }
                    }
                    else
                    {
                        currentIndex = 0;
                        actualIndex = currentIndex++;
                    }
                    jsonString = connMan.jsonString;

                    Console.WriteLine(actualIndex);
                    Console.WriteLine((actualIndex % 48) == 0);
                    Console.WriteLine($"{actualIndex}: {(actualIndex) % 48}, {actualIndex + 1}: {(actualIndex + 1) % 48}, {actualIndex - 1}: {(actualIndex - 1) % 48}");
                    if (((actualIndex - 1) % 48) == 0)
                    {
                        currentPage -= 1;
                        jsonString = jsonString.Replace($"\"page\":{currentPage + 1}", $"\"page\":{currentPage}");
                        Console.WriteLine(jsonString);
                        actualPage -= 1;
                        currentIndex = 0;
                        actualIndex--;

                    }
                    else
                    {
                        actualIndex--;
                        currentIndex--;
                    }
                    Console.WriteLine(actualIndex);
                    Console.WriteLine(currentIndex);
                    await hanime.PostAsync(currentIndex, actualIndex, jsonString);

                    EmbedBuilder builder = hanime.embedBuilder;
                    ButtonBuilder lastButton = new ButtonBuilder() { Label = "Last", IsDisabled = false, Style = ButtonStyle.Primary, CustomId = "backButton" };
                    ButtonBuilder nextButton = new ButtonBuilder() { Label = "Next", IsDisabled = false, Style = ButtonStyle.Primary, CustomId = "nextButton" };

                    if (actualIndex == 1)
                    {
                        lastButton.IsDisabled = true;
                    }

                    ComponentBuilder componentBuilder = new ComponentBuilder()
                        .WithButton(lastButton)
                        .WithButton(nextButton);

                    await component.UpdateAsync(x =>
                    {
                        x.Content = null;
                        x.Components = componentBuilder.Build();
                        x.Embed = builder.Build();
                    });

                    Console.WriteLine($"Actual Index: {actualIndex}\nCurrent Index: {currentIndex}\nActual Page: {actualPage}\nCurrent Page: {currentPage}\n");
                    comStr = @$"UPDATE hanime_requests SET search_res = {currentIndex} WHERE CAST(message_id as varchar) = '{component.Message.Id}'";
                    bool exe = connMan.SetHanimeSqlCommandExecute(comStr);
                    comStr = @$"UPDATE hanime_requests SET current_page = {currentPage} WHERE CAST(message_id as varchar) = '{component.Message.Id}'";
                    bool _exe = connMan.SetHanimeSqlCommandExecute(comStr);
                    var sqlCmdBuilder = new SqlCommandBuilder();
                    comStr = @$"UPDATE hanime_requests SET json_string = '{sqlCmdBuilder.QuoteIdentifier(jsonString)}' WHERE CAST(message_id as varchar) = '{component.Message.Id}'";
                    bool __exe = connMan.SetHanimeSqlCommandExecute(comStr);
                    connMan.Close();
                    break;
                case "nextButton":
                    // Creating hanime Object and some vars
                    Hanime.Hanime hanime1 = new Hanime.Hanime();
                    int currentPage1;
                    int actualPage1;
                    string jsonString1 = "";
                    int currentIndex1;
                    int actualIndex1;

                    // Database Connection + select and check
                    SQLConnectionManager connMan1 = new SQLConnectionManager();
                    var connStr1 = @"Server=KOTLIN\SQLEXPRESS;Database=hentaibot;Trusted_Connection=True;MultipleActiveResultSets=true;";
                    var comStr1 = $"SELECT * FROM hanime_requests WHERE CAST(message_id as varchar) = '{component.Message.Id}'";
                    bool cc1 = connMan1.CreateConnection(connStr1);
                    bool open1 = connMan1.Open();
                    bool execute1 = connMan1.GetHanimeSqlCommandExecute(comStr1);
                    if (!cc1) break;
                    if (!open1) break;
                    if (!execute1) break;

                    if (connMan1.currentPage != -1)
                    {
                        currentPage1 = connMan1.currentPage;
                        actualPage1 = currentPage1 + 1;
                    } else
                    {
                        currentPage1 = 0;
                        actualPage1 = 0;
                    }
                    if (connMan1.searchRes != -1)
                    {
                        currentIndex1 = connMan1.searchRes;
                        actualIndex1 = currentIndex1 + 1;

                        if (currentPage1 > 0)
                        {
                            actualIndex1 = actualIndex1 + (currentPage1 * 48);
                        }
                    } else
                    {
                        currentIndex1 = 0;
                        actualIndex1 = currentIndex1++;
                    }
                    jsonString1 = connMan1.jsonString;

                    Console.WriteLine(actualIndex1);
                    Console.WriteLine(actualIndex1 % 48);
                    if ((actualIndex1 % 48) == 0 && currentIndex1 != 0)
                    {
                        currentPage1 += 1;
                        jsonString1 = jsonString1.Replace($"\"page\":{currentPage1-1}", $"\"page\":{currentPage1}");
                        Console.WriteLine(jsonString1);
                        actualPage1 += 1;
                        currentIndex1 = 0;
                        actualIndex1++;

                    }
                    else
                    {
                        actualIndex1++;
                        currentIndex1++;
                    }

                    await hanime1.PostAsync(currentIndex1, actualIndex1, jsonString1);

                    EmbedBuilder builder1 = hanime1.embedBuilder;
                    ButtonBuilder lastButton1 = new ButtonBuilder() { Label = "Last", IsDisabled = false, Style = ButtonStyle.Primary, CustomId = "backButton" };
                    ButtonBuilder nextButton1 = new ButtonBuilder() { Label = "Next", IsDisabled = false, Style = ButtonStyle.Primary, CustomId = "nextButton" };
                    
                    if (actualIndex1 == hanime1.MaxHits)
                    {
                        nextButton1.IsDisabled = true;
                    }

                    ComponentBuilder componentBuilder1 = new ComponentBuilder()
                        .WithButton(lastButton1)
                        .WithButton(nextButton1);

                    await component.UpdateAsync(x =>
                    {
                        x.Content = null;
                        x.Components = componentBuilder1.Build();
                        x.Embed = builder1.Build();
                    });

                    Console.WriteLine($"Actual Index: {actualIndex1}\nCurrent Index: {currentIndex1}\nActual Page: {actualPage1}\nCurrent Page: {currentPage1}\n");
                    comStr = @$"UPDATE hanime_requests SET search_res = {currentIndex1} WHERE CAST(message_id as varchar) = '{component.Message.Id}'";
                    bool exe1 = connMan1.SetHanimeSqlCommandExecute(comStr);
                    comStr = @$"UPDATE hanime_requests SET current_page = {currentPage1} WHERE CAST(message_id as varchar) = '{component.Message.Id}'";
                    bool exe2 = connMan1.SetHanimeSqlCommandExecute(comStr);
                    var sqlCmdBuilder1 = new SqlCommandBuilder();
                    comStr = @$"UPDATE hanime_requests SET json_string = '{sqlCmdBuilder1.QuoteIdentifier(jsonString1)}' WHERE CAST(message_id as varchar) = '{component.Message.Id}'";
                    bool exe3 = connMan1.SetHanimeSqlCommandExecute(comStr);
                    connMan1.Close();
                    break;
            }
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);
            if (message.Author.IsBot) return;

            int argPos = 0;
            if (message.HasStringPrefix("hb.", ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
                if (result.Error.Equals(CommandError.UnmetPrecondition)) await message.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}