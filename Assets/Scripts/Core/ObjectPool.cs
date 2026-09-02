using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private readonly Func<T> _factory;
    private readonly Transform _container;
    private readonly Action<T> _onGet;
    private readonly Action<T> _onReturn;
    private readonly Stack<T> _items = new Stack<T>();

    public ObjectPool(Func<T> factory, Transform container, Action<T> onGet, Action<T> onReturn)
    {
        _factory = factory;
        _container = container;
        _onGet = onGet;
        _onReturn = onReturn;
    }

    public void Prewarm(int count)
    {
        for (int index = 0; index < count; index++)
        {
            T item = _factory();
            item.gameObject.SetActive(false);
            item.transform.SetParent(_container);
            _items.Push(item);
        }
    }

    public T Rent(Vector3 position, Quaternion rotation)
    {
        T item = _items.Count > 0 ? _items.Pop() : _factory();

        item.transform.SetParent(null);
        item.transform.SetPositionAndRotation(position, rotation);
        item.gameObject.SetActive(true);

        _onGet?.Invoke(item);

        return item;
    }

    public void Return(T item)
    {
        _onReturn?.Invoke(item);

        item.gameObject.SetActive(false);
        item.transform.SetParent(_container);

        _items.Push(item);
    }
}