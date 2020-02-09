using System.Collections.Generic;

namespace Core.Common
{
    public class MultiValueDictionary<TKey, TValue> : Dictionary<TKey, List<TValue>>
    {
        public void Add(TKey key, TValue value)
        {
            if (!this.ContainsKey(key))
                this.Add(key, new List<TValue>());

            if (!this[key].Contains(value))
                this[key].Add(value);
        }

        public void AddValues(TKey key, IEnumerable<TValue> values)
        {
            if (values != null)
            {
                foreach (var val in values)
                    this.Add(key, val);
            }
        }

        public void Clear(TKey key)
        {
            if (this.ContainsKey(key))
                this[key].Clear();
        }

        public bool ContainsValue(TValue value)
        {
            bool ret = false;

            var enumerator = this.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var list = enumerator.Current.Value;
                if (list.Contains(value))
                {
                    ret = true;
                    break;
                }
            }

            return ret;
        }

        public bool Remove(TKey key, TValue value)
        {
            bool ret = false;

            if (this.ContainsKey(key) && this[key].Contains(value))
                ret = this[key].Remove(value);

            return ret;
        }

        public string GetStringValue(TKey key, string joinParam = ";")
        {
            if (!this.ContainsKey(key))
                return null;

            return string.Join(joinParam, this[key]);
        }
    }
}
