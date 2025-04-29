using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlanetTweaks.Utils;

public class IndexedDictionary<K, V> : IDictionary<K, V> {
    public V this[K key] {
        get {
            int index = Keys.IndexOf(key);
            return index == -1 ? default : Values[index];
        }
        set {
            int index = Keys.IndexOf(key);
            if(index != -1) Values[index] = value;
        }
    }

    ICollection<K> IDictionary<K, V>.Keys => Keys;
    ICollection<V> IDictionary<K, V>.Values => Values;

    public List<K> Keys = [];
    public List<V> Values = [];

    public int Count => Keys.Count;

    public bool IsReadOnly => false;

    public void Add(K key, V value) {
        if(ContainsKey(key)) throw new ArgumentException("item already exists!");
        Keys.Add(key);
        Values.Add(value);
    }

    public void Add(KeyValuePair<K, V> item) {
        Add(item.Key, item.Value);
    }

    public void Insert(int index, K key, V value) {
        if(ContainsKey(key)) throw new ArgumentException("item already exists!");
        Keys.Insert(index, key);
        Values.Insert(index, value);
    }

    public void Insert(int index, KeyValuePair<K, V> item) {
        Insert(index, item.Key, item.Value);
    }

    public void Replace(int index, K key, V value) {
        int i = Keys.IndexOf(key);
        if(i != -1 && i != index) throw new ArgumentException("item already exists!");
        Keys[index] = key;
        Values[index] = value;
    }

    public void Replace(int index, KeyValuePair<K, V> item) {
        Replace(index, item.Key, item.Value);
    }

    public void Replace(K prevKey, K key, V value) {
        int index = Keys.IndexOf(prevKey);
        if(index != -1) Replace(index, key, value);
    }

    public void Replace(K prevKey, KeyValuePair<K, V> item) {
        Replace(prevKey, item.Key, item.Value);
    }

    public void Clear() {
        Keys.Clear();
        Values.Clear();
    }

    public bool Contains(KeyValuePair<K, V> item) {
        return this[item.Key]?.Equals(item.Value) ?? false;
    }

    public bool ContainsKey(K key) {
        return Keys.Contains(key);
    }

    public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) => throw new NotImplementedException();

    public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => Keys.Select((t, i) => new KeyValuePair<K, V>(t, Values[i])).GetEnumerator();

    public bool Remove(K key) {
        int index = Keys.IndexOf(key);
        if(index == -1) return false;
        Keys.RemoveAt(index);
        Values.RemoveAt(index);
        return true;
    }

    public bool Remove(KeyValuePair<K, V> item) => Remove(item.Key);

    public bool TryGetValue(K key, out V value) {
        int index = Keys.IndexOf(key);
        if(index == -1) {
            value = default;
            return false;
        }
        value = Values[index];
        return true;
    }

    public KeyValuePair<K, V> ElementAt(int index) => new(Keys[index], Values[index]);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}