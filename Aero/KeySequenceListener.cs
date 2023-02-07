using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aero
{
    public class KeySequenceListener : MonoBehaviour
    {
        // This has to be a class because structs are copied by value
        private class Entry
        {
            public bool destroy;
            public int index;
            public KeyCode[] sequence;
            public UnityEngine.Events.UnityAction callback;
        }
        private static List<Entry> s_entries = new List<Entry>();

        private void Update()
        {
            if (UnityEngine.Input.anyKeyDown)
            {
                for (int i = 0; i < s_entries.Count; ++i)
                {
                    Entry entry = s_entries[i];
                    CheckEntry(entry);
                }
            }
        }

        private static void CheckEntry(Entry entry)
        {
            if (UnityEngine.Input.GetKeyDown(entry.sequence[entry.index]))
            {
                ++entry.index;

                //Check if we finished the sequence
                if (entry.index == entry.sequence.Length)
                {
                    entry.callback();
                    if (entry.destroy)
                    {
                        s_entries.Remove(entry);
                    }
                    else
                    {
                        entry.index = 0;
                    }
                }
            }
            else
            {
                entry.index = 0;
            }
        }

        public static void AddSequence(KeyCode[] sequence, UnityEngine.Events.UnityAction callback, bool destroyOnCompletion = false)
        {
            Entry entry = new Entry();
            entry.sequence = sequence;
            entry.destroy = destroyOnCompletion;
            entry.callback = callback;
            s_entries.Add(entry);
        }
    }
}
