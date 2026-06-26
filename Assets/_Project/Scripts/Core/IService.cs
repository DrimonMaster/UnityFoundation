namespace UnityFoundation.Core
{
    public interface IService
    {
        InitPriority Priority { get; }
        bool IsReady { get; }
        void Initialize();
        void Dispose();
    }
}