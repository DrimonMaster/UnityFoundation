using TMPro;
using UnityEngine;

namespace UnityFoundation.Services
{
    public class GameOverScreen : MonoBehaviour, IScreen
    {
        private void Awake()
        {
            var textGO = new GameObject("Label", typeof(RectTransform));
            textGO.transform.SetParent(transform, false);
            var rt = (RectTransform)textGO.transform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            var tmp = textGO.AddComponent<TextMeshProUGUI>();
            tmp.text = "Game Over";
            tmp.fontSize = 48;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
        }

        public void OnShow() => $"[UI] {nameof(GameOverScreen)} shown".Log(LogCategory.UI);
        public void OnHide() => $"[UI] {nameof(GameOverScreen)} hidden".Log(LogCategory.UI);
    }
}
