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
using Microsoft.Extensions.Logging;
using System.Text;
using Octokit;


namespace DevBot.Services.API {

    class GithubService  {
        private readonly IConfigurationRoot _settings;
        private readonly GitHubClient githubClient;

        public GithubService(IConfigurationRoot settings) {
            _settings = settings;
            githubClient = new GitHubClient(new ProductHeaderValue("DevBot"));
        }

        

    }

}