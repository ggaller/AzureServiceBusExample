using Microsoft.VisualStudio.TestTools.UnitTesting;
using static FunctionApp.SendMessage;

namespace UnitTests
{
    [TestClass]
    public class InboxRuleTest
    {
        [TestMethod]
        public void NotAcceptWithNullorEmptyDestination()
        {
            var target = new InboxRule();

            Assert.IsFalse(target.Accepted(null));
            Assert.IsFalse(target.Accepted(string.Empty));
        }

        [TestMethod]
        public void NotAcceptWithEmptyQueues()
        {
            var target = new InboxRule { Queues = string.Empty };

            Assert.IsFalse(target.Accepted("nl"));
        }

        [TestMethod]
        public void CanAccept()
        {
            var target = new InboxRule { Queues = "en,pl" };

            Assert.IsTrue(target.Accepted("en"));
        }

        [TestMethod]
        public void NotAccept()
        {
            var target = new InboxRule { Queues = "en,pl" };

            Assert.IsFalse(target.Accepted("gb"));
        }
    }
}
