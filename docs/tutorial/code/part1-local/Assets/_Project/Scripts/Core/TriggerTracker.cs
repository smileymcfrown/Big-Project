using System.Collections.Generic;
using UnityEngine;

namespace DumplingKitchen.Core
{
    /// <summary>
    /// Keeps track of which components of type T are inside a trigger volume.
    ///
    /// Why not just a List? An object with several colliders fires OnTriggerEnter once
    /// PER COLLIDER, so we count enters/exits per object and only treat it as "gone"
    /// when the count drops back to zero.
    ///
    /// Used by the heat zone (pots on the stove), pots (ingredients inside) and the
    /// serving counter. Write it once, reuse it three times.
    /// </summary>
    public sealed class TriggerTracker<T> where T : Component
    {
        private readonly Dictionary<T, int> _counts = new();
        private readonly List<T> _scratch = new();

        // Returning the concrete KeyCollection (not IEnumerable<T>) lets foreach use its
        // struct enumerator, which allocates nothing. Going through an interface would
        // box the enumerator and create garbage every physics step.
        public Dictionary<T, int>.KeyCollection Items => _counts.Keys;
        public int Count => _counts.Count;

        /// <returns>True if this collider brought a NEW object into the volume.</returns>
        public bool Add(Collider other)
        {
            if (!TryResolve(other, out T item))
                return false;

            _counts.TryGetValue(item, out int count);
            _counts[item] = count + 1;
            return count == 0;
        }

        /// <returns>True if the object has now completely left the volume.</returns>
        public bool Remove(Collider other)
        {
            if (!TryResolve(other, out T item) || !_counts.TryGetValue(item, out int count))
                return false;

            if (count <= 1)
            {
                _counts.Remove(item);
                return true;
            }

            _counts[item] = count - 1;
            return false;
        }

        /// <summary>
        /// Destroyed objects never fire OnTriggerExit. Call this before iterating
        /// to drop any entries Unity has destroyed.
        /// </summary>
        public void RemoveDestroyed()
        {
            _scratch.Clear();
            foreach (T item in _counts.Keys)
            {
                if (item == null) // Unity's overloaded == is true for destroyed objects
                    _scratch.Add(item);
            }

            foreach (T dead in _scratch)
                _counts.Remove(dead);
        }

        public void Clear() => _counts.Clear();

        private static bool TryResolve(Collider other, out T item)
        {
            // Prefer the Rigidbody's GameObject: that is "the object" for physics purposes.
            Rigidbody body = other.attachedRigidbody;
            item = body != null ? body.GetComponent<T>() : other.GetComponentInParent<T>();
            return item != null;
        }
    }
}
