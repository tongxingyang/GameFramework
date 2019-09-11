using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameFramework.UI.UIExtension.UILayout
{
    public delegate void ItemDelegate(GameObject go, int index);
    public delegate void LayoutListItemDelegate(GameObject list, GameObject item, int index);
    
    public class LayoutItem : UIBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public int Index;
        public object Data;
        protected ItemDelegate onItemInit;
        protected ItemDelegate onItemClick;
        protected ItemDelegate onItemClickButton;
        protected ItemDelegate onItemLongPress;
        protected UnityEvent<bool> onItemSelect;
        protected UnityEvent<bool> onItemSelectInv;

        private PointerEventData pressEventData = null;
        private float pressTime = 0.0f;
        private float longPressTime = 1.0f;
        private bool isOn = false;
        private Transform cacheTransform;
        private RectTransform cacheRectTransform;

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

        public virtual bool IsOn
        {
            get { return isOn; }
            set
            {
                isOn = value;
                onItemSelect?.Invoke(isOn);
                onItemSelectInv?.Invoke(!isOn);
            }
        }

        public ItemDelegate OnItemInit
        {
            get { return onItemInit; }
            set { onItemInit = value; }
        }

        public ItemDelegate OnItemClick
        {
            get { return onItemClick; }
            set { onItemClick = value; }
        }

        public ItemDelegate OnItemLongPress
        {
            get { return onItemLongPress; }
            set { onItemLongPress = value; }
        }

        public ItemDelegate OnItemClickButton
        {
            get { return onItemClickButton; }
            set
            {
                onItemClickButton = value;
                InitChildButtons();
            }
        }

        public static LayoutItem GetLayoutItem(GameObject g)
        {
            LayoutItem item = g.GetComponent<LayoutItem>() ?? g.AddComponent<LayoutItem>();
            return item;
        }

        protected override void Awake()
        {
        }

        protected override void Start()
        {
        }

        void Update()
        {
            if (pressEventData != null)
            {
                pressTime += Time.unscaledDeltaTime;
                if (pressTime > longPressTime)
                {
                    pressEventData.eligibleForClick = false;
                    pressTime = 0.0f;
                    pressEventData = null;
                    OnItemLongPress?.Invoke(gameObject, Index);
                }
            }
        }

        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            pressEventData = null;
            pressTime = 0;
        }

        protected override void OnDestroy()
        {
            pressEventData = null;
            pressTime = 0;
            Data = null;
            onItemInit = null;
            onItemClick = null;
            onItemClickButton = null;
            onItemLongPress = null;
            onItemSelect = null;
            onItemSelectInv = null;
        }

        public virtual void UpdateItem(int index, bool isNeedInit)
        {
            Index = index;
            gameObject.name = "item-" + index;
            if (isNeedInit)
            {
                OnItemInit?.Invoke(gameObject, index);
            }
        }

        private void OnItemButtonClicked(GameObject go)
        {
            OnItemClickButton?.Invoke(go, Index);
        }

        private void InitChildButtons()
        {
            Button[] buttons = gameObject.GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons)
            {
                btn.onClick.AddListener(delegate() { OnItemButtonClicked(btn.gameObject); });
            }
        }

        public void SetPosition(Vector2 anchoredPos)
        {
            CacheRectTransform.anchoredPosition = anchoredPos;
        }

        public virtual void SetData(object data)
        {
            Data = data;
        }

        public virtual void OnButtonClickHandler(GameObject obj)
        {
        }

        public virtual void OnButtonSelectHandler(GameObject value)
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnItemClick?.Invoke(gameObject, Index);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            pressEventData = eventData;
            pressTime = 0;
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (pressEventData != null && eventData.pointerId == pressEventData.pointerId)
            {
                pressEventData = null;
            }
        }
    }
}