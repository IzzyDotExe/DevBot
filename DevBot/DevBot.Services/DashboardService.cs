using DSharpPlus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using Serilog;

namespace DevBot.Services {
    
    class DashboardService {

        private readonly IServiceProvider _provider;
        private readonly DiscordClient _client;
        private readonly IConfigurationRoot _settings;
        private Process _dashboardProc = null;

        public DashboardService(IConfigurationRoot settings, DiscordClient client, IServiceProvider provider) {
            this._settings = settings;
            this._client = client;
            this._provider = provider;
            Log.Logger.Information("Dashboard service loaded!");

        }

        private async Task Init(string args) {

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = _settings.GetSection("flask:pythonpath").Value;
            start.Arguments = string.Format("{0} {1}", "./DevBot.Dashboard/__init__.py", args);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            _dashboardProc = Process.Start(start);
            using (StreamReader reader = _dashboardProc.StandardOutput)
            {
                string result = await reader.ReadToEndAsync();
                Log.Logger.Information(this.RemoveSpecialCharacters(result));
            }
            
        }

        private string RemoveSpecialCharacters(string str) {
            StringBuilder sb = new StringBuilder();

            foreach (char c in str) {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_') {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public async Task Start() {

            if (_dashboardProc is null) {
                await this.Init(" ");
                return;
            }

            if (_dashboardProc.HasExited) {
                await this.Init(" ");
            }
        }

        public void Stop() {
            _dashboardProc.Kill();
        }

        public async Task Restart() {
            _dashboardProc.Kill();
            await this.Init(" ");
        }


    }

}