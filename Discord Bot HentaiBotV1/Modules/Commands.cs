using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Net.NetworkInformation;
using System.Data.SqlClient;

namespace Discord_Bot_HentaiBotV1
{
    public static class BaseValues
    {
        public static Color MainColor = new Color(212, 45, 83);

        public static Color SecColor = new Color(189, 78, 78);

        public static Color TagColor = new Color(65, 99, 210);

        public static EmbedFooterBuilder embedFooter = new EmbedFooterBuilder() { Text = "Vanargand#0001", IconUrl = "https://cdn.discordapp.com/avatars/432565859110879233/5a943ae9f69e96b765e74a1329f4810a.webp?size=128" };

        // Setting BaseURL
        public static string BaseURL = "https://search.htv-services.com/";

        // Setting ImageURL
        public static string ImageURL = "";

    }

    public class Commands : ModuleBase<SocketCommandContext>
    {
        

        // Setting embed footer so I dont have to remake it for every command that uses this
        public EmbedFooterBuilder embed_footer = new EmbedFooterBuilder() { Text = "Vanargand#0001", IconUrl = "https://cdn.discordapp.com/avatars/432565859110879233/5a943ae9f69e96b765e74a1329f4810a.webp?size=128" };

        // Setting Main Color for later use
        public Color MainColor = new Color(212,45,83);

        // Basic ping command
        [Command("ping"), Alias("PING", "Ping", "p", "P")]
        public async Task Ping()
        {
            Ping pinger = new Ping();
            PingReply reply = pinger.Send("discord.com");
            EmbedFooterBuilder embedFooter = new EmbedFooterBuilder() { Text = $"{Context.User.Username}#{Context.User.Discriminator}", IconUrl = $"{Context.User.GetAvatarUrl()}" };
            var embed = new EmbedBuilder() { Title = "Pong!", Description = $"{reply.RoundtripTime}", Footer = embedFooter, Timestamp = DateTimeOffset.Now, Color = MainColor};
            await ReplyAsync(String.Empty, false, embed.Build());
        }

        // Gets Hentai
        [Command("getHentai"), Alias("hentai", "Hentai", "HENTAI", "gethentai", "getHentai", "Gethentai", "GetHentai", "GETHENTAI", "hanime", "Hanime", "HANIME", "H", "h")]
        public async Task getHentai(string option0, string option1=null, string option2=null, string option3=null, string option4=null)
        {

            // Main Pre
            Hanime.Hanime hanime = new Hanime.Hanime();
            Hanime.Collection collection = Hanime.Interpreter.Interpret(option0, option1, option2, option3, option4);
            await hanime.PostAsync(collection.SearchText, collection.Tags, collection.Brands, collection.Blacklist, collection.OrderBy, collection.Ordering, collection.Page, collection.TagMode);


            // Discord Interaction
            EmbedBuilder builder = hanime.embedBuilder;
            ButtonBuilder lastButton = new ButtonBuilder() { Label = "Last", IsDisabled = false, Style = ButtonStyle.Primary, CustomId = "backButton" };
            ButtonBuilder nextButton = new ButtonBuilder() { Label = "Next", IsDisabled = false, Style = ButtonStyle.Primary, CustomId = "nextButton" };
            var componentBuilder = new ComponentBuilder()
                .WithButton(lastButton)
                .WithButton(nextButton);
            var message = await ReplyAsync(String.Empty, false, builder.Build(), component: componentBuilder.Build());
            if (collection.Page == 0)
            {
                lastButton.IsDisabled = true;
            }
            // Database connection and comit!
            var connStr = @"Server=KOTLIN\SQLEXPRESS;Database=hentaibot;Trusted_Connection=True;MultipleActiveResultSets=true;";
            var messageId = message.Id;
            var sqlCmdBuilder = new SqlCommandBuilder();
            var comStr = @$"INSERT INTO hanime_requests (message_id, search_res, json_string, current_page) VALUES ('{messageId}', 0, '{sqlCmdBuilder.QuoteIdentifier(hanime.jsonString)}', {collection.Page})";
            Console.WriteLine("Trying to push to database!");
            Handler.SQLConnectionManager connectionManager = new Handler.SQLConnectionManager();
            bool cc = connectionManager.CreateConnection(connStr);
            bool open = connectionManager.Open();
            bool execute = connectionManager.SetHanimeSqlCommandExecute(comStr);

            // End Debug
            Console.WriteLine(messageId);
            Console.WriteLine($"CreateConnection successful = {cc}\nOpen successful = {open}\nExecute Successful = {execute}");
        }

        [Command("Tags"), Alias("Tag", "tag", "TAG", "TAGS", "tags")]
        public async Task Tags(string tag = "")
        {
            EmbedBuilder embed = new EmbedBuilder();
            if (tag != "")
            {
                Hanime.Tag Tag = Hanime.Tags.FromNameSlugToTag(tag);
                embed.Color = BaseValues.SecColor;
                embed.Title = Tag.Name;
                embed.Fields = new List<EmbedFieldBuilder>() {
                    new EmbedFieldBuilder() { Name = "Description", IsInline = false, Value = Tag.Description},
                    new EmbedFieldBuilder() { Name = "Slug", IsInline = true, Value = Tag.Slug },
                    new EmbedFieldBuilder() { Name = "Internal Id", IsInline = true, Value = Tag.Id }
                };
                embed.Timestamp = DateTimeOffset.Now;
                embed.Author = new EmbedAuthorBuilder() { Name = Tag.ImageAuthorName, Url = Tag.ImageAuthorURL };
                embed.ImageUrl = Tag.ImageURL;
                
            } else
            {
                embed = new EmbedBuilder() { Title = "Tags List", Description = "Tags:\n" };

                foreach (Hanime.Tag Tag in Hanime.Tags.tags)
                {
                    embed.Description += Tag.Name + "\n";
                }
            }
            embed.Color = BaseValues.TagColor;
            await ReplyAsync(String.Empty, false, embed.Build());
        }

        [Command("help")]
        public async Task Help(string cmd = "", string option = "")
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed = NoContextToLogic(cmd, option, embed);
            embed.Color = BaseValues.SecColor;
            await ReplyAsync(String.Empty, false, embed.Build());
        }

        [Command("setBaseURL")]
        public async Task setBaseURL(string _url)
        {
            BaseValues.BaseURL = _url;
            var embed = new EmbedBuilder() { Title = "BaseURL set!", Description = $"BaseURL set to {BaseValues.BaseURL} to make sure it is set use checkBaseURL!", Color = MainColor };
            await ReplyAsync(String.Empty, false, embed.Build());
        }

        [Command("checkBaseURL")]
        public async Task checkBaseURL()
        {
            var embed = new EmbedBuilder() { Title = $"Current BaseURL = {BaseValues.BaseURL}", Color = MainColor };
            await ReplyAsync(String.Empty, false, embed.Build());
        }

        [Command("setImageURL")]
        public async Task setImageURL(string _url)
        {
            BaseValues.ImageURL = _url;
            var embed = new EmbedBuilder() { Title = "ImageURL set!", Description = $"ImageURL set to {BaseValues.ImageURL} to make sure it is set use checkImageURL!", Color = MainColor };
            await ReplyAsync(String.Empty, false, embed.Build());
        }

        [Command("checkImageURL")]
        public async Task checkImageURL()
        {
            var embed = new EmbedBuilder() { Title = $"Current ImageURL = {BaseValues.ImageURL}", Color = MainColor };
            await ReplyAsync(String.Empty, false, embed.Build());
        }

        public async Task ReplyAsync(string content, bool isTTS, EmbedBuilder embed)
        {
            await ReplyAsync(content, isTTS, embed.Build());
        }

        public EmbedBuilder NoContextToLogic(string cmd, string option, EmbedBuilder embed)
        {
            if (cmd != "")
            {
                if (cmd.ToUpper() == "TAG" || cmd.ToUpper() == "TAGS")
                {
                    var embedFields = new List<EmbedFieldBuilder>()
                    {
                        new EmbedFieldBuilder() { Name = "Information", IsInline=false, Value = "Shows information about the specific tag or a list of all tags." },
                        new EmbedFieldBuilder() { Name = "Usage", IsInline = false, Value = "hb.Tag [tag]"}
                    };
                    embed = new EmbedBuilder() { Title = "Help - Tag" };
                    embed.Fields = embedFields;
                }
                else if (cmd.ToUpper() == "HENTAI" || cmd.ToUpper() == "GETHENTAI")
                {
                    if (option.ToUpper() == "ST" || option.ToUpper() == "SEARCH" || option.ToUpper() == "SEARCHTEXT")
                    {
                        embed = new EmbedBuilder() { Title = "Help - getHentai ST", Description = "Usage:" };
                        var embedFields = new List<EmbedFieldBuilder>()
                        {
                            new EmbedFieldBuilder() { Name = "Information", IsInline=false, Value = "Searching for a specific input text." },
                            new EmbedFieldBuilder() { Name = "Usage", IsInline = false, Value = "hb.getHentai \"ST:Baka\" or \"Baka\""}
                        };
                        embed.Fields = embedFields;

                    }
                    else if (option.ToUpper() == "T" || option.ToUpper() == "TAGS" || option.ToUpper() == "TAG")
                    {
                        embed = new EmbedBuilder() { Title = "Help - getHentai T" };

                        var embedFields = new List<EmbedFieldBuilder>()
                        {
                            new EmbedFieldBuilder() { Name = "Information", IsInline=false, Value = "Searching for a specific tag." },
                            new EmbedFieldBuilder() { Name = "Usage", IsInline = false, Value = "hb.getHentai \"T:Big boobs,Blow job\""},
                            new EmbedFieldBuilder() { Name = "Tags", IsInline = false, Value = "#"}
                        };
                        int x = 0;

                        foreach (Hanime.Tag Tag in Hanime.Tags.tags)
                        {
                            if (x == 0)
                            {
                                embedFields[2].Value += Tag.Name;
                                x++;
                            }
                            else
                            {
                                embedFields[2].Value += ", " + Tag.Name;
                            }
                        }
                        embedFields[2].Value = embedFields[2].Value.ToString().Replace("#", "");
                        embed.Fields = embedFields;
                    }
                    else if (option.ToUpper() == "BR" || option.ToUpper() == "BRANDS")
                    {
                        embed = new EmbedBuilder() { Title = "Help - getHentai BR" };
                        var embedFields = new List<EmbedFieldBuilder>()
                        {
                            new EmbedFieldBuilder() { Name = "Information", IsInline=false, Value = "Searching for a specific tag. Either in AND or OR mode." },
                            new EmbedFieldBuilder() { Name = "Usage", IsInline = false, Value = "hb.getHentai \"T:Big boobs,Blow job\""},
                            new EmbedFieldBuilder() { Name = "Brands", IsInline = false, Value = "#"}
                        };
                        int x = 0;
                        int n = 2;
                        foreach (Hanime.Brand brand in Hanime.Brands.brands)
                        {
                            if (x == 0)
                            {

                                embedFields[n].Value += brand.Name;
                                x++;
                            }
                            else
                            {
                                if ((embedFields[n].Value + ", " + brand.Name).Length > 1024)
                                {
                                    embedFields.Add(new EmbedFieldBuilder() { Name = "ㅤ", IsInline = false, Value = brand.Name });
                                    n++;
                                } else
                                {
                                    embedFields[n].Value += ", " + brand.Name;
                                }

                            }
                        }
                        embedFields[2].Value = embedFields[2].Value.ToString().Replace("#", "");
                        embed.Fields = embedFields;
                    }
                    else if (option.ToUpper() == "B" || option.ToUpper() == "BLACKLIST")
                    {
                        embed = new EmbedBuilder() { Title = "Help - getHentai B" };

                        var embedFields = new List<EmbedFieldBuilder>()
                        {
                            new EmbedFieldBuilder() { Name = "Information", IsInline=false, Value = "Excluding specific tag." },
                            new EmbedFieldBuilder() { Name = "Usage", IsInline = false, Value = "hb.getHentai \"B:Big boobs,Blow job\""},
                            new EmbedFieldBuilder() { Name = "Tags", IsInline = false, Value = "#"}
                        };
                        int x = 0;
                        foreach (Hanime.Tag Tag in Hanime.Tags.tags)
                        {
                            if (x == 0)
                            {
                                embedFields[2].Value += Tag.Name;
                                x++;
                            } else
                            {
                                embedFields[2].Value += ", " + Tag.Name;

                            }
                        }
                        embedFields[2].Value = embedFields[2].Value.ToString().Replace("#", "");
                        embed.Fields = embedFields;
                    }
                    else if (option.ToUpper() == "O" || option.ToUpper() == "ORDERING")
                    {
                        embed = new EmbedBuilder() { Title = "Help - getHentai O" };
                        var embedFields = new List<EmbedFieldBuilder>()
                        {
                            new EmbedFieldBuilder() { Name = "Information", IsInline=false, Value = "List of results shown ascending or descending order." },
                            new EmbedFieldBuilder() { Name = "Usage", IsInline = false, Value = "hb.getHentai \"O:desc\" or \"O:asc\""},
                            new EmbedFieldBuilder() { Name = "Type", IsInline = false, Value = "#"}
                        };
                        int x = 0;
                        foreach (Hanime.Ordering ordering in Hanime.Orders.orders)
                        {
                            if (x == 0)
                            {
                                embedFields[2].Value += ordering.Name;
                                x++;
                            } else
                            {
                                embedFields[2].Value += ", " + ordering.Name;
                            }
                        }
                        embedFields[2].Value = embedFields[2].Value.ToString().Replace("#", "");
                        embed.Fields = embedFields;
                    }
                    else if (option.ToUpper() == "OB" || option.ToUpper() == "ORDERBY")
                    {
                        embed = new EmbedBuilder() { Title = "Help - getHentai OB" };
                        var embedFields = new List<EmbedFieldBuilder>()
                        {
                            new EmbedFieldBuilder() { Name = "Information", IsInline=false, Value = "List of results will be sorted according to this." },
                            new EmbedFieldBuilder() { Name = "Usage", IsInline = false, Value = "hb.getHentai \"OB:likes\""},
                            new EmbedFieldBuilder() { Name = "Sort types", IsInline = false, Value = "#"}
                        };
                        int x = 0;
                        foreach (Hanime.Orderby orderby in Hanime.OrderBys.orderbys)
                        {
                            if (x == 0)
                            {
                                embedFields[2].Value += orderby.Name;
                                x++;
                            } else
                            {
                                embedFields[2].Value += ", " + orderby.Name;
                            }
                        }
                        embedFields[2].Value = embedFields[2].Value.ToString().Replace("#", "");
                        embed.Fields = embedFields;
                    }
                    else if (option.ToUpper() == "TM" || option.ToUpper() == "TAGMODE")
                    {
                        embed = new EmbedBuilder() { Title = "Help - getHentai TM" };
                        var embedFields = new List<EmbedFieldBuilder>()
                        {
                            new EmbedFieldBuilder() { Name = "Information", IsInline=false, Value = "The tag mode that should be used." },
                            new EmbedFieldBuilder() { Name = "Usage", IsInline = false, Value = "hb.getHentai \"TM:AND\""},
                            new EmbedFieldBuilder() { Name = "Tag modes", IsInline = false, Value = "#"}
                        };
                        int x = 0;
                        foreach (Hanime.TagMode tag in Hanime.TagModes.tagmodes)
                        {
                            if (x == 0)
                            {
                                embedFields[2].Value += tag.Name;
                                x++;
                            } else
                            {
                                embedFields[2].Value += ", " + tag.Name;
                            }
                        }
                        embedFields[2].Value = embedFields[2].Value.ToString().Replace("#", "");
                        embed.Fields = embedFields;
                    }
                    else if (option.ToUpper() == "P" || option.ToUpper() == "PAGE")
                    {
                        embed = new EmbedBuilder() { Title = "Help - getHentai P", Description = "Usage:\n getHentai \"P:5\" " };
                        var embedFields = new List<EmbedFieldBuilder>()
                        {
                            new EmbedFieldBuilder() { Name = "Information", IsInline=false, Value = "The page that should be searched." },
                            new EmbedFieldBuilder() { Name = "Usage", IsInline = false, Value = "hb.getHentai \"P:5\""}
                        };
                        embed.Fields = embedFields;
                    }
                    else
                    {
                        embed = new EmbedBuilder() { Title = "Help - getHentai", Color = BaseValues.SecColor, Timestamp = DateTimeOffset.Now };
                        //, Description = "getHentai option1 [option2] [option3] [option4] [option5] - Searches for a hentai on Hanime\n\nOptions:\nSearch Text - [] & [ST]\nTags - [T](tag 1,tag2,tag 3)\nBrands - [BR](brand 1,brand2, brand 3)\nBlacklist - [B](Blacklisted Tag1,2,Tag 3)\nOrder By - [OB]\nOrder - [O]\nPage - [P]\nTag Mode - [TM] \n\nExample: getHentai \"T:Teacher,Boob job\" \"OB:likes\""
                        var embedFields = new List<EmbedFieldBuilder>()
                        {
                            new EmbedFieldBuilder() { Name = "Information", IsInline=false, Value = "Command to search for hentai on hanime.tv." },
                            new EmbedFieldBuilder() { Name = "Usage", IsInline = false, Value = "hb.getHentai option1 [optional]x4"},
                            new EmbedFieldBuilder() { Name = "Example", IsInline = false, Value = "hb.getHentai \"RPG\" \"B:Loli,Anal\" \"T:Big boobs,Blow job\" \"OB:views\""}
                        };
                        embed.Fields = embedFields;
                    }
                }
                else
                {
                    embed = new EmbedBuilder() { Title = "Help - General", Color = BaseValues.SecColor, Timestamp = DateTimeOffset.Now };
                    var embedFields = new List<EmbedFieldBuilder>()
                        {
                            new EmbedFieldBuilder() { Name = "Help", IsInline=false, Value = "Shows this command.\nhb.help [command] [option]" },
                            new EmbedFieldBuilder() { Name = "Tag", IsInline = false, Value = "Shows a list of tags or information about a specific tag.\nhb.Tag [tag]"},
                            new EmbedFieldBuilder() { Name = "getHentai", IsInline = false, Value = "Shows search results of a hanime search.\nhb.getHentai option [options]\nMax options 5"}
                        };
                    embed.Fields = embedFields;
                }
            }
            else
            {
                embed = new EmbedBuilder() { Title = "Help - General", Color = BaseValues.SecColor, Timestamp = DateTimeOffset.Now };
                var embedFields = new List<EmbedFieldBuilder>()
                        {
                            new EmbedFieldBuilder() { Name = "Help", IsInline=false, Value = "Shows this command.\nhb.help [command] [option]" },
                            new EmbedFieldBuilder() { Name = "Tag", IsInline = false, Value = "Shows a list of tags or information about a specific tag.\nhb.Tag [tag]"},
                            new EmbedFieldBuilder() { Name = "getHentai", IsInline = false, Value = "Shows search results of a hanime search.\nhb.getHentai option [options]\nMax options 5"}
                        };
                embed.Fields = embedFields;
            }
            return embed;
        }
        
    }
}
    