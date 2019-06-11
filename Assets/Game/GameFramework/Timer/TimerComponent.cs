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

        public float GetTimerLeftTime(int serialID)
        {
            return timerManager.GetTimerLeftTime(serialID);
        }

        public float GetTimerCurrentTime(int serialID)
        {
            return timerManager.GetTimerCurrentTime(serialID);
        }

        public void ResetTimerTotalTime(int serialID, int totalTime)
        {
            timerManager.ResetTimerTotalTime(serialID,totalTime);
        }

        public void ResetTimer(int serialID)
        {
            timerManager.ResetTimer(serialID);
        }

        public void ResumeTimer(int serialID)
        {
            timerManager.ResetTimer(serialID);
        }

        public void PauseTimer(int serialID)
        {
            timerManager.PauseTimer(serialID);
        }

        public Timer GetTimer(int serialID)
        {
            return timerManager.GetTimer(serialID);
        }

        public void RemoveTimer(TimerManager.OnTimeUpHandler callBack, enTimerType enTimeType)
        {
            timerManager.RemoveTimer(callBack,enTimeType);
        }

        public void RemoveTimer(int serialID)
        {
            timerManager.RemoveTimer(serialID);
        }

        public int AddTimer(int totalTime, int loopTimes, enTimerType timerType, TimerManager.OnTimeUpHandler callBcak, params object[] parms)
        {
            return timerManager.AddTimer(totalTime, loopTimes, timerType, callBcak, parms);
        }
        
        
    }
}