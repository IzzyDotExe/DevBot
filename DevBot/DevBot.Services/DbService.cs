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

        private Dictionary<string, Dictionary<long, string>> _userSettingsCache = new Dictionary<string, Dictionary<long, string>>();

        public DbService (IConfigurationRoot settings, DiscordClient client) {
            this._settings = settings;
            this._client = client;

            _connectionString = new NpgsqlConnectionStringBuilder() {
                Host = _settings.GetSection("postgres:host").Value,
                Username = _settings.GetSection("postgres:username").Value,
                Password = _settings.GetSection("postgres:passwd").Value,
                Database = _settings.GetSection("postgres:database").Value
            }.ConnectionString;

            Log.Logger.Information("Database service loaded!");
        }
        
        public async Task BuildCaches() {
            await using (var cmd = new NpgsqlCommand("SELECT * FROM usersettings", _connection)) {
                await using (var reader = await cmd.ExecuteReaderAsync()) {

                    while (await reader.ReadAsync()) {
                        long gid = reader.GetInt64(reader.GetOrdinal("user_id"));
                        string setting = reader.GetString(reader.GetOrdinal("user_setting"));
                        string value = reader.GetString(reader.GetOrdinal("user_value"));

                        if (!_userSettingsCache.ContainsKey(setting)) {
                            _userSettingsCache[setting] = new Dictionary<long, string>();
                        }

                        _userSettingsCache[setting][gid] = value;
                    }
                }
            }

        }

        public async Task CreateConnection() {
            this._connection = new NpgsqlConnection(_connectionString);
            await this._connection.OpenAsync();
        }

        public string FetchLocale(ulong userId) {
            
            try {
                return _userSettingsCache["locale"][(long)userId];
            } catch (KeyNotFoundException) {
                return "english";
            }
            

        }

        public async Task SetLocale(ulong userId, string locale) {

            await using (var cmd = new NpgsqlCommand("INSERT INTO usersettings (user_id, user_setting, user_value) VALUES ($1, 'locale', $2) ON CONFLICT ON CONSTRAINT usersettings_pkey DO UPDATE SET user_value = $2 WHERE usersettings.user_setting = 'locale' AND usersettings.user_id = $1", _connection)) {
                cmd.Parameters.AddWithValue((long) userId);
                cmd.Parameters.AddWithValue(locale);
                await cmd.ExecuteNonQueryAsync();
            }

            if (!_userSettingsCache.ContainsKey("locale")) {
                _userSettingsCache["locale"] = new Dictionary<long, string>();
            }
            _userSettingsCache["locale"][(long)userId] = locale;
        }

    }


}

