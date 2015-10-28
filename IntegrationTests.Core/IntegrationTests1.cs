using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntegrationTests.Core.Helpers;
using System.Collections.Generic;
using Aggregator.ConsoleApp;
using Aggregator.Core.Context;
using Aggregator.Core.Monitoring;
using Aggregator.Core;

namespace IntegrationTests.Core
{
    [TestClass]
    public class IntegrationTests1
    {
        string projectName = "TfsAggregatorTest";

        private TfsConnection Tfs = null;

        [TestInitialize]
        public void ConnectTfs()
        {
            if (Tfs == null)
            {
                Tfs = new TfsConnection("http://tfsserver13:8080/tfs/DefaultCollection/", "", "", "tfsserver13");

                //Destroy all WorkItems in the Project
                Tfs.DestroyAllWisInProject(projectName);
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void TestMethod1()
        {
            string policyFileName = "D:\\Git\\TfsAggregator\\IntegrationTests.Core\\ConfigurationsForTests\\NoOp.policies";
            

            int firstWorkItem;

            //Create the work item(s)
            WorkItemState initState = new WorkItemState()
            {
                FieldValues = {
                    new FieldValuePair() { FieldReferenceName = "System.Title", FieldValue = "Demo Work Item 1" },
                    new FieldValuePair() { FieldReferenceName = "System.Description", FieldValue = "Demo Description" }
                }
            };
            firstWorkItem = Tfs.CreateWi(initState, "Product Backlog Item", projectName);

            //Apply some changes to work item(s)
            WorkItemState newState = new WorkItemState()
            {
                FieldValues = {
                    new FieldValuePair() { FieldReferenceName = "System.Title", FieldValue = "Demo Work Item 1 Updated!" },
                    new FieldValuePair() { FieldReferenceName = "System.Description", FieldValue = "Demo Description Updated!" }
                }
            };
            Tfs.SetupWiState(newState, firstWorkItem);

            //Apply the specified policy using the Aggregator
            int aggregatorReturnCode = Tfs.ApplyAggregatorPolicy(policyFileName, projectName, firstWorkItem);

            //Verify result
            WorkItemState desiredState = new WorkItemState()
            {
                FieldValues = {
                    new FieldValuePair() { FieldReferenceName = "System.Title", FieldValue = "Demo Work Item 1 Updated!" },
                    new FieldValuePair() { FieldReferenceName = "System.Description", FieldValue = "Demo Description Updated!" }
                }
            };

            Assert.AreEqual(aggregatorReturnCode, 0);

            Tfs.AssertWiState(desiredState, firstWorkItem);

            //Clean up after the test
            Tfs.DestroyWis(new List<int> { firstWorkItem });
        }
    }
}
