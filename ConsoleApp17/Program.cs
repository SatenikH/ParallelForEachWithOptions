using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelForEachWithOptions
{
    public class Program
    {
        static void Main(string[] args)
        {
            Parallel.ParallelForEachWithOptions(Enumerable.Range(0, 19), new ParallelOptions() { MaxDegreeOfParallelism = GetNumParallelOperations() }, (currentFile) =>
            {
                Console.WriteLine("{0} on Task {1}", currentFile, Thread.CurrentThread.ManagedThreadId);
            });

            Console.ReadLine();
        }
        /// <summary>
        /// Return max degree of parallelism
        /// </summary>
        /// <returns>Max degree of parallelism</returns>
        public static int GetNumParallelOperations()
        {
            int maxDegree = Environment.ProcessorCount;
            return maxDegree;
        }
    }
    /// <summary>
    /// Provides support for parallel loops.
    /// </summary>
    public class Parallel
    {
        /// <summary>
        ///  Implementation parallel.ParallelForEachWithOptions method with the following signatures
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="source">anenumerable data source</param>
        /// <param name="parallelOptions">Is an object that configures the behavior of thisoperation</param>
        /// <param name="body">delegate thatis invoked once per iteration</param>
        public static void ParallelForEachWithOptions<T>(IEnumerable<T> source, ParallelOptions parallelOptions, Action<T> body)
        {
            if (source == null)
                throw new ArgumentNullException("enumerable");
            if (body == null)
                throw new ArgumentNullException("action");
            var resetEvents = new List<ManualResetEvent>();

            foreach (var item in source)
            {
                var evt = new ManualResetEvent(false);
                ThreadPool.SetMaxThreads(parallelOptions.MaxDegreeOfParallelism, parallelOptions.MaxDegreeOfParallelism);
                ThreadPool.QueueUserWorkItem((i) =>
                {
                    body((T)i);
                    evt.Set();
                }, item);
                resetEvents.Add(evt);
            }

            foreach (var re in resetEvents)
                re.WaitOne();
        }
    }
}
