using Npgsql;
using DSharpPlus; 
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Serilog;
using System.Data.Common;
using System.Collections.Generic;

namespace DevBot.Services {
    
    class DbService {

        private readonly IConfigurationRoot _settings;
        private readonly DiscordClient _client;
        private readonly string _connectionString;
        private NpgsqlConnection _connection;

        private Dictionary<string, Dictionary<long, string>> _guildSettingsCache = new Dictionary<string, Dictionary<long, string>>();

        public DbService (IConfigurationRoot settings, DiscordClient client) {
            this._settings = settings;
            this._client = client;

            _connectionString = new NpgsqlConnectionStringBuilder() {
                Host = _settings.GetSection("postgres:host").Value,
                Username = _settings.GetSection("postgres:username").Value,
                Password = _settings.GetSection("postgres:passwd").Value,
                Database = _settings.GetSection("postgres:database").Value
            }.ConnectionString;
        }
        
        public async Task BuildCaches() {
            await using (var cmd = new NpgsqlCommand("SELECT * FROM guildsettings", _connection)) {
                await using (var reader = await cmd.ExecuteReaderAsync()) {

                    while (await reader.ReadAsync()) {
                        long gid = reader.GetInt64(reader.GetOrdinal("guild_id"));
                        string setting = reader.GetString(reader.GetOrdinal("guild_setting"));
                        string value = reader.GetString(reader.GetOrdinal("guild_value"));

                        if (!_guildSettingsCache.ContainsKey(setting)) {
                            _guildSettingsCache[setting] = new Dictionary<long, string>();
                        }

                        _guildSettingsCache[setting][gid] = value;
                    }
                }
            }

        }

        public async Task CreateConnection() {
            this._connection = new NpgsqlConnection(_connectionString);
            await this._connection.OpenAsync();
        }

        public async Task RecordId(long userid) {
            await using (var cmd = new NpgsqlCommand("INSERT INTO userdata (user_id) VALUES ($1)", this._connection)) {
                cmd.Parameters.AddWithValue(userid);
                await cmd.ExecuteNonQueryAsync();
            } 
        }

        public string FetchLocale(ulong guildId) {
            
            return _guildSettingsCache["locale"][(long)guildId];

        }

        public async Task SetLocale(ulong guildId, string locale) {

            await using (var cmd = new NpgsqlCommand("UPDATE guildsettings SET guild_value = $1 WHERE guild_setting = 'locale' AND guild_id = $2")) {
                cmd.Parameters.AddWithValue(locale);
                cmd.Parameters.AddWithValue((long)guildId);
                await cmd.ExecuteNonQueryAsync();
            }

            _guildSettingsCache["locale"][(long)guildId] = locale;
        }

    }


}

