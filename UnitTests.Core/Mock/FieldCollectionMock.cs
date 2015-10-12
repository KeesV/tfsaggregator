using System.Collections;
using System.Collections.Generic;

using Aggregator.Core.Extensions;
using Aggregator.Core.Interfaces;
using Aggregator.Core.Monitoring;

namespace UnitTests.Core.Mock
{
    internal class FieldCollectionMock : IFieldCollection
    {
        private readonly Dictionary<string, IField> fields = new Dictionary<string, IField>();

        private readonly WorkItemMock workItemMock;

        private readonly ILogEvents logger;

        public FieldCollectionMock(WorkItemMock workItemMock, ILogEvents logger)
        {
            this.workItemMock = workItemMock;
            this.logger = logger;
        }

        public IField this[string name]
        {
            get
            {
                if (!this.fields.ContainsKey(name))
                {
                    IField field = new FieldMock(this.workItemMock, name, this.logger);
                    this.fields.Add(name, new DoubleFixFieldDecorator(field, this.logger));
                }

                return this.fields[name];
            }

            set
            {
                this.fields[name] = value;
            }
        }

        public IEnumerator<IField> GetEnumerator()
        {
            return this.fields.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}