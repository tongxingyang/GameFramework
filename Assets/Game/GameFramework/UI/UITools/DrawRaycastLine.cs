#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.UI.UITools
{
    public class DrawRaycastLine : MonoBehaviour
    {
        private Vector3[] Corners = new Vector3[4];
        public Color Color = Color.blue;

        void OnDrawGizmos()
        {
            foreach (MaskableGraphic maskableGraphic in GameObject.FindObjectsOfType<MaskableGraphic>())
            {
                if (maskableGraphic.raycastTarget)
                {
                    RectTransform rectTransform = maskableGraphic.transform as RectTransform;
                    if (rectTransform != null) rectTransform.GetWorldCorners(Corners);
                    Gizmos.color = Color;
                    for (int i = 0; i < 4; i++)
                    {
                        Gizmos.DrawLine(Corners[i], Corners[(i + 1) % 4]);
                    }
                }
            }
        }
    }
}
#endif