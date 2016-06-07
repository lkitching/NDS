using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace NDS
{
    /// <summary>Represents an insert operation into a collection.</summary>
    /// <typeparam name="T">The type of item to insert.</typeparam>
    public interface IInsertable<T>
    {
        /// <summary>Inserts an item into the underlying collection.</summary>
        /// <param name="item">The item to insert.</param>
        void Insert(T item);
    }

    /// <summary>Extension methods for <see cref=IInsertable"/>.</summary>
    public static class InsertableExtensions
    {
        /// <summary>Creates an insertable whose insert operation pushes items onto the given stack.</summary>
        /// <typeparam name="T">The type of items in the stack.</typeparam>
        /// <param name="stack">The stack to push items into on insert.</param>
        /// <returns>An insertable to push items onto <paramref name="stack"/>.</returns>
        public static IInsertable<T> PushInserter<T>(this IStack<T> stack)
        {
            Contract.Requires(stack != null);
            return new ActionInserter<T>(stack.Push);
        }

        /// <summary>Creates an insertable whose insert operation enqueues item on the given queue.</summary>
        /// <typeparam name="T">The type of items in the queue.</typeparam>
        /// <param name="queue">The queue to enqueue inserted item to.</param>
        /// <returns>An insertable to enqueue items onto <paramref name="queue"/>.</returns>
        public static IInsertable<T> EnqueueInserter<T>(this IQueue<T> queue)
        {
            Contract.Requires(queue != null);
            return new ActionInserter<T>(queue.Enqueue);
        }

        /// <summary>Creates an insertable whose insert operation associates pairs in the given map.</summary>
        /// <typeparam name="TKey">The key type for the map.</typeparam>
        /// <typeparam name="TValue">The value type for the map.</typeparam>
        /// <param name="map">The map to associate inserted pairs into.</param>
        /// <returns>An insertable to associated pairs into <paramref name="map"/>.</returns>
        public static IInsertable<KeyValuePair<TKey, TValue>> AssocInserter<TKey, TValue>(this IMap<TKey, TValue> map)
        {
            return new ActionInserter<KeyValuePair<TKey, TValue>>(map.Assoc);
        }

        /// <summary>
        /// Creates an insertable whose insert operation adds pairs in the given map. 
        /// This operation throws an exception if the key already exists in the map.
        /// </summary>
        /// <typeparam name="TKey">The key type for the map.</typeparam>
        /// <typeparam name="TValue">The value type for the map.</typeparam>
        /// <param name="map">The map to add inserted pairs into.</param>
        /// <returns>An insertable to add pairs into <paramref name="map"/>.</returns>
        public static IInsertable<KeyValuePair<TKey, TValue>> AddInserter<TKey, TValue>(this IMap<TKey, TValue> map)
        {
            return new ActionInserter<KeyValuePair<TKey, TValue>>(map.Add);
        }

        /// <summary>Inserts each item in a sequence to an insertable.</summary>
        /// <typeparam name="T">The type of items to insert.</typeparam>
        /// <param name="inserter">The <see cref="IInsertable{T}"/> to insert the items into.</param>
        /// <param name="seq">The sequence whose items to add.</param>
        public static void InsertAll<T>(this IInsertable<T> inserter, IEnumerable<T> seq)
        {
            Contract.Requires(inserter != null);
            Contract.Requires(seq != null);

            foreach(T item in seq)
            {
                inserter.Insert(item);
            }
        }
    }

    /// <summary><see cref="IInsertable{T}"/> implementation which performs the given action on insert.</summary>
    /// <typeparam name="T">The type of items to insert.</typeparam>
    public class ActionInserter<T> : IInsertable<T>
    {
        private readonly Action<T> insertAction;

        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="insertAction">The action to execute for the insert operation.</param>
        public ActionInserter(Action<T> insertAction)
        {
            Contract.Requires(insertAction != null);
            this.insertAction = insertAction;
        }

        /// <summary>Invokes the action given on construction with <paramref name="item"/>.</summary>
        /// <param name="item">The item to insert.</param>
        public void Insert(T item)
        {
            this.insertAction(item);
        }
    }
}
