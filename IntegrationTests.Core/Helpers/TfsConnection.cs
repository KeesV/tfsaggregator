using Aggregator.ConsoleApp;
using Aggregator.Core;
using Aggregator.Core.Context;
using Aggregator.Core.Monitoring;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.Core.Helpers
{
    public class TfsConnection
    {
        private TfsTeamProjectCollection Tfs { get; set; }
        private WorkItemStore _workItemStore { get; set; }

        /// <summary>
        /// Opens a connection to TFS and creates the necessary services to interact with it
        /// </summary>
        /// <param name="tpcUrl">The URL of the TPC to connect to</param>
        /// <param name="userName">The username to use</param>
        /// <param name="password">The password to use</param>
        /// <param name="domain">The domain to use</param>
        public TfsConnection(string tpcUrl, string userName, string password, string domain)
        {
            NetworkCredential nc = new NetworkCredential(userName, password, domain);

            try
            {
                Tfs = new TfsTeamProjectCollection(new Uri(tpcUrl), nc);
                Tfs.Authenticate();
                Tfs.EnsureAuthenticated();

                _workItemStore = new WorkItemStore(Tfs);
            }
            catch (WebException ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Creates a work item in the specified project
        /// </summary>
        /// <param name="fieldValues">A list of field/value pairs to fill the new work item with</param>
        /// <param name="workItemType">The work item type to create</param>
        /// <param name="project">The project in which to create the work item</param>
        /// <returns>The Id of the newly created work item</returns>
        public int CreateWi(WorkItemState stateToCreate, string workItemType, string project)
        {

            Project teamProject = _workItemStore.Projects[project];
            WorkItemType wit = teamProject.WorkItemTypes[workItemType];

            WorkItem wi = new WorkItem(wit);

            foreach (FieldValuePair fv in stateToCreate.FieldValues)
            {
                wi.Fields[fv.FieldReferenceName].Value = fv.FieldValue;
            }

            wi.Save();

            return wi.Id;
        }

        /// <summary>
        /// Change a specified work item
        /// </summary>
        /// <param name="fieldValues">The fields/values to change</param>
        /// <param name="workItemId">The Id of the work item to change</param>
        public void SetupWiState(WorkItemState newState, int workItemId)
        {
            WorkItem wi = _workItemStore.GetWorkItem(workItemId);

            foreach (FieldValuePair fv in newState.FieldValues)
            {
                wi.Fields[fv.FieldReferenceName].Value = fv.FieldValue;
            }

            wi.Save();
        }

        /// <summary>
        /// Destroys work items with the given Ids
        /// </summary>
        /// <param name="wisToDestroy">The Ids of the work items to destroy</param>
        public void DestroyWis(List<int> wisToDestroy)
        {
            var errors = _workItemStore.DestroyWorkItems(wisToDestroy);
            if (errors.Count > 0)
            {
                throw new ApplicationException("Failed to destroy all work items!");
            }
        }

        /// <summary>
        /// Destroys all Work Items in a specific project
        /// </summary>
        /// <param name="project">The project to destroy all Work Items in</param>
        public void DestroyAllWisInProject(string project)
        {
            WorkItemCollection workItemCollection = _workItemStore.Query("SELECT [System.Id] FROM WorkItems WHERE [System.TeamProject] = '" + project +"'");

            List<int> idsToDestroy = new List<int>();
            foreach (WorkItem wi in workItemCollection)
            {
                idsToDestroy.Add(wi.Id);
            }
            DestroyWis(idsToDestroy);
        }

        /// <summary>
        /// Applies a specified TFS Aggregator policy to the specified Project and Work Item
        /// </summary>
        /// <param name="policyFileName">The policy file to apply</param>
        /// <param name="project">The Team Project to apply the policy to</param>
        /// <param name="workItemId">The ID of the work item to apply the policy to</param>
        /// <returns></returns>
        public int ApplyAggregatorPolicy(string policyFileName, string project, int workItemId)
        {
            var logger = new DebugEventLogger(LogLevel.Warning);
            var context = new RequestContext(Tfs.Uri.ToString(), project);
            var runtime = RuntimeContext.GetContext(
                () => policyFileName,
                context,
                logger,
                (collectionUri, toImpersonate, logEvents) =>
                    new Aggregator.Core.Facade.WorkItemRepository(collectionUri, toImpersonate, logEvents));

            using (EventProcessor eventProcessor = new EventProcessor(runtime))
            {
                try
                {
                    var workItemIds = new Queue<int>();
                    workItemIds.Enqueue(workItemId);

                    ProcessingResult result = null;
                    while (workItemIds.Count > 0)
                    {
                        context.CurrentWorkItemId = workItemIds.Dequeue();
                        var notification = context.Notification;

                        logger.StartingProcessing(context, notification);
                        result = eventProcessor.ProcessEvent(context, notification);
                        logger.ProcessingCompleted(result);

                        foreach (var savedId in eventProcessor.SavedWorkItems)
                        {
                            workItemIds.Enqueue(savedId);
                        }
                    }
                    return result.StatusCode;
                }

                catch (Exception e)
                {
                    logger.ProcessEventException(e);
                    return 1;
                }
            }
        }

        public void AssertWiState(WorkItemState desiredState, int workItemId)
        {
            WorkItem wi = _workItemStore.GetWorkItem(workItemId);

            foreach(FieldValuePair fv in desiredState.FieldValues)
            {
                Assert.AreEqual(fv.FieldValue, wi.Fields[fv.FieldReferenceName].Value);
            }
        }
    }
}
