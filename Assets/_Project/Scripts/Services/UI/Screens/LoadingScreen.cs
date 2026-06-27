using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityFoundation.Services
{
    public class LoadingScreen : MonoBehaviour, IScreen
    {
        public ScreenLayer Layer => ScreenLayer.System;

        private void Awake()
        {
            var bg = gameObject.AddComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0.8f);

            var textGO = new GameObject("Label", typeof(RectTransform));
            textGO.transform.SetParent(transform, false);
            var rt = (RectTransform)textGO.transform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            var tmp = textGO.AddComponent<TextMeshProUGUI>();
            tmp.text = "Loading...";
            tmp.fontSize = 36;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
        }

        public void OnShow() => $"[UI] {nameof(LoadingScreen)} shown".Log(LogCategory.UI);
        public void OnHide() => $"[UI] {nameof(LoadingScreen)} hidden".Log(LogCategory.UI);
    }
}
