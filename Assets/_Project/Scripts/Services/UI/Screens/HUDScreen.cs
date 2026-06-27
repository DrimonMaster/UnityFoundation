using TMPro;
using UnityEngine;

namespace UnityFoundation.Services
{
    public class HUDScreen : MonoBehaviour, IScreen
    {
        public ScreenLayer Layer => ScreenLayer.HUD;

        private void Awake()
        {
            var textGO = new GameObject("Label", typeof(RectTransform));
            textGO.transform.SetParent(transform, false);
            var rt = (RectTransform)textGO.transform;
            rt.anchorMin = new Vector2(0f, 0.9f);
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            var tmp = textGO.AddComponent<TextMeshProUGUI>();
            tmp.text = "HUD";
            tmp.fontSize = 24;
            tmp.alignment = TextAlignmentOptions.TopRight;
            tmp.color = Color.green;
        }

        public void OnShow() => $"[UI] {nameof(HUDScreen)} shown".Log(LogCategory.UI);
        public void OnHide() => $"[UI] {nameof(HUDScreen)} hidden".Log(LogCategory.UI);
    }
}
