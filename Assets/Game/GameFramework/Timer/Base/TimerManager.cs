using System;
using System.Collections.Generic;
using GameFramework.Pool.ReferencePool;

namespace GameFramework.Timer
{
    public sealed class TimerManager
    {
        public delegate void OnTimeUpHandler(int timeSquence, params object[] parms);
        private Dictionary<enTimerType, List<Timer>> timers;
        public static int SerialID;
        public TimerManager()
        {
            timers = new Dictionary<enTimerType, List<Timer>>();
            for (int i = 0; i < Enum.GetValues(typeof(enTimerType)).Length; i++)
            {
                timers.Add((enTimerType)i,new List<Timer>());
            }
            SerialID = 0;
        }

        public void Shutdown()
        {
            RemoveAllTimer();
            timers.Clear();
            timers = null;
            SerialID = 0;
        }
        
        public int AddTimer(int totalTime, int loopTimes, enTimerType timerType, OnTimeUpHandler callBcak, params object[] parms)
        {
            SerialID++;
            timers[timerType].Add(ReferencePool.Acquire<Timer>().Init(SerialID,totalTime,loopTimes,timerType,callBcak,parms));
            return SerialID;
        }
        
        public void RemoveTimer(int serialID)
        {
            foreach (KeyValuePair<enTimerType,List<Timer>> keyValuePair in timers)
            {
                var list = keyValuePair.Value;
                foreach (Timer timer in list)
                {
                    if (timer.IsSerialIdMatch(serialID))
                    {
                        ReferencePool.Release(timer);
                        list.Remove(timer);
                        return;
                    }
                }
            }
        }
        
        public void RemoveTimer(OnTimeUpHandler callBack, enTimerType enTimeType)
        {
            foreach (Timer timer in timers[enTimeType])
            {
                if (timer.IsCallBackMatch(callBack))
                {
                    ReferencePool.Release(timer);
                    timers[enTimeType].Remove(timer);
                    return;
                }
            }
        }
        
        public Timer GetTimer(int serialID)
        {
            foreach (KeyValuePair<enTimerType,List<Timer>> keyValuePair in timers)
            {
                var list = keyValuePair.Value;
                foreach (Timer timer in list)
                {
                    if (timer.IsSerialIdMatch(serialID))
                    {
                        return timer;
                    }
                }
            }
            return null;
        }
        
        public void PauseTimer(int serialID)
        {
            Timer timer = GetTimer(serialID);
            timer?.TimerPause();
        }
        
        public void ResumeTimer(int serialID)
        {
            Timer timer = GetTimer(serialID);
            timer?.TimerResume();
        }
        
        public void ResetTimer(int serialID)
        {
            Timer timer = GetTimer(serialID);
            timer?.TimerReset();
        }
        
        public void ResetTimerTotalTime(int serialID, int totalTime)
        {
            Timer timer = GetTimer(serialID);
            timer?.ResetTotalTime(totalTime);
        }

        public float GetTimerCurrentTime(int serialID)
        {
            Timer timer = GetTimer(serialID);
            if (timer != null)
            {
                return timer.CurrentTime;
            }
            return -1;
        }
        
        public float GetTimerLeftTime(int serialID)
        {
            Timer timer = GetTimer(serialID);
            if (timer != null)
            {
                return timer.GetLeftTime();
            }
            return -1;
        }
        
        public void RemoveAllTimer()
        {
            foreach (KeyValuePair<enTimerType,List<Timer>> keyValuePair in timers)
            {
                var list = keyValuePair.Value;
                foreach (Timer timer in list)
                {
                    ReferencePool.Release<Timer>(timer);
                }
                list.Clear();
            }
        }
        
        private void TimerUpdate(int detalTime, enTimerType timerType)
        {
            var list = timers[timerType];
            int i = 0;
            while (i < list.Count)
            {
                if (list[i].IsFinish)
                {
                    ReferencePool.Release(list[i]);
                    list.RemoveAt(i);
                }
                else
                {
                    list[i].OnUpdate(detalTime);
                    i++;
                }
            }
        }
        
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            TimerUpdate((int)elapseSeconds * 1000, enTimerType.TimeScale);
            TimerUpdate((int)realElapseSeconds * 1000, enTimerType.NoTimeScale);
        }
    }
}