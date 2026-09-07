using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private readonly Func<T> _factory;
    private readonly Transform _container;
    private readonly Stack<T> _items = new Stack<T>();

    public ObjectPool(Func<T> factory, Transform container)
    {
        _factory = factory;
        _container = container;
    }

    public void Prewarm(int count)
    {
        if (count <= 0)
            return;

        for (int index = 0; index < count; index++)
        {
            T item = CreateItem();
            _items.Push(item);
        }
    }

    public T Rent(Vector3 position, Quaternion rotation)
    {
        T item = _items.Count > 0 ? _items.Pop() : CreateItem();

        item.transform.SetParent(null);
        item.transform.SetPositionAndRotation(position, rotation);
        item.gameObject.SetActive(true);

        return item;
    }

    public void Return(T item)
    {
        if (item == null)
            return;

        item.gameObject.SetActive(false);
        item.transform.SetParent(_container);

        _items.Push(item);
    }

    private T CreateItem()
    {
        T item = _factory();
        item.gameObject.SetActive(false);
        item.transform.SetParent(_container);

        return item;
    }
}