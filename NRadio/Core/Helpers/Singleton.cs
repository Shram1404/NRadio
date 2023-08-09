using System;
using System.Collections.Concurrent;

namespace NRadio.Core.Helpers
{
    public static class Singleton<T>
        where T : new()
    {
        private static ConcurrentDictionary<Type, T> instances = new ConcurrentDictionary<Type, T>();

        public static T Instance
        {
            get
            {
                return instances.GetOrAdd(typeof(T), (t) => new T());
            }
        }
    }
}
