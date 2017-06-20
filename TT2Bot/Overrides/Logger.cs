﻿using System;
using TitanBot.Logger;

namespace TT2Bot
{
    class Logger : TitanBotLogger
    {
        public Logger() { }

        public Logger(LogSeverity logLevel, string logLocation) : base(logLevel, logLocation) { }

        public override void Log(ILoggable entry)
        {
            if (!ShouldLog(entry.Severity))
                return;
            Console.Out.WriteLineAsync(entry.ToString());
            base.Log(entry);
        }
    }
}