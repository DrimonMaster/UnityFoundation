using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFoundation.Services
{
    [CreateAssetMenu(fileName = "LogSettings", menuName = "UnityFoundation/LogSettings")]
    public class LogSettings : ScriptableObject
    {
        [Serializable]
        public class CategorySettings
        {
            public LogCategory Category;
            public bool  EnabledInEditor = true;
            public bool  EnabledInBuild  = true;
            public Color Color           = Color.white;
        }

        [SerializeField] private List<CategorySettings> _categories = new();

        private Dictionary<LogCategory, CategorySettings> _lookup;

        private void OnEnable()
        {
            if (_categories.Count == 0)
                PopulateDefaults();
            BuildLookup();
        }

        public bool IsEnabled(LogCategory category)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (_lookup == null) BuildLookup();
            if (!_lookup.TryGetValue(category, out var s)) return true;
#if UNITY_EDITOR
            return s.EnabledInEditor;
#else
            return s.EnabledInBuild;
#endif
#else
            return false;
#endif
        }

        public string BuildMessage(object message, LogCategory category)
        {
            if (_lookup == null) BuildLookup();
            if (!_lookup.TryGetValue(category, out var s))
                return $"[{category}] {message}";

            var hex = ColorUtility.ToHtmlStringRGB(s.Color);
            return $"<color=#{hex}><b>[{category}]</b></color> {message}";
        }

        private void PopulateDefaults()
        {
            var colors = new Dictionary<LogCategory, Color>
            {
                { LogCategory.Core,      Color.white                  },
                { LogCategory.UI,        Color.cyan                   },
                { LogCategory.Network,   Color.yellow                 },
                { LogCategory.Data,      Color.green                  },
                { LogCategory.GameState, Color.magenta                },
                { LogCategory.Bootstrap, new Color(1f, 0.5f, 0f)     }, // orange
                { LogCategory.Event,     Color.red                    },
            };

            foreach (LogCategory cat in Enum.GetValues(typeof(LogCategory)))
            {
                var color = colors.TryGetValue(cat, out var c) ? c : Color.white;
                _categories.Add(new CategorySettings { Category = cat, Color = color });
            }
        }

        private void BuildLookup()
        {
            _lookup = new Dictionary<LogCategory, CategorySettings>();
            foreach (var entry in _categories)
                _lookup[entry.Category] = entry;
        }
    }
}
