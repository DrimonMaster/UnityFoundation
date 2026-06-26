using System.Collections.Generic;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public class UIService : IUIService
    {
        private readonly Stack<string> _screenStack = new();

        public InitPriority Priority => InitPriority.Critical;
        public bool IsReady { get; private set; }

        public void Initialize()
        {
            IsReady = true;
            "[Lifecycle] UIService initialized".Log(LogCategory.Lifecycle);
        }

        public void Dispose()
        {
            "[Lifecycle] UIService disposed".Log(LogCategory.Lifecycle);
            _screenStack.Clear();
            IsReady = false;
        }

        public void Push(string screenId) => _screenStack.Push(screenId); // TODO: instantiate and show screen
        public void Pop() { if (_screenStack.Count > 0) _screenStack.Pop(); } // TODO: hide and destroy screen
        public void Replace(string screenId) { Pop(); Push(screenId); }
    }
}
