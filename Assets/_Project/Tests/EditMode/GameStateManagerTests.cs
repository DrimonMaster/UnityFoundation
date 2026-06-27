using NUnit.Framework;
using UnityFoundation.Core;
using UnityFoundation.Services;

namespace UnityFoundation.Tests
{
    [TestFixture]
    public class GameStateManagerTests
    {
        private EventBus _eventBus;
        private GameStateManager _gsm;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new EventBus();
            _eventBus.Initialize();
            ServiceLocator.Register<IEventBus>(_eventBus);

            _gsm = new GameStateManager();
            _gsm.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            _gsm.Dispose();
            ServiceLocator.Clear();
        }

        [Test]
        public void TransitionTo_ValidState_ChangesCurrentState()
        {
            _gsm.TransitionTo(GameState.Gameplay);
            Assert.That(_gsm.CurrentState, Is.EqualTo(GameState.Gameplay));
        }

        [Test]
        public void TransitionTo_SameState_IsIgnored()
        {
            // Initial state is Loading — transition to same state should be ignored
            var stateChanges = 0;
            _gsm.OnStateChanged += (_, _) => stateChanges++;
            _gsm.TransitionTo(GameState.Loading);
            Assert.That(stateChanges, Is.EqualTo(0));
        }

        [Test]
        public void TransitionTo_PublishesGameStateChangedEvent()
        {
            GameStateChangedEvent? received = null;
            _eventBus.Subscribe<GameStateChangedEvent>(e => received = e);
            _gsm.TransitionTo(GameState.MainMenu);
            Assert.That(received.HasValue, Is.True);
            Assert.That(received.Value.To, Is.EqualTo(GameState.MainMenu));
        }

        [Test]
        public void OnStateChanged_FiredWithCorrectFromAndTo()
        {
            GameState? from = null, to = null;
            _gsm.OnStateChanged += (f, t) => { from = f; to = t; };
            _gsm.TransitionTo(GameState.Gameplay);
            Assert.That(from, Is.EqualTo(GameState.Loading));
            Assert.That(to, Is.EqualTo(GameState.Gameplay));
        }
    }
}
