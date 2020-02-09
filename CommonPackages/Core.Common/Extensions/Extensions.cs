using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace Core.Common
{
    public static class Extensions
    {
        public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dict, Action<TKey, TValue> action)
        {
            if (dict == null || dict.Count == 0 || action == null)
                return;

            foreach (var kvp in dict)
                action(kvp.Key, kvp.Value);
        }

        public static void SplitAndAddValues<TKey>(this MultiValueDictionary<TKey, string> multiDict, TKey key, string values, string delimiter = ";")
        {
            var vals = values.Split(delimiter);
            multiDict.AddValues(key, vals);
        }

        public static T GetAndSet<T>(this Interlock<T> objValue, T value)
        {
            var val = objValue.Value;

            Interlocked.Exchange(ref objValue, value);

            return val;
        }

        public static void AddSeperately<TKey>(this IDictionary<TKey, StringValues> multiDict, TKey key, StringValues values)
        {
            if (multiDict == null)
                return;

            if(!multiDict.ContainsKey(key))
                multiDict.Add(key, values);
            else
            {
                var vals = multiDict[key].ToList();
                vals.AddRange(values);
                multiDict[key] = vals.Distinct().ToArray();
            }
        }

        public static void AddSeperately<TKey>(this IDictionary<TKey, StringValues> multiDict, IDictionary<TKey, StringValues> dictToAdd)
        {
            if (multiDict == null || dictToAdd == null)
                return;

            foreach (var kvp in dictToAdd)
                multiDict.AddSeperately(kvp.Key, kvp.Value);
        }

        public static Dictionary<TKey, TValue> CreateDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            if (dict == null)
                return null;

            Dictionary<TKey, TValue> retDict = new Dictionary<TKey, TValue>();

            foreach (var kvp in dict)
                retDict.Add(kvp.Key, kvp.Value);

            return retDict;
        }
    }
}
