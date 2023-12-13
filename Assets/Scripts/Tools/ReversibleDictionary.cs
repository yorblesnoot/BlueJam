using System.Collections.Generic;

public class ReversibleDictionary<K, V> 
{
    public Dictionary<K, V> forward = new();
    public Dictionary<V, K> reverse = new();

    public int Count { get { return forward.Count; } }
    public V this[K key]
    {
        get { return forward[key]; }
    }

    public K this[V key]
    {
        get { return reverse[key]; }
    }

    public void Add(K key, V value)
    {
        forward.Add(key, value);
        reverse.Add(value, key);
    }

    public void Remove(K key)
    {
        if (!forward.TryGetValue(key, out V value)) return;
        reverse.Remove(value);
        forward.Remove(key);
    }

    public bool Contains(K key)
    {
        if(forward.ContainsKey(key)) return true;
        return false;
    }

    public bool Contains(V val)
    {
        if (reverse.ContainsKey(val)) return true;
        return false;
    }

    public bool TryGetValue(K key, out V value)
    {
        return forward.TryGetValue(key, out value);
    }

    public bool TryGetValue(V key, out K value)
    {
        return reverse.TryGetValue(key, out value);
    }
}
