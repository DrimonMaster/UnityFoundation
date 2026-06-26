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
            public bool EnabledInEditor = true;
            public bool EnabledInBuild  = false;
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

        private void PopulateDefaults()
        {
            foreach (LogCategory cat in Enum.GetValues(typeof(LogCategory)))
                _categories.Add(new CategorySettings { Category = cat });
        }

        private void BuildLookup()
        {
            _lookup = new Dictionary<LogCategory, CategorySettings>();
            foreach (var entry in _categories)
                _lookup[entry.Category] = entry;
        }
    }
}
