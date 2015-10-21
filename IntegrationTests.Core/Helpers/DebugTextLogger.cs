using Aggregator.Core.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.Core.Helpers
{
    internal class DebugTextLogger : ITextLogger
    {
        public LogLevel MinimumLogLevel
        {
            get;
            set;
        }

        public DebugTextLogger(LogLevel minimumLogLevel)
        {
            this.MinimumLogLevel = minimumLogLevel;
        }

        public void Log(LogLevel level, string format, params object[] args)
        {
            string message = args != null ? string.Format(format, args: args) : format;
            string levelAsString = level.ToString();
            System.Diagnostics.Debug.WriteLine(message, levelAsString);
        }
    }
}
