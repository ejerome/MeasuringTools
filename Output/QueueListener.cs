﻿using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MeasuringTools.Output
{
    /// <summary>
    /// Simple interface to implement a <see cref="ConcurrentQueue{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IQueueListener<T>
    {
        /// <summary>
        /// Our standard Queue waiting to be processed.
        /// </summary>
        ConcurrentQueue<T> Queue { get; }
    }

    /// <summary>
    /// Represent a listener which run on its own thread
    /// and poll the <see cref="IQueueListener{T}.Queue"/> in order to
    /// <see cref="Execute(T)"/> each dequeued value to print it in console (a consumer).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class QueueListener<T> : IQueueListener<T>
    {
        /// <summary>
        /// Our standard Queue waiting to be processed.
        /// </summary>
        public ConcurrentQueue<T> Queue { get; protected set; } = new ConcurrentQueue<T>();

        /// <summary>
        /// Used to stop the thread.
        /// </summary>
        private volatile bool _stop = false;

        /// <summary>
        /// Stop the running thread.
        /// </summary>
        public void StopListening()
        {
            _stop = true;
        }

        /// <summary>
        /// Call this to start the thread waiting to dequeue ASAP
        /// there is a new value to <see cref="Execute(T)"/>.
        /// </summary>
        public void StartListening()
        {
            _stop = false;

            var t = new Thread(Listen);
            t.IsBackground = true;
            t.Priority = ThreadPriority.BelowNormal;
            t.Start();
        }

        /// <summary>
        /// <see cref="Execute(T)"/> a new available value in loop.
        /// </summary>
        private void Listen()
        {
            while (!_stop)
            {
                T val;
                if (Queue.TryDequeue(out val))
                {
                    Execute(val);
                }
            }
        }

        /// <summary>
        /// Fill free to implement concrete <see cref="AbstractQueueListener{T}"/>.\n
        /// By default write the data into the console.
        /// </summary>
        /// <param name="val"></param>
        public abstract void Execute(T val);
    }
}
