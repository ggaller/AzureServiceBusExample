using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static FunctionApp.SendMessage;

namespace UnitTests
{
    [TestClass]
    public class SendMessageTest
    {
        [TestMethod]
        public void FrobiddenWithNullRule()
        {
            var result = (StatusCodeResult)Run(
                new Message(), 
                null,
                A.Fake<IAsyncCollector<Message>>(),
                A.Fake<ILogger>()).Result;

            Assert.AreEqual(403, result.StatusCode);
        }

        [TestMethod]
        public void BadResponseWithNullMessage()
        {
            var result = (StatusCodeResult)Run(
                null,
                A.Dummy<InboxRule>(),
                A.Fake<IAsyncCollector<Message>>(),
                A.Fake<ILogger>()).Result;

            Assert.AreEqual(400, result.StatusCode);
        }

        [TestMethod]
        public void FrobiddenWithNotAcceptedRule()
        {
            var result = (StatusCodeResult)Run(
                new Message {To = "gb" },
                new InboxRule {Queues ="fr,pl" },
                A.Fake<IAsyncCollector<Message>>(),
                A.Fake<ILogger>()).Result;

            Assert.AreEqual(403, result.StatusCode);
        }

        [TestMethod]
        public void OkWithAcceptedRule()
        {
            var result = (StatusCodeResult)Run(
                new Message { To = "gb" },
                new InboxRule { Queues = "fr,pl,gb" },
                A.Fake<IAsyncCollector<Message>>(),
                A.Fake<ILogger>()).Result;
            Assert.AreEqual(200, result.StatusCode);
        }

        [TestMethod]
        public void NotFoundWithIncorrectQueue()
        {
            var serviceBus = A.Fake<IAsyncCollector<Message>>();
            A.CallTo(serviceBus).
                Throws(new Microsoft.Azure.ServiceBus.MessagingEntityNotFoundException("Error"));

            var result = (StatusCodeResult)Run(
                new Message { To = "gb" },
                new InboxRule { Queues = "fr,pl,gb" },
                serviceBus,
                A.Fake<ILogger>()).Result;

            Assert.AreEqual(404, result.StatusCode);
        }
    }
}
