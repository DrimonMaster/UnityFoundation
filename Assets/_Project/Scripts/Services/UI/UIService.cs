using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public class UIService : IUIService
    {
        private readonly Stack<IScreen> _screenStack = new();
        private readonly Dictionary<Type, IScreen> _screens = new();
        private Canvas _canvas;
        private IDisposable _eventSub;

        public InitPriority Priority => InitPriority.Critical;
        public bool IsReady { get; private set; }

        public void Initialize()
        {
            _canvas = CreateCanvas();
            _eventSub = ServiceLocator.Get<IEventBus>().Subscribe<GameStateChangedEvent>(OnGameStateChanged);
            IsReady = true;
            "[Lifecycle] UIService initialized".Log(LogCategory.Lifecycle);
        }

        public void Dispose()
        {
            _eventSub?.Dispose();
            "[Lifecycle] UIService disposed".Log(LogCategory.Lifecycle);
            _screenStack.Clear();
            _screens.Clear();
            IsReady = false;
        }

        public void Show<T>() where T : MonoBehaviour, IScreen
        {
            var screen = GetOrCreate<T>();
            ((MonoBehaviour)screen).gameObject.SetActive(true);
            screen.OnShow();
            $"[UI] Show {typeof(T).Name}".Log(LogCategory.UI);
        }

        public void Hide<T>() where T : MonoBehaviour, IScreen
        {
            var type = typeof(T);
            if (!_screens.TryGetValue(type, out var screen)) return;
            screen.OnHide();
            ((MonoBehaviour)screen).gameObject.SetActive(false);
            $"[UI] Hide {type.Name}".Log(LogCategory.UI);
        }

        public void Push<T>() where T : MonoBehaviour, IScreen
        {
            if (_screenStack.Count > 0)
            {
                var top = _screenStack.Peek();
                top.OnHide();
                ((MonoBehaviour)top).gameObject.SetActive(false);
            }
            var screen = GetOrCreate<T>();
            ((MonoBehaviour)screen).gameObject.SetActive(true);
            screen.OnShow();
            _screenStack.Push(screen);
            $"[UI] Push {typeof(T).Name} (depth: {_screenStack.Count})".Log(LogCategory.UI);
        }

        public void Pop()
        {
            if (_screenStack.Count == 0) return;
            var current = _screenStack.Pop();
            current.OnHide();
            ((MonoBehaviour)current).gameObject.SetActive(false);
            $"[UI] Pop {current.GetType().Name} (depth: {_screenStack.Count})".Log(LogCategory.UI);
            if (_screenStack.Count > 0)
            {
                var top = _screenStack.Peek();
                ((MonoBehaviour)top).gameObject.SetActive(true);
                top.OnShow();
            }
        }

        public void Replace<T>() where T : MonoBehaviour, IScreen
        {
            if (_screenStack.Count > 0)
            {
                var current = _screenStack.Pop();
                current.OnHide();
                ((MonoBehaviour)current).gameObject.SetActive(false);
            }
            var screen = GetOrCreate<T>();
            ((MonoBehaviour)screen).gameObject.SetActive(true);
            screen.OnShow();
            _screenStack.Push(screen);
            $"[UI] Replace → {typeof(T).Name}".Log(LogCategory.UI);
        }

        private void OnGameStateChanged(GameStateChangedEvent evt)
        {
            switch (evt.To)
            {
                case GameState.MainMenu:  Push<MainMenuScreen>(); break;
                case GameState.Paused:    Push<PauseScreen>(); break;
                case GameState.GameOver:  Replace<GameOverScreen>(); break;
                case GameState.Gameplay:  Pop(); break;
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
                go = UnityEngine.Object.Instantiate(prefab, _canvas.transform);
            }
            else
            {
                go = new GameObject(type.Name, typeof(RectTransform));
                go.transform.SetParent(_canvas.transform, false);
                var rt = (RectTransform)go.transform;
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
                go.AddComponent<T>();
            }

            var screen = go.GetComponent<IScreen>();
            go.SetActive(false);
            _screens[type] = screen;
            return screen;
        }

        private static Canvas CreateCanvas()
        {
            var go = new GameObject("UICanvas");
            UnityEngine.Object.DontDestroyOnLoad(go);
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            go.AddComponent<GraphicRaycaster>();
            return canvas;
        }
    }
}
