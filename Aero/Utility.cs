using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Encoding = System.Text.Encoding;

using GameObject = UnityEngine.GameObject;
using Coroutine = UnityEngine.Coroutine;
using System.Linq;

namespace Aero
{
    public static class Utility
    {
        private const int kSecondsInADay = 86400;
        private const int kSecondsInAnHour = 3600;
        private const int kSecondsInAnMinute = 60;
        private const int kUILayer = 5;

        // Set this to any component instance
        private static MonoBehaviour s_monoBehaviour = GameManager.instance; 
        private static Dictionary<GameObject, Coroutine> s_panelRoutines = new Dictionary<GameObject, Coroutine>();

        public enum TimeScale
        {
            kDays,
            kHours,
            kMinutes,
            kSeconds,
        }

        public static string TotalSecondsToTimeDisplay(double totalSeconds, TimeScale timeScale)
        {
            double seconds = totalSeconds % 60f;
            if (timeScale == TimeScale.kSeconds)
                return seconds.ToString("00.00");

            int days = (int)totalSeconds / kSecondsInADay;
            int hours = (int)totalSeconds / kSecondsInAnHour - (days * kSecondsInADay);
            int minutes = (int)totalSeconds / kSecondsInAnMinute - (hours * kSecondsInAnHour);
            if (timeScale == TimeScale.kDays)
                return days.ToString("D2") + hours.ToString("D2") + ":" + minutes.ToString("D2") + ":" + seconds.ToString("00.00");
            if (timeScale == TimeScale.kHours)
                return hours.ToString("D2") + ":" + minutes.ToString("D2") + ":" + seconds.ToString("00.00");
            else if (timeScale == TimeScale.kMinutes)
                return minutes.ToString("D2") + ":" + seconds.ToString("00.00");

            return String.Empty; 
        }

        public static void Refresh(GameObject gameObject)
        {
            Coroutine coroutine = null;
            if (s_panelRoutines.TryGetValue(gameObject, out coroutine) && coroutine != null)
                s_monoBehaviour.StopCoroutine(coroutine);

            s_panelRoutines[gameObject] = s_monoBehaviour.StartCoroutine(RefreshRoutine(gameObject));
        }

        private static IEnumerator RefreshRoutine(GameObject gameObject)
        {
            gameObject.SetActive(true);
            yield return null;
            gameObject.SetActive(false);
            gameObject.SetActive(true);
            yield return null;
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        public static Vector3 ScreenToWorldPosition2D(Vector3 screenPosition)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 1;
            return worldPosition;
        }

        public static void SaveToFile(string filename, string data)
        {
            FileStream file = File.Open(filename, FileMode.Create);
            file.Write(Encoding.ASCII.GetBytes(data), 0, data.Length);
            file.Close();
        }

        public static string LoadFromFile(string filename)
        {
            FileStream file = File.Open(filename, FileMode.OpenOrCreate);
            byte[] byteArray = new byte[file.Length];
            file.Read(byteArray, 0, (int)file.Length);
            file.Close();
            return Encoding.ASCII.GetString(byteArray);
        }

        // Returns the first hit, or null if didn't hit UI
        public static GameObject IsMouseOverUI()
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = UnityEngine.Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, results);
            foreach (var result in results)
            {
                if (result.gameObject.layer == kUILayer)
                    return result.gameObject;
            }
            return null;
        }

        public static string Plural(string singular, string plural, int count)
        {
            return count == 1 ? singular : plural;
        }

        // Cast check for determining all colliders within a cone. It begins with OverlapCircle, 
        //  then filters out colliders ouside the given angle.
        // https://github.com/walterellisfun/ConeCast/blob/master/ConeCastExtension.cs
        // I didn't like the above implementation because it resulted in a distorted cone, and I preferred
        //  a rounded cone. However, I did reference it.
        //
        //  range - radius of overlap (results in a nice rounded cone)
        //  direction - center of cone, outwards
        //  origin - where the cone begins
        //  angle - measured from left of cone to right of cone
        //  Returns: array of applicable colliders
        public static Collider2D[] ConeCastAll(Vector2 origin, float range, Vector2 direction, float halfAngle, Collider2D[] ignoreList = null)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, range);
            List<Collider2D> inCone = new List<Collider2D>();
            foreach (var hit in hits)
            {
                Vector2 point = hit.ClosestPoint(origin);
                Vector2 directionToHit = point - origin;
                float angle = Vector2.Angle(direction, directionToHit.normalized);
                if (angle < halfAngle)
                {
                    if (ignoreList != null && ignoreList.Contains(hit))
                        continue;

                    inCone.Add(hit);
                }
            }
            return inCone.ToArray();
        }
    }

    public static class ColorUtility
    {
        static GradientColorKey[] s_durabilityColors =
        {
            new GradientColorKey(Color.cyan, 1),
            new GradientColorKey(Color.green, 0.9f),
            new GradientColorKey(Color.yellow, 0.5f),
            new GradientColorKey(Color.red, 0)
        };
        static GradientColorKey[] s_rainbowColors =
        {
            new GradientColorKey(Color.red, 0),
            new GradientColorKey(Color.yellow, .18f),
            new GradientColorKey(Color.green, .36f),
            new GradientColorKey(Color.cyan, .52f),
            new GradientColorKey(Color.blue, .70f),
            new GradientColorKey(Color.magenta, .85f),
            new GradientColorKey(Color.red, 1)
        };
        static GradientAlphaKey[] s_solidGradientAlpha =
        {
            new GradientAlphaKey(1, 0),
            new GradientAlphaKey(1, 1),
        };

        static Gradient s_gradient = new Gradient();

        public static Color DurabilityColor(float percentage)
        {
            s_gradient.SetKeys(s_durabilityColors, s_solidGradientAlpha);
            return s_gradient.Evaluate(Mathf.Clamp01(percentage));
        }

        public static Color RainbowColor(float percentage)
        {
            s_gradient.SetKeys(s_rainbowColors, s_solidGradientAlpha);
            return s_gradient.Evaluate(Mathf.Clamp01(percentage));
        }
    }

    public struct ButtonData
    {
        public string name;
        public UnityAction callback;

        public ButtonData(string name, UnityAction callback)
        {
            this.name = name;
            this.callback = callback;
        }
    }

    public struct TransformData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public void Zero()
        {
            position = Vector3.zero;
            rotation = Quaternion.identity;
            scale = Vector3.one;
        }

        public void Assign(UnityEngine.Transform transform)
        {
            position = transform.position;
            rotation = transform.rotation;
            scale = transform.localScale;
        }

        public static bool operator ==(TransformData left, TransformData right)
        {
            return left.position == right.position && left.rotation == right.rotation && left.scale == right.scale;
        }

        public static bool operator !=(TransformData left, TransformData right)
        {
            return !(left == right);
        }

        public static bool operator ==(UnityEngine.Transform left, TransformData right)
        {
            return left.position == right.position && left.rotation == right.rotation && left.localScale == right.scale;
        }

        public static bool operator !=(UnityEngine.Transform left, TransformData right)
        {
            return !(left == right);
        }

        public static bool operator ==(TransformData left, UnityEngine.Transform right)
        {
            return right == left;
        }

        public static bool operator !=(TransformData left, UnityEngine.Transform right)
        {
            return !(right == left);
        }

        public override string ToString()
        {
            return $"({position}), ({rotation}), ({scale})";
        }
    }
}
