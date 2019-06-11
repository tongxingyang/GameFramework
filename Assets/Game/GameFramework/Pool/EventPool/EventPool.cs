using System;
using System.Collections.Generic;
using GameFramework.Debug;

namespace GameFramework.Pool.EventPool
{
    public class EventPool<T> where T :  GameEventArgs
    {
        private readonly Dictionary<enEventID, LinkedList<EventHandler<T>>> eventHandlers;
        private readonly Queue<Event<T>> events;
        private readonly enEventPoolMode eventPoolMode;
        private EventHandler<T> defaultHandler;
        public int EventHandlerCount => eventHandlers?.Count ?? 0;
        public int EventCount => events?.Count ?? 0;
        public List<Event<T>> EventLists;
        
        public EventPool(enEventPoolMode mode)
        {
            eventHandlers = new Dictionary<enEventID, LinkedList<EventHandler<T>>>();
            events = new Queue<Event<T>>();
            EventLists = new List<Event<T>>();
            eventPoolMode = mode;
            defaultHandler = null;
        }
        
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            lock (events)
            {
                while (events.Count > 0)
                {
                    Event<T> e = events.Dequeue();
                    HandleEvent(e.Sender, e.EventArgs);
                    e.Reset();
                }
            }
        }
        
        public void Shutdown()
        {
            Clear();
            EventLists.Clear();
            eventHandlers.Clear();
            defaultHandler = null;
        }

        private void Clear()
        {
            lock (events)
            {
                while (events.Count > 0)
                {
                    Event<T> e = events.Dequeue();
                    ReferencePool.ReferencePool.Release<T>(e.EventArgs);
                    e.Reset();
                }
            }
        }
        
        public int Count(enEventID id)
        {
            if (eventHandlers.TryGetValue(id, out var handlers))
            {
                return handlers.Count;
            }
            return 0;
        }
        
        public bool Check(enEventID id, EventHandler<T> handler)
        {
            if (handler == null)
            {
                Debuger.LogError("Event handler is invalid.");
            }
            if (!eventHandlers.TryGetValue(id, out var handlers))
            {
                return false;
            }
            return handlers.Contains(handler);
        }
        
        public void Subscribe(enEventID id, EventHandler<T> handler)
        {
            if (!eventHandlers.TryGetValue(id, out var handlers))
            {
                handlers = new LinkedList<EventHandler<T>>();
                handlers.AddLast(handler);
                eventHandlers.Add(id, handlers);
            }
            else if ((eventPoolMode & enEventPoolMode.AllowMultiHandler) == 0)
            {
                Debuger.LogError(Utility.StringUtility.Format("Event '{0}' not allow multi handler.", id.ToString()));
            }
            else if ((eventPoolMode & enEventPoolMode.AllowDuplicateHandler) == 0 && Check(id, handler))
            {
                Debuger.LogError(Utility.StringUtility.Format("Event '{0}' not allow duplicate handler.", id.ToString()));
            }
            else
            {
                handlers.AddLast(handler);
            }
        }
        
        public void Unsubscribe(enEventID id, EventHandler<T> handler)
        {
            if (handler == null)
            {
                Debuger.LogError("Event handler is invalid.");
            }

            if (!eventHandlers.TryGetValue(id, out var handlers))
            {
                Debuger.LogError(Utility.StringUtility.Format("Event '{0}' not exists any handler.", id.ToString()));
            }

            if (!handlers.Remove(handler))
            {
                Debuger.LogError(Utility.StringUtility.Format("Event '{0}' not exists specified handler.", id.ToString()));
            }
        }
        
        public void SetDefaultHandler(EventHandler<T> handler)
        {
            defaultHandler = handler;
        }

        public Event<T> GetEvent()
        {
            foreach (Event<T> eventList in EventLists)
            {
                if (eventList.IsUsed == false)
                {
                    return eventList;
                }
            }
            Event<T> @event = new Event<T>();
            EventLists.Add(@event);
            return @event;
        }
        
        public void Fire(object sender, T e)
        {
            Event<T> eventNode = GetEvent();
            eventNode.IsUsed = true;
            eventNode.Sender = sender;
            eventNode.EventArgs = e;
            lock (events)
            {
                events.Enqueue(eventNode);
            }
        }
        
        public void FireNow(object sender, T e)
        {
            HandleEvent(sender, e);
        }
        
        private void HandleEvent(object sender, T e)
        {
            enEventID eventId = e.EventID;
            bool noHandlerException = false;
            if (eventHandlers.TryGetValue(eventId, out var handlers) && handlers.Count > 0)
            {
                LinkedListNode<EventHandler<T>> current = handlers.First;
                while (current != null)
                {
                    LinkedListNode<EventHandler<T>> next = current.Next;
                    current.Value(sender, e);
                    current = next;
                }
            }
            else if (defaultHandler != null)
            {
                defaultHandler(sender, e);
            }
            else if ((eventPoolMode & enEventPoolMode.AllowNoHandler) == 0)
            {
                noHandlerException = true;
            }

            ReferencePool.ReferencePool.Release<T>(e);

            if (noHandlerException)
            {
                Debuger.LogError(Utility.StringUtility.Format("Event '{0}' not allow no handler.", eventId.ToString()));
            }
        }
    }
}