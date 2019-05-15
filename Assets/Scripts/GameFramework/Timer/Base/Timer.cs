using System;
using GameFramework.Pool.ReferencePool;

namespace GameFramework.Timer
{
    public class Timer : EventArgs,IReference
    {
        private int squence = -1;
        private object[] parms = null;
        private int loopTimes = -1;
        private enTimerType timerType;
        private float totalTime = 0f;
        private float currentTime = 0f;
        private bool isFinish = true;
        private bool isRunning = false;
        private TimerManager.OnTimeUpHandler callBack = null;

        public int Squence => squence;
        public object[] Parms => parms;
        public int LoopTimes => loopTimes;
        public enTimerType TimeType;
        public float TotalTime => totalTime;
        public float CurrentTime => currentTime;
        public bool IsFinish => isFinish;
        public bool IsRunning => isRunning;
        public TimerManager.OnTimeUpHandler CallBack => callBack;
        
        public void Reset()
        {
            this.squence = -1;
            this.parms = null;
            this.loopTimes = -1;
            this.totalTime = 0f;
            this.currentTime = 0f;
            this.callBack = null;
            this.isRunning = false;
            this.isFinish = true;
        }

        public Timer Init(int squence,float totalTime,int loopTimes,enTimerType timerType,TimerManager.OnTimeUpHandler callBack,params object[] parms)
        {
            if (loopTimes <= 0)
            {
                loopTimes = -1;
            }
            this.squence = squence;
            this.totalTime = totalTime;
            this.loopTimes = loopTimes;
            this.timerType = timerType;
            this.callBack = callBack;
            this.parms = parms;
            this.isFinish = false;
            this.isRunning = true;
            this.currentTime = 0f;
            return this;
        }
        
        public bool IsSquenceMatch(int squence)
        {
            if (this.squence == squence)
            {
                return true;
            }
            return false;
        }

        public bool IsDelegateMatch(TimerManager.OnTimeUpHandler callbBack)
        {
            if (this.callBack == callbBack)
            {
                return true;
            }
            return false;
        }

        public float GetLeftTime()
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
            currentTime = 0f;
        }

        public void TimerResume()
        {
            isRunning = true;
        }

        public void ResetTotalTime(float totalTime)
        {
            if (this.totalTime == totalTime)
            {
                return;
            }
            this.totalTime = totalTime;
            TimerReset();
        }

        public void Update(float detelTime)
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
                    if (callBack != null)
                    {
                        callBack(squence,parms);
                    }

                    TimerReset();
                    loopTimes--;
                }
            }
        }
    }
}