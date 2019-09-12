using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace I2.TextAnimation
{
    [ExecuteInEditMode]
	[AddComponentMenu("")]
    public class TextAnimationsManager : MonoBehaviour
    {
        // TextAnimation components that are running animations
        List<TextAnimation> mUpdate_Animations = new List<TextAnimation>();

        #region Setup
        void Initialize()
        {
            StartCoroutine(UpdateAnimations());
        }

         public static void RegisterAnimation(TextAnimation se)
        {
            if (!Application.isPlaying)
                return;
            var manager = singleton;

            if (!manager.mUpdate_Animations.Contains(se))
                manager.mUpdate_Animations.Add(se);
        }

        public static void UnregisterAnimation(TextAnimation se)
        {
            if (!Application.isPlaying)
                return;
            var manager = singleton;
            manager.mUpdate_Animations.Remove(se);
        }
        #endregion


        IEnumerator UpdateAnimations()
        {
            List<TextAnimation> tempList = new List<TextAnimation>();
            while (true)
            {
                if (mUpdate_Animations.Count == 0)
                {
                    while (mUpdate_Animations.Count <= 0)
                        yield return null;
                }

                tempList.AddRange(mUpdate_Animations);

                // Update all animations and then remove the ones that are not longer playing
                for (int i = 0; i < tempList.Count; ++i)
                {
					var anim = tempList[i];
                    if (!anim.UpdateAnimations())
                        mUpdate_Animations.Remove(anim);
                }
                tempList.Clear();

                yield return null;
            }
        }

        #region Singleton
        static TextAnimationsManager mSingleton;
        static TextAnimationsManager singleton
        {
            get
            {
                if (mSingleton == null)
                {
                    mSingleton = (TextAnimationsManager)FindObjectOfType(typeof(TextAnimationsManager));

                    if (mSingleton == null)
                    {
                        GameObject go = new GameObject();
                        go.hideFlags = go.hideFlags | HideFlags.HideAndDontSave;
                        //go.hideFlags = go.hideFlags | HideFlags.DontSave;
                        go.name = "[singleton] TextAnimationsManager";

                        mSingleton = go.AddComponent<TextAnimationsManager>();
                        mSingleton.Initialize();
                    }

                }

                return mSingleton;
            }
        }

        void OnDestroy()
        {
            if (mSingleton == this)
                mSingleton = null;
        }

        void Awake()
        {
            if (mSingleton == null)
            {
                mSingleton = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            if (mSingleton != this)
            {
                Destroy(gameObject);
            }
        }
        #endregion

    }
}