using DSharpPlus;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog;


namespace DevBot.Services {
    
    class LocaleService {

        private readonly IConfigurationRoot _settings;
        private readonly DiscordClient _client;
        private readonly DbService _dbService;

        private readonly Dictionary<string, IConfigurationRoot> _locales = new Dictionary<string, IConfigurationRoot>();
        
        public LocaleService(DbService dbservice, DiscordClient client, IConfigurationRoot settings) {
            
            _settings = settings;
            _client = client;
            _dbService = dbservice;

        }

        public void BuildLocales() {

            string[] locales = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "locales"), "*.json");

            foreach (string locale in locales) {
                string localename = locale.Split("\\locales\\")[1].Split(".json")[0];
                IConfigurationRoot localeObject = new ConfigurationBuilder()
                                                .AddJsonFile(locale)
                                                .Build();
                _locales.Add(localename, localeObject);
            }

        }

        public string TranslatableText(string key, ulong guildid) {

            string guildlocalename = _dbService.FetchLocale(guildid);

            IConfigurationRoot guildLocale = _locales[guildlocalename];

            return guildLocale.GetSection(key).Value;
    
        }

    }

}