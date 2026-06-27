namespace UnityFoundation.Services
{
    public interface IScreen
    {
        ScreenLayer Layer { get; }
        void OnShow();
        void OnHide();
    }
}
