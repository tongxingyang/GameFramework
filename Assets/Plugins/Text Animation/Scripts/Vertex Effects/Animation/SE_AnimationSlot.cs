using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

namespace I2.TextAnimation
{
	[Serializable]
	public class SE_AnimationSlot
	{
		public string _LocalSerializedData;
		public SE_AnimationPreset _Preset;

		//public event System.Action<SE_Animation> _OnFinished;

		public SE_Animation _Animation
		{
			get
			{
				if (mAnimation == null)
					CreateAnimation();
				return mAnimation;
			}
		}

		public SE_Animation CreateAnimation()
		{
			if (_Preset != null)
				mAnimation = _Preset.CreateAnimation();
			else
				mAnimation = SE_Animation.LoadFromSerializedData(_LocalSerializedData) ?? new SE_Animation();
			return mAnimation;
		}

		public void Play( TextAnimation se )
		{
			var anim = _Animation;
			if (anim != null)
				anim.Play(se);
		}

		public bool IsPlaying()
		{
			var anim = _Animation;
			return (anim != null && anim.IsPlaying);
		}

		public string GetName()
		{
			if (_Preset != null)
				return _Preset.name;
			var NameKey = "XMLSchema\" Name=\"";
			int idx = _LocalSerializedData.IndexOf(NameKey);
			if (idx>=0)
			{
				idx+=NameKey.Length;
				int idx2 = _LocalSerializedData.IndexOf("\"", idx+1);
				return _LocalSerializedData.Substring(idx, idx2 - idx);
			}
			return "Custom";
		}

		SE_Animation mAnimation;
	}
}
