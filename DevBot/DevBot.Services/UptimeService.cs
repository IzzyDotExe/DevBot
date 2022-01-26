using System;
using System.Timers;
using System.Diagnostics;
using Serilog;

namespace DevBot.Services {

    class UptimeService {

        public readonly Stopwatch _uptime = new Stopwatch();
        public UptimeService() {
            _uptime.Start();
            Log.Logger.Information("Uptime service loaded!");
        }

    }

}