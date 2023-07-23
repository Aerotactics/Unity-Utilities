using Aero;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine
{
    public static class Extensions
    {
        public static void Assign(this Transform left, Transform right)
        {
            left.position = right.position;
            left.rotation = right.rotation;
            left.localScale = right.localScale;
        }

        public static void Assign(this Transform left, Aero.TransformData right)
        {
            left.position = right.position;
            left.rotation = right.rotation;
            left.localScale = right.scale;
        }

        public static void Zero(this Transform transform)
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.zero;
        }

        public static void DestroyAndClear(this List<GameObject> list)
        {
            int count = list.Count;
            for (int i = 0; i < count; ++i)
            {
                UnityEngine.GameObject.Destroy(list[i]);
            }
            list.Clear();
        }

        // https://en.wikipedia.org/wiki/Fisher-Yates_shuffle
        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = list.Count - 1; i > 0; --i)
            {
                int j = Random.Range(0, i + 1);
                T t = list[j];
                list[j] = list[i];
                list[i] = t;
            }
        }
    }
}
