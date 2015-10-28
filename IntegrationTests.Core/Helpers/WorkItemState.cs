using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.Core.Helpers
{
    /// <summary>
    /// Describes the state of a work item
    /// </summary>
    public class WorkItemState
    {
        /// <summary>
        /// A list of fields and their corresponding values in this work item state
        /// </summary>
        public List<FieldValuePair> FieldValues { get; set; }

        public WorkItemState()
        {
            FieldValues = new List<FieldValuePair>();
        }
    }
}
