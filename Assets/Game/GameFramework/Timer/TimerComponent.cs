using GameFramework.Base;
using GameFramework.Utility.Singleton;
using UnityEngine;

namespace GameFramework.Timer
{
    [DisallowMultipleComponent]
    public class TimerComponent : GameFrameworkComponent
    {
        public override int Priority => SingletonMono<GameFramework>.GetInstance().TimerPriority;
        
        private TimerManager timerManager = null;
        public override void OnAwake()
        {
            base.OnAwake();
            timerManager = new TimerManager();
        }

        public override void Shutdown()
        {
            base.Shutdown();
            timerManager.Shutdown();
            timerManager = null;
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            timerManager.OnUpdate(elapseSeconds,realElapseSeconds);
        }

        public void RemoveAllTimer()
        {
            timerManager.RemoveAllTimer();
        }

        public float GetTimerLeftTime(int squence)
        {
            return timerManager.GetTimerLeftTime(squence);
        }

        public float GetTimerCurrentTime(int squence)
        {
            return timerManager.GetTimerCurrentTime(squence);
        }

        public void ResetTimerTotalTime(int squence, float totalTime)
        {
            timerManager.ResetTimerTotalTime(squence,totalTime);
        }

        public void ResetTimer(int squence)
        {
            timerManager.ResetTimer(squence);
        }

        public void ResumeTimer(int squence)
        {
            timerManager.ResetTimer(squence);
        }

        public void PauseTimer(int squence)
        {
            timerManager.PauseTimer(squence);
        }

        public Timer GetTimer(int squence)
        {
            return timerManager.GetTimer(squence);
        }

        public void RemoveTimer(TimerManager.OnTimeUpHandler callBack, enTimerType enTimeType)
        {
            timerManager.RemoveTimer(callBack,enTimeType);
        }

        public void RemoveTimer(int squence)
        {
            timerManager.RemoveTimer(squence);
        }

        public int AddTimer(float totalTime, int loopTimes, enTimerType timerType, TimerManager.OnTimeUpHandler callBcak,
            params object[] parms)
        {
            return timerManager.AddTimer(totalTime, loopTimes, timerType, callBcak, parms);
        }
        
        
    }
}