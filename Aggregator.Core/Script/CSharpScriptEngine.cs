﻿using Aggregator.Core.Interfaces;
using Aggregator.Core.Monitoring;
using Aggregator.Core.Script;

namespace Aggregator.Core
{
    using Microsoft.CSharp;

    public class CSharpScriptEngine : DotNetScriptEngine<CSharpCodeProvider>
    {
        public CSharpScriptEngine(ILogEvents logger, bool debug)
            : base(logger, debug)
        {
        }

        protected override int LinesOfCodeBeforeScript
        {
            get
            {
                return 14;
            }
        }

        protected override string WrapScript(string scriptName, string script)
        {
            return "namespace " + this.Namespace + @"
{
  using Microsoft.TeamFoundation.WorkItemTracking.Client;
  using Aggregator.Core;
  using Aggregator.Core.Extensions;
  using Aggregator.Core.Interfaces;
  using Aggregator.Core.Navigation;
  using Aggregator.Core.Monitoring;
  using System.Linq;

  public class " + this.ClassPrefix + scriptName + @" : Aggregator.Core.Script.IDotNetScript
  {
    public object RunScript(Aggregator.Core.Interfaces.IWorkItemExposed self, Aggregator.Core.Interfaces.IWorkItemRepositoryExposed store, Aggregator.Core.Monitoring.IRuleLogger logger)
    {
" + script + @"
      return null;
    }
  }
}
";
        }
    }
}
