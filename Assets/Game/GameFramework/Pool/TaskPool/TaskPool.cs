using System.Collections.Generic;

namespace GameFramework.Pool.TaskPool
{
    public sealed class TaskPool<T> where T : ITask
    {
        private bool isPause = false;
        private readonly Stack<ITaskAgent<T>> freeAgents = null;
        private readonly LinkedList<ITaskAgent<T>> workingAgents = null;
        private readonly LinkedList<T> waitAgents = null;

        public int FreeAgentsCount => freeAgents?.Count ?? 0;
        public int WorkingAgentsCount => workingAgents?.Count ?? 0;
        public int WaitAgentsCount => waitAgents?.Count ?? 0;
        public int TotalAgentsCount => FreeAgentsCount + WorkingAgentsCount;

        public bool Pause
        {
            get => isPause;
            set
            {
                if (isPause == value)
                {
                    return;
                }
                isPause = value;
            }
        }

        public TaskPool()
        {
            freeAgents = new Stack<ITaskAgent<T>>();
            workingAgents = new LinkedList<ITaskAgent<T>>();
            waitAgents = new LinkedList<T>();
        }

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if(isPause) return;
            LinkedListNode<ITaskAgent<T>> current = workingAgents.First;
            while (current != null)
            {
                if (current.Value.Task.Done)
                {
                    LinkedListNode<ITaskAgent<T>> next = current.Next;
                    current.Value.OnReset();
                    freeAgents.Push(current.Value);
                    workingAgents.Remove(current);
                    current = next;
                    continue;
                }
                current.Value.OnUpdate(elapseSeconds,realElapseSeconds);
                current = current.Next;
            }
            while (WaitAgentsCount > 0 && FreeAgentsCount > 0)
            {
                ITaskAgent<T> agent = freeAgents.Pop();
                T task = waitAgents.First.Value;
                waitAgents.RemoveFirst();
                LinkedListNode<ITaskAgent<T>> agentNode = workingAgents.AddLast(agent);
                agent.OnStart(task);
                if (task.Done)
                {
                    agent.OnReset();
                    workingAgents.Remove(agentNode);
                    freeAgents.Push(agent);
                }
            }
        }

        public void ShotDown()
        {
            while (FreeAgentsCount>0)
            {
                freeAgents.Pop().ShotDown();
            }
            freeAgents.Clear();
            foreach (ITaskAgent<T> workingAgent in workingAgents)
            {
                workingAgent.ShotDown();
            }
            workingAgents.Clear();
            LinkedListNode<T> current = waitAgents.First;
            while (current != null)
            {
                current.Value.Clear();
                current = current.Next;
            }
            waitAgents.Clear();

        }

        public void AddAgent(ITaskAgent<T> agent)
        {
            agent.Initialize();
            freeAgents.Push(agent);
        }

        public void AddTask(T task)
        {
            LinkedListNode<T> current = waitAgents.First;
            while (current != null)
            {
                if (task.Priority > current.Value.Priority)
                {
                    break;
                }
                current = current.Next;
            }
            if (current != null)
            {
                waitAgents.AddBefore(current, task);
            }
            else
            {
                waitAgents.AddLast(task);
            }
        }

        public T RemoveTask(int serialId)
        {
            foreach (var waitAgent in waitAgents)
            {
                if (waitAgent.SerialId == serialId)
                {
                    waitAgent.Clear();
                    waitAgents.Remove(waitAgent);
                    return waitAgent;
                }
            }
            foreach (ITaskAgent<T> workingAgent in workingAgents)
            {
                if (workingAgent.Task.SerialId == serialId)
                {
                    workingAgent.OnReset();
                    freeAgents.Push(workingAgent);
                    workingAgents.Remove(workingAgent);
                    return workingAgent.Task;
                }
            }
            return default(T);
        }

        public void RemoveAllTasks()
        {
            foreach (ITaskAgent<T> workingAgent in workingAgents)
            {
                workingAgent.OnReset();
                freeAgents.Push(workingAgent);
            }
            workingAgents.Clear();

            LinkedListNode<T> current = waitAgents.First;
            while (current != null)
            {
                current.Value.Clear();
                current = current.Next;
            }
            waitAgents.Clear();
        }
    }
}