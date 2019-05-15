using System;
using System.Collections.Generic;
using GameFramework.Pool.ReferencePool;

namespace GameFramework.Timer
{
    public class TimerManager
    {
        public delegate void OnTimeUpHandler(int timeSquence, params object[] parms);
        private List<Timer>[] timers;
        private static int timerSquence;

        public TimerManager()
        {
            this.timers = new List<Timer>[Enum.GetValues(typeof(enTimerType)).Length];
            for (int i = 0; i < timers.Length; i++)
            {
                timers[i] = new List<Timer>();
            }
            timerSquence = 0;
        }

        public void Shutdown()
        {
            RemoveAllTimer();
            timers = null;
            timerSquence = 0;
        }
        
        public int AddTimer(float totalTime, int loopTimes, enTimerType timerType, OnTimeUpHandler callBcak, params object[] parms)
        {
            timerSquence++;
            timers[timerType == enTimerType.NoTimeScale ? 0 : 1].Add(ReferencePool.Acquire<Timer>().Init(timerSquence,totalTime,loopTimes,timerType,callBcak,parms));
            return timerSquence;
        }
        
        public void RemoveTimer(int squence)
        {
            for (int i = 0; i < timers.Length; i++)
            {
                List<Timer> list = timers[i];
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j].IsSquenceMatch(squence))
                    {
                        list[j].TimerFinish();
                        return;
                    }
                }
            }
        }
        
        public void RemoveTimer(OnTimeUpHandler callBack, enTimerType enTimeType)
        {
            List<Timer> list = timers[enTimeType == enTimerType.NoTimeScale ? 0 : 1];
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsDelegateMatch(callBack))
                {
                    list[i].TimerFinish();
                    return;
                }
            }
        }
        
        public Timer GetTimer(int squence)
        {
            for (int i = 0; i < timers.Length; i++)
            {
                List<Timer> list = timers[i];
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j].IsSquenceMatch(squence))
                    {
                        return list[j];
                    }
                }
            }
            return null;
        }
        
        public void PauseTimer(int squence)
        {
            Timer timer = GetTimer(squence);
            if (timer != null)
            {
                timer.TimerPause();
            }
        }
        
        public void ResumeTimer(int squence)
        {
            Timer timer = GetTimer(squence);
            if (timer != null)
            {
                timer.TimerResume();
            }
        }
        
        public void ResetTimer(int squence)
        {
            Timer timer = GetTimer(squence);
            if (timer != null)
            {
                timer.TimerReset();
            }
        }
        
        public void ResetTimerTotalTime(int squence, float totalTime)
        {
            Timer timer = GetTimer(squence);
            if (timer != null)
            {
                timer.ResetTotalTime(totalTime);
            }
        }

        public float GetTimerCurrentTime(int squence)
        {
            Timer timer = GetTimer(squence);
            if (timer != null)
            {
                return timer.CurrentTime;
            }
            return -1;
        }
        
        public float GetTimerLeftTime(int squence)
        {
            Timer timer = GetTimer(squence);
            if (timer != null)
            {
                return timer.GetLeftTime();
            }
            return -1;
        }
        
        public void RemoveAllTimer()
        {
            for (int i = 0; i < timers.Length; i++)
            {
                List<Timer> list = timers[i];
                for (int j = 0; j < list.Count; j++)
                {
                    ReferencePool.Release(list[j]);
                }
                timers[i].Clear();
            }
        }
        
        private void TimerUpdate(float detalTime, enTimerType timerType)
        {
            List<Timer> list = timers[timerType == enTimerType.NoTimeScale ? 0 : 1];
            int i = 0;
            while (i < list.Count)
            {
                if (list[i].IsFinish)
                {
                    list[i].Reset();
                    ReferencePool.Release(list[i]);
                    list.RemoveAt(i);
                }
                else
                {
                    list[i].Update(detalTime);
                    i++;
                }
            }
        }
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            TimerUpdate(elapseSeconds, enTimerType.TimeScale);
            TimerUpdate(realElapseSeconds, enTimerType.NoTimeScale);
        }
    }
}