using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace I2.TextAnimation
{
    public class SE_Animation
    {
        [XmlAttribute]public string Name = "Custom";
        public SE_AnimSequence[] _Sequences = new SE_AnimSequence[0];

        public enum ePlayback { Single, Loop, PingPong };
        [XmlAttribute]public ePlayback _Playback = ePlayback.Single;
        [XmlAttribute]public bool _Backwards;
        [XmlAttribute]public int _PlaybackTimes = -1;
        [XmlAttribute]public float _ExtraTimeFinal, _ExtraTimePerLoop;

        public event System.Action<SE_Animation> _OnFinished = delegate { };


        #region Playback Variables

        [NonSerialized][XmlIgnore] public bool IsPlaying;
        [NonSerialized][XmlIgnore] public float mTotalTime; // Total time of a single loop
        [NonSerialized][XmlIgnore] public float mTime,      // Time within the loop duration: mTime=Repeat(mRealTime, loopDuration)
                                                mRealTime;  // total time that go bigger than loop duration
        [NonSerialized][XmlIgnore] public int mRandomSeed;
        [NonSerialized][XmlIgnore] public int mNumElements; // Num elements in the TextAnimation component (used to know when the InitTime should be executed again if the text changes)



        #endregion

        public void Play(TextAnimation se)
        {
            if (se != null)
            {
				if (!se.mPlayingAnimations.Contains( this ))
				{
					se.mPlayingAnimations.Add( this );
					TextAnimationsManager.RegisterAnimation( se );
				}
                se.MarkWidgetAsChanged();
                _OnFinished += (anim) => { se._OnAnimation_Finished.Invoke(anim); };
            }

			IsPlaying = true;
			mTime = mRealTime = 0;
			mNumElements = -1;
			mRandomSeed = -1;
			InitTimes( se, true );
        }

        public void Stop( TextAnimation se, bool executeCallback = true )
        {
            if (IsPlaying)
            {
                for (int i = 0; i < _Sequences.Length; ++i)
                    if (_Sequences[i]._Enabled)
                        _Sequences[i].OnStop(se, this, i);

                IsPlaying = false;
                if (executeCallback && _OnFinished != null)
                    _OnFinished(this);

                se.mPlayingAnimations.Remove(this);
                se.MarkWidgetAsChanged(true, true);
            }
            _OnFinished = null;
        }

        public void AnimUpdate(float dt, TextAnimation se, ref bool makeMaterialDirty, ref bool makeVerticesDirty)
        {
            if (se == null)
                return;

            InitTimes(se);
            if (mNumElements <= 0)
                return;

            if (mRealTime >= 0)
            {
                mRealTime += dt;

                switch (_Playback)
                {
                    case SE_Animation.ePlayback.Loop:
                        mTime = Mathf.Repeat(mRealTime, mTotalTime);
                        break;

                    case SE_Animation.ePlayback.PingPong:
                        {
                            mTime = Mathf.Repeat(mRealTime, 2*mTotalTime);
                            if (mTime > mTotalTime)
                                mTime = mTotalTime - (mTime - mTotalTime);

                            break;
                        }

                    default: //case ePlayback.Single:
                        mTime = mRealTime;
                        break;
                }
                if (_Backwards)
                    mTime = mTotalTime - mTime;

                float mFinalTime = mTotalTime;
                if (_Playback != SE_Animation.ePlayback.Single)
                    mFinalTime = _PlaybackTimes <= 0 ? -1 : (mTotalTime * _PlaybackTimes);

                if (mFinalTime>0 && mRealTime >= (mFinalTime+ _ExtraTimeFinal))
                {
                    mRealTime = mFinalTime;
                    Stop(se);
                }
            }

            for (int i = 0, imax = _Sequences.Length; i < imax; ++i)
                if (_Sequences[i]._Enabled)
                    _Sequences[i].UpdateSequence(dt, se, this, i, ref makeMaterialDirty, ref makeVerticesDirty);
        }

        public int GetCurrentLoop()
        {
            float loopDuration = _Playback == SE_Animation.ePlayback.PingPong ? 2 * mTotalTime : mTotalTime;
            float t = mRealTime;
            if (t<0.0001f) t=0.001f;
            return Mathf.CeilToInt(t / loopDuration)-1;
        }

        public void Apply_Characters(TextAnimation se)
        {
            InitTimes(se);
            if (mNumElements <= 0)
                return;

            for (int i = 0, imax = _Sequences.Length; i < imax; ++i)
                if (_Sequences[i]._Enabled)
                    _Sequences[i].Apply_Characters(se, this, i);
        }

        public void InitTimes(TextAnimation se, bool force=false)
        {
            if (mRandomSeed < 0)
                mRandomSeed = DRandom.GetSeed();

            int nElements = (se == null) ? 1 : TextAnimation.mCharacters.Size;
            if (mNumElements == nElements && !force)
                return;

            mNumElements = nElements;

            mTotalTime = 0;
            for (int i = 0, imax = _Sequences.Length; i < imax; ++i)
                //if (_Sequences[i]._Enabled || force)
                {
                    _Sequences[i].InitTimes(se, this, i, nElements);

                    float seqDuration = _Sequences[i].mTotalTime;
                    if (mTotalTime < 0 || seqDuration < 0)
                        mTotalTime = float.MaxValue;
                    else
                        mTotalTime = mTotalTime > seqDuration ? mTotalTime : seqDuration;
                }
            mTotalTime += _ExtraTimePerLoop;
        }

        static System.Type[] mAnimSequenceTypes = {
            typeof(SE_AnimSequence_Alpha),
            typeof(SE_AnimSequence_Position), typeof(SE_AnimSequence_Scale), typeof(SE_AnimSequence_Rotation), typeof(SE_AnimSequence_Color)
        };
        public static SE_Animation LoadFromSerializedData(string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;

            try
            {
                // TEMPORAL FIX:  (Remove in a couple versions, once every user had time to update their animations)
                data = data.Replace(">Explicit<", ">Replace<");


                var reader = new System.IO.StringReader(data);
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(SE_Animation), mAnimSequenceTypes);

                return serializer.Deserialize(reader) as SE_Animation;
            }
            catch (Exception e) {
                Debug.Log(e);
            }

            return null;
        }

        public static string SaveSerializedData(SE_Animation anim )
        {
            try
            {
                var writer = new System.IO.StringWriter(System.Globalization.CultureInfo.InvariantCulture);
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(SE_Animation), mAnimSequenceTypes);
                serializer.Serialize(writer, anim);
                //Debug.Log(writer.ToString());
                return writer.ToString();
            }
            catch (Exception) { }
            return null;
        }

    }
}
