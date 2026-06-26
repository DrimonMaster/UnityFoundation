namespace UnityFoundation.Core
{
    public enum InitPriority
    {
        Critical,   // blocks game start, no loader shown
        Important,  // loader shown until ready
        Optional    // background init, game does not wait
    }
}
