using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParallelLoops
{
    static class Parallel
    {
        public static void ParallelFor(int fromInclusive, int toExclusive, Action<int> body)
        {
            if (body == null)
            {
                throw new ArgumentNullException("body is null");
            }

            if (toExclusive <= fromInclusive)
            {
                return;
            }

            int indexOfTasks = 0;

            Task[] tasks = new Task[Environment.ProcessorCount];

            while (indexOfTasks != Environment.ProcessorCount && fromInclusive <= toExclusive)
            {
                int currentIndex = fromInclusive;
                tasks[indexOfTasks++] = Task.Run(() => body(currentIndex));
                fromInclusive++;
            }

            while (fromInclusive <= toExclusive)
            {
                int currentIndex = fromInclusive;
                tasks[(Task.WaitAny(tasks))] = Task.Run(() => body(currentIndex));
                fromInclusive++;
            }

        }

        public static void ParallelForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body)
        {
            if (body == null)
            {
                throw new ArgumentNullException("body is null");
            }

            if (source == null)
            {
                throw new ArgumentNullException("source is null");
            }

            IEnumerator<TSource> enumerator = source.GetEnumerator();

            Task[] tasks = new Task[Environment.ProcessorCount];

            int indexOfTasks = 0;

            while (indexOfTasks != Environment.ProcessorCount)
            {
                if (enumerator.MoveNext())
                {
                    TSource current = enumerator.Current;
                    tasks[indexOfTasks++] = Task.Run(() => body(current));
                }
            }

            while (enumerator.MoveNext())
            {
                TSource current = enumerator.Current;
                tasks[Task.WaitAny(tasks)] = Task.Run(() => body(current));
            }
            
        }

        public static void ParallelForEachWithOptions<TSource>(IEnumerable<TSource> source, ParallelOptions parallelOptions, Action<TSource> body)
        {
            if (body == null)
            {
                throw new ArgumentNullException("body is null");
            }

            if (source == null)
            {
                throw new ArgumentNullException("source is null");
            }

            if (parallelOptions == null)
            {
                throw new ArgumentNullException("parallelOptions is null");
            }

            TaskFactory taskFactory = new TaskFactory(parallelOptions.CancellationToken,TaskCreationOptions.None,TaskContinuationOptions.None, parallelOptions.TaskScheduler);

            int maxCountOfTasks = (parallelOptions.MaxDegreeOfParallelism == -1) ? Environment.ProcessorCount : parallelOptions.MaxDegreeOfParallelism;

            int indexOfTasks = 0;

            Task[] tasks = new Task[maxCountOfTasks];

            IEnumerator<TSource> enumerator = source.GetEnumerator();

            while(indexOfTasks != maxCountOfTasks && enumerator.MoveNext())
            {
                if (parallelOptions.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                TSource current = enumerator.Current;
                tasks[indexOfTasks++] = taskFactory.StartNew(() => body(current));
            }

            while (enumerator.MoveNext())
            {
                if (parallelOptions.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                TSource current = enumerator.Current;
                tasks[Task.WaitAny(tasks)] = taskFactory.StartNew(() => body(current));
            }
        }
    }
}
