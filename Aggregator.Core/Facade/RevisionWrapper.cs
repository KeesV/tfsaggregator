using System;

using Aggregator.Core.Interfaces;
using Aggregator.Core.Monitoring;

using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Aggregator.Core.Facade
{
    public class RevisionWrapper : IRevision
    {
        private readonly Revision revision;

        private readonly ILogEvents logger;

        public RevisionWrapper(Revision revision, ILogEvents logger)
        {
            this.revision = revision;
            this.logger = logger;
        }

        public object this[string name]
        {
            get
            {
                return this.revision[name];
            }
        }

        public IFieldCollection Fields
        {
            get
            {
                return new FieldCollectionWrapper(this.revision.Fields, this.logger);
            }
        }

        public int Index
        {
            get
            {
                return this.revision.Index;
            }
        }
    }
}
