using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Aero
{
    public static class Events
    {
        private static Dictionary<string, UnityEvent> s_events = new Dictionary<string, UnityEvent>();

        private static UnityEvent Get(string name)
        {
            if(!s_events.ContainsKey(name))
                s_events[name] = new UnityEvent();
            return s_events[name];
        }

        public static void Invoke(string name)
        {
            Get(name).Invoke();
        }

        public static void AddListener(string name, UnityAction callback)
        {
            Get(name).AddListener(callback);
        }
    }

    public static class Events<T>
    {
        private static Dictionary<string, UnityEvent<T>> s_events = new Dictionary<string, UnityEvent<T>>();

        private static UnityEvent<T> Get(string name)
        {
            if (!s_events.ContainsKey(name))
                s_events[name] = new UnityEvent<T>();
            return s_events[name];
        }

        public static void Invoke(string name, T thing)
        {
            Get(name).Invoke(thing);
        }

        public static void AddListener(string name, UnityAction<T> callback)
        {
            Get(name).AddListener(callback);
        }
    }

    public static class Events<T, T2>
    {
        private static Dictionary<string, UnityEvent<T, T2>> s_events = new Dictionary<string, UnityEvent<T, T2>>();

        private static UnityEvent<T, T2> Get(string name)
        {
            if (!s_events.ContainsKey(name))
                s_events[name] = new UnityEvent<T, T2>();
            return s_events[name];
        }

        public static void Invoke(string name, T param, T2 param2)
        {
            Get(name).Invoke(param, param2);
        }

        public static void AddListener(string name, UnityAction<T, T2> callback)
        {
            Get(name).AddListener(callback);
        }
    }
}
