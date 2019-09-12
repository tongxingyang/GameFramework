using UnityEngine;
using System.Collections;

namespace I2.TextAnimation
{
    public class PlayAnimations : MonoBehaviour
    {
        public float _Time = 0;
        public string[] _Animations;

        public IEnumerator Start()
        {
            if (_Animations == null || _Animations.Length == 0)
                yield break;

            var se = GetComponent<TextAnimation>();

            int index = 0;
            while (true)
            {
                se.StopAllAnimations();
                var anim = se.PlayAnim(_Animations[index]);
                index = (index + 1) % _Animations.Length;

                while (anim.IsPlaying)
                    yield return null;
                yield return new WaitForSeconds(_Time);
            }
        }
    }
}