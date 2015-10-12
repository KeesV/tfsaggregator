using System;

using Aggregator.Core.Extensions;

using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Aggregator.Core.Interfaces
{
    /// <summary>
    /// Decouples Core from TFS Client API <see cref="Microsoft.TeamFoundation.WorkItemTracking.Client.Field"/>
    /// </summary>
    public interface IField
    {
        string Name { get; }

        string ReferenceName { get; }

        FieldValue Value { get; set; }

        FieldStatus Status { get; }

        FieldValue OriginalValue { get; }

        Type DataType { get; }
    }
}
