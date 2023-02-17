using UnityEngine;

namespace RotatingRoutes.Util.ObjectPooling
{
    public interface IPool<T> where T: Component
    {
        void SetOriginal(T original);
        void SetParent(Transform parent);
        T Get();
        void Clear();
    }
}