using Npgsql;
using DSharpPlus; 
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace DevBot.Services {
    
    class Psql {

        private readonly IConfigurationRoot _settings;
        private readonly DiscordClient _client;

        public Psql (IConfigurationRoot settings, DiscordClient client) {
            this._settings = settings;
            this._client = client;
        }

        

    }


}

