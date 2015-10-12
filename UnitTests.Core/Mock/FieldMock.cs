using System;
using System.Management.Automation.Runspaces;
using System.Reflection.Emit;

using Aggregator.Core.Extensions;
using Aggregator.Core.Interfaces;
using Aggregator.Core.Monitoring;

using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace UnitTests.Core.Mock
{
    internal class FieldMock : IField
    {
        private readonly WorkItemMock workItemMock;

        private readonly ILogEvents logger;

        private object value;

        public FieldMock(WorkItemMock workItemMock, string name, ILogEvents logger)
        {
            this.workItemMock = workItemMock;
            this.logger = logger;
            this.Name = name;
        }

        public string Name { get; set; }

        public string ReferenceName { get; set; }

        public FieldValue Value
        {
            get
            {
                return this.value == null ? null : new FieldValue(this.DataType, this.logger, this.value);
            }

            set
            {
                object proposed;
                proposed = value == null ? null : Convert.ChangeType(value, this.DataType);

                object original;
                original = this.OriginalValue == null ? null : Convert.ChangeType(this.OriginalValue, this.DataType);

                this.value = proposed;

                if (original != proposed)
                {
                    this.workItemMock.IsDirty = true;
                }
            }
        }

        public FieldStatus Status { get; set; }

        public FieldValue OriginalValue { get; set; }

        private Type dataType;

        public Type DataType
        {
            get
            {
                return this.dataType ?? this.value?.GetType() ?? typeof(object);
            }

            set
            {
                this.dataType = value;
            }
        }
    }
}
