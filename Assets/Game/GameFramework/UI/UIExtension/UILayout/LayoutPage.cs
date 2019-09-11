using System.Collections.Generic;
using GameFramework.UI.UITools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameFramework.UI.UIExtension.UILayout
{
    public class LayoutPage : MonoBehaviour, IEndDragHandler,IBeginDragHandler
    {
        public delegate void UpdateGridItem(GameObject item, int index, int pageIndex);
        
        private float startPos;
        private float endPos;
        private float currentScPos = 0;
        private GameObject maxPageObj;
        private GameObject minPageObj;
        public GameObject content;
        public GameObject scroll;
        public GameObject itemPrefab;
        private bool isHorizontal;
        private UpdateGridItem m_updateItem;
        public int pageCount { get; set; }
        private int cellsMaxCountInPage;
        private float gapH;
        private float gapV;
        private float itemWidth;
        private float itemHeight;
        private RectTransform contentRectTf;
        private List<GameObject> pageList = null;
        private List<List<GameObject>> pageItemList = null;
        public int cellCount { get; set; }
        private int rows;
        private int columns;
        public int CurPageIndex { get; set; }
        private ScrollRect sr;
        private Vector2 pageSize;
        private int lastPageItemCount;
        private float pageStep;
        
        public void init(int rows, int columns,int cellCount, bool isHorizontal = true, float gapH = 5,float gapV = 5,UpdateGridItem updateItem = null)
        {
            this.rows = rows;
            this.columns = columns;
            this.cellsMaxCountInPage = rows * columns;
            if (cellCount < 0) cellCount = 0;
            if (this.scroll == null) return;
            if (this.content == null) return;
            this.m_updateItem = updateItem;
            this.cellCount = cellCount;
            this.isHorizontal = isHorizontal;
            this.gapH = gapH;
            this.gapV = gapV;
            this.CurPageIndex = 1;
            this.itemWidth = this.itemPrefab.GetComponent<RectTransform>().sizeDelta.x;
            this.itemHeight = this.itemPrefab.GetComponent<RectTransform>().sizeDelta.y;

            this.pageSize = new Vector2(this.columns * (this.itemWidth + this.gapH)-this.gapH,
                this.rows * (this.itemHeight + this.gapV)-this.gapV);
            this.scroll.GetComponent<RectTransform>().sizeDelta = pageSize;
            this.sr = this.scroll.GetComponent<ScrollRect>();
            this.sr.horizontal = this.isHorizontal;
            this.sr.vertical = !this.isHorizontal;
            this.contentRectTf = this.content.GetComponent<RectTransform>();
            this.content.transform.localPosition = new Vector3(0, 0);
            this.reloadData(cellCount);
        }
        
        public void reloadData(int cellCount)
        {
            if (cellCount % this.cellsMaxCountInPage == 0)
                this.pageCount = cellCount / this.cellsMaxCountInPage;
            else
                this.pageCount = cellCount / this.cellsMaxCountInPage + 1;
            
            pageStep = 1f / (this.pageCount-1);
            this.lastPageItemCount = cellCount % this.cellsMaxCountInPage;
            if (lastPageItemCount == 0) lastPageItemCount = this.cellsMaxCountInPage;
            if (this.pageCount > 3)
            {
                this.createLoopPageItem(this.itemPrefab, 3, true);
            }
            else
            {
                this.createLoopPageItem(this.itemPrefab, pageCount, false);
            }
           
            this.updateContentSize();
            this.layoutPage(this.pageCount>3);
            this.updatePageItemActive();
            this.startRefreshPage();
        }
        
        private void updatePageItemActive()
        {
            if (this.pageList == null) return;
            if (this.pageList.Count == 0) return;
            if (this.pageCount > 3)
            {
                foreach (GameObject t in this.pageList)
                {
                    if (t.gameObject.name == "page" + 0)
                    {
                        UGUITools.SetVisible(t.transform,false);
                    }
                    else if (t.gameObject.name == "page" + (this.pageCount + 1))
                    {
                        UGUITools.SetVisible(t.transform,false);
                    }
                    else
                    {
                        UGUITools.SetVisible(t.transform,true);
                    }
                }
            }
        }
        
        private void layoutPage(bool isloop)
        {
            if (this.pageList == null) return;
            int count = this.pageList.Count;
            if (isloop)
            {
                for (int i = -1; i < count - 1; ++i)
                {
                    GameObject pageGo = this.pageList[i + 1];
                    if (!this.isHorizontal)
                        pageGo.transform.localPosition = new Vector3(0,  - this.pageSize.y * i);
                    else
                        pageGo.transform.localPosition = new Vector3( + this.pageSize.x * i, 0);
                }
            }
            else
            {
                for (int i = 0; i < count; ++i)
                {
                    GameObject pageGo = this.pageList[i];
                    if (!this.isHorizontal)
                        pageGo.transform.localPosition = new Vector3(0,  - this.pageSize.y * i);
                    else
                        pageGo.transform.localPosition = new Vector3( + this.pageSize.x * i, 0);
                }
            }
            
        }
        

        private void startRefreshPage()
        {
            if (this.pageList == null || this.pageList.Count == 0) return;
            if (this.pageCount > 3)
            {
                for (int i = 0; i < pageList.Count; i++)
                {
                    if (pageList[i].transform.localScale == Vector3.zero)
                    {
                        continue;
                    }
                    List<GameObject> itemList = this.pageItemList[i];
                    int count = itemList.Count;
                    for (int j = 0; j < count; ++j)
                    {
                        int itemIndex = j;
                        GameObject item = itemList[j];
                        if (itemIndex <= this.cellCount - 1)
                        {
                            this.m_updateItem?.Invoke(item, itemIndex, i);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < this.pageItemList.Count ; ++i)
                {
                    List<GameObject> itemList = this.pageItemList[i];
                    int count = itemList.Count;
                    
                    for (int j = 0; j < count; ++j)
                    {
                        int itemIndex = j;
                        GameObject item = itemList[j];
                        if (itemIndex <= this.cellCount - 1)
                        {
                            this.m_updateItem?.Invoke(item, itemIndex, i+1);
                        }
                        else
                        {
                            UGUITools.SetVisible(item.transform,false);
                        }
                    }
                    
                }
            }
        }

        private void createLoopPageItem(GameObject prefab, int createPageCount,bool isLoop)
        {
            if (createPageCount <= 0) return;
            if (this.pageList == null) 
                this.pageList = new List<GameObject>();
    
            if (this.pageItemList == null)
                this.pageItemList = new List<List<GameObject>>();
    
            for (int i = 0; i < createPageCount; ++i)
            {
                GameObject pageContainer = new GameObject();
                pageContainer.AddComponent<RectTransform>();
                pageContainer.AddComponent<CanvasRenderer>();
                pageContainer.transform.SetParent(this.content.gameObject.transform);
                pageContainer.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
                pageContainer.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                pageContainer.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
                pageContainer.GetComponent<RectTransform>().sizeDelta = this.pageSize;
                pageContainer.transform.localScale = new Vector3(1, 1, 1);
                List<GameObject> list = new List<GameObject>();
                this.pageItemList.Add(list);
                if (isLoop)
                {
                    pageContainer.name = "page" + i;
                }
                else
                {
                    pageContainer.name = "page" + (i+1);
                }
                this.pageList.Add(pageContainer);

                int row = 0;
                int col = 0;
                for (int j = 0; j < this.cellsMaxCountInPage; ++j)
                {
                    GameObject item = MonoBehaviour.Instantiate(prefab, new Vector3(0, 0), new Quaternion());
                    item.transform.SetParent(pageContainer.transform);
                    item.transform.localScale = new Vector3(1, 1, 1);
                    list.Add(item);
                    float x = col * (this.itemWidth + this.gapH);
                    float y = row * -(this.itemHeight + this.gapV);
                    item.transform.localPosition = new Vector3(x, y);
                    col++;
                    if (col == this.columns)
                    {
                        col = 0;
                        row++;
                    }
                }
            }
        }
     
        
        private void updateContentSize()
        {
            Vector2 size;
            if (this.isHorizontal)
                size = new Vector2(this.pageSize.x * this.pageCount,
                    this.pageSize.y);
            else
                size = new Vector2(this.pageSize.x,
                    this.pageSize.y * this.pageCount);
            this.contentRectTf.sizeDelta = size;
        }


        
        private void refreshPage(int pageindex,bool isright)
        {
            if (this.CurPageIndex == 1)
            {
                return;
            }
            List<GameObject> itemList = this.pageItemList[pageindex];
            int count = itemList.Count;
            if (isright && this.CurPageIndex == (this.pageCount-1) && this.lastPageItemCount != 0)
            {
                for (int i = 0; i < count; i++)
                {
                    var item = itemList[i];
                    if (i < this.lastPageItemCount)
                    {
                        UGUITools.SetVisible(item.transform,true);
                        this.m_updateItem?.Invoke(item, i, this.CurPageIndex+1);
                    }
                    else
                    {
                        UGUITools.SetVisible(item.transform,false);
                    }
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    var item = itemList[i];
                    UGUITools.SetVisible(item.transform,true);
                    this.m_updateItem?.Invoke(item, i, isright?this.CurPageIndex+1:this.CurPageIndex-1);
                }
            }
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            endPos = sr.horizontalNormalizedPosition;
            if (startPos > endPos) 
            {
                if ((startPos - endPos) > pageStep / 2 && this.CurPageIndex > 1)
                {
                    currentScPos = (currentScPos -= pageStep);
                    if (currentScPos < 0)
                    {
                        currentScPos = 0;
                    }
                    sr.horizontalNormalizedPosition = currentScPos;
                    if (this.pageCount > 3)
                    {
                        var pageindex = (this.CurPageIndex + 1) % 3;
                        this.pageList[pageindex].transform.localPosition = new Vector3(
                            this.pageList[pageindex].transform.localPosition.x - this.pageSize.x * 3,
                            this.pageList[pageindex].transform.localPosition.y,
                            this.pageList[pageindex].transform.localPosition.z);
                        this.CurPageIndex--;
                        this.pageList[pageindex].transform.gameObject.name = "page" + (this.CurPageIndex - 1);
                        this.refreshPage(pageindex,false);
                    }
                    else
                    {
                        this.CurPageIndex--;
                    }
                   
                    this.updatePageItemActive();
                }
                else
                {
                    sr.horizontalNormalizedPosition = startPos;
                }
            }
            else
            {
                if ((endPos - startPos) > pageStep / 2 && this.CurPageIndex < this.pageCount)
                {
                    currentScPos = (currentScPos += pageStep);
                    if (currentScPos > 1)
                    {
                        currentScPos = 1;
                    }
                    sr.horizontalNormalizedPosition = currentScPos;
                    if (this.pageCount > 3)
                    {
                        var pageindex = (this.CurPageIndex - 1) % 3;
                        this.pageList[pageindex].transform.localPosition = new Vector3(
                            this.pageList[pageindex].transform.localPosition.x + this.pageSize.x * 3,
                            this.pageList[pageindex].transform.localPosition.y,
                            this.pageList[pageindex].transform.localPosition.z);
                        this.CurPageIndex++;
                        this.pageList[pageindex].transform.gameObject.name = "page" + (this.CurPageIndex + 1);
                        this.refreshPage(pageindex,true);
                    }
                    else
                    {
                        this.CurPageIndex++;
                    }
                   
                    this.updatePageItemActive();
                }
                else
                {
                    sr.horizontalNormalizedPosition = startPos;
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            startPos = sr.horizontalNormalizedPosition;
        }

        public void GotoPage(int page)
        {
            if(page<1 || page>this.pageCount || CurPageIndex == page) return;
            var newPos = (page-1) * pageStep;
            if (newPos > 1)
            {
                newPos = 1;
            }
            if (this.pageCount > 3)
            {
                var currentPage = this.CurPageIndex;
                if (currentPage - page > 0)
                {
                    for (int i = 0; i < currentPage - page; i++)
                    {
                        currentScPos = (currentScPos -= pageStep);
                        if (currentScPos < 0)
                        {
                            currentScPos = 0;
                        }
                        sr.horizontalNormalizedPosition = currentScPos;
                        var pageindex = (this.CurPageIndex + 1) % 3;
                        this.pageList[pageindex].transform.localPosition = new Vector3(
                            this.pageList[pageindex].transform.localPosition.x - this.pageSize.x * 3,
                            this.pageList[pageindex].transform.localPosition.y,
                            this.pageList[pageindex].transform.localPosition.z);
                        this.CurPageIndex--;
                        this.pageList[pageindex].transform.gameObject.name = "page" + (this.CurPageIndex - 1);
                        this.refreshPage(pageindex,false);
                    }
                    this.updatePageItemActive();
                }
                else
                {
                    for (int i = 0; i < page - currentPage; i++)
                    {
                        currentScPos = (currentScPos += pageStep);
                        if (currentScPos > 1)
                        {
                            currentScPos = 1;
                        }
                        sr.horizontalNormalizedPosition = currentScPos;
                        var pageindex = (this.CurPageIndex - 1) % 3;
                        this.pageList[pageindex].transform.localPosition = new Vector3(
                            this.pageList[pageindex].transform.localPosition.x + this.pageSize.x * 3,
                            this.pageList[pageindex].transform.localPosition.y,
                            this.pageList[pageindex].transform.localPosition.z);
                        this.CurPageIndex++;
                        this.pageList[pageindex].transform.gameObject.name = "page" + (this.CurPageIndex + 1);
                        this.refreshPage(pageindex,true);
                    }
                    this.updatePageItemActive();
                }                
            }
            else
            {
                CurPageIndex = page;
                sr.horizontalNormalizedPosition = newPos;
            }
        }

        public GameObject GetPageItem(int offect, int index)
        {
            //offect 0 当前显示页面  = -1 当前显示页面的前一页  = 1 当前显示页面的后一页 
            //支持取 当前page前一页 当前page 当前page后一页 // index从0开始
            var newPageIndex = this.CurPageIndex + offect;
            if (newPageIndex < 1 || newPageIndex > this.pageCount)
            {
                return null;
            }
            for (int i = 0; i < pageList.Count; i++)
            {
                if (pageList[i].name == "page"+newPageIndex)
                {
                    List<GameObject> itemList = this.pageItemList[i];
                    return itemList[index];
                }
            }
            return null;
        }
    }
}