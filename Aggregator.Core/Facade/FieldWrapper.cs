using System;
using System.Globalization;

using Aggregator.Core.Extensions;
using Aggregator.Core.Interfaces;
using Aggregator.Core.Monitoring;

using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Aggregator.Core.Facade
{
    public class FieldWrapper : IField
    {
        private readonly Field tfsField;

        private readonly ILogEvents logger;

        public FieldWrapper(Field field, ILogEvents logger)
        {
            this.tfsField = field;
            this.logger = logger;
        }

        public string Name
        {
            get
            {
                return this.tfsField.Name;
            }
        }

        public string ReferenceName
        {
            get
            {
                return this.tfsField.ReferenceName;
            }
        }

        public FieldValue Value
        {
            get
            {
                return this.tfsField.Value == null
                    ? null
                    : new FieldValue(this.DataType, this.logger, this.tfsField.Value);
            }

            set
            {
                this.tfsField.Value = value == null ? null : Convert.ChangeType(value, this.DataType);
            }
        }

        public FieldStatus Status
        {
            get
            {
                return this.tfsField.Status;
            }
        }

        public FieldValue OriginalValue
        {
            get
            {
                return this.tfsField.OriginalValue == null
                    ? null
                    : new FieldValue(this.DataType, this.logger, this.tfsField.OriginalValue);
            }
        }

        public Type DataType
        {
            get
            {
                return this.tfsField.FieldDefinition.SystemType;
            }
        }
    }
}
