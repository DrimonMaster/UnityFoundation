using System.Collections.Generic;
using System.Threading.Tasks;
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public class DataService : IDataService
    {
        private readonly Dictionary<string, object> _cache = new();

        public InitPriority Priority => InitPriority.Critical;
        public bool IsReady { get; private set; }

        public void Initialize() { }

        public async Task StartAsync()
        {
            // TODO: deserialize save file from disk into _cache
            await Task.CompletedTask;
            IsReady = true;
        }

        public void Dispose()
        {
            _cache.Clear();
            IsReady = false;
        }

        public bool TryLoad<T>(string key, out T data) where T : class
        {
            if (_cache.TryGetValue(key, out var obj) && obj is T typed)
            {
                data = typed;
                return true;
            }
            data = null;
            return false;
        }

        public void Save<T>(string key, T data) where T : class
        {
            _cache[key] = data;
            // TODO: persist to disk
        }
    }
}
