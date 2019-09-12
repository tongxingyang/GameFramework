using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

namespace I2.TextAnimation
{
	[Serializable]
	public class UnityEventSEAnimation : UnityEvent<SE_Animation> { }

	public partial class TextAnimation
	{
		public enum eTimeSource { Game, Real };
		public eTimeSource _TimeSource = eTimeSource.Game;

		public int _OnEnable_PlayAnim = -1;
		public SE_AnimationSlot[] _AnimationSlots = new SE_AnimationSlot[0];

		public UnityEventSEAnimation _OnAnimation_Finished = new UnityEventSEAnimation();


		private float mLastRealtimeUpdate;
		[NonSerialized] public List<SE_Animation> mPlayingAnimations = new List<SE_Animation>();

		public void ModifyVertices ()
		{
			mWidgetRectMin = mRect.min;
			mWidgetRectMax = mRect.max;

			for (int i = 0; i<mPlayingAnimations.Count; ++i)
				mPlayingAnimations[i].Apply_Characters( this );
		}


		public bool UpdateAnimations()
		{
			//---- Compute Delta Time ------------------
			float dt = 0;
			if (_TimeSource==eTimeSource.Game)
				dt = Time.deltaTime;
			else
			{
				float currentRealtime = Time.realtimeSinceStartup;
				if (mLastRealtimeUpdate>0)
					dt = currentRealtime - mLastRealtimeUpdate;
				mLastRealtimeUpdate = currentRealtime;
			}
			//---- Update Animations ------------------
			bool makeMaterialDirty = false;
			bool makeVerticesDirty = false;

			for (int i=mPlayingAnimations.Count-1; i>=0; --i)
			{
				var anim = mPlayingAnimations[i];
				if (anim.IsPlaying)
					anim.AnimUpdate(dt, this, ref makeMaterialDirty, ref makeVerticesDirty);
			}

			if (makeMaterialDirty || makeVerticesDirty)
				MarkWidgetAsChanged( makeVerticesDirty, makeMaterialDirty );

			return mPlayingAnimations.Count>0;
		}

		public SE_AnimationSlot GetAnimationSlot( string slotName )
		{
			foreach (var slot in _AnimationSlots)
				if (slot.GetName() == slotName)
					return slot;
			return null;
		}

		public SE_AnimationSlot GetAnimationSlot( SE_AnimationPreset preset)
		{
			foreach (var slot in _AnimationSlots)
				if (slot._Preset == preset)
					return slot;
			return null;
		}

		public SE_Animation GetPlayingAnimation(string animName)
		{
			for (int i = 0; i < mPlayingAnimations.Count; ++i)
				if (mPlayingAnimations[i].Name == animName)
					return mPlayingAnimations[i];
			return null;
		}


		public void StopAllAnimations( bool ExecuteCallbacks = true )
		{
			for (int i = mPlayingAnimations.Count-1; i>=0 ; --i)
				mPlayingAnimations[i].Stop(this, ExecuteCallbacks);
			mPlayingAnimations.Clear();
			MarkWidgetAsChanged(true, true);
		}

		public SE_Animation GetPlayingAnimation()
		{
			return mPlayingAnimations.Count == 0 ? null : mPlayingAnimations[0];
		}

		public void StopAnimation(string animationName, bool ExecuteCallbacks = true)
		{
			for (int i = 0; i < mPlayingAnimations.Count; ++i)
				if (mPlayingAnimations[i].Name == animationName)
				{
					mPlayingAnimations[i].Stop(this, ExecuteCallbacks);
					return;
				}
		}

		public void StopAnimation(SE_AnimationPreset preset, bool ExecuteCallbacks=true)
		{
			if (preset == null)
				return;

			// Check if the preset is from the animation's slots
			var slot = GetAnimationSlot(preset);
			if (slot!=null)
			{
				var anim = slot._Animation;
				if (!mPlayingAnimations.Contains(anim))
					return;

				anim.Stop(this, ExecuteCallbacks);
				return;
			}

			// if not, stop animation by name
			StopAnimation(preset.name, ExecuteCallbacks);
		}

		public void PlayAnimation(string slotName)
		{
			PlayAnim(slotName);
		}

		public void PlayAnimation(int slotIndex=0)
		{
			PlayAnim(slotIndex);
		}


		public void PlayAnimation(SE_AnimationPreset preset)
		{
			PlayAnim(preset);
		}

		public SE_Animation PlayAnim(SE_AnimationPreset preset)
		{
			if (preset == null)
				return null;

			SE_Animation anim;

			var slot = GetAnimationSlot(preset);
			if (slot != null)
			{
				anim = slot._Animation;
				anim.Play(this);
				return anim;
			}

			anim = preset.CreateAnimation();
			anim.Play(this);
			return anim;
		}


		public SE_Animation PlayAnim(string slotName)
		{
			var slot = GetAnimationSlot(slotName);
			if (slot != null)
			{
				var anim = slot._Animation;
				anim.Play(this);
				return anim;
			}

			return null;
		}

		public SE_Animation PlayAnim ( int slotIndex=0 )
		{
			if (slotIndex >= _AnimationSlots.Length)
				return null;

			var slot = _AnimationSlots[slotIndex];
			if (slot != null)
			{
				var anim = slot._Animation;
				anim.Play( this );
				return anim;
			}

			return null;
		}


	}
}
