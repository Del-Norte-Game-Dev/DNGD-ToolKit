using UnityEngine;
using System.Collections.Generic;

public class ObjectPool<T> where T : Component, IPoolable
{
    private T prefab;
    private Queue<T> pool = new Queue<T>();
    private Transform parent;

    public ObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            var obj = GameObject.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public T Get()
    {
        T obj = pool.Count > 0
            ? pool.Dequeue()
            : GameObject.Instantiate(prefab, parent);

        obj.gameObject.SetActive(true);
        obj.OnSpawn();

        return obj;
    }

    public void Return(T obj)
    {
        obj.OnDespawn();
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}