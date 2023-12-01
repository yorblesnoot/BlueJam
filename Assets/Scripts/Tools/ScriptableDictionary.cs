using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScriptableDictionary<K, V> : ScriptableObject
{
    public int Count { get { return entries.Count; } }
    public IEnumerable<K> Keys { get { return entries.Select(x => x.key); } }
    public IEnumerable<V> Values { get { return entries.Select(x => x.value); } }
    public V this[K key]
    {
        get { return GetOutput(key); }
    }

    [SerializeField] List<Entry> entries;

    Dictionary<K, V> dictionary;

    [Serializable]
    class Entry
    {
        public K key;
        public V value;
    }

    V GetOutput(K key)
    {
        if (dictionary == null)
        {
            dictionary = new();
            foreach (Entry entry in entries)
            {
                dictionary.Add(entry.key, entry.value);
            }
        }
        return dictionary[key];
    }
}
