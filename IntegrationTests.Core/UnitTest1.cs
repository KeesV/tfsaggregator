using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntegrationTests.Core.Helpers;
using System.Collections.Generic;

namespace IntegrationTests.Core
{
    [TestClass]
    public class IntegrationTests
    {
        private TfsConnection Tfs = null;

        [TestInitialize]
        public void ConnectTfs()
        {
            if(Tfs == null)
            {
                Tfs = new TfsConnection("http://tfsserver13:8080/tfs/DefaultCollection/", "tfssetup", "P@ssw0rd", "TFSSERVER13");

                //Destroy all WorkItems in the Project
                Tfs.DestroyAllWisInProject("TfsAggregatorTest");
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void TestMethod1()
        {
            List<int> createdWis = new List<int>();

            List<FieldValuePair> fvps = new List<FieldValuePair>()
            {
                new FieldValuePair() {FieldReferenceName = "System.Title", FieldValue = "Demo Work Item 1" },
                new FieldValuePair() {FieldReferenceName = "System.Description", FieldValue = "Demo Description" }
            };
            createdWis.Add(Tfs.CreateWi(fvps, "Product Backlog Item", "TfsAggregatorTest"));


            Tfs.DestroyWis(createdWis);
        }
    }
}
