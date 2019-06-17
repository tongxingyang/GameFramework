using System;
using GameFramework.Pool.ReferencePool;

namespace GameFramework.Timer
{
    public class Timer : EventArgs ,IReference
    {
        private int serialId = -1;
        private object[] parms = null;
        private int loopTimes = -1;
        private enTimerType timerType;
        private int totalTime = 0;
        private int currentTime = 0;
        private bool isFinish = true;
        private bool isRunning = false;
        private TimerManager.OnTimeUpHandler callBack = null;

        public int SerialId => serialId;
        public object[] Parms => parms;
        public int LoopTimes => loopTimes;
        public enTimerType TimeType => timerType;
        public int TotalTime => totalTime;
        public int CurrentTime => currentTime;
        public bool IsFinish => isFinish;
        public bool IsRunning => isRunning;
        public TimerManager.OnTimeUpHandler CallBack => callBack;

        public void Reset()
        {
            this.serialId = -1;
            this.parms = null;
            this.loopTimes = -1;
            this.totalTime = 0;
            this.currentTime = 0;
            this.callBack = null;
            this.isRunning = false;
            this.isFinish = true;
        }

        public Timer Init(int serialId, int totalTime, int loopTimes, enTimerType timerType,TimerManager.OnTimeUpHandler callBack, params object[] parms)
        {
            if (loopTimes <= 0)
            {
                loopTimes = -1;
            }
            this.serialId = serialId;
            this.totalTime = totalTime;
            this.loopTimes = loopTimes;
            this.timerType = timerType;
            this.callBack = callBack;
            this.parms = parms;
            this.isFinish = false;
            this.isRunning = true;
            this.currentTime = 0;
            return this;
        }

        public bool IsSerialIdMatch(int serialId)
        {
            if (this.serialId == serialId)
            {
                return true;
            }
            return false;
        }

        public bool IsCallBackMatch(TimerManager.OnTimeUpHandler callbBack)
        {
            if (this.callBack == callbBack)
            {
                return true;
            }
            return false;
        }

        public int GetLeftTime()
        {
            return totalTime - currentTime;
        }

        public void TimerFinish()
        {
            isFinish = true;
        }

        public void TimerPause()
        {
            isRunning = false;
        }

        public void TimerReset()
        {
            currentTime = 0;
        }

        public void TimerResume()
        {
            isRunning = true;
        }

        public void ResetTotalTime(int totalTime)
        {
            if (this.totalTime == totalTime)
            {
                return;
            }
            this.totalTime = totalTime;
            TimerReset();
        }

        public void OnUpdate(int detelTime)
        {
            if (this.isFinish || !this.isRunning)
            {
                return;
            }
            if (loopTimes == 0)
            {
                isFinish = true;
            }
            else
            {
                currentTime += detelTime;
                if (currentTime >= totalTime)
                {
                    callBack?.Invoke(serialId, parms);
                    TimerReset();
                    loopTimes--;
                }
            }
        }
    }
}