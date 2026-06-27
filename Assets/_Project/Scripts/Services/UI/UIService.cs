using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public class UIService : IUIService
    {
        private readonly Stack<IScreen> _baseStack = new();
        private readonly Stack<IScreen> _overlayStack = new();
        private readonly List<IScreen> _hudScreens = new();
        private readonly List<IScreen> _systemScreens = new();
        private readonly Dictionary<Type, IScreen> _screens = new();
        private readonly Dictionary<ScreenLayer, Canvas> _canvases = new();
        private IDisposable _eventSub;

        public InitPriority Priority => InitPriority.Critical;
        public bool IsReady { get; private set; }

        public void Initialize()
        {
            _canvases[ScreenLayer.Base]    = CreateCanvas("Canvas_Base",    0);
            _canvases[ScreenLayer.Overlay] = CreateCanvas("Canvas_Overlay", 10);
            _canvases[ScreenLayer.HUD]     = CreateCanvas("Canvas_HUD",     20);
            _canvases[ScreenLayer.System]  = CreateCanvas("Canvas_System",  30);

            _eventSub = ServiceLocator.Get<IEventBus>().Subscribe<GameStateChangedEvent>(OnGameStateChanged);
            IsReady = true;
            "[Lifecycle] UIService initialized".Log(LogCategory.Lifecycle);
        }

        public void Dispose()
        {
            _eventSub?.Dispose();
            "[Lifecycle] UIService disposed".Log(LogCategory.Lifecycle);
            _baseStack.Clear();
            _overlayStack.Clear();
            _hudScreens.Clear();
            _systemScreens.Clear();
            _screens.Clear();
            IsReady = false;
        }

        public void Show<T>() where T : MonoBehaviour, IScreen
        {
            var screen = GetOrCreate<T>();
            switch (screen.Layer)
            {
                case ScreenLayer.Base:
                    ClearStack(_baseStack);
                    Activate(screen);
                    _baseStack.Push(screen);
                    break;
                case ScreenLayer.Overlay:
                    Activate(screen);
                    _overlayStack.Push(screen);
                    break;
                case ScreenLayer.HUD:
                    if (!_hudScreens.Contains(screen)) _hudScreens.Add(screen);
                    Activate(screen);
                    break;
                case ScreenLayer.System:
                    if (!_systemScreens.Contains(screen)) _systemScreens.Add(screen);
                    Activate(screen);
                    break;
            }
            $"[UI] Show {typeof(T).Name} ({screen.Layer})".Log(LogCategory.UI);
        }

        public void Hide<T>() where T : MonoBehaviour, IScreen
        {
            var type = typeof(T);
            if (!_screens.TryGetValue(type, out var screen)) return;
            Deactivate(screen);
            switch (screen.Layer)
            {
                case ScreenLayer.Base:    RemoveFromStack(_baseStack, screen); break;
                case ScreenLayer.Overlay: RemoveFromStack(_overlayStack, screen); break;
                case ScreenLayer.HUD:     _hudScreens.Remove(screen); break;
                case ScreenLayer.System:  _systemScreens.Remove(screen); break;
            }
            $"[UI] Hide {type.Name}".Log(LogCategory.UI);
        }

        public void Push<T>() where T : MonoBehaviour, IScreen
        {
            var screen = GetOrCreate<T>();
            Activate(screen);
            _overlayStack.Push(screen);
            $"[UI] Push {typeof(T).Name} (overlay depth: {_overlayStack.Count})".Log(LogCategory.UI);
        }

        public void Pop()
        {
            if (_overlayStack.Count == 0) return;
            var current = _overlayStack.Pop();
            Deactivate(current);
            $"[UI] Pop {current.GetType().Name} (overlay depth: {_overlayStack.Count})".Log(LogCategory.UI);
        }

        public void Replace<T>() where T : MonoBehaviour, IScreen
        {
            ClearStack(_baseStack);
            var screen = GetOrCreate<T>();
            Activate(screen);
            _baseStack.Push(screen);
            $"[UI] Replace (Base) → {typeof(T).Name}".Log(LogCategory.UI);
        }

        public void HideAll()
        {
            ClearStack(_overlayStack);
            ClearStack(_baseStack);
            "[UI] HideAll — Base + Overlay cleared".Log(LogCategory.UI);
        }

        private void OnGameStateChanged(GameStateChangedEvent evt)
        {
            switch (evt.To)
            {
                case GameState.MainMenu:  Replace<MainMenuScreen>(); break;
                case GameState.Gameplay:  Replace<MainMenuScreen>(); Show<HUDScreen>(); break;
                case GameState.Paused:    Push<PauseScreen>(); break;
                case GameState.GameOver:  Push<GameOverScreen>(); break;
            }
        }

        private IScreen GetOrCreate<T>() where T : MonoBehaviour, IScreen
        {
            var type = typeof(T);
            if (_screens.TryGetValue(type, out var cached))
                return cached;

            var prefab = Resources.Load<GameObject>($"Screens/{type.Name}");
            GameObject go;
            if (prefab != null)
            {
                go = UnityEngine.Object.Instantiate(prefab);
                go.SetActive(false);
            }
            else
            {
                go = new GameObject(type.Name, typeof(RectTransform));
                go.AddComponent<T>();
            }

            var screen = go.GetComponent<IScreen>();
            go.transform.SetParent(_canvases[screen.Layer].transform, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            go.SetActive(false);

            _screens[type] = screen;
            return screen;
        }

        private static void Activate(IScreen screen)
        {
            ((MonoBehaviour)screen).gameObject.SetActive(true);
            screen.OnShow();
        }

        private static void Deactivate(IScreen screen)
        {
            screen.OnHide();
            ((MonoBehaviour)screen).gameObject.SetActive(false);
        }

        private static void ClearStack(Stack<IScreen> stack)
        {
            while (stack.Count > 0)
                Deactivate(stack.Pop());
        }

        private static void RemoveFromStack(Stack<IScreen> stack, IScreen target)
        {
            var items = stack.ToArray(); // top-first
            stack.Clear();
            for (var i = items.Length - 1; i >= 0; i--)
                if (items[i] != target) stack.Push(items[i]);
        }

        private static Canvas CreateCanvas(string name, int sortingOrder)
        {
            var go = new GameObject(name);
            UnityEngine.Object.DontDestroyOnLoad(go);
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            go.AddComponent<GraphicRaycaster>();
            return canvas;
        }
    }
}
