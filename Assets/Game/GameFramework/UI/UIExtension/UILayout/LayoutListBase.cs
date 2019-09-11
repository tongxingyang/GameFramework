using System.Collections.Generic;
using GameFramework.UI.UITools;
using GameFramework.Utility.Extension;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameFramework.UI.UIExtension.UILayout
{
    public abstract class LayoutListBase : UIBehaviour
    {
        public class GNewPrivatePool<T> where T : class
        {
            protected Stack<T> _PoolItems;
            private int _MaxSize;

            public const int DefualtSize = 32;

            public GNewPrivatePool()
            {
                _PoolItems = new Stack<T>();
                _MaxSize = DefualtSize;
            }

            public GNewPrivatePool(int pool_size)
            {
                _PoolItems = new Stack<T>();
                if (pool_size < 0)
                    _MaxSize = DefualtSize;
                else
                    _MaxSize = pool_size;
            }

            public void SetSize(int count)
            {
                if (_PoolItems.Count < count)
                {
                    _MaxSize = count;
                }
            }

            public bool PutIn(T t)
            {
                if (t == null) return false;

                bool rv;

                if (_PoolItems.Count < _MaxSize)
                {
                    _PoolItems.Push(t);
                    rv = true;
                }
                else
                {
                    rv = false;
                }

                return rv;
            }

            public T TakeOut()
            {
                if (_PoolItems.Count > 0)
                {
                    T t = _PoolItems.Pop();

                    return t;
                }
                return null;
            }

            public void Clear()
            {
                _PoolItems.Clear();
            }
        }
        private Transform cacheTransform;
        private RectTransform cacheRectTransform;
        private bool isInited = false;
        protected int itemCount;
        protected Vector4 realBound = Vector4.zero;
        private GNewPrivatePool<LayoutItem> _Pool;

        protected bool IsInited => isInited;
        [SerializeField] protected int rowCount = 1;
        [SerializeField] protected int columnCount = 1;
        [SerializeField] protected Vector2 spcing = Vector2.one;
        [SerializeField] protected Vector4 padding = Vector4.zero;
        [SerializeField] protected UGUITools.enLayoutDirection Direction = UGUITools.enLayoutDirection.Horizontal;
        [SerializeField] protected RectTransform templateRect;
        [SerializeField] protected UGUITools.enLayoutAlign Align = UGUITools.enLayoutAlign.Center;
        [SerializeField] protected bool IsInverseDirection = false;
        [SerializeField] protected bool IsExpandItem = false;

        public LayoutListItemDelegate InitItemCallBack;
        public LayoutListItemDelegate LongPressCallBack;
        public LayoutListItemDelegate ClickItemCallBack;
        public LayoutListItemDelegate ClickItemButtonCallBack;

        public bool NoScroll;
        public bool HasChildButton;
        public bool CenterOnMinimu;
        public int MinimuCount;
        public bool SingleSelect;
        public int PoolSize = -1;
        protected ScrollRect ScrollRect;
        protected int SingleSelectIndex = -1;
        public bool IsVertical => Direction == UGUITools.enLayoutDirection.Vertical;
        public int ItemCount => itemCount;

        public int RowCount
        {
            get { return rowCount; }
            set { rowCount = value; }
        }

        public int ColumnCount
        {
            get { return columnCount; }
            set { columnCount = value; }
        }

        public Vector2 Spacing
        {
            get { return spcing; }
            set { spcing = value; }
        }

        public Vector4 Padding
        {
            get { return padding; }
            set { padding = value; }
        }

        public RectTransform TemplateRect
        {
            get { return templateRect; }
            set { templateRect = value; }
        }

        public int PageDiv => IsVertical ? ColumnCount : RowCount;

        protected virtual Vector2 TemplateRectSize =>
            TemplateRect != null ? TemplateRect.rect.size : new Vector2(100, 100);

        public float TemplateDiv => IsVertical ? TemplateRectSize.y + Spacing.y : TemplateRectSize.x + Spacing.x;
        public int PageSize => RowCount * ColumnCount;
        public float SpacingDiv => IsVertical ? Spacing.y : Spacing.x;
        public float DirSign => IsInverseDirection ? 1.0f : -1.0f;

        public float RelativePos =>
            IsVertical ? CacheRectTransform.anchoredPosition.y : CacheRectTransform.anchoredPosition.x;

        public float CurDirectionPos => -DirSign * RelativePos;
        public Transform CacheTransform => cacheTransform ?? (cacheTransform = transform);

        public RectTransform CacheRectTransform
        {
            get
            {
                if (cacheRectTransform == null)
                {
                    cacheRectTransform = CacheTransform as RectTransform;
                }
                return cacheRectTransform ?? (cacheRectTransform = gameObject.AddComponent<RectTransform>());
            }
        }

        public abstract LayoutItem GetListItem(int index);
        public abstract void RefreshItem(int index);
        public abstract void AddItem(int index, int count);
        public abstract void RemoveItem(int index, int count);

        protected void Init()
        {
            if (!isInited)
            {
                isInited = true;
                OnInit();
            }
        }

        protected virtual void OnInit()
        {
            if (templateRect == null)
            {
                if (CacheTransform.childCount > 0)
                {
                    templateRect = CacheTransform.GetChild(0) as RectTransform;
                }
            }
            if (templateRect != null)
            {
                UGUITools.SetVisible(templateRect, false);
            }
            if (rowCount < 1) rowCount = 1;
            if (columnCount < 1) columnCount = 1;
            if (!NoScroll)
            {
                ScrollRect = UGUITools.SecureComponetInParent<ScrollRect>(CacheRectTransform);
            }
            if (ScrollRect != null)
            {
                ScrollRect.horizontal = !IsVertical;
                ScrollRect.vertical = IsVertical;
            }

            _Pool = new GNewPrivatePool<LayoutItem>(PoolSize);
        }

        protected virtual void OnClickItem(GameObject item, int index)
        {
            ClickItemCallBack?.Invoke(this.gameObject, item, index);
        }

        protected virtual void OnLongPressItem(GameObject item, int index)
        {
            LongPressCallBack?.Invoke(this.gameObject, item, index);
        }

        protected virtual void OnShowItem(GameObject item, int index)
        {
            InitItemCallBack?.Invoke(this.gameObject, item, index);
        }

        protected virtual void OnClickItemButton(GameObject item, int index)
        {
            ClickItemButtonCallBack?.Invoke(this.gameObject, item, index);
        }

        protected override void Awake()
        {
            base.Awake();
            Init();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            ScrollRect = null;
            _Pool?.Clear();
            InitItemCallBack = null;
            ClickItemCallBack = null;
            ClickItemButtonCallBack = null;
        }

        public void ScrollToPosition(float flatPos)
        {
            Init();
            if (ScrollRect != null)
            {
                ScrollRect.StopMovement();
                Vector2 v = CacheRectTransform.anchoredPosition;
                if (IsVertical)
                {
                    v.y = UGUITools.ClampScrollPos(flatPos, CacheRectTransform, ScrollRect) * -DirSign;
                }
                else
                {
                    v.x = UGUITools.ClampScrollPos(flatPos, CacheRectTransform, ScrollRect) * DirSign;
                }

                CacheRectTransform.anchoredPosition = v;
            }
        }

        public virtual void ScrollToStep(int step)
        {
            Init();
            float pos = ScrollStepToScrollPos(step);
            ScrollToPosition(pos);
        }

        protected float ScrollStepToScrollPos(int step)
        {
            if (step < 0) step = 0;
            float pos = step * TemplateDiv;
            if (IsVertical)
            {
                if (IsInverseDirection)
                {
                    pos += Padding.w;
                }
                else
                {
                    pos += Padding.y;
                }
            }
            else
            {
                if (IsInverseDirection)
                {
                    pos += Padding.z;
                }
                else
                {
                    pos += Padding.x;
                }
            }
            return pos;
        }

        protected int ScrollPosToScrollStep(float pos)
        {
            pos -= SpacingDiv;
            int spos = Mathf.FloorToInt(pos / TemplateDiv);
            return spos;
        }

        public GameObject GetItem(int index)
        {
            LayoutItem item = GetListItem(index);
            if (item != null)
            {
                return item.gameObject;
            }
            return null;
        }

        public virtual void SetItemCount(int count)
        {
            if (count < 0)
            {
                return;
            }
            SingleSelectIndex = -1;
            itemCount = count;
            RePaint();
        }

        protected LayoutItem TryCreateItem()
        {
            LayoutItem rv = null;
            rv = _Pool.TakeOut();
            if (rv == null)
            {
                RectTransform item = UGUITools.Instantiate(TemplateRect) as RectTransform;
                if (item != null)
                {
                    item.SetParent(CacheTransform, false);

                    LayoutItem item_com = item.GetComponent<LayoutItem>() ?? item.gameObject.AddComponent<LayoutItem>();

                    if (this.InitItemCallBack != null)
                    {
                        item_com.OnItemInit = this.OnShowItem;
                    }

                    if (this.ClickItemCallBack != null)
                    {
                        item_com.OnItemClick = this.OnClickItem;
                    }

                    if (this.LongPressCallBack != null)
                    {
                        item_com.OnItemLongPress = this.OnLongPressItem;
                    }

                    if (HasChildButton && this.ClickItemButtonCallBack != null)
                    {
                        item_com.OnItemClickButton = this.OnClickItemButton;
                    }

                    rv = item_com;
                }
            }

            if (rv != null)
            {
                UGUITools.SetVisible(rv.CacheRectTransform, true);
            }
            return rv;
        }

        protected void DisposeItem(List<LayoutItem> items, int index)
        {
            if (items.Count > index)
            {
                LayoutItem item = items[index];
                if (item != null)
                {
                    if (_Pool.PutIn(item))
                    {
                        UGUITools.SetVisible(item.CacheRectTransform, false);
                    }
                    else
                    {
                        Destroy(item.gameObject);
                    }
                }
                items.RemoveAt(index);
            }
        }

        public void SetSelection(int index)
        {
            if (SingleSelect)
            {
                if (SingleSelectIndex != index)
                {
                    LayoutItem item = GetListItem(SingleSelectIndex);
                    if (item != null)
                    {
                        item.IsOn = false;
                    }
                    SingleSelectIndex = index;

                    item = GetListItem(SingleSelectIndex);
                    if (item != null)
                    {
                        item.IsOn = true;
                    }
                    else
                    {
                        SingleSelectIndex = -1;
                    }
                }
            }
        }

        public void SetMultiSelection(int index, bool isOn)
        {
            if (!SingleSelect)
            {
                LayoutItem item = GetListItem(index);
                if (item != null)
                {
                    item.IsOn = isOn;
                }
            }
        }

        public void SetColAndRow(int col, int row)
        {
            RowCount = row;
            ColumnCount = col;
        }

        public virtual void RePaint()
        {
            Init();
            RecalculateBound();
            UpdateContents();
        }

        protected virtual void RecalculateBound()
        {
            if (IsExpandItem)
            {
                if (IsVertical) ColumnCount = 1;
                else RowCount = 1;
            }
            SetPivotAndAnchors(CacheRectTransform, Align);
            realBound = RealRecalculateBound(ItemCount);
            if (realBound.z < 0)
            {
                realBound.z = CacheRectTransform.rect.width;
            }
            if (realBound.w < 0)
            {
                realBound.w = CacheRectTransform.rect.height;
            }
            Vector2 size = new Vector2(realBound.z, realBound.w);
            CacheRectTransform.anchoredPosition = Vector2.zero;
            CacheRectTransform.SetRectTransformDeltaSize(size);
            FitView();
        }

        protected virtual void SetPivotAndAnchors(RectTransform rect, UGUITools.enLayoutAlign align)
        {
            if (IsExpandItem)
            {
                if (IsVertical)
                {
                    if (align == UGUITools.enLayoutAlign.BottomLeft || align == UGUITools.enLayoutAlign.BottomRight)
                        align = UGUITools.enLayoutAlign.Bottom;
                    else if (align == UGUITools.enLayoutAlign.TopLeft || align == UGUITools.enLayoutAlign.TopRight)
                        align = UGUITools.enLayoutAlign.Top;
                    else if (align == UGUITools.enLayoutAlign.Left || align == UGUITools.enLayoutAlign.Right) align = UGUITools.enLayoutAlign.Center;
                }
                else
                {
                    if (align == UGUITools.enLayoutAlign.BottomLeft || align == UGUITools.enLayoutAlign.TopLeft) align = UGUITools.enLayoutAlign.Left;
                    else if (align == UGUITools.enLayoutAlign.TopRight || align == UGUITools.enLayoutAlign.BottomRight)
                        align = UGUITools.enLayoutAlign.Right;
                    else if (align == UGUITools.enLayoutAlign.Top || align == UGUITools.enLayoutAlign.Bottom) align = UGUITools.enLayoutAlign.Center;
                }
            }

            if (ScrollRect != null)
            {
                if (IsVertical)
                {
                    if (align == UGUITools.enLayoutAlign.BottomLeft || align == UGUITools.enLayoutAlign.Left ||
                        align == UGUITools.enLayoutAlign.TopLeft)
                        align = IsInverseDirection ? UGUITools.enLayoutAlign.BottomLeft : UGUITools.enLayoutAlign.TopLeft;
                    else if (align == UGUITools.enLayoutAlign.BottomRight || align == UGUITools.enLayoutAlign.Right ||
                             align == UGUITools.enLayoutAlign.TopRight)
                        align = IsInverseDirection ? UGUITools.enLayoutAlign.BottomRight : UGUITools.enLayoutAlign.TopRight;
                    else align = IsInverseDirection ? UGUITools.enLayoutAlign.Bottom : UGUITools.enLayoutAlign.Top;
                }
                else
                {
                    if (align == UGUITools.enLayoutAlign.BottomLeft || align == UGUITools.enLayoutAlign.Bottom ||
                        align == UGUITools.enLayoutAlign.BottomRight)
                        align = IsInverseDirection ? UGUITools.enLayoutAlign.BottomRight : UGUITools.enLayoutAlign.BottomLeft;
                    else if (align == UGUITools.enLayoutAlign.TopLeft || align == UGUITools.enLayoutAlign.Top ||
                             align == UGUITools.enLayoutAlign.TopRight)
                        align = IsInverseDirection ? UGUITools.enLayoutAlign.TopRight : UGUITools.enLayoutAlign.TopLeft;
                    else align = IsInverseDirection ? UGUITools.enLayoutAlign.Right : UGUITools.enLayoutAlign.Left;
                }
            }

            Vector2 pos = UGUITools.GetAlignedPivot(align);
            TemplateRect.pivot = pos;
            TemplateRect.anchorMax = pos;
            TemplateRect.anchorMin = pos;
            AdjustItemAnchor(rect, false);
        }

        public void AdjustItemAnchor(RectTransform rectItem, bool isItem = true)
        {
            if (IsExpandItem)
            {
                if (IsVertical)
                {
                    rectItem.pivot = TemplateRect.pivot;
                    rectItem.anchorMin = new Vector2(0, rectItem.pivot.y);
                    rectItem.anchorMax = new Vector2(1, rectItem.pivot.y);
                    if (isItem)
                    {
                        rectItem.offsetMin = new Vector2(Padding.x, rectItem.offsetMin.y);
                        rectItem.offsetMax = new Vector2(-Padding.z, rectItem.offsetMax.y);
                    }
                    else
                    {
                        rectItem.offsetMin = Vector2.zero;
                        rectItem.offsetMax = Vector2.zero;
                    }
                }
                else
                {
                    rectItem.pivot = TemplateRect.pivot;
                    rectItem.anchorMin = new Vector2(rectItem.pivot.x, 0);
                    rectItem.anchorMax = new Vector2(rectItem.pivot.x, 1);

                    if (isItem)
                    {
                        rectItem.offsetMin = new Vector2(rectItem.offsetMin.x, Padding.w);
                        rectItem.offsetMax = new Vector2(rectItem.offsetMax.x, -Padding.y);
                    }
                    else
                    {
                        rectItem.offsetMin = Vector2.zero;
                        rectItem.offsetMax = Vector2.zero;
                    }
                }
            }
            else
            {
                rectItem.anchorMin = rectItem.anchorMax = rectItem.pivot = TemplateRect.pivot;
            }
        }

        protected virtual void UpdateContents()
        {
        }

        protected virtual Vector4 RealRecalculateBound(int total)
        {
            Vector4 pack = new Vector4();

            if (IsVertical)
            {
                pack.x = (IsMiniState() && total < ColumnCount) ? total : ColumnCount;
                pack.y = Mathf.CeilToInt(total / (float) ColumnCount);
            }
            else
            {
                pack.y = (IsMiniState() && total < RowCount) ? total : RowCount;
                pack.x = Mathf.CeilToInt(total / (float) RowCount);
            }

            if (IsExpandItem && IsVertical)
            {
                pack.z = -1;
            }
            else
            {
                pack.z = pack.x * (TemplateRectSize.x + Spacing.x) + Padding.x + Padding.z;
                if (pack.x > 0)
                    pack.z -= Spacing.x;
            }

            if (IsExpandItem && !IsVertical)
            {
                pack.w = -1;
            }
            else
            {
                pack.w = pack.y * (TemplateRectSize.y + Spacing.y) + Padding.y + Padding.w;
                if (pack.y > 0)
                    pack.w -= Spacing.y;
            }
            return pack;
        }

        protected void FitView()
        {
            if (CenterOnMinimu)
            {
                Vector2 center = CacheRectTransform.pivot;
                Vector2 pos = CacheRectTransform.anchoredPosition;
                if (this.itemCount < this.MinimuCount)
                {
                    if (ScrollRect != null)
                    {
                        ScrollRect.enabled = false;
                    }
                    if (IsVertical)
                    {
                        center.y = 0.5f;
                        pos.y = 0;
                    }
                    else
                    {
                        center.x = 0.5f;
                        pos.x = 0;
                    }
                    CacheRectTransform.anchorMax = CacheRectTransform.anchorMin = CacheRectTransform.pivot = center;
                    CacheRectTransform.anchoredPosition = pos;
                }
                else
                {
                    if (ScrollRect != null)
                    {
                        ScrollRect.enabled = true;
                    }
                }
            }
        }

        public void EnableScroll(bool enable)
        {
            Init();
            if (ScrollRect != null)
            {
                ScrollRect.enabled = enable;
            }
        }

        bool IsMiniState()
        {
            return CenterOnMinimu && (this.itemCount < this.MinimuCount);
        }

        // todo txy
        protected Vector2 IndexToPosition(int index, RectTransform item)
        {
            int x, y;
            if (IsVertical)
            {
                y = index / ColumnCount;
                x = index - y * ColumnCount;
            }
            else
            {
                x = index / RowCount;
                y = index - x * RowCount;
            }
            Vector2 pivot = item.pivot;
            Vector2 pp = new Vector2(x * (TemplateRectSize.x + Spacing.x), y * (TemplateRectSize.y + Spacing.y));
            if (IsVertical)
            {
                pp.y *= DirSign;
                if (IsInverseDirection)
                {
                    pp.y += Padding.w;
                }
                else
                {
                    pivot.y = pivot.y - 1;
                    pp.y -= Padding.y;
                }

                if (!IsExpandItem)
                {
                    pp.x += Padding.x - pivot.x * (realBound.z - TemplateRectSize.x);
                }
                else
                {
                    pp.x = Mathf.Lerp(Padding.x, -Padding.z, pivot.x);
                }
                pp.y += -pivot.y * (realBound.w - TemplateRectSize.y);
            }
            else
            {
                pp.x *= -DirSign;
                if (IsInverseDirection)
                {
                    pivot.x = pivot.x - 1;
                    pp.x -= Padding.z;
                }
                else
                {
                    pp.x += Padding.x;
                }

                if (!IsExpandItem)
                {
                    pp.y += Padding.y + (1 - pivot.y) * (TemplateRectSize.y - realBound.w);
                }
                else
                {
                    pp.y = Mathf.Lerp(-Padding.w, Padding.y, pivot.y);
                }
                pp.y *= -1;
                pp.x += -pivot.x * (realBound.z - TemplateRectSize.x);
            }

            return pp;
        }
    }
}