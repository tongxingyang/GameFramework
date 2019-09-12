using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using I2.TextAnimation;

namespace I2.TextAnimation
{
	public class TimedEffect : MonoBehaviour
	{
		public string[] _Dialog;
		public Text _Text;
		public TextAnimation _Animation;

		int IndexCurrentDialog;


		public void OnEnable()
		{
			// Play the first Dialog
			IndexCurrentDialog = -1;
			ShowNextText();
		}

		public void OnDisable()
		{
			_Animation.StopAllAnimations();
			CancelInvoke();
		}

		void ShowNextText()
		{
			// Move to the next dialog, or 0 if we got the last one
			IndexCurrentDialog = (IndexCurrentDialog + 1) % _Dialog.Length;

			// Show the text and Play the animation
			_Text.text = _Dialog[IndexCurrentDialog];

			_Animation.StopAllAnimations();
			_Animation.PlayAnim(IndexCurrentDialog);

			Invoke("ShowNextText", 1);
		}
	}
}