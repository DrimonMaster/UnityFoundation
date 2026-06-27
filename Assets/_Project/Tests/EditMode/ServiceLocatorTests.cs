using System;
using NUnit.Framework;
using UnityFoundation.Core;

namespace UnityFoundation.Tests
{
    [TestFixture]
    public class ServiceLocatorTests
    {
        [TearDown]
        public void TearDown() => ServiceLocator.Clear();

        [Test]
        public void Register_AndGet_ReturnsCorrectService()
        {
            var service = new StubService();
            ServiceLocator.Register<IStubService>(service);
            Assert.That(ServiceLocator.Get<IStubService>(), Is.EqualTo(service));
        }

        [Test]
        public void Get_UnregisteredService_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => ServiceLocator.Get<IStubService>());
        }

        [Test]
        public void TryGet_UnregisteredService_ReturnsFalse()
        {
            var result = ServiceLocator.TryGet<IStubService>(out _);
            Assert.That(result, Is.False);
        }

        [Test]
        public void Unregister_ThenGet_ThrowsException()
        {
            ServiceLocator.Register<IStubService>(new StubService());
            ServiceLocator.Unregister<IStubService>();
            Assert.Throws<InvalidOperationException>(() => ServiceLocator.Get<IStubService>());
        }

        [Test]
        public void Clear_DisposesAllServices()
        {
            var service = new StubService();
            ServiceLocator.Register<IStubService>(service);
            ServiceLocator.Clear();
            Assert.That(service.IsDisposed, Is.True);
        }

        private interface IStubService : IService { }

        private class StubService : IStubService
        {
            public InitPriority Priority => InitPriority.Critical;
            public bool IsReady => true;
            public bool IsDisposed { get; private set; }
            public void Initialize() { }
            public void Dispose() => IsDisposed = true;
        }
    }
}
