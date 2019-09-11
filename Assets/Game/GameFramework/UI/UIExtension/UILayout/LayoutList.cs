using System.Collections.Generic;

namespace GameFramework.UI.UIExtension.UILayout
{
    public class LayoutList : LayoutListBase
    {
        private List<LayoutItem> viewList = new List<LayoutItem>();

        public override void RefreshItem(int index)
        {
            Init();
            if (index < 0 || index >= viewList.Count)
            {
                return;
            }
            UpdateItem(viewList, index, true);
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
            RecalculateBound();
            for (int i = 0; i < count; i++)
            {
                LayoutItem g_item = TryCreateItem();
                viewList.Insert(index + i, g_item);
                UpdateItem(viewList, index + i, true);
            }

            for (int i = index + count, max = viewList.Count; i < max; i++)
            {
                UpdateItem(viewList, i, false);
            }
        }

        public override void RemoveItem(int index, int count)
        {
            if (!IsInited) return;

            if (index < 0 || index >= ItemCount)
            {
                return;
            }

            if (count < 1)
            {
                return;
            }

            itemCount -= count;
            if (ItemCount < index) itemCount = index;
            RecalculateBound();

            while (viewList.Count > ItemCount)
            {
                DisposeItem(viewList, index);
            }

            for (int i = index, max = viewList.Count; i < max; i++)
            {
                UpdateItem(viewList, i, false);
            }
        }

        public override LayoutItem GetListItem(int index)
        {
            if (IsInited)
            {
                if (index > -1 && index < viewList.Count)
                {
                    LayoutItem item = viewList[index];
                    if (item != null)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            viewList.Clear();
            viewList = null;
        }

        protected override void UpdateContents()
        {
            InternalUpdateContent(0);
        }

        private void InternalUpdateContent(int unchangCount)
        {
            for (int i = unchangCount; i < ItemCount; i++)
            {
                UpdateItem(viewList, i, true);
            }

            while (viewList.Count > ItemCount)
            {
                DisposeItem(viewList, viewList.Count - 1);
            }
        }

        private void UpdateItem(List<LayoutItem> items, int pos, bool isNeedInit)
        {
            LayoutItem item = null;
            if (items.Count > pos)
            {
                item = items[pos];
            }
            else
            {
                item = TryCreateItem();
                items.Add(item);
            }

            AdjustItemAnchor(item.CacheRectTransform);
            item.SetPosition(IndexToPosition(pos, item.CacheRectTransform));
            item.UpdateItem(pos, isNeedInit);

            if (SingleSelect)
            {
                item.IsOn = (SingleSelectIndex == pos);
            }
        }
    }
}