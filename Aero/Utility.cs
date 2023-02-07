using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using System.IO;
using Encoding = System.Text.Encoding;

namespace Aero
{
    public static class Utility
    {
        private const int kSecondsInADay = 86400;
        private const int kSecondsInAnHour = 3600;
        private const int kSecondsInAnMinute = 60;
        private const int kUILayer = 5;

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

        public static Vector3 ScreenToWorldPosition2D(Vector3 screenPosition)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 1;
            return worldPosition;
        }

        public static void DestroyAndClear(List<GameObject> list)
        {
            int count = list.Count;
            for (int i = 0; i < count; ++i)
            {
                UnityEngine.GameObject.Destroy(list[i]);
            }
            list.Clear();
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

#pragma warning disable 
    public struct Transform
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public static Aero.Transform New = new Aero.Transform();

        public void Assign(UnityEngine.Transform other)
        {
            position = other.position;
            rotation = other.rotation;
            scale = other.localScale;
        }
        public static bool operator ==(Transform left, Transform right)
        {
            return left.position == right.position && left.rotation == right.rotation && left.scale == right.scale;
        }
        public static bool operator !=(Transform left, Transform right)
        {
            return !(left == right);
        }
        public static bool operator ==(UnityEngine.Transform left, Transform right)
        {
            return left.position == right.position && left.rotation == right.rotation && left.localScale == right.scale;
        }
        public static bool operator !=(UnityEngine.Transform left, Transform right)
        {
            return !(left == right);
        }
        public static bool operator ==(Transform left, UnityEngine.Transform right)
        {
            return right == left;
        }
        public static bool operator !=(Transform left, UnityEngine.Transform right)
        {
            return !(right == left);
        }
    }
#pragma warning restore

    public struct Button
    {
        public string name;
        public UnityAction callback;

        public Button(string name, UnityAction callback)
        {
            this.name = name;
            this.callback = callback;
        }
    }
}

namespace UnityEngine
{
    public static class Extensions
    {
        public static void Assign(this Transform left, Aero.Transform right)
        {
            left.position = right.position;
            left.rotation = right.rotation;
            left.localScale = right.scale;
        }

        public static void Assign(this Transform left, Transform right)
        {
            left.position = right.position;
            left.rotation = right.rotation;
            left.localScale = right.localScale;
        }
    }
}
