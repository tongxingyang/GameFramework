using System.Collections.Generic;
using GameFramework.UI.UITools;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.UI.UIExtension.UILayout
{
    public class LayoutLoopList : LayoutListBase
    {
        struct LoopInfo
        {
            public int Left { get; set; }
            public int Length { get; set; }

            public static bool operator !=(LoopInfo a, LoopInfo b)
            {
                return a.Left != b.Left || a.Length != b.Length;
            }

            public static bool operator ==(LoopInfo a, LoopInfo b)
            {
                return !(a != b);
            }

            private bool Equals(LoopInfo other)
            {
                return Left == other.Left && Length == other.Length;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is LoopInfo && Equals((LoopInfo) obj);
            }

            public override int GetHashCode()
            {
                return this.ToString().GetHashCode();
            }

            public override string ToString()
            {
                return string.Format("[{0},{1}]", Left, Length);
            }

            public void SetZero()
            {
                Left = 0;
                Length = 0;
            }

            public int GetOffset(int p)
            {
                return p - Left;
            }

            public LoopInfo Clamp(LoopInfo other)
            {
                LoopInfo newPack = new LoopInfo {Left = Mathf.Max(Left, other.Left)};
                newPack.Length = Mathf.Min(Left + Length, other.Left + other.Length) - newPack.Left;
                return newPack;
            }
        }

        private List<LayoutItem> viewList = new List<LayoutItem>(0);
        private LoopInfo viewRange;
        private RectTransform viewRect;
        private int ScrollSteps => IsVertical ? (int) realBound.y : (int) realBound.x;
        private int PageStepLen => IsVertical ? RowCount : ColumnCount;

        public float PreOutLook = 1f;

        protected override void OnInit()
        {
            base.OnInit();
            if (PreOutLook < 1)
            {
                PreOutLook = 1;
            }
        }

        public override void RePaint()
        {
            Init();
            RecalculateBound();
            viewRange.SetZero();
            UpdateContents();
        }

        protected override void UpdateContents()
        {
            UpdatePageSize();
            LoopInfo clampedRange = CalcViewRange();
            if (clampedRange != viewRange)
            {
                UpdateViewRange(clampedRange.Left);
            }
        }

        public override LayoutItem GetListItem(int index)
        {
            if (IsInited)
            {
                int offset = index - viewRange.Left * PageDiv;
                if (offset > -1 && offset < viewList.Count)
                {
                    LayoutItem item = viewList[offset];
                    return item;
                }
            }
            return null;
        }

        public override void RefreshItem(int index)
        {
            Init();
            int offset = index - viewRange.Left * PageDiv;
            if (offset < 0 || offset >= viewList.Count)
            {
                return;
            }
            UpdateItem(viewList, offset, OffsetByPage(viewRange, offset));
        }

        public override void AddItem(int index, int count)
        {
            Init();
            if (index < 0 || index > ItemCount)
            {
                return;
            }
            if (count < 1)
            {
                return;
            }
            itemCount += count;
            int max = Mathf.Min(viewList.Count + count, PageSize);
            int deltaSize = max - viewList.Count;
            for (int i = 0; i < deltaSize; i++)
            {
                viewList.Add(null);
            }
            int offset = GetPageOffset(viewRange, index);
            if (offset < 0) offset = 0;
            for (int i = offset; i < viewList.Count; i++)
            {
                UpdateItem(viewList, i, OffsetByPage(viewRange, i));
            }
            RecalculateBound();
            UpdateContents();
        }

        public override void RemoveItem(int index, int count)
        {
            if (IsInited)
            {
                if (index < 0 || index >= ItemCount)
                {
                    return;
                }
                if (count < 1)
                {
                    return;
                }
                itemCount -= count;
                if (itemCount < index) itemCount = index;
                if (itemCount < 0) itemCount = 0;
                int offset = GetPageOffset(viewRange, index);
                int max = GetPageOffset(viewRange, ItemCount);
                if (offset < 0) offset = 0;
                for (int i = offset; i < viewList.Count && i < max; i++)
                {
                    UpdateItem(viewList, i, OffsetByPage(viewRange, i));
                }
                for (int i = viewList.Count; i > max && i > 0; i--)
                {
                    DisposeItem(viewList, i - 1);
                }
                RecalculateBound();
                UpdateContents();
            }
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            viewList.Clear();
            viewList = null;
        }

        private void UpdatePageSize()
        {
            if (viewRect == null)
            {
                ScrollRect scr = ScrollRect;
                if (scr != null)
                {
                    viewRect = scr.viewport ?? scr.GetComponent<RectTransform>();
                }
            }
            if (viewRect == null)
            {
                enabled = false;
                return;
            }
            if (IsVertical)
            {
                int h = Mathf.CeilToInt(viewRect.rect.height / TemplateDiv + PreOutLook * 2) + 1;
                RowCount = h;
            }
            else
            {
                int w = Mathf.CeilToInt(viewRect.rect.width / TemplateDiv + PreOutLook * 2) + 1;
                ColumnCount = w;
            }
        }

        public bool IsListItemVisible(int index, float err = 1f)
        {
            if (IsInited)
            {
                RectTransform rt_v = ScrollRect.viewport;
                LayoutItem item = GetListItem(index);
                if (item != null)
                {
                    Rect rect_child = UGUITools.GetRelativeRect(rt_v, item.CacheRectTransform);
                    Rect rect_v = rt_v.rect;
                    if (IsVertical)
                    {
                        bool is_inside = rect_child.yMin + err >= rect_v.yMin && rect_child.yMax - err <= rect_v.yMax;
                        return is_inside;
                    }
                    else
                    {
                        bool is_inside = rect_child.xMin + err >= rect_v.xMin && rect_child.xMax - err <= rect_v.xMax;
                        return is_inside;
                    }
                }
            }
            return false;
        }

        public override void ScrollToStep(int step)
        {
            base.ScrollToStep(step / PageDiv);
            UpdateContents();
        }

        private void MoveItems(List<LayoutItem> items, int start, int count, int dir)
        {
            if (dir != 0)
            {
                if (dir > 0)
                {
                    for (int i = start + count - 1; i >= start; i--)
                    {
                        SwapItem(items, i + dir, i);
                    }
                }
                else
                {
                    for (int i = start; i <= start + count - 1; i++)
                    {
                        SwapItem(viewList, i + dir, i);
                    }
                }
            }
        }

        private void SwapItem(List<LayoutItem> items, int destPos, int srcPos)
        {
            LayoutItem tmp = items[destPos];
            items[destPos] = items[srcPos];
            items[srcPos] = tmp;
        }

        private void UpdateItem(List<LayoutItem> items, int pos, int index)
        {
            if (items.Count > pos)
            {
                if (items[pos] == null)
                {
                    items[pos] = TryCreateItem();
                }

                LayoutItem item = items[pos];
                AdjustItemAnchor(item.CacheRectTransform);
                item.SetPosition(IndexToPosition(index, item.CacheRectTransform));
                item.UpdateItem(index, true);
                if (SingleSelect)
                {
                    item.IsOn = (SingleSelectIndex == index);
                }
            }
        }

        private int GetPageOffset(LoopInfo view, int index)
        {
            return index - view.Left * PageDiv;
        }

        private int OffsetByPage(LoopInfo view, int offset)
        {
            return offset + view.Left * PageDiv;
        }

        private void Update()
        {
            if (ItemCount > 0)
            {
                UpdateContents();
            }
        }

        private void UpdateViewRange(int scrollStep)
        {
            LoopInfo target = new LoopInfo
            {
                Left = scrollStep,
                Length = PageStepLen
            };
            int viewCount = Mathf.Min(ItemCount - target.Left * PageDiv, PageSize);
            if (viewCount < 0) viewCount = 0;
            int deltaSize = viewCount - viewList.Count;
            for (int i = 0; i < deltaSize; i++)
            {
                viewList.Add(null);
            }
            LoopInfo intersection = target.Clamp(viewRange);
            int sstart = 0;
            int scount = 0;
            if (intersection.Length > 0)
            {
                sstart = Mathf.Max(0, target.GetOffset(intersection.Left) * PageDiv);
                scount = Mathf.Min(intersection.Length * PageDiv, viewCount);
                var sdir = target.GetOffset(viewRange.Left) * PageDiv;
                MoveItems(viewList, sstart - sdir, scount, sdir);
            }
            for (int i = 0; i < -deltaSize; i++)
            {
                DisposeItem(viewList, viewList.Count - 1);
            }
            int a = sstart;
            int b = a + scount - 1;
            for (int i = 0; i < viewCount; i++)
            {
                if (i < a || i > b)
                {
                    UpdateItem(viewList, i, OffsetByPage(target, i));
                }
            }
            viewRange.Left = scrollStep;
            viewRange.Length = PageStepLen;
        }

        private LoopInfo CalcViewRange()
        {
            float curPos = CurDirectionPos - (TemplateDiv) * PreOutLook - Padding.y;
            int scrollIndex = ScrollPosToScrollStep(curPos);
            LoopInfo clampedRange = new LoopInfo
            {
                Left = scrollIndex + PageStepLen > ScrollSteps ? ScrollSteps - PageStepLen : scrollIndex
            };
            if (clampedRange.Left < 0) clampedRange.Left = 0;
            clampedRange.Length = PageStepLen;
            return clampedRange;
        }
    }
}