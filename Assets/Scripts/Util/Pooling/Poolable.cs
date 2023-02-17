using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RotatingRoutes.Util.ObjectPooling
{
    /// <summary>
    /// Poolable.
    /// Make sure only one Poolable object is attached to the GameObject
    /// </summary>
    [System.Serializable]
    public abstract class Poolable<T> : MonoBehaviour where T : Component
    {
        private class ObjectPool : IPool<T>
        {
            private T _original;
            private Queue<Poolable<T>> _objects = new();
            private const int MinSize = 5;

            private Transform _poolParent;

            public void SetParent(Transform parent) => _poolParent = parent;
            public void SetOriginal(T original)
            {
                if (original == null)
                    return;

                _original = original;
            }

            public T Get()
            {
                Poolable<T> obj;
                do
                {
                    if (!_objects.Any())
                        Fill();
                    obj = _objects.Dequeue();
                } while (obj == null);

                obj._isInPool = false;
                obj.gameObject.SetActive(true);
                return obj as T;
            }

            public void Clear()
            {
                while (_objects.Count > 0)
                    Destroy(_objects.Dequeue());
            }

            public void Add(Poolable<T> obj)
            {
                _objects.Enqueue(obj);
                obj._isInPool = true;
                obj.gameObject.SetActive(false);
            }

            private void Fill()
            {
                if (_poolParent == null)
                    _poolParent = new GameObject(_original.GetType().Name + " Pool").transform;

                while (_objects.Count < MinSize)
                    Add(Instantiate(_original.gameObject, _poolParent).GetComponent<Poolable<T>>());
            }

            public void DeleteFromPool(Poolable<T> obj)
            {
                var queue = new Queue<Poolable<T>>();
                while (_objects.Count > 0)
                {
                    var o = _objects.Dequeue();
                    int first = o.gameObject.GetInstanceID();
                    int second = obj.gameObject.GetInstanceID();
                    if (first != second)
                        queue.Enqueue(o);
                }

                _objects = queue;
            }
        }

        private bool _isInPool;

        private static readonly ObjectPool _pool = new();

        public static IPool<T> Pool => _pool;

        /// <summary>
        /// Revert object to start state.
        /// </summary>
        public abstract void Reset();

        public void MoveToPool()
        {
            if (_isInPool)
                throw new InvalidOperationException("Object is already in the pool");
            _pool.Add(this);
            Reset();
        }

    }
}