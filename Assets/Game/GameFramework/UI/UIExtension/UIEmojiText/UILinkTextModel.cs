using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.UI.UIExtension.UIEmojiText
{
    public class UILinkTextModel
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public string LinkText { get; set; }
        public List<Rect> Rects { get; set; }
        public void AddRect(Rect rect)
        {
            if (Rects == null)
            {
                Rects = new List<Rect>();
            }
            Rects.Add(rect);
        }
    }
}