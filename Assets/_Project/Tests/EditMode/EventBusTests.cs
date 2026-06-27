using System.Collections.Generic;
using NUnit.Framework;
using UnityFoundation.Services;

namespace UnityFoundation.Tests
{
    [TestFixture]
    public class EventBusTests
    {
        private EventBus _eventBus;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new EventBus();
            _eventBus.Initialize();
        }

        [TearDown]
        public void TearDown() => _eventBus.Dispose();

        private struct TestEvent { public int Value; }

        [Test]
        public void Subscribe_AndPublish_HandlerReceivesEvent()
        {
            var received = 0;
            _eventBus.Subscribe<TestEvent>(e => received = e.Value);
            _eventBus.Publish(new TestEvent { Value = 42 });
            Assert.That(received, Is.EqualTo(42));
        }

        [Test]
        public void Unsubscribe_AfterDispose_HandlerNotCalled()
        {
            var called = 0;
            var token = _eventBus.Subscribe<TestEvent>(_ => called++);
            token.Dispose();
            _eventBus.Publish(new TestEvent());
            Assert.That(called, Is.EqualTo(0));
        }

        [Test]
        public void SubscribeOnce_CalledOnlyOnce()
        {
            var called = 0;
            _eventBus.SubscribeOnce<TestEvent>(_ => called++);
            _eventBus.Publish(new TestEvent());
            _eventBus.Publish(new TestEvent());
            Assert.That(called, Is.EqualTo(1));
        }

        [Test]
        public void Priority_HigherPriorityHandlerCalledFirst()
        {
            var order = new List<int>();
            _eventBus.Subscribe<TestEvent>(_ => order.Add(1),  priority: 1);
            _eventBus.Subscribe<TestEvent>(_ => order.Add(10), priority: 10);
            _eventBus.Subscribe<TestEvent>(_ => order.Add(5),  priority: 5);
            _eventBus.Publish(new TestEvent());
            Assert.That(order, Is.EqualTo(new[] { 10, 5, 1 }));
        }

        [Test]
        public void Publish_NoSubscribers_NoException()
        {
            Assert.DoesNotThrow(() => _eventBus.Publish(new TestEvent()));
        }
    }
}
