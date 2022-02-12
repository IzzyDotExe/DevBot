using DevBot;
using DSharpPlus;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Serilog;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace DevBot.Services.API {

    class StackExchangeOwner {

        public int accountId {get; internal set;}
        public int reputation {get; internal set;}
        public int userId {get; internal set;}
        public string userType {get; internal set;}
        public string profileImage {get; internal set;}
        public string displayName {get; internal set;}
        public string link {get; internal set;}


        public StackExchangeOwner(JsonElement jData) {

            accountId = jData.GetProperty("account_id").GetInt32();
            reputation = jData.GetProperty("reputation").GetInt32();
            userId = jData.GetProperty("user_id").GetInt32();
            userType = jData.GetProperty("user_type").GetString();
            profileImage = jData.GetProperty("profile_image").GetString();
            displayName = jData.GetProperty("display_name").GetString();
            link = jData.GetProperty("link").GetString();

        }
    }

    class StackExchangeQuestion {

        public List<string> tags {get; internal set;}
        public StackExchangeOwner owner {get; internal set;}
        public bool isAnswered {get; internal set;}
        public int viewCount {get; internal set;}
        public int acceptedAnswerId {get; internal set;}
        public int answerCount {get; internal set;}
        public int score {get; internal set;}
        public DateTime lastActivityDate {get; internal set;}
        public DateTime creationDate {get; internal set;}
        public DateTime lastEditDate {get; internal set;}
        public int questionId {get; internal set;}
        public string link {get; internal set;}
        public string title {get; internal set;}


        public StackExchangeQuestion(JsonElement jData) {

            tags = new List<string>();

            
            foreach (var item in jData.GetProperty("tags").EnumerateArray()) {
                tags.Add(item.GetString());
            }

            owner = new StackExchangeOwner(jData.GetProperty("owner"));

            isAnswered = jData.GetProperty("is_answered").GetBoolean();
            viewCount = jData.GetProperty("view_count").GetInt32();

            try {
                acceptedAnswerId = jData.GetProperty("accepted_answer_id").GetInt32();
            } catch (KeyNotFoundException ex) {

            }
            
            answerCount = jData.GetProperty("answer_count").GetInt32();
            score = jData.GetProperty("score").GetInt32();

            lastActivityDate = UnixTimeStampToDateTime(jData.GetProperty("last_activity_date").GetDouble());
            creationDate = UnixTimeStampToDateTime(jData.GetProperty("creation_date").GetDouble());
            try {
                lastEditDate = UnixTimeStampToDateTime(jData.GetProperty("last_edit_date").GetDouble());
            } catch (KeyNotFoundException ex) {
                
            }
        
            questionId = jData.GetProperty("question_id").GetInt32();
            link = jData.GetProperty("link").GetString();
            title = jData.GetProperty("title").GetString();

        }

        public static DateTime UnixTimeStampToDateTime( double unixTimeStamp )
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
            return dateTime;
        }

    }

    class StackExchangeService {
        private readonly IConfigurationRoot _settings;
        private readonly HttpClient webClient;

        private readonly string[] topTagsStackOverflow = new string[] {"javascript",
                                                          " js", 
                                                          "python",
                                                          ".py",
                                                          "discord", 
                                                          " java", 
                                                          "c#", 
                                                          " cs",
                                                          ".net", 
                                                          "php", 
                                                          "android", 
                                                          "html", 
                                                          "jquery", 
                                                          " c++", 
                                                          " cpp", 
                                                          "css", 
                                                          " ios", 
                                                          "mysql", 
                                                          "sql",  
                                                          "rust", 
                                                          "node.js", 
                                                          " node", 
                                                          "arrays", 
                                                          " c ", 
                                                          "asp.net",
                                                          "reactjs",
                                                          " json", 
                                                          " ruby",
                                                          "ruby-on-rails",
                                                          "sql-server",
                                                          " swift",
                                                          "python-3.x",
                                                          "objective-c",
                                                          "django",
                                                          "angular",
                                                          "angularjs",
                                                          "excel",
                                                          "regex",
                                                          "pandas",
                                                          "iphone",
                                                          "ajax",
                                                          "linux"};

        private readonly string[] topTagsAskUbuntu = new string[] {"boot",
                                                                   "networking",
                                                                   "command-line",
                                                                   "14.04",
                                                                   "16.04",
                                                                   "drivers",
                                                                   "dual-boot",
                                                                   "12.04",
                                                                   "18.04",
                                                                   "apt",
                                                                   "server",
                                                                   "partitioning",
                                                                   "20.04",
                                                                   "wireless",
                                                                   "grub2",
                                                                   "nvidia",
                                                                   "bash",
                                                                   "gnome",
                                                                   "unity",
                                                                   "package-management",
                                                                   "system-installation",
                                                                   "upgrade",
                                                                   "sound",
                                                                   "software-installation",
                                                                   "usb",
                                                                   "kernel",
                                                                   "permissions",
                                                                   "mount",
                                                                   "installation",
                                                                   "scripts",
                                                                   "updates",
                                                                   "graphics",
                                                                   "keyboard",
                                                                   "ssh",
                                                                   "kubuntu",
                                                                   "virtualbox"};
        public StackExchangeService(IConfigurationRoot settings) {
            _settings = settings;
            webClient = new HttpClient();
        }

        public List<string> parseforStackTags(string input) {
            List<string> tags = new List<string>();
            foreach (string tag in topTagsStackOverflow) {
                if (input.Contains(tag)) {
                    tags.Add(tag);
                }
            }
            return tags;
        }

        public List<string> parseforUbuntuTags(string input) {
            List<string> tags = new List<string>();
            foreach (string tag in topTagsAskUbuntu) {
                if (input.Contains(tag)) {
                    tags.Add(tag);
                }
            }
            return tags;
        }

        public async Task<List<StackExchangeQuestion>> SearchUbuntu(string searchQuery) {
            
            try {

                var detectedtags = parseforUbuntuTags(searchQuery);
                var tagsString = new StringBuilder();

                foreach (string tag in detectedtags) {
            
                    if (!tagsString.ToString().Contains(tag)) {
                        tagsString.Append($"{tag};");
                    }
                    
                }
                try {
                    tagsString.Remove(tagsString.Length-1, 1);
                } catch (ArgumentOutOfRangeException ex) {

                }
                
                string clientKey = _settings.GetSection("stack:clientkey").Value;

                string request_url = $"https://api.stackexchange.com/2.3/search/advanced?tagged={tagsString.ToString()}&key={clientKey}&pagesize=5&site=askubuntu&q={searchQuery}";

                byte[] stringTask = await webClient.GetByteArrayAsync(request_url);
                
                string jsonData = Unzip(stringTask);
                
                JsonDocument jsonDoc = JsonDocument.Parse(jsonData);
                JsonElement root = jsonDoc.RootElement;

                var items = root.GetProperty("items");
                Log.Logger.Information($"{root.GetProperty("quota_remaining").GetInt32().ToString()} Remaining requests");
                var itemArray = items.EnumerateArray();

                List<StackExchangeQuestion> questions = new List<StackExchangeQuestion>();

                foreach (var item in itemArray) {
                    questions.Add(new StackExchangeQuestion(item));
                }

                return questions;


            } catch (Exception err) {
                Log.Logger.Error(err.ToString());
                return new List<StackExchangeQuestion>();
            }

        }

        public async Task<List<StackExchangeQuestion>> SearchAnswers(string searchQuery) {
            
            try {

                var detectedtags = parseforStackTags(searchQuery);
                var tagsString = new StringBuilder();

                foreach (string tag in detectedtags) {
                    if (tag == ".py") {
                        tagsString.Append("python;");
                    } 
                    else if (!tagsString.ToString().Contains(tag)) {
                        tagsString.Append($"{tag};");
                    }
                    
                }
                try {
                    tagsString.Remove(tagsString.Length-1, 1);
                } catch (ArgumentOutOfRangeException ex) {

                }
                
                string clientKey = _settings.GetSection("stack:clientkey").Value;

                string request_url = $"https://api.stackexchange.com/2.3/search/advanced?tagged={tagsString.ToString()}&key={clientKey}&pagesize=5&site=stackoverflow&q={searchQuery}";

                byte[] stringTask = await webClient.GetByteArrayAsync(request_url);
                
                string jsonData = Unzip(stringTask);
                
                JsonDocument jsonDoc = JsonDocument.Parse(jsonData);
                JsonElement root = jsonDoc.RootElement;

                var items = root.GetProperty("items");
                Log.Logger.Information($"{root.GetProperty("quota_remaining").GetInt32().ToString()} Remaining requests");
                var itemArray = items.EnumerateArray();

                List<StackExchangeQuestion> questions = new List<StackExchangeQuestion>();

                foreach (var item in itemArray) {
                    questions.Add(new StackExchangeQuestion(item));
                }

                return questions;


            } catch (Exception err) {
                Log.Logger.Error(err.ToString());
                return new List<StackExchangeQuestion>();
            }

        }

        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }
  
        public static string Unzip(byte[] bytes) {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream()) {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress)) {
                    //gs.CopyTo(mso);
                    CopyTo(gs, mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }
    
    }

}