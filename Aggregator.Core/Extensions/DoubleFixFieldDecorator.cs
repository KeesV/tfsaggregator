using System;
using System.Globalization;

using Aggregator.Core.Interfaces;
using Aggregator.Core.Monitoring;

using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Aggregator.Core.Extensions
{
    public class DoubleFixFieldDecorator : IField
    {
        private readonly IField decoratedField;

        public DoubleFixFieldDecorator(IField decoratedField, ILogEvents logger)
        {
            this.decoratedField = decoratedField;
        }

        public string Name
        {
            get
            {
                return this.decoratedField.Name;
            }
        }

        public string ReferenceName
        {
            get
            {
                return this.decoratedField.ReferenceName;
            }
        }

        public FieldValue Value
        {
            get
            {
                return this.decoratedField.Value;
            }

            set
            {
                if (
                    value != null
                    && this.Value != null
                    && this.DataType == typeof(double))
                {
                    CultureInfo invariant = CultureInfo.InvariantCulture;
                    decimal current = decimal.Parse(((double)this.Value).ToString(invariant), invariant);
                    decimal proposed = decimal.Parse(((double)value).ToString(invariant), invariant);

                    // Ignore when the same value is assigned.
                    if (current == proposed)
                    {
                        return;
                    }
                }

                this.decoratedField.Value = value;
            }
        }

        public FieldStatus Status
        {
            get
            {
                return this.decoratedField.Status;
            }
        }

        public FieldValue OriginalValue
        {
            get
            {
                return this.decoratedField.OriginalValue;
            }
        }

        public Type DataType
        {
            get
            {
                return this.decoratedField.DataType;
            }
        }
    }
}
