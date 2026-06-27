namespace UnityFoundation.Services
{
    public record GameStateChangedEvent(GameState From, GameState To);
}
