using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public class JsonDataService : IDataService
    {
        private readonly Dictionary<string, object> _cache = new();
        private string _savesDir;

        public InitPriority Priority => InitPriority.Critical;
        public bool IsReady { get; private set; }

        public void Initialize()
        {
            _savesDir = Path.Combine(Application.persistentDataPath, "saves");
            "[Lifecycle] DataService initialized".Log(LogCategory.Lifecycle);
        }

        public async Task StartAsync()
        {
            if (!Directory.Exists(_savesDir))
                Directory.CreateDirectory(_savesDir);
            IsReady = true;
            "[Lifecycle] DataService ready".Log(LogCategory.Lifecycle);
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            "[Lifecycle] DataService disposed".Log(LogCategory.Lifecycle);
            _cache.Clear();
            IsReady = false;
        }

        public async Task<T> LoadAsync<T>(string key) where T : class, new()
        {
            if (_cache.TryGetValue(key, out var cached))
                return (T)cached;

            var path = GetPath(key);
            if (!File.Exists(path))
                return new T();

            $"[Data] Loading {key}".Log(LogCategory.Data);
            var json = await Task.Run(() => File.ReadAllText(path));
            var data = JsonUtility.FromJson<T>(json);
            _cache[key] = data;
            return data;
        }

        public async Task SaveAsync<T>(string key, T data) where T : class
        {
            _cache[key] = data;
            var json = JsonUtility.ToJson(data, prettyPrint: true);
            var path = GetPath(key);
            $"[Data] Saving {key}".Log(LogCategory.Data);
            await Task.Run(() => File.WriteAllText(path, json));
        }

        public bool HasKey(string key) => File.Exists(GetPath(key));

        public void DeleteKey(string key)
        {
            _cache.Remove(key);
            var path = GetPath(key);
            if (File.Exists(path))
                File.Delete(path);
        }

        private string GetPath(string key) => Path.Combine(_savesDir, $"{key}.json");
    }
}
