using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

namespace RotatingRoutes.Util.Extensions
{
    public static class ExtensionMethods
    {
        public static IEnumerable<T> ReverseIf<T>(this IEnumerable<T> value, bool condition)
        {
            if (condition)
                value = value.Reverse();
            return value;
        }

        public static void RefreshLayoutGroupsImmediateAndRecursive(this RectTransform root)
        {
            foreach (var layoutGroup in root.GetComponentsInChildren<LayoutGroup>())
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
            }
        }

        public static T RandomElement<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            // note: creating a Random instance each call may not be correct for you,
            // consider a thread-safe static instance
            var r = new System.Random();
            var list = enumerable as IList<T> ?? enumerable.ToList();
            return list.Count == 0 ? default(T) : list[r.Next(0, list.Count)];
        }

        public static bool ContainsLayer(this LayerMask layermask, int layer)
        {
            return layermask == (layermask | (1 << layer));
        }
        public static void SetGameLayerRecursive(this GameObject initialGO, int layerValue)
        {
            initialGO.layer = layerValue;
            foreach (Transform child in initialGO.transform)
            {
                child.gameObject.layer = layerValue;

                Transform hasChildren = child.GetComponentInChildren<Transform>(true);
                if (hasChildren != null)
                    SetGameLayerRecursive(child.gameObject, layerValue);
            }
        }
        public static int GetHighestLayer(this LayerMask layermask)
        {
            int result = layermask.value > 0 ? 0 : 31;
            while (layermask.value > 1)
            {
                layermask.value >>= 1;
                result++;
            }
            return result;
        }
        public static void DestroyChildren(this Transform self, bool immediate = false)
        {
            for (int i = self.childCount - 1; i >= 0; i--)
            {
                if (immediate)
                    Object.DestroyImmediate(self.GetChild(i).gameObject);
                else
                    Object.Destroy(self.GetChild(i).gameObject);
            }
        }
        public static void DisableChildren(this Transform self)
        {
            foreach (Transform child in self)
                child.gameObject.SetActive(false);
        }
        public static string[] SplitCamelCase(this string self)
        {
            return Regex.Split(self, @"(?<!^)(?=[A-Z])");
        }

        ///<summary>2.Remap(1, 3, 0, 10) = 5 </summary>
        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return Mathf.Clamp(
                (value - from1) / (to1 - from1) * (to2 - from2) + from2,
                Mathf.Min(from2, to2),
                Mathf.Max(from2, to2)
            );
        }

        public static float RandomSign(this float value)
        {
            return UnityEngine.Random.Range(0, 2) == 0 ? value : -value;
        }

        public static string[] SelectedElementsNames(this Enum value) => value.ToString().Split(", ", StringSplitOptions.RemoveEmptyEntries);

        public static List<T> ToList<T>(this T[] self)
        {
            int size = self.Length;
            List<T> result = new List<T>(size);
            for (int i = 0; i < size; i++)
                result.Add(self[i]);
            return result;
        }

        public static void AddIfNotNull<T>(this List<T> self, T value)
        {
            if (value == null)
                return;
            self.Add(value);
        }

        public static T Parse<T>(this DataRow self, string name)
        {
            var typeConverter = TypeDescriptor.GetConverter(typeof(T));
            return (T)typeConverter.ConvertFromInvariantString(self[name].ToString());
        }

        #region Vectors

        public static Vector2 ToVector2(this Vector3 self) => new Vector2(self.x, self.y);

        public static Vector3 ToVector3(this Vector2 self) => new Vector3(self.x, self.y, 0f);

        public static bool IsNaN(this Vector2 self) => float.IsNaN(self.x) || float.IsNaN(self.y);

        public static bool IsNaN(this Vector3 self) => float.IsNaN(self.x) || float.IsNaN(self.y) || float.IsNaN(self.z);

        public static Vector2 Inverse(this Vector2 self) => new Vector2(1 / self.x, 1 / self.y);

        public static Vector3 Inverse(this Vector3 self) => new Vector3(1 / self.x, 1 / self.y, 1 / self.y);

        public static Vector3 NormalizeAngle(this Vector3 eulerAngle)
        {
            var delta = eulerAngle;

            if (delta.x > 180)
                delta.x -= 360;
            else if (delta.x < -180)
                delta.x += 360;

            if (delta.y > 180)
                delta.y -= 360;
            else if (delta.y < -180)
                delta.y += 360;

            if (delta.z > 180)
                delta.z -= 360;
            else if (delta.z < -180)
                delta.z += 360;

            return new Vector3(delta.x, delta.y, delta.z);
        }
        #endregion

        #region Quaternions

        public static float Pitch(this Quaternion q) =>
            Mathf.Rad2Deg * Mathf.Atan2(2 * q.x * q.w - 2 * q.y * q.z, 1 - 2 * q.x * q.x - 2 * q.z * q.z);

        public static float Yaw(this Quaternion q) =>
            Mathf.Rad2Deg * Mathf.Atan2(2 * q.y * q.w - 2 * q.x * q.z, 1 - 2 * q.y * q.y - 2 * q.z * q.z);

        public static float Roll(this Quaternion q) =>
            Mathf.Rad2Deg * Mathf.Asin(2 * q.x * q.y + 2 * q.z * q.w);

        #endregion

        #region Coroutines

        public static IEnumerator Timer<T>(this T self, float delay, Action<T> action)
        {
            yield return new WaitForSeconds(delay);
            action.Invoke(self);
        }

        /// <summary>
        /// Waits for given number of seconds in unscaled time and then invokes given action
        /// </summary>
        public static IEnumerator UnscaledTimer<T>(this T self, float delay, Action<T> action)
        {
            yield return new WaitForSecondsRealtime(delay);
            action.Invoke(self);
        }

        public static IEnumerator InvokeAfterDelay<T>(this T self, float delay, Action<T> action)
        {
            yield return new WaitForSeconds(delay);
            action.Invoke(self);
            yield return null;
        }

        private static IEnumerator WaitAndInvoke(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action();
        }

        public static IEnumerator InvokeAfterDelay(this MonoBehaviour self, float delay, Action action)
        {
            var enumerator = WaitAndInvoke(delay, action);
            self.StartCoroutine(enumerator);
            return enumerator;
        }

        /// <summary>
        /// Invokes method after time and stops previously invoked coroutine
        /// </summary>
        public static void SafeInvoke(this MonoBehaviour self, ref IEnumerator coroutine, Action action,
            float delay = 0f)
        {
            if (coroutine != null)
                self.StopCoroutine(coroutine);
            coroutine = self.InvokeAfterDelay(delay, action);
        }

        public static void SafeStopCoroutine(this MonoBehaviour self, ref IEnumerator coroutine)
        {
            if (coroutine != null)
                self.StopCoroutine(coroutine);
        }

        #endregion

        #region Components

        public static T GetComponentOnlyInChildren<T>(this Component self) where T : Component
        {
            foreach (Transform obj in self.transform)
            {
                var comp = obj.GetComponentInChildren<T>();
                if (comp != null)
                    return comp;
            }

            return null;
        }

        public static T GetComponentOnlyInChildren<T>(this GameObject self) where T : Component
            => self.transform.GetComponentOnlyInChildren<T>();

        public static List<T> GetComponentsOnlyInChildren<T>(this Component self)
        {
            var ret = new List<T>();
            foreach (Transform obj in self.transform)
                ret.AddRange(obj.GetComponentsInChildren<T>());
            return ret;
        }

        public static List<T> GetComponentsOnlyInChildren<T>(this GameObject self)
            => self.transform.GetComponentsOnlyInChildren<T>();

        public static GameObject[] FindGameObjectsInChildrenWithTag(this Transform self, string tag)
            => self.TraverseThisAndChildren(x => x.CompareTag(tag) ? x.gameObject : null).ToArray();

        public static GameObject[] FindGameObjectsInChildrenWithTag(this GameObject self, string tag)
            => self.transform.FindGameObjectsInChildrenWithTag(tag);

        public static GameObject[] FindGameObjectsInChildrenWithTags(this Transform self, List<string> tags)
            => self.TraverseThisAndChildren(x => x.CompareTags(tags) ? x.gameObject : null).ToArray();

        public static GameObject[] FindGameObjectsInChildrenWithTags(this GameObject self, List<string> tags)
            => self.transform.FindGameObjectsInChildrenWithTags(tags);

        public static bool CompareTags(this Transform self, List<string> tags) => tags.Any(self.CompareTag);

        public static List<Transform> Children(this Transform self)
        {
            var children = new List<Transform>();
            foreach (Transform tr in self)
                children.Add(tr);
            return children;
        }

        public static Transform GetLastChild(this Transform self)
            => self.childCount > 0 ? self.GetChild(self.childCount - 1) : null;

        public static bool IsLastSibling(this Transform self)
            => self.parent == null || self.GetSiblingIndex() == self.parent.childCount - 1;

        #endregion

        public static T Null<T>(this T self) where T : Object => self == null ? null : self;

        public static GameObject Null(this GameObject self) => self == null ? null : self;

        private static List<T> TraverseThisAndChildren<T>(this Transform self, Func<Transform, T> predicate)
        {
            var objs = new List<T>();
            void Add(T x) { if (x != null) objs.Add(x); }
            Add(predicate(self));
            foreach (Transform t in self)
            {
                Add(predicate(t));
                objs.AddRange(t.TraverseThisAndChildren(predicate));
            }

            return objs;
        }
    }
}