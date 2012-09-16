namespace CompileThis.BawBag.Jabbr.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public interface ILookup<in TKey, out TValue>
    {
        TValue this[TKey key] { get; }
        bool Contains(TKey key);
        TValue GetValueOrDefault(TKey key);
    }

    public interface IReadOnlyLookupList<in TKey, out TValue> : IReadOnlyCollection<TValue>, ILookup<TKey, TValue>
    { }

    public interface ILookupList<in TKey, TValue> : ICollection<TValue>, ILookup<TKey, TValue>
        where TValue : class 
    {
        bool Remove(TKey key);
    }

    public class LookupList<TKey, TValue> : ILookupList<TKey, TValue>, IReadOnlyLookupList<TKey, TValue>
        where TValue : class 
    {
        private readonly List<TValue> _values;
        private readonly Dictionary<TKey, TValue> _lookup;

        private readonly Func<TValue, TKey> _keyLookup; 

        public LookupList(Func<TValue, TKey> keyLookup)
        {
            _values = new List<TValue>();
            _lookup = new Dictionary<TKey, TValue>();
            
            _keyLookup = keyLookup;
        }

        public LookupList(Func<TValue, TKey> keyLookup, int capacity)
        {
            _values = new List<TValue>(capacity);
            _lookup = new Dictionary<TKey, TValue>(capacity);

            _keyLookup = keyLookup;
        }

        public TValue this[TKey key]
        {
            get { return _lookup[key]; }
        }

        public void Add(TValue item)
        {
            var key = _keyLookup(item);

            _lookup.Add(key, item);
            _values.Add(item);
        }

        public void Clear()
        {
            _lookup.Clear();
            _values.Clear();
        }

        public bool Contains(TKey key)
        {
            return _lookup.ContainsKey(key);
        }

        public bool Contains(TValue item)
        {
            var key = _keyLookup(item);

            return Contains(key);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            _values.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _values.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(TKey key)
        {
            var value = _lookup[key];

            var removedKey = _lookup.Remove(key);
            var removedValue = _values.Remove(value);

            return removedKey || removedValue;
        }

        public bool Remove(TValue value)
        {
            var key = _keyLookup(value);

            var removedKey = _lookup.Remove(key);
            var removedValue = _values.Remove(value);

            return removedKey || removedValue;
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        public TValue GetValue(TKey key)
        {
            return _lookup[key];
        }

        public TValue GetValueOrDefault(TKey key)
        {
            TValue value;
            return _lookup.TryGetValue(key, out value) ? value : default(TValue);
        }
    }
}
