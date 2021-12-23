using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;
using Discord;
using Discord.Commands;

namespace Discord_Bot_HentaiBotV1.Hanime
{
    public class Interpreter
    {
        public static Collection Interpret(string option0, string option1 = null, string option2 = null, string option3 = null, string option4 = null)
        {
            Collection collection = new Collection();
            List<string> Options = new List<string>() { option0, option1, option2, option3, option4 };
            List<string> Tests = new List<string>() { "ST", "T", "BR", "B", "OB", "O", "P", "TM" };
            foreach (string option in Options)
            {
                if (option == null) continue;
                List<string> optionSp = new List<string>();
                try
                {
                    optionSp = option.Split(":").ToList();
                }
                catch (Exception)
                {
                    optionSp[0] = option;

                }
                foreach (var test in Tests)
                {
                    if (optionSp[0] == option)
                    {
                        if (collection.SearchText != null)
                        {
                            Console.WriteLine("Problem with this request! -> Ignore");
                        }
                        else
                        {
                            collection.SearchText = optionSp[0];
                        }
                    }
                    else if (optionSp[0] == test)
                    {
                        string Type = OptionType.Map(Tests.IndexOf(optionSp[0]));
                        if (Type == "SEARCHTEXT")
                        {
                            collection.SearchText = optionSp[1];
                        }
                        else if (Type == "TAGS")
                        {
                            List<string> tags = optionSp[1].Split(",").ToList();
                            List<string> trueTags = new List<string>();
                            foreach (string tag in tags)
                            {
                                var realTag = Tags.FromNameSlugToTag(tag);
                                if (realTag != null)
                                {
                                    trueTags.Add(realTag.Name);
                                }
                            }
                            collection.Tags = trueTags;
                        }
                        else if (Type == "BLACKLIST")
                        {
                            List<string> tags = optionSp[1].Split(",").ToList();
                            List<string> trueTags = new List<string>();
                            foreach (string tag in tags)
                            {
                                var realTag = Tags.FromNameSlugToTag(tag);
                                if (realTag != null)
                                {
                                    trueTags.Add(realTag.Name);
                                }
                            }
                            collection.Blacklist = trueTags;
                        }
                        else if (Type == "BRANDS")
                        {
                            List<string> brands = optionSp[1].Split(",").ToList();
                            List<string> trueBrands = new List<string>();
                            foreach (string brand in brands)
                            {
                                var realBrand = Brands.FromNameSlugToBrand(brand);
                                if (realBrand != null)
                                {
                                    trueBrands.Add(realBrand.Name);
                                }
                            }
                            collection.Brands = trueBrands;
                        }
                        else if (Type == "ORDERING")
                        {
                            List<string> orders = optionSp[1].Split(",").ToList();
                            string trueOrder = String.Empty;
                            foreach (string order in orders)
                            {
                                var realOrder = Orders.FromNameSlugToOrder(order);
                                if (realOrder != null)
                                {
                                    trueOrder = realOrder.Name;
                                }
                            }
                            collection.Ordering = trueOrder;
                        }
                        else if (Type == "ORDERBY")
                        {
                            List<string> orders = optionSp[1].Split(",").ToList();
                            string trueOrder = String.Empty;
                            foreach (string order in orders)
                            {
                                var realOrder = Orders.FromNameSlugToOrder(order);
                                if (realOrder != null)
                                {
                                    trueOrder = realOrder.Name;
                                }
                            }
                            collection.OrderBy = trueOrder;
                        }
                        else if (Type == "TAGMODE")
                        {
                            List<string> orders = optionSp[1].Split(",").ToList();
                            string trueOrder = String.Empty;
                            foreach (string order in orders)
                            {
                                var realOrder = Orders.FromNameSlugToOrder(order);
                                if (realOrder != null)
                                {
                                    trueOrder = realOrder.Name;
                                }
                            }
                            collection.TagMode = trueOrder;
                        }
                        else if (Type == "PAGE")
                        {
                            List<string> pages = optionSp[1].Split(",").ToList();
                            if (pages.Count > 1)
                            {
                                Console.WriteLine("Pages cant have more than 1 member!");
                            }
                            else if (pages == null)
                            {
                                Console.WriteLine("Pages cant be null");
                            }
                            collection.Page = Int32.Parse(pages[0]);
                        }
                        else
                        {
                            Console.WriteLine("Undefined!");
                        }

                    }
                }
            }
            if (collection.SearchText == null)
            {
                collection.SearchText = "";
            }
            if (collection.Tags == null)
            {
                collection.Tags = new List<string>();
            }
            if (collection.Blacklist == null)
            {
                collection.Blacklist = new List<string>();
            }
            if (collection.Brands == null)
            {
                collection.Brands = new List<string>();
            }
            if (collection.TagMode == null)
            {
                collection.TagMode = "AND";
            }
            if (collection.OrderBy == null)
            {
                collection.OrderBy = "released_at_unix";
            }
            if (collection.Ordering == null)
            {
                collection.Ordering = "desc";
            }
            if (collection.Page == 0)
            {
                collection.Page = 0;
            }
            return collection;
        }
    }

    public class Hanime
    {
        private readonly HttpClient client = new HttpClient();

        public EmbedBuilder embedBuilder = null;

        public string jsonString = "";

        public HanimeJson Data = new HanimeJson();

        public long MaxHits = -1;

        public long MaxPages = -1;

        public Array array = new[] { new
        {
            search_text = (string)null,
            tags = new List<string>(),
            brands = new List<string>(),
            blacklist = new List<string>(),
            order_by = (string)null,
            ordering = (string)null,
            page = (int?)-1,
            tag_mode = (string)null
        }
        }.ToArray();

        public static EmbedBuilder GetEmbedBuilder()
        {
            return new EmbedBuilder() { Title = "Pre", Description = "Pre", Timestamp = DateTimeOffset.Now, Color = BaseValues.MainColor };
        }

        public static EmbedBuilder SetEmbedBuilder(EmbedBuilder BaseBuilder, string jsonString, int index)
        {
            List<Hentai> res = JsonConvert.DeserializeObject<List<Hentai>>(jsonString);
            Console.WriteLine($"jsonString Deserialzed to {res.GetType()}, ObjCount: {res.Count}");
            BaseBuilder.Title = $"{res[index].Name}";
            BaseBuilder.Description = $"{res[index].Description}";
            return BaseBuilder;
        }

        public async Task PostAsync(int index, string search_text, List<string> tags, List<string> brands, List<string> blacklist, string order_by = "created_at_unix", string ordering = "desc", int page = 0, string tag_mode = "AND")
        {
            var dataobject = new
            {
                search_text = search_text,
                tags = tags,
                brands = brands,
                blacklist = blacklist,
                order_by = order_by,
                ordering = ordering,
                page = page,
                tag_mode = tag_mode
            };
            R2H req2Hen = new R2H();
            await req2Hen.Request2Hentai(dataobject, client, index);
            Hentai hentai = req2Hen.Hentai;
            long MaxPages = req2Hen.MaxPages;
            long MaxHits = req2Hen.MaxHits;
            jsonString = req2Hen.JS;
            int actualIndex;
            if (page > 0)
            {
                actualIndex = index + (page * 48);
            }
            else
            {
                actualIndex = index + 1;
            }
            int actualPage = page + 1;
            var embedFooter = new EmbedFooterBuilder() { Text = $"{actualIndex}/{MaxHits} {actualPage}/{MaxPages}" };
            var embed = new EmbedBuilder() { Title = hentai.Name, Author = new EmbedAuthorBuilder() { Name = hentai.Brand, Url = $"https://hanime.tv/browse/brands/{hentai.Brand.ToUrlSlug()}" }, Url = $"https://hanime.tv/videos/hentai/{hentai.Slug.ToUrlSlug()}", ImageUrl = hentai.PosterUrl, Description = hentai.Description.StripHTML(), Footer = embedFooter, Timestamp = DateTimeOffset.Now, Color = BaseValues.MainColor };

            embedBuilder = embed;
        }



        public async Task PostAsync(int index, int actualIndex, string jsonstring)
        {
            int page = int.Parse(jsonstring.Split(",").ToList()[6].Split(":")[1]);
            int actualPage = page + 1;
            R2H req2Hen = new R2H();
            await req2Hen.Request2Hentai(jsonstring, client, index);
            Hentai hentai = req2Hen.Hentai;
            long MaxPages = req2Hen.MaxPages;
            long MaxHits = req2Hen.MaxHits;
            jsonString = req2Hen.JS;
            var embedFooter = new EmbedFooterBuilder() { Text = $"{actualIndex}/{MaxHits} {actualPage}/{MaxPages}" };
            var embed = new EmbedBuilder() { Title = hentai.Name, Author = new EmbedAuthorBuilder() { Name = hentai.Brand, Url = $"https://hanime.tv/browse/brands/{hentai.Brand.ToUrlSlug()}" }, Url = $"https://hanime.tv/videos/hentai/{hentai.Slug.ToUrlSlug()}", ImageUrl = hentai.PosterUrl, Description = hentai.Description.StripHTML(), Footer = embedFooter, Timestamp = DateTimeOffset.Now, Color = BaseValues.MainColor };

            embedBuilder = embed;
        }

        public async Task PostAsync(string search_text, List<string> tags, List<string> brands, List<string> blacklist, string order_by = "created_at_unix", string ordering = "desc", int page = 0, string tag_mode = "AND", int index = 0)
        {
            try
            {
                var dataobject = new
                {
                    search_text = search_text,
                    tags = tags,
                    brands = brands,
                    blacklist = blacklist,
                    order_by = order_by,
                    ordering = ordering,
                    page = page,
                    tag_mode = tag_mode
                };
                R2H req2Hen = new R2H();
                await req2Hen.Request2Hentai(dataobject, client, index);
                Hentai hentai = req2Hen.Hentai;
                long MaxPages = req2Hen.MaxPages;
                long MaxHits = req2Hen.MaxHits;
                jsonString = req2Hen.JS;
                int actualIndex;
                if (page > 0)
                {
                    actualIndex = index + 1 + (page * 48);
                }
                else
                {
                    actualIndex = index + 1;
                }
                int actualPage = page + 1;
                var embedFooter = new EmbedFooterBuilder() { Text = $"{actualIndex}/{MaxHits} {actualPage}/{MaxPages}" };
                var embed = new EmbedBuilder() { Title = hentai.Name, Author = new EmbedAuthorBuilder() { Name = hentai.Brand, Url = $"https://hanime.tv/browse/brands/{hentai.Brand.ToUrlSlug()}" }, Url = $"https://hanime.tv/videos/hentai/{hentai.Slug}", ImageUrl = hentai.PosterUrl, Description = hentai.Description.StripHTML(), Footer = embedFooter, Timestamp = DateTimeOffset.Now, Color = BaseValues.MainColor };

                embedBuilder = embed;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }

    public class DataObject
    {
        public string search_text { get; set; }
        public List<string> tags { get; set; }
        public List<string> brands { get; set; }
        public List<string> blacklist { get; set; }
        public string order_by { get; set; }
        public string ordering { get; set; }
        public int page { get; set; }
        public string tag_mode { get; set; }
    }

    public class HentaiRequestObj
    {
        public string search_text { get; set; }
        public List<string> tags { get; set; }
        public List<string> brands { get; set; }
        public List<string> blacklist { get; set; }
        public string orderby { get; set; }
        public string ordering { get; set; }
        public int page { get; set; }
        public string tagmode { get; set; }
        public int index { get; set; }
        public int actualIndex { get; set; }
        public long MaxHits { get; set; }
        public long MaxPages { get; set; }
    }

    public class R2H
    {
        public Hentai Hentai = new Hentai().Empty();

        public long MaxHits = 0;

        public long MaxPages = 0;

        public string JS = "";

        public async Task Request2Hentai<TValue>(TValue value, HttpClient client, int index = 0)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(value);
            JS = json;
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(BaseValues.BaseURL, data);
            string result = await response.Content.ReadAsStringAsync();
            HanimeJson hanimeJson = HanimeJson.FromJson(result);
            MaxHits = hanimeJson.NbHits;
            MaxPages = hanimeJson.NbPages;
            Console.WriteLine("Base JSON successfully converted!");
            List<Hentai> res = JsonConvert.DeserializeObject<List<Hentai>>(hanimeJson.Hits);
            Hentai = res[index];
        }

        public async Task Request2Hentai(string jsonstring, HttpClient client, int index = 0)
        {
            JS = jsonstring;
            StringContent data = new StringContent(jsonstring, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(BaseValues.BaseURL, data);
            string result = await response.Content.ReadAsStringAsync();
            HanimeJson hanimeJson = HanimeJson.FromJson(result);
            MaxHits = hanimeJson.NbHits;
            MaxPages = hanimeJson.NbPages;
            Console.WriteLine("Base JSON successfully converted!");
            List<Hentai> res = JsonConvert.DeserializeObject<List<Hentai>>(hanimeJson.Hits);
            Hentai = res[index];
        }
    }

    public static class Extensions
    {
        public static string StripHTML(this string str)
        {
            str = str.Replace("<p>", "");
            str = str.Replace("</p>", "");
            str = str.Replace("<br>", "");
            return str;
        }

        public static string ToUrlSlug(this string str)
        {
            str = str.ToLower();
            str = str.Replace(" ", "-");
            return str;
        }
    }

    public class OptionType
    {
        static int UNDEFINED = -1;
        static int SEARCHTEXT = 0;
        static int TAGS = 1;
        static int BRANDS = 2;
        static int BLACKLIST = 3;
        static int ORDERBY = 4;
        static int ORDERING = 5;
        static int PAGE = 6;
        static int TAGMODE = 7;

        public static string Map(int x)
        {
            if (x == TAGMODE)
            {
                return "TAGMODE";
            }
            else if (x == SEARCHTEXT)
            {
                return "SEARCHTEXT";
            }
            else if (x == TAGS)
            {
                return "TAGS";
            }
            else if (x == BRANDS)
            {
                return "BRANDS";
            }
            else if (x == BLACKLIST)
            {
                return "BLACKLIST";
            }
            else if (x == ORDERBY)
            {
                return "ORDERBY";
            }
            else if (x == ORDERING)
            {
                return "ORDERING";
            }
            else if (x == PAGE)
            {
                return "PAGE";
            }
            else
            {
                return "UNDEFINED";
            }

        }
    }

    public class Collection
    {
        public string SearchText { get; set; }

        public List<string> Tags { get; set; }

        public List<string> Brands { get; set; }

        public List<string> Blacklist { get; set; }

        public string OrderBy { get; set; }

        public string Ordering { get; set; }

        public int Page { get; set; }

        public string TagMode { get; set; }
    }

    public class Interpret
    {
        public int Type { get; set; }

        string SearchText { get; set; }

        List<string> Tags { get; set; }

        List<string> Brands { get; set; }

        List<string> Blacklist { get; set; }

        string OrderBy { get; set; }

        int Page { get; set; }

        string TagMode { get; set; }
    }

    public class Tag
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Id { get; set; }

        public string Slug { get; set; }

        public string ImageURL { get; set; }

        public string ImageAuthorURL { get; set; }

        public string ImageAuthorName { get; set; }
    }

    public class Brand
    {
        public string Name { get; set; }

        public int Id { get; set; }

        public string Slug { get; set; }
    }

    public class Tags
    {
        public static List<Tag> tags = new List<Tag>() {
            new Tag() { Name = "3D", Description = "3D Animated hentai.", Id=1, Slug="3D",
                ImageAuthorName = "ランセ＠COM3D2",
                ImageAuthorURL = "https://www.pixiv.net/en/users/40031915",
                ImageURL = "https://i.ibb.co/gzY1hdq/3d.png"},
            new Tag() { Name = "Ahegao", Description = "Chicks who get fucked silly. Tongues hanging out, eyes rolling back, that good stuff!", Id=2, Slug="AHEGAO",
                ImageAuthorName = "DDD",
                ImageAuthorURL = "https://www.pixiv.net/en/users/16702099",
                ImageURL = "https://i.ibb.co/gwwttBY/ahegao.png"},
            new Tag() { Name = "Anal", Description = "The wrong whole. But it feels so right.", Id=3, Slug="ANAL",
                ImageAuthorName = "華粉",
                ImageAuthorURL = "https://www.pixiv.net/en/users/12080659",
                ImageURL = "https://i.ibb.co/kQFMqfH/anal.png"},
            new Tag() { Name = "Bdsm", Description = "An overlapping abbrevation of Bondage and Discipline (BD), Dominance and Submission (DS), Sadism and Masochism (SM).", Id=4, Slug="BDSM",
                ImageAuthorName = "HornyOni",
                ImageAuthorURL = "https://www.pixiv.net/en/users/44741005",
                ImageURL = "https://i.ibb.co/wYFn4bC/bdsm.png"},
            new Tag() { Name = "Big Boobs", Description = "BIg Milk Bags.Huge Melons.Extra Large Titties. Fat girls chest.\"Things on thin girl's chests that are either silicone filled or gifted by god.\"", Id=5, Slug="BIGBOOBS",
                ImageAuthorName = "MagicWritings",
                ImageAuthorURL = "https://www.patreon.com/magicwritings",
                ImageURL = "https://i.ibb.co/R7JnyqM/bigboobs.jpg"},
            new Tag() { Name = "Blow Job", Description = "Oral sex, where guys get to relax, sit back and take in the view. I heard it's quite a mouthful.", Id=6, Slug="BLOWJOB",
                ImageAuthorName = "Erkerut",
                ImageAuthorURL = "https://www.pixiv.net/en/users/3890201",
                ImageURL = "https://i.ibb.co/qJGKqm9/blowjob.jpg"},
            new Tag() { Name = "Bondage", Description = "SM without a ton of accessories.", Id = 7, Slug = "BONDAGE",
                ImageAuthorName = "ふぇむと",
                ImageAuthorURL = "https://www.pixiv.net/en/users/226488",
                ImageURL = "https://i.ibb.co/7KpDLZG/bondage2.png"
                //https://i.ibb.co/WVsMNzJ/bondage.png
                },
            new Tag() { Name = "Boob Job", Description = "Sit back and relax...", Id = 8, Slug = "BOOBJOB",
                ImageAuthorName = "slutwhore",
                ImageAuthorURL = "https://www.reddit.com/user/slutwhore/posts",
                ImageURL = "https://i.ibb.co/cCYbS7g/boobjob.png"},
            new Tag() { Name = "Censored", Description = "If you want this: Find help dude!", Id = 9, Slug = "CENSORED",
                ImageAuthorName = "Jezz G",
                ImageAuthorURL = "https://www.pixiv.net/en/users/33044800",
                ImageURL = "https://i.ibb.co/yk19V7w/censored.jpg"},
            new Tag() { Name = "Comedy", Description = "Do you need a laugh while fapping?", Id = 10, Slug = "COMEDY",
                ImageAuthorName = "某は猫",
                ImageAuthorURL = "https://www.pixiv.net/en/users/60327465",
                ImageURL = "https://i.ibb.co/JnCGh3p/comedy.jpg"},
            new Tag() { Name = "Cosplay", Description = "Cosplay is when chicks dress up as different characters - like a nurse, a cop, a cheerleader, your mom.", Id = 11, Slug = "COSPLAY",
                ImageAuthorName = "fastrunner2024",
                ImageAuthorURL = "https://www.pixiv.net/en/users/10944996",
                ImageURL = "https://i.ibb.co/sF8xyd2/cosplay.png"},
            new Tag() { Name = "Creampie", Description = "PLEASE DON'T CUM INSIDE! ~ ~ ~ NOOOOOOO! I AM GOING TO GET PREGNANT!", Id = 12, Slug = "CREAMPIE",
                ImageAuthorName = "kumasteam",
                ImageAuthorURL = "https://rule34.world/kumasteam",
                ImageURL = "https://i.ibb.co/G55gWFX/creampie.jpg"},
            new Tag() { Name = "Dark Skin", Description = "Ain't a racist ? Put your money where your mouth is and fuck a dark skinned chick!", Id = 13, Slug = "DARKSKIN",
                ImageAuthorName = "olchas",
                ImageAuthorURL = "https://rule34.xxx/index.php?page=post&s=list&tags=olchas",
                ImageURL = "https://i.ibb.co/D8yLF4L/darkskin.jpg"},
            new Tag() { Name = "Facial", Description = "Involves a dude ejaculating on a chick's face. Mostly cause she ain't up for swallowing..", Id = 14, Slug = "FACIAL",
                ImageAuthorName = "Rule34",
                ImageAuthorURL = "https://rule34.xxx/index.php?page=post&s=view&id=2694135",
                ImageURL = "https://i.ibb.co/jh4yptt/facial.gif"},
            new Tag() { Name = "Fantasy", Description = "Someone's dream way/place to have sex.", Id = 15, Slug = "FANTASY",
                ImageAuthorName = "う★どんこ",
                ImageAuthorURL = "https://www.pixiv.net/en/users/60295808",
                ImageURL = "https://i.ibb.co/5W0KxrB/fantasy.jpg"},
            new Tag() { Name = "Filmed", Description = "Wait Nooo don't record me in this situation!", Id = 16, Slug = "FILMED",
                ImageAuthorName = "文月みそか",
                ImageAuthorURL = "https://www.pixiv.net/en/users/166078",
                ImageURL = "https://i.ibb.co/K7DCmrX/filmed.jpg"},
            new Tag() { Name = "Foot Job", Description = "Who knew someone's feet could be put to good use...", Id = 17, Slug = "FOOTJOB",
                ImageAuthorName = "Wiresetc",
                ImageAuthorURL = "https://www.deviantart.com/wiresetc/gallery",
                ImageURL = "https://i.ibb.co/pdQJZg5/footjob.jpg"},
            new Tag() { Name = "Futanari", Description = "Chicks with dicks.", Id = 18, Slug = "FUTANARI",
                ImageAuthorName = "AnimeFlux",
                ImageAuthorURL = "https://www.hentai-foundry.com/user/AnimeFlux/profile",
                ImageURL = "https://i.ibb.co/DCZwj5r/futanari.jpg"},
            new Tag() { Name = "Gangbang", Description = "When 3 or more people gang up on a single person of the opposite sex, we have this lovely situation play out.", Id = 19, Slug = "GANGBANG",
                ImageAuthorName = "辻善",
                ImageAuthorURL = "https://www.pixiv.net/en/users/249950",
                ImageURL = "https://i.ibb.co/WcTPgXJ/gangbang.jpg"},
            new Tag() { Name = "Glasses", Description = "Chicks with glasses ? Nerds got horny too!", Id = 20, Slug = "Glasses",
                ImageAuthorName = "う★どんこ",
                ImageAuthorURL = "https://www.pixiv.net/en/users/60295808",
                ImageURL = "https://i.ibb.co/n7G12rN/glasses.jpg"},
            new Tag() { Name = "Hand Job", Description = "If a person with a dick gets someone to jerk them off.", Id = 21, Slug = "HANDJOB",
                ImageAuthorName = "Kasagi_uwu",
                ImageAuthorURL = "https://www.pixiv.net/en/users/33937885",
                ImageURL = "https://i.ibb.co/Tcr4WZK/handjob.png"},
            new Tag() { Name = "Harem", Description = "One guy, multiple chicks. Ever wonder what happens when we replace a dumbest anime protagonist with an actual male?", Id = 22, Slug = "HAREM",
                ImageAuthorName = "Kotoha Kenzaki",
                ImageAuthorURL = "https://www.pixiv.net/en/users/69481490",
                ImageURL = "https://i.ibb.co/CsvJsTB/harem.jpg"},
            new Tag() { Name = "HD", Description = "Videos in the HD(High definition) standard resolution.", Id = 23, Slug = "HD",
                ImageAuthorName = "イマジン孝二",
                ImageAuthorURL = "https://www.pixiv.net/en/users/18836671",
                ImageURL = "https://i.ibb.co/pdhzWDX/hd.jpg"},
            new Tag() { Name = "Horror", Description = "Is it already halloween? Don't die out there man!", Id = 24, Slug = "HORROR", 
                ImageAuthorName = "BloodiRose",
                ImageAuthorURL = "https://www.pixiv.net/en/users/4588711", 
                ImageURL = "https://i.ibb.co/0BVNZzB/horror.png"},
            new Tag() { Name = "Incest", Description = "My little sister can't be this cute!", Id = 25, Slug = "INCEST", 
                ImageAuthorName = "HentaiArtist2k", 
                ImageAuthorURL = "https://www.pixiv.net/en/users/45703611", 
                ImageURL = "https://i.ibb.co/tp95pT6/incest.jpg"},
            new Tag() { Name = "Inflation", Description = "When a chick's stomach or boobs get bigger cause of magic or cause of cum overload. Yes, that's a thing.", Id = 26, Slug = "INFLATION", 
                ImageAuthorName = "The Aya/彩", 
                ImageAuthorURL = "https://www.pixiv.net/en/users/26687534", 
                ImageURL = "https://i.ibb.co/q0xHrGL/inflation.png"},
            new Tag() { Name = "Loli", Description = "Don't worry, we won't tell the cops...they already know.", Id = 27, Slug = "LOLICON", 
                ImageAuthorName = "北桐すずめ", 
                ImageAuthorURL = "https://www.pixiv.net/en/users/45876889",
                ImageURL = "https://i.ibb.co/Pm2z18x/loli.png"},
            new Tag() { Name = "Maid", Description = "Someone that cleans, cooks food, and to please there master however they can...", Id = 28, Slug = "MAID",
                ImageAuthorName = "東雲あす", 
                ImageAuthorURL = "https://www.pixiv.net/en/users/23677411", 
                ImageURL = "https://i.ibb.co/F55T21c/maid.png"},
            new Tag() { Name = "Masturbation", Description = "Making it yourself.", Id = 29, Slug = "MASTURBATION",
                ImageAuthorName = "リリア", 
                ImageAuthorURL = "https://www.pixiv.net/en/users/16707782",
                ImageURL = "https://i.ibb.co/Qp2GN1R/masturbation.png"},
            new Tag() { Name = "Milf", Description = "Hot older woman. The 2D kind.", Id = 30, Slug = "MILF",
                ImageAuthorName = "HWA NA JUNG",
                ImageAuthorURL = "https://www.pixiv.net/en/users/3872834",
                ImageURL = "https://i.ibb.co/w0stk7f/milf.png"},
            new Tag() { Name = "Mind Break", Description = "Corrupting the innocent and turning them into sex crazed friends, what more could you possibly ask for?", Id = 31, Slug = "MINDBREAK",
                ImageAuthorName = "PTS",
                ImageAuthorURL = "https://www.pixiv.net/en/users/46727138",
                ImageURL = "https://i.ibb.co/X4knNpK/mindbreak.jpg"},
            new Tag() { Name = "Mind Control", Description = "Someone taking over other people to do completely normal and not lewd things!", Id = 32, Slug = "MINDCONTROL",
                ImageAuthorName = "Migi Rider",
                ImageAuthorURL = "https://www.pixiv.net/en/users/13793698",
                ImageURL = "https://i.ibb.co/16h6MnR/mindcontrol.png"},
            new Tag() { Name = "Monster", Description = "A synonym for human.", Id = 33, Slug = "MONSTER", 
                ImageAuthorName = "Aiasuru",
                ImageAuthorURL = "https://www.pixiv.net/en/users/29784355",
                ImageURL = "https://i.ibb.co/K5Z9yzD/monster.jpg"},
            new Tag() { Name = "Nekomimi", Description = "Literally meaning \"Cat-ear\" in Japanese. Usually a girl in cosplay (costume) wearing cat ears and tail.", Id = 34, Slug = "NEKOMIMI",
                ImageAuthorName = "Cottonコットン",
                ImageAuthorURL = "https://www.pixiv.net/en/users/65937263", 
                ImageURL = "https://i.ibb.co/VJJjdxK/nekomimi.png"},
            new Tag() { Name = "Netorare", Description = "When the person you love cheats on you with someone else - willingly or unwillingly.", Id = 35, Slug = "NTR",
                ImageAuthorName = "RobynChan", 
                ImageAuthorURL = "https://www.pixiv.net/en/users/69684635",
                ImageURL = "https://i.ibb.co/xhL8s6F/netorare.jpg"},
            new Tag() { Name = "Nurse", Description = "Someone who nurses you back to health by draining you out. It works. Really.", Id = 36, Slug = "NURSE", 
                ImageAuthorName = "ProfNote", 
                ImageAuthorURL = "https://www.pixiv.net/en/users/5163944", 
                ImageURL = "https://i.ibb.co/fX2xJKg/nurse.png"},
            new Tag() { Name = "Orgy", Description = "Five or more consenting persons engaging sexual intercourse in the same place over the same period of time. Synonymous with group sex. Common props include drugs, alcohol, toys, etc. but are not neccesary.", Id = 37, Slug = "ORGY", 
                ImageAuthorName = "Blue triangles",
                ImageAuthorURL = "https://www.pixiv.net/en/users/57140839",
                ImageURL = "https://i.ibb.co/vP7GS0T/orgy.png"},
            new Tag() { Name = "Plot", Description = "Mum I am only watching it for the plot! I mean it!", Id = 38, Slug = "PLOT",
                ImageAuthorName = "伝説の牛丼",
                ImageAuthorURL = "https://www.pixiv.net/en/users/73732383", 
                ImageURL = "https://i.ibb.co/YBQTGGZ/plot.jpg"},
            new Tag() { Name = "POV", Description = "POV (Point of View) makes you feels like you're the dude in the clip.", Id = 39, Slug = "POV",
                ImageAuthorName = "Bunbury",
                ImageAuthorURL = "https://www.pixiv.net/en/users/51013825", 
                ImageURL = "https://i.ibb.co/2twM8yN/pov.png"},
            new Tag() { Name = "Pregnant", Description = "No need to wear a condom cause pregnant chicks can't get any more pregnant ;)", Id = 40, Slug = "PREGNANT",
                ImageAuthorName = "秋空",
                ImageAuthorURL = "https://www.pixiv.net/en/users/20291650",
                ImageURL = "https://i.ibb.co/fGM4mg2/pregnant.jpg"},
            new Tag() { Name = "Public Sex", Description = "Getting a room is so over rated! Lets just do it out here in broad daylight, there's no way someone will see us.", Id = 41, Slug = "PUBLICSEX",
                ImageAuthorName = "ま",
                ImageAuthorURL = "https://www.pixiv.net/en/users/260215",
                ImageURL = "https://i.ibb.co/r4JT9pJ/publicsex.png"},
            new Tag() { Name = "Rape", Description = "RAPE. Small word, long sentence.", Id = 42, Slug = "RAPE",
                ImageAuthorName = "大呂亮TairyoRyo",
                ImageAuthorURL = "https://www.pixiv.net/en/users/1039091",
                ImageURL = "https://i.ibb.co/D74fpGb/rape.jpg"},
            new Tag() { Name = "Reverse Rape", Description = "When a woman tries to put her anus or vagina on a man's penis when he doesnt want it or wants her to do it.", Id = 43, Slug = "REVERSERAPE",
                ImageAuthorName = "Lobizon Wachin", 
                ImageAuthorURL = "https://www.pixiv.net/en/users/27724191",
                ImageURL = "https://i.ibb.co/Lgkfqfy/reverserape.jpg"},
            new Tag() { Name = "Rimjob", Description = "Licking ass is never a good idea, butt fuck it...", Id = 44, Slug = "RIMJOB",
                ImageAuthorName = "Glazen",
                ImageAuthorURL = "https://www.pixiv.net/en/users/18727858",
                ImageURL = "https://i.ibb.co/7yCHJq3/rimjob.jpg"},
            new Tag() { Name = "Scat", Description = "To chow down on large amounts of big, hot, shit.", Id = 45, Slug = "SCAT",
                ImageAuthorName = "浅賀葵＠カーディン発売中", 
                ImageAuthorURL = "https://www.pixiv.net/en/users/233417", 
                ImageURL = "https://i.ibb.co/7vMvn70/scat.jpg"},
            new Tag() { Name = "School Girl", Description = "Girls that are still in school, or grown ass woman wearing school uniforms. It's hot either way.", Id = 46, Slug = "SCHOOLGIRL",
                ImageAuthorName = "J_Artur",
                ImageAuthorURL = "https://www.pixiv.net/en/users/67722448",
                ImageURL = "https://i.ibb.co/P12WFTh/schoolgirl.jpg"},
            new Tag() { Name = "Shota", Description = "Young boys got horny too!", Id = 47, Slug = "SHOTA",
                ImageAuthorName = "BRLL", 
                ImageAuthorURL = "https://www.pixiv.net/en/users/1704605",
                ImageURL = "https://i.ibb.co/f29KFK4/shota.jpg"},
            new Tag() { Name = "Softcore", Description = "Pornographic material that does not show penetration, genitalia, or actual sexual activity, opposite of hardcore.", Id = 48, Slug = "SOFTCORE",
                ImageAuthorName = "✎Hachi-nee⁀➷",
                ImageAuthorURL = "https://www.pixiv.net/en/users/72741927",
                ImageURL = "https://i.ibb.co/sPWcY2S/softcore.png"},
            new Tag() { Name = "Swimsuit", Description = "Anime chicks in a swimsuit. A tight swimsuit.", Id = 49, Slug = "SWIMSUIT",
                ImageAuthorName = "shift",
                ImageAuthorURL = "https://www.pixiv.net/en/users/44033958",
                ImageURL = "https://i.ibb.co/CJKr1G2/swimsuit.png"},
            new Tag() { Name = "Teacher", Description = "Someone who teaches things like math, reading, history, and sex.", Id = 50, Slug = "TEACHER",
                ImageAuthorName = "雪白牙斬",
                ImageAuthorURL = "https://www.pixiv.net/en/users/674329",
                ImageURL = "https://i.ibb.co/9Vp3Bnm/teacher.jpg"},
            new Tag() { Name = "Tentacle", Description = "A large rapeing arm of an octopus who rapes innocent japanese school children and is feared by sailors", Id = 51, Slug = "TENTACLE",
                ImageAuthorName = "すみっこ太郎",
                ImageAuthorURL = "https://www.pixiv.net/en/users/74843344", 
                ImageURL = "https://i.ibb.co/SfGQj3r/tentacle.jpg"},
            new Tag() { Name = "Threesome", Description = "Like bringing a plus one to a 2 person party. Bul less awkward and more fun.", Id = 52, Slug = "THREESOME",
                ImageAuthorName = "ぷにぷにとうふ", 
                ImageAuthorURL = "https://www.pixiv.net/en/users/11044155",
                ImageURL = "https://i.ibb.co/6ZWfBXz/threesome.png"},
            new Tag() { Name = "Toys", Description = "Vibrators, dildos, and all kinds of Nintendo type of shit.", Id = 53, Slug = "TOYS",
                ImageAuthorName = "Sodomi Pigtail",
                ImageAuthorURL = "https://www.pixiv.net/en/users/45017726", 
                ImageURL = "https://i.ibb.co/rtLtF9R/toys.png"},
            new Tag() { Name = "Trap", Description = "A boy that is so feminine, he looks like a girl; often more attractive than other girls.", Id = 54, Slug = "TRAP",
                ImageAuthorName = "pruzhka", 
                ImageAuthorURL = "https://www.pixiv.net/en/users/7708546", 
                ImageURL = "https://i.ibb.co/wsBQfNV/trap.jpg"},
            new Tag() { Name = "Tsundere", Description = "We all know a tsundere chick. They hate you at first, but then in a bizarre turn of events, end up fucking you in a closet.", Id = 55, Slug = "TSUNDERE", 
                ImageAuthorName = "ddal",
                ImageAuthorURL = "https://www.pixiv.net/en/users/267137",
                ImageURL = "https://i.ibb.co/JFrWwKt/tsundere.jpg"},
            new Tag() { Name = "Ugly Bastard", Description = "Ugly and fat sometime old and sweaty, oily and gross, 60 percent are rich and they got a magical dic* if you got foke you will fall in love with them no matter how ugly they are.", Id = 56, Slug = "UGLYBASTARD", 
                ImageAuthorName = "NicoNicoSacolin",
                ImageAuthorURL = "https://www.pixiv.net/en/users/72303341", 
                ImageURL = "https://i.ibb.co/qgTRftn/uglybastard.png"},
            new Tag() { Name = "Uncensored", Description = "I CAN SEE AGAIN!", Id = 57, Slug = "UNCENSORED", 
                ImageAuthorName = "Akane Sak", 
                ImageAuthorURL = "https://www.pixiv.net/en/users/17090114", 
                ImageURL = "https://i.ibb.co/fdTQjgy/uncensored.jpg"},
            new Tag() { Name = "Vanilla", Description = "Just straight up normal, conventional sex. The kind the Church approves.", Id = 58, Slug = "VANILLA", 
                ImageAuthorName = "yuna", 
                ImageAuthorURL = "https://www.pixiv.net/en/users/434305",
                ImageURL = "https://i.ibb.co/mvwwNgj/vanilla.jpg"},
            new Tag() { Name = "Virgin", Description = "Someone untouched....until now.", Id = 59, Slug = "VIRGIN", 
                ImageAuthorName = "とまとじごく", 
                ImageAuthorURL = "https://www.pixiv.net/en/users/1735727", 
                ImageURL = "https://i.ibb.co/p2JWyxb/virgin.jpg"},
            new Tag() { Name = "Watersports", Description = "Similar to water boarding but with the sense of pleasure instead of torture. It consists of massive amounts of urine to ones face with plastic wrap covering it so the recipient can feel the warmth of urine and also experience the feeling of drowning as well. Similar to a hot carl.", Id = 60, Slug = "WATERSPORTS", 
                ImageAuthorName = "Stinkek", 
                ImageAuthorURL = "https://www.pixiv.net/en/users/36796715",
                ImageURL = "https://i.ibb.co/DKSvJft/watersports.png"},
            new Tag() { Name = "X-Ray", Description = "An inside view of the action.", Id = 61, Slug = "XRAY",
                ImageAuthorName = "wweed",
                ImageAuthorURL = "https://www.pixiv.net/en/users/4338019", 
                ImageURL = "https://i.ibb.co/Lz4tKvN/xray.png"},
            new Tag() { Name = "Yaoi", Description = "The opposite of Yuri.", Id = 62, Slug = "YAOI",
                ImageAuthorName = "翔太",
                ImageAuthorURL = "https://www.pixiv.net/en/users/426846", 
                ImageURL = "https://i.ibb.co/PQz3P1k/yaoi.png"},
            new Tag() { Name = "Yuri", Description = "The opposite of Yaoi.", Id = 63, Slug = "YURI", 
                ImageAuthorName = "CodeNamePibu", 
                ImageAuthorURL = "https://www.pixiv.net/en/users/15311515", 
                ImageURL = "https://i.ibb.co/t8nKrQ8/yuri.png"}
            //new Tag() { Name = "", Description = "", Id = "", Slug = ""}
        };

        public static Tag FromSlugToTag(string slug)
        {
            foreach (Tag tag in tags)
            {
                if (slug.ToUpper() == tag.Slug.ToUpper())
                {
                    return tag;
                }
            }
            return null;
        }

        public static Tag FromNameToTag(string name)
        {
            foreach (Tag tag in tags)
            {
                if (name.ToUpper() == tag.Name.ToUpper())
                {
                    return tag;
                }
            }
            return null;
        }

        public static Tag FromNameSlugToTag(string str)
        {
            foreach (Tag tag in tags)
            {
                if (str.ToUpper() == tag.Name.ToUpper()) return tag;
                if (str.ToUpper() == tag.Slug.ToUpper()) return tag;
            }
            return null;
        }
    }

    public class Brands
    {
        public static List<Brand> brands = new List<Brand>()
        {
            new Brand() { Name = "@ OZ", Slug = "@OZ", Id = 1},
            new Brand() { Name = "37c-Binetsu", Slug = "37C", Id = 2},
            new Brand() { Name = "Almond Collective", Slug = "ALMONDCOLLECTIVE", Id = 3},
            new Brand() { Name = "Armour", Slug = "ARMOUR", Id = 4},
            new Brand() { Name = "Animac", Slug = "ANIMAC", Id = 5},
            new Brand() { Name = "Arms", Slug = "ARMS", Id = 6},
            new Brand() { Name = "Blue Eyes", Slug = "BLUE EYES", Id = 7},
            new Brand() { Name = "Bootleg", Slug = "BOOTLEG", Id = 8},
            new Brand() { Name = "BreakBottle", Slug = "BREAKBOTTLE", Id = 9},
            new Brand() { Name = "BugBug", Slug = "BUGBUG", Id = 10},
            new Brand() { Name = "Bunnywalker", Slug = "BUNNYWALKER", Id = 11},
            new Brand() { Name = "Celeb", Slug = "CELEB", Id = 12},
            new Brand() { Name = "Central Park Media", Slug = "CENTRALPARKMEDIA", Id = 13},
            new Brand() { Name = "ChiChinoya", Slug = "CHICHINOYA", Id = 14},
            new Brand() { Name = "Circle Tribute", Slug = "CIRCLETRIBUTE", Id = 15},
            new Brand() { Name = "CoCoans", Slug = "COCOANS", Id = 16},
            new Brand() { Name = "Collaboration Works", Slug = "COLLABORATIONWORKS", Id = 17},
            new Brand() { Name = "Cosmus", Slug = "COSMUS", Id = 18},
            new Brand() { Name = "Cranberry", Slug = "CRANBERRY", Id = 19},
            new Brand() { Name = "Crimson", Slug = "CRIMSON", Id = 20},
            new Brand() { Name = "D3", Slug = "D3", Id = 21},
            new Brand() { Name = "Daiei", Slug = "DAIEI", Id = 22},
            new Brand() { Name = "demodemon", Slug = "DEMODEMON", Id = 23},
            new Brand() { Name = "Digital Works", Slug = "DIGITALWORKS", Id = 24},
            new Brand() { Name = "Discovery", Slug = "DISCOVERY", Id = 25},
            new Brand() { Name = "EBIMARU-DO", Slug = "EBIMARUDO", Id = 26},
            new Brand() { Name = "Echo", Slug = "ECHO", Id = 27},
            new Brand() { Name = "ECOLONUN", Slug = "ECOLONUN", Id = 28},
            new Brand() { Name = "Edge", Slug = "EDGE", Id = 29},
            new Brand() { Name = "Erozuki", Slug = "EROZUKI", Id = 30},
            new Brand() { Name = "evee", Slug = "EVEE", Id = 31},
            new Brand() { Name = "FINAL FUCK 7", Slug = "FINALFUCK7", Id = 32},
            new Brand() { Name = "Five Ways", Slug = "FIVEWAYS", Id = 33},
            new Brand() { Name = "Front Line", Slug = "FRONTLINE", Id = 34},
            new Brand() { Name = "fruit", Slug = "FRUIT", Id = 35},
            new Brand() { Name = "GOLD BEAR", Slug = "GOLDBEAR", Id = 36},
            new Brand() { Name = "gomasioken", Slug = "GOMASIOKEN", Id = 37},
            new Brand() { Name = "Green Bunny", Slug = "GREENBUNNY", Id = 38},
            new Brand() { Name = "Hoods Entertainment", Slug = "HOODSENTERTAINMENT", Id = 39},
            new Brand() { Name = "Hot Bear", Slug = "HOTBEAR", Id = 40},
            new Brand() { Name = "Hykobo", Slug = "HYKOBO", Id = 41},
            new Brand() { Name = "Jellyfish", Slug = "JELLYFISH", Id = 42},
            new Brand() { Name = "Jumondo", Slug = "JUMONDO", Id = 43},
            new Brand() { Name = "kate_sai", Slug = "KATESAI", Id = 44},
            new Brand() { Name = "KENZsoft", Slug = "KENZSOFT", Id = 45},
            new Brand() { Name = "King Bee", Slug = "KINGBEE", Id = 46},
            new Brand() { Name = "Knack", Slug = "KNACK", Id = 47},
            new Brand() { Name = "Kuril", Slug = "KURIL", Id = 48},
            new Brand() { Name = "L", Slug = "L", Id = 49},
            new Brand() { Name = "Lemon Heart", Slug = "LEMONHEART", Id = 50},
            new Brand() { Name = "Lilix", Slug = "LILIX", Id = 51},
            new Brand() { Name = "Lune Pictures", Slug = "LUNEPICTURES", Id = 52},
            new Brand() { Name = "Magic Bus", Slug = "MAGICBUS", Id = 53},
            new Brand() { Name = "Magin Label", Slug = "MAGINLABEL", Id = 54},
            new Brand() { Name = "Marigold", Slug = "MARIGOLD", Id = 55},
            new Brand() { Name = "Mary Jane", Slug = "MARYJANE", Id = 56},
            new Brand() { Name = "Media Blasters", Slug = "MEDiABLASTERS", Id = 57},
            new Brand() { Name = "MediaBank", Slug = "MEDIABANK", Id = 58},
            new Brand() { Name = "Moon Rock", Slug = "MOONROCK", Id = 59},
            new Brand() { Name = "Moonstone Cherry", Slug = "MOONSTONECHERRY", Id = 60},
            new Brand() { Name = "MSPictures", Slug = "MSPICTURES", Id = 61},
            new Brand() { Name = "Nihikime no Dozeu", Slug = "NIHIKIMENODOZEU", Id = 62},
            new Brand() { Name = "NuTech Digital", Slug = "NUTECHDIGITAL", Id = 63},
            new Brand() { Name = "Pashmina", Slug = "PASHMINA", Id = 64},
            new Brand() { Name = "Pink Pineapple", Slug = "PINKPINEAPPLE", Id = 65},
            new Brand() { Name = "Pinkbell", Slug = "PINKBELL", Id = 66},
            new Brand() { Name = "Pixy Soft", Slug = "PIXYSOFT", Id = 67},
            new Brand() { Name = "Pocorno Premium", Slug = "POCORNOPREMIUM", Id = 68},
            new Brand() { Name = "PoRO", Slug = "PORO", Id = 69},
            new Brand() { Name = "Project No.9", Slug = "PROJECTNO9", Id = 70},
            new Brand() { Name = "Queen Bee", Slug = "QUEENBEE", Id = 71},
            new Brand() { Name = "Rabbit Gate", Slug = "RABBITGATE", Id = 72},
            new Brand() { Name = "sakamotoJ", Slug = "SAKAMOTOJ", Id = 73},
            new Brand() { Name = "SANDWICHWORKS", Slug = "SANDWICHWORKS", Id = 74},
            new Brand() { Name = "Schoolzone", Slug = "SCHOOLZONE", Id = 75},
            new Brand() { Name = "seismic", Slug = "SEISMIC", Id = 76},
            new Brand() { Name = "SELFISH", Slug = "SELFISH", Id = 77},
            new Brand() { Name = "Seven", Slug = "SEVEN", Id = 78},
            new Brand() { Name = "Shadow Prod. Co.", Slug = "SHADOWPRODCO", Id = 79},
            new Brand() { Name = "Shinyusha", Slug = "SHINYUSHA", Id = 80},
            new Brand() { Name = "Showten", Slug = "SHOWTEN", Id = 81},
            new Brand() { Name = "Soft on Demand", Slug = "SOFTONDEMAND", Id = 82},
            new Brand() { Name = "STARGATE3D", Slug = "STARGATE3D", Id = 83},
            new Brand() { Name = "Studio 9 Maiami", Slug = "STUDIO9MAIAMI", Id = 84},
            new Brand() { Name = "Studio Akai Shohosen", Slug = "STUDIOAKAISHOHOSEN", Id = 85},
            new Brand() { Name = "Studio Deen", Slug = "STUDIODEEN", Id = 86},
            new Brand() { Name = "Studio Fantasia", Slug = "STUDIOFANTASIA", Id = 87},
            new Brand() { Name = "Studio FOW", Slug = "STUDIOFOW", Id = 88},
            new Brand() { Name = "studio GGB", Slug = "STUDIOGGB", Id = 89},
            new Brand() { Name = "Studio Zealot", Slug = "STUDIOZEALOT", Id = 90},
            new Brand() { Name = "Suzuki Mirano", Slug = "SUZUKIMIRANO", Id = 91},
            new Brand() { Name = "SYLD", Slug = "SYLD", Id = 92},
            new Brand() { Name = "T-Rex", Slug = "TREX", Id = 93},
            new Brand() { Name = "TOHO", Slug = "TOHO", Id = 94},
            new Brand() { Name = "Toranoana", Slug = "TORANOANA", Id = 95},
            new Brand() { Name = "TYS Work", Slug = "TYSWORK", Id = 96},
            new Brand() { Name = "Umemaro-3D", Slug = "UMEMARO3D", Id = 97},
            new Brand() { Name = "Union Cho", Slug = "UNIONCHO", Id = 98},
            new Brand() { Name = "Valkyria", Slug = "VALKYRIA", Id = 99},
            new Brand() { Name = "Vanilla", Slug = "VANILLA", Id = 100},
            new Brand() { Name = "White Bear", Slug = "WHITEBEAR", Id = 101},
            new Brand() { Name = "X City", Slug = "XCITY", Id = 102},
            new Brand() { Name = "Y.O.U.C.", Slug = "YOUC", Id = 103},
            new Brand() { Name = "yosino", Slug = "YOSINO", Id = 104},
            new Brand() { Name = "ZIZ", Slug = "ZIZ", Id = 105}
        };

        public static Brand FromSlugToBrand(string slug)
        {
            foreach (Brand brand in brands)
            {
                if (slug.ToUpper() == brand.Slug.ToUpper())
                {
                    return brand;
                }
            }
            return null;
        }

        public static Brand FromNameToBrand(string name)
        {
            foreach (Brand brand in brands)
            {
                if (name.ToUpper() == brand.Name.ToUpper())
                {
                    return brand;
                }
            }
            return null;
        }

        public static Brand FromNameSlugToBrand(string str)
        {
            foreach (Brand brand in brands)
            {
                if (str.ToUpper() == brand.Name.ToUpper()) return brand;
                if (str.ToUpper() == brand.Slug.ToUpper()) return brand;
            }
            return null;
        }
    }

    public class Orderby
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Id { get; set; }
    }

    public class OrderBys
    {
        public static List<Orderby> orderbys = new List<Orderby>()
        {
            new Orderby() { Name = "created_at_unix", Slug = "CREATED", Id = 1 },
            new Orderby() { Name = "released_at_unix", Slug = "RELEASE", Id = 2 },
            new Orderby() { Name = "likes", Slug = "LIKES", Id = 3 },
            new Orderby() { Name = "views", Slug = "VIEWS", Id = 4 },
            new Orderby() { Name = "title_sortable", Slug = "TITLE", Id = 5 },
        };

        public static Orderby FromSlugToOrderby(string slug)
        {
            foreach (Orderby orderby in orderbys)
            {
                if (slug.ToUpper() == orderby.Slug.ToUpper())
                {
                    return orderby;
                }
            }
            return null;
        }

        public static Orderby FromNameToOrderby(string name)
        {
            foreach (Orderby orderby in orderbys)
            {
                if (name.ToUpper() == orderby.Name.ToUpper())
                {
                    return orderby;
                }
            }
            return null;
        }

        public static Orderby FromNameSlugToOrderby(string str)
        {
            foreach (Orderby orderby in orderbys)
            {
                if (str.ToUpper() == orderby.Name.ToUpper()) return orderby;
                if (str.ToUpper() == orderby.Slug.ToUpper()) return orderby;
            }
            return null;
        }
    }

    public class TagMode
    {
        public string Name { get; set; }

        public string Slug { get; set; }

        public int Id { get; set; }
    }

    public class TagModes
    {
        public static List<TagMode> tagmodes = new List<TagMode>()
        {
            new TagMode() { Name = "AND", Slug = "&&", Id = 1 },
            new TagMode() { Name = "OR", Slug = "||", Id = 2 }
        };

        public static TagMode FromSlugToTagMode(string slug)
        {
            foreach (TagMode tagmode in tagmodes)
            {
                if (slug.ToUpper() == tagmode.Slug.ToUpper())
                {
                    return tagmode;
                }
            }
            return null;
        }

        public static TagMode FromNameToTagMode(string name)
        {
            foreach (TagMode tagmode in tagmodes)
            {
                if (name.ToUpper() == tagmode.Name.ToUpper())
                {
                    return tagmode;
                }
            }
            return null;
        }

        public static TagMode FromNameSlugToTagMode(string str)
        {
            foreach (TagMode tagmode in tagmodes)
            {
                if (str.ToUpper() == tagmode.Name.ToUpper()) return tagmode;
                if (str.ToUpper() == tagmode.Slug.ToUpper()) return tagmode;
            }
            return null;
        }
    }

    public class Ordering
    {
        public string Name { get; set; }

        public string Slug { get; set; }

        public int Id { get; set; }
    }

    public class Orders
    {
        public static List<Ordering> orders = new List<Ordering>()
        {
            new Ordering() { Name = "asc", Slug = "ASCENDING", Id = 1 },
            new Ordering() { Name = "desc", Slug = "DESCENDING", Id = 2 }
        };

        public static Ordering FromSlugToOrder(string slug)
        {
            foreach (Ordering order in orders)
            {
                if (slug.ToUpper() == order.Slug.ToUpper())
                {
                    return order;
                }
            }
            return null;
        }

        public static Ordering FromNameToOrder(string name)
        {
            foreach (Ordering order in orders)
            {
                if (name.ToUpper() == order.Name.ToUpper())
                {
                    return order;
                }
            }
            return null;
        }

        public static Ordering FromNameSlugToOrder(string str)
        {
            foreach (Ordering order in orders)
            {
                if (str.ToUpper() == order.Name.ToUpper()) return order;
                if (str.ToUpper() == order.Slug.ToUpper()) return order;
            }
            return null;
        }
    }

    public partial class Hentai
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long Id { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("titles", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Titles { get; set; }

        [JsonProperty("slug", NullValueHandling = NullValueHandling.Ignore)]
        public string Slug { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("views", NullValueHandling = NullValueHandling.Ignore)]
        public long Views { get; set; }

        [JsonProperty("interests", NullValueHandling = NullValueHandling.Ignore)]
        public long Interests { get; set; }

        [JsonProperty("poster_url", NullValueHandling = NullValueHandling.Ignore)]
        public string PosterUrl { get; set; }

        [JsonProperty("cover_url", NullValueHandling = NullValueHandling.Ignore)]
        public string CoverUrl { get; set; }

        [JsonProperty("brand", NullValueHandling = NullValueHandling.Ignore)]
        public string Brand { get; set; }

        [JsonProperty("brand_id", NullValueHandling = NullValueHandling.Ignore)]
        public long BrandId { get; set; }

        [JsonProperty("is_censored", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsCensored { get; set; }

        [JsonProperty("rating", NullValueHandling = NullValueHandling.Ignore)]
        public long Rating { get; set; }

        [JsonProperty("likes", NullValueHandling = NullValueHandling.Ignore)]
        public long Likes { get; set; }

        [JsonProperty("dislikes", NullValueHandling = NullValueHandling.Ignore)]
        public long Dislikes { get; set; }

        [JsonProperty("downloads", NullValueHandling = NullValueHandling.Ignore)]
        public long Downloads { get; set; }

        [JsonProperty("monthly_rank", NullValueHandling = NullValueHandling.Ignore)]
        public long MonthlyRank { get; set; }

        [JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Tags { get; set; }

        [JsonProperty("created_at", NullValueHandling = NullValueHandling.Ignore)]
        public long CreatedAt { get; set; }

        [JsonProperty("released_at", NullValueHandling = NullValueHandling.Ignore)]
        public long ReleasedAt { get; set; }


        public Hentai Empty()
        {
            return new Hentai();
        }
    }

    public partial class Hentai
    {
        public static Hentai FromJson(string json) => JsonConvert.DeserializeObject<Hentai>(json, Converter.Settings);
    }

    public static class HSerialize
    {
        public static string ToJson(this Hentai self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public partial class HanimeJson
    {
        [JsonProperty("page")]
        public long Page { get; set; }

        [JsonProperty("nbPages")]
        public long NbPages { get; set; }

        [JsonProperty("nbHits")]
        public long NbHits { get; set; }

        [JsonProperty("hitsPerPage")]
        public long HitsPerPage { get; set; }

        [JsonProperty("hits")]
        public string Hits { get; set; }
    }

    public partial class HanimeJson
    {
        public static HanimeJson FromJson(string json) => JsonConvert.DeserializeObject<HanimeJson>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this HanimeJson self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
