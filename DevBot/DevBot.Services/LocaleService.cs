using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog;


namespace DevBot.Services {
    
    enum Language
    {
        [ChoiceName("English")]
        english,
        [ChoiceName("Spanish")]
        spanish,
        [ChoiceName("French")]
        french
    }

    class LocaleService {

        class NoLocaleTextSet : Exception {
            public NoLocaleTextSet(string message) : base(message) {}
        }
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
                string localename = locale.Split("/locales/")[1].Split(".json")[0];
                IConfigurationRoot localeObject = new ConfigurationBuilder()
                                                .AddJsonFile(locale)
                                                .Build();
                _locales.Add(localename, localeObject);
            }

        }

        public string TranslatableText(string key, ulong userId) {

            string userlocalename = _dbService.FetchLocale(userId);

            IConfigurationRoot userLocale = _locales[userlocalename];

            string translatedText = userLocale.GetSection(key).Value;

            if (translatedText is null) {
                throw(new NoLocaleTextSet($"{key} has no locale text set for language: {userlocalename}"));
            }

            return translatedText;
    
        }

    }

}