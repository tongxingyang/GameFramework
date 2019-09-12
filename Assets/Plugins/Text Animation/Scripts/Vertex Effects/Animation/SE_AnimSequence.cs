using UnityEngine;
using System.Collections;
using System;
using System.Xml.Serialization;

namespace I2.TextAnimation
{

    [Serializable]
    public class SE_AnimSequence
    {
        #region Variables

        public string _Name = "";

        public enum eStartType { OnAnimStart, WithPrevious, AfterPrevious, AllPrevious, AllUsedElements }
        public eStartType _StartType = eStartType.OnAnimStart;
        public float _StartDelay;
        public float _Separation;  // At what progress, the next element will start animating
        public float _RandomStart; // how much to randomize the start of animating each element

        public enum eDurationType { TotalTime, PerElement };
        public eDurationType _DurationType = eDurationType.TotalTime;
        public float _Duration = 1;
        public float _DurationRandom_Slower = 0;
        public float _DurationRandom_Faster = 0;

        public SE_Animation.ePlayback _Playback;
        public bool _Backwards;
        public int _PlaybackTimes = 1;

        public AnimationCurve _EasingCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

        public enum eElements       { Letters, Words, Lines }
        public eElements        _TargetType = eElements.Letters;
        public enum eTargetRange    { All, First, Last, Range, Custom };
        public eTargetRange _TargetRangeType;

        public int _TargetRangeStart,      // used for targetRage.Range
                    _TargetRangeAmount;    // targetRage.First, targetRage.Last, targetRage.Range, targetRage.Range
        public int[] _TargetRangeCustom;   // only elements 3rd, 5th, 10th   {3, 5, 10}

        public bool _InitAllElements = false; // Set From value in all elements when the animation starts playing

        public bool _Enabled = true;

        #endregion


        #region Playback Variables

         [NonSerialized][XmlIgnore] public float mTotalTime,   // This is the total time counting the delay and loops: mTotalTime = mDelay + mLoopDuration * _PlaybackTimes;
                                                 mLoopDuration, mDelay;
         [NonSerialized][XmlIgnore] public float mTotalLoopDuration; // 2*mLoopDuration if its PingPong  (Anim sequence goes from  mDelay and last for mTotalLoopDuration)

         [NonSerialized][XmlIgnore] public int mElementRangeStart, mElementRangeEnd;
         [NonSerialized][XmlIgnore] public float mElementDuration, mElementDelay;


        #endregion

        public virtual string GetTypeName() { return "None";  }

        public bool HasFinished()
        {
            return false;
        }

        public virtual void OnStop(TextAnimation se, SE_Animation anim, int sequenceIndex)
        {
        }

        public virtual void InitTimes(TextAnimation se, SE_Animation anim, int sequenceIndex, int nElements)
        {
            switch (_StartType)
            {
                case eStartType.AfterPrevious:
                    {
                        if (sequenceIndex > 0)
                            mDelay = anim._Sequences[sequenceIndex-1].mTotalTime;
                        break;
                    }

                case eStartType.AllPrevious:
                    {
                        mDelay = 0;
                        for (int i=0; i<sequenceIndex; ++i)
                            mDelay = Mathf.Max(mDelay, anim._Sequences[sequenceIndex].mTotalTime);
                        break;
                    }

                case eStartType.WithPrevious:
                    {
                        if (sequenceIndex > 0)
                            mDelay = anim._Sequences[sequenceIndex-1].mDelay;
                        break;
                    }

                case eStartType.AllUsedElements:
                    {
                        mDelay = 0;
                        for (int i = 0; i < sequenceIndex; ++i)
                            if (anim._Sequences[sequenceIndex].IsUsingCommonElements(this))
                                mDelay = Mathf.Max(mDelay, anim._Sequences[sequenceIndex].mTotalTime);
                        break;
                    }

                default: 
                //case eStartType.OnAnimStart:
                    {
                        mDelay = 0;
                        break;
                    }
            }

            mDelay += _StartDelay;


            //--[ Get Animation Range ]--------------------------------

            mElementRangeStart = 0;
            mElementRangeEnd = nElements;
            switch (_TargetRangeType)
            {
                case eTargetRange.All:
                    mElementRangeStart = 0;
                    mElementRangeEnd = nElements;
                    break;

                case eTargetRange.First:
                    mElementRangeStart = 0;
                    mElementRangeEnd = _TargetRangeAmount;
                    break;
                case eTargetRange.Last:
                    mElementRangeStart = nElements-_TargetRangeAmount;
                    mElementRangeEnd = nElements;
                    break;
                case eTargetRange.Range:
                    mElementRangeStart = _TargetRangeStart;
                    mElementRangeEnd = _TargetRangeStart + _TargetRangeAmount;
                    break;
            }
            nElements = mElementRangeEnd - mElementRangeStart;


            //--[ Find Times ]--------------------------------

            if (_DurationType == eDurationType.PerElement)
            {
                mElementDuration = _Duration;
                mLoopDuration = mElementDuration + (mElementDuration  * _Separation * (nElements-1));
            }
            else
            {
                mLoopDuration = _Duration;
                mElementDuration = mLoopDuration / (_Separation * nElements - _Separation + 1);
            }
            mElementDelay = mElementDuration * _Separation;
            mTotalLoopDuration = _Playback == SE_Animation.ePlayback.PingPong ? 2*mLoopDuration : mLoopDuration;

            if (_Playback == SE_Animation.ePlayback.Loop || _Playback == SE_Animation.ePlayback.PingPong)
            {
                if (_PlaybackTimes > 0)
                    mTotalTime = mDelay + mLoopDuration * _PlaybackTimes;
                else
                    mTotalTime = float.MaxValue;
            }
            else
                mTotalTime = mDelay + mLoopDuration;
        }

        public virtual void UpdateSequence( float dt, TextAnimation se, SE_Animation anim, int sequenceIndex, ref bool makeMaterialDirty, ref bool makeVerticesDirty )
        {
        }

        public virtual void Apply_Characters(TextAnimation se, SE_Animation anim, int sequenceIndex)
        {
        }

        public float GetProgress( float time, SE_Animation anim, int characterID)
        {
            //--[ Verify that the animation is playing ]--------------------------------
            if (time < mDelay) return -1;
            if (time > mTotalTime) time = mTotalTime;

            //--[ Only accept elements in the Characters Range ]--------------------------------

            int elementId = characterID;
            int nElements = TextAnimation.mCharacters.Size;

            if (mElementRangeEnd <= mElementRangeStart || elementId < mElementRangeStart || elementId >= mElementRangeEnd)
                return -1;

            //--[ only accept those elements in the Custom array ]--------------------------------

            if (_TargetRangeType==eTargetRange.Custom && _TargetRangeCustom!=null)
            {
                bool found = false;
                for (int i=0, imax=_TargetRangeCustom.Length; i<imax; ++i)
                    if (_TargetRangeCustom[i]==characterID)
                    {
                        found = true;
                        break;
                    }
                if (!found)
                    return -1;
            }

            if (time >= mTotalTime)
                return 1;

            time -= mDelay;

            nElements = mElementRangeEnd - mElementRangeStart;
            elementId -= mElementRangeStart;


            //--[ Apply Playback ]--------------------------------

            switch (_Playback)
            {
                case SE_Animation.ePlayback.Loop:
                    time = Mathf.Repeat(time, mLoopDuration);
                    mElementDelay -= 0.5f*mElementDelay / (float)nElements; 
                    break;

                case SE_Animation.ePlayback.PingPong:
                    {
                        time = Mathf.Repeat(time, 2 * mLoopDuration);
                        mElementDelay -= 0.5f * mElementDelay / (float)nElements;
                        if (time > mLoopDuration)
                        {
                            time = mLoopDuration - (time - mLoopDuration);
                            time -= 0.25f * mElementDelay;
                        }
                        break;
                    }

                default: //case ePlayback.Single:
                    break;
            }


            //--[ Find element progress and apply Easing ]--------------------------------
            float startTime, elementDuration;
            GetElementTimes(anim, elementId, out startTime, out elementDuration);


            float t = (time - startTime) / elementDuration;
            t = t > 1 ? 1 : t;
            return t;
        }

        public void GetElementTimes(SE_Animation anim, int index, out float startTime, out float elementDuration)
        {
            startTime = index * mElementDelay;
            elementDuration = mElementDuration;
            float loopTime = mTotalTime - mDelay;

            int randomSeedBase = anim.mRandomSeed + 10 * anim.GetCurrentLoop() - DRandom.mCurrentSeed;

            if (_DurationRandom_Slower > 0 || _DurationRandom_Faster > 0)
            {
                float durRangeSlower = _DurationRandom_Slower * loopTime;
                float durRangeFaster = _DurationRandom_Faster * elementDuration;

                float minDuration = elementDuration - durRangeFaster; if (minDuration < 0.001f) minDuration = 0.001f;
                float maxDuration = elementDuration + durRangeSlower; if (maxDuration > loopTime - startTime) maxDuration = loopTime - startTime;
                elementDuration = DRandom.Get(index+ randomSeedBase, minDuration, maxDuration);
            }

            if (_RandomStart > 0)
            {
                float randRange = _RandomStart * loopTime;
                float minStart = startTime - randRange; if (minStart < 0) minStart = 0;
                float maxStart = startTime + randRange; if (maxStart > loopTime - elementDuration) maxStart = loopTime - elementDuration;
                startTime = DRandom.Get(index + randomSeedBase, minStart, maxStart);
            }

            if (_Backwards)
            {
                startTime = mTotalTime - (startTime + mDelay) -elementDuration;
            }
        }

        public float GetFinalTime( SE_Animation anim, int index )
        {
            float start, eDuration;
            GetElementTimes(anim, index, out start, out eDuration);
            return start + eDuration;
        }

 
        public virtual int GetRandomSeed(SE_Animation anim, int sequenceIndex)
        {
            float t = anim.mTime - mDelay;
            if (t < 0.0001f) t = 0.0001f;
            return anim.mRandomSeed + (anim.GetCurrentLoop()*anim._Sequences.Length + Mathf.CeilToInt(t / mTotalLoopDuration))*20*sequenceIndex;
        }


        private bool IsUsingCommonElements(SE_AnimSequence sequence)
        {
            if (mElementRangeStart >= sequence.mElementRangeEnd || mElementRangeEnd <= sequence.mElementRangeStart)
                return false;

            if (_TargetRangeType==eTargetRange.Custom && _TargetRangeCustom!=null && sequence._TargetRangeCustom==null)
            {
                for (int i = 0; i < _TargetRangeCustom.Length; ++i)
                    if (_TargetRangeCustom[i] >= sequence.mElementRangeStart && _TargetRangeCustom[i] < sequence.mElementRangeEnd)
                        return true;
            }

            if (_TargetRangeCustom == null && sequence._TargetRangeCustom != null && sequence._TargetRangeType == eTargetRange.Custom)
            {
                for (int i = 0; i < sequence._TargetRangeCustom.Length; ++i)
                    if (sequence._TargetRangeCustom[i] >= mElementRangeStart && sequence._TargetRangeCustom[i] < mElementRangeEnd)
                        return true;
            }

            if (_TargetRangeType == eTargetRange.Custom && _TargetRangeCustom != null && sequence._TargetRangeCustom != null && sequence._TargetRangeType == eTargetRange.Custom)
            {
                for (int i = 0; i < _TargetRangeCustom.Length; ++i)
                    for (int j = 0; j < _TargetRangeCustom.Length; ++j)
                        if (_TargetRangeCustom[i] == sequence._TargetRangeCustom[j])
                            return true;
            }

            return false;
        }

        public SE_AnimSequence Clone()
        {
            return this.MemberwiseClone() as SE_AnimSequence;
        }

#if UNITY_EDITOR
        public virtual void InspectorGUI()
        {
        }
#endif

    }
}
