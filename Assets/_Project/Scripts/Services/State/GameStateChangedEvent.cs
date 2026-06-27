namespace UnityFoundation.Services
{
    public struct GameStateChangedEvent
    {
        public GameState From;
        public GameState To;

        public GameStateChangedEvent(GameState from, GameState to)
        {
            From = from;
            To = to;
        }
    }
}
