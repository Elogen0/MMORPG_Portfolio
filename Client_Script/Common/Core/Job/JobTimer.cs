using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace Kame.Core.Job
{
    struct JobTimerElem : IComparable<JobTimerElem>
    {
        public int execTick; // 실행시간
        public IJob job;

        public int CompareTo(JobTimerElem other)
        {
            return other.execTick - execTick; //작으면 작을수로 먼저 튀어나오게
        }
    }

    class JobTimer
    {
        PriorityQueue<JobTimerElem> _pq = new PriorityQueue<JobTimerElem>();
        object _lock = new object();

        public void Push(IJob job, int tickAfter = 0)
        {
            JobTimerElem jobElement;
            jobElement.execTick = System.Environment.TickCount + tickAfter;
            jobElement.job = job;

            lock (_lock)
            {
                _pq.Push(jobElement);
            }
        }

        public void ForceExecuteAll()
        {
            List<IJob> jobList = new List<IJob>();
            lock (_lock)
            {
                while (_pq.Count != 0)
                {
                    var jobElem = _pq.Pop();
                    jobList.Add(jobElem.job);
                }
            }

            foreach (var job in jobList)
            {
                job.Execute();
            }
        }

        public void Flush()
        {
            while (true)
            {
                int now = System.Environment.TickCount;
                JobTimerElem jobElement;

                lock (_lock)
                {
                    if (_pq.Count == 0)
                        break;

                    jobElement = _pq.Peek();
                    if (jobElement.execTick > now)
                        break;

                    _pq.Pop();
                }

                jobElement.job.Execute();
            }
        }
    }
}
