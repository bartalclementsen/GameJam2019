using Core.Mediators;
using NUnit.Framework;
using System;

namespace Core.Tests.Mediators
{
    public class MessengerTests : TestingContext<Messenger>
    {
        public class UnitTestMessage : IMessage
        {
            public object Sender { get; }
        }


        /* ----------------------------------------------------------------------------  */
        /*                                    Publish                                    */
        /* ----------------------------------------------------------------------------  */
        [Test]
        public void Publish_Should_publish_message()
        {
            //Arrange
            Messenger unit = GetUnit();

            //Act
            //Assert
            unit.Publish(new UnitTestMessage());
        }

        [Test]
        public void Publish_Should_throw_argument_null_exception_on_null()
        {
            //Arrange
            Messenger unit = GetUnit();

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => unit.Publish(null));
        }

        /* ----------------------------------------------------------------------------  */
        /*                                   SUBSCRIBE                                   */
        /* ----------------------------------------------------------------------------  */
        [Test]
        public void Subscribe_Should_throw_argument_null_exception_on_null()
        {
            //Arrange
            Messenger unit = GetUnit();
            Action<IMessage> emptyAction = null;

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => unit.Subscribe(emptyAction));
        }

        [Test]
        public void Subscribe_Should_add_subscription_and_generate_a_subscription_token()
        {
            //Arrange
            Messenger unit = GetUnit();

            //Act
            ISubscriptionToken subscriptionToken = unit.Subscribe((UnitTestMessage message) => { });

            //Assert
            Assert.NotNull(subscriptionToken);
        }

        /* ----------------------------------------------------------------------------  */
        /*                                  UNSUBSCRIBE                                  */
        /* ----------------------------------------------------------------------------  */
        [Test]
        public void Unsubscribe_Should_unsubscribe_token()
        {
            //Arrange
            Messenger unit = GetUnit();

            ISubscriptionToken subscriptionToken = unit.Subscribe((UnitTestMessage message) => { });

            //Act
            unit.Unsubscribe(subscriptionToken);

            //Assert

        }

        /* ----------------------------------------------------------------------------  */
        /*                               PUBLISH / RECIEVE                               */
        /* ----------------------------------------------------------------------------  */
        [Test]
        public void Should_publish_event_to_subscribers()
        {
            //Arrange
            Messenger unit = GetUnit();

            UnitTestMessage unityTestMessage = new UnitTestMessage();

            //Act
            UnitTestMessage message1 = null;
            unit.Subscribe((UnitTestMessage message) =>
            {
                message1 = message;
            });
            UnitTestMessage message2 = null;
            unit.Subscribe((UnitTestMessage message) =>
            {
                message2 = message;
            });

            unit.Publish(unityTestMessage);

            //Assert
            Assert.AreEqual(unityTestMessage, message1);
            Assert.AreEqual(unityTestMessage, message2);
        }

        [Test]
        public void Should_unsubscribe_then_publish_event_to_subscribers()
        {
            //Arrange
            Messenger unit = GetUnit();

            UnitTestMessage unityTestMessage = new UnitTestMessage();

            //Act
            UnitTestMessage message1 = null;
            ISubscriptionToken subscriptionToken = unit.Subscribe((UnitTestMessage message) =>
            {
                message1 = message;
            });
            UnitTestMessage message2 = null;
            unit.Subscribe((UnitTestMessage message) =>
            {
                message2 = message;
            });

            unit.Unsubscribe(subscriptionToken);

            unit.Publish(unityTestMessage);

            //Assert
            Assert.Null(message1);
            Assert.AreEqual(unityTestMessage, message2);
        }

        [Test]
        public void Should_unsubscribe_on_dispose_then_publish_event_to_subscribers()
        {
            //Arrange
            Messenger unit = GetUnit();

            UnitTestMessage unityTestMessage = new UnitTestMessage();

            //Act
            UnitTestMessage message1 = null;
            ISubscriptionToken subscriptionToken = unit.Subscribe((UnitTestMessage message) =>
            {
                message1 = message;
            });
            UnitTestMessage message2 = null;
            unit.Subscribe((UnitTestMessage message) =>
            {
                message2 = message;
            });

            subscriptionToken.Dispose();

            unit.Publish(unityTestMessage);

            //Assert
            Assert.Null(message1);
            Assert.AreEqual(unityTestMessage, message2);
        }
    }
}
