using Aggregator.Core.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.Core.Helpers
{
    internal class DebugEventLogger : TextLogComposer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebugEventLogger"/> class.
        /// </summary>
        /// <param name="minimumLogLevel">The minimum log level to show.</param>
        public DebugEventLogger(LogLevel minimumLogLevel)
            : base(new DebugTextLogger(minimumLogLevel))
        {
        }
    }
}
