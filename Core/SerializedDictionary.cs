using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flux.Core
{
    [Serializable]
    public class SerializedDictionary<TKey, TValue> : IEnumerable
    {
        [field: SerializeField] public List<TKey> Keys { get; private set; } = new();
        [field: SerializeField] public List<TValue> Values { get; private set; } = new();

        public int Count { get { return Keys.Count; } }

        public SerializedDictionary(params (TKey, TValue)[] inputs)
        {
            foreach ((TKey key, TValue value) in inputs)
            {
                Add(key, value);
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (Keys.Contains(key))
                {
                    return Values[Keys.IndexOf(key)];
                }
                else
                {
                    return default;
                }
            }

            set
            {
                if (Keys.Contains(key))
                {
                    Values[Keys.IndexOf(key)] = value;
                }
                else
                {
                    Add_Internal(key, value);
                }
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = Keys.IndexOf(key);

            if (index != -1)
            {
                value = Values[index];
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (Keys.Contains(key))
            {
                return;
            }

            Add_Internal(key, value);
        }

        private void Add_Internal(TKey key, TValue value)
        {
            Keys.Add(key);
            Values.Add(value);
        }

        public void Remove(TKey key)
        {
            if (!Keys.Contains(key))
            {
                return;
            }

            int index = Keys.IndexOf(key);
            Keys.RemoveAt(index);
            Values.RemoveAt(index);
        }

        public void Clear()
        {
            Keys.Clear();
            Values.Clear();
        }

        public IEnumerator<(TKey, TValue)> GetEnumerator()
        {
            for (int i = 0; i < Keys.Count; i++)
            {
                yield return (Keys[i], Values[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}