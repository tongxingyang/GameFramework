using UnityEngine;

namespace GameFramework.Update
{
    public class UpdatePanel
    {
        private GameObject gameHotUpdate = null;
        public GameObject GameHotUpdate
        {
            get
            {
                if (gameHotUpdate == null)
                {
                    GameObject prefab = Resources.Load<GameObject>("UI/PanelHotUpdate");
                    gameHotUpdate = Object.Instantiate(prefab);
                    gameHotUpdate.transform.SetParent(AppConst.GlobalCahce.PanelRoot);
                    RectTransform rectTransform = gameHotUpdate.GetComponent<RectTransform>();
                    rectTransform.offsetMin = Vector2.zero;
                    rectTransform.offsetMax = Vector2.zero;
                    rectTransform.anchoredPosition3D = Vector3.zero;
                    gameHotUpdate.transform.localScale = Vector3.one;
                    gameHotUpdate.SetActive(false);
                }
                return gameHotUpdate;
            }
        }
        
        private GameObject gameHotUpdateMessageBox = null;
        public GameObject GameHotUpdateMessageBox
        {
            get
            {
                if (gameHotUpdateMessageBox == null)
                {
                    GameObject prefab = Resources.Load<GameObject>("UI/PanelHotUpdateMessageBox");
                    gameHotUpdateMessageBox = Object.Instantiate(prefab);
                    gameHotUpdateMessageBox.transform.SetParent(AppConst.GlobalCahce.PanelRoot);
                    RectTransform rectTransform = gameHotUpdateMessageBox.GetComponent<RectTransform>();
                    rectTransform.offsetMin = Vector2.zero;
                    rectTransform.offsetMax = Vector2.zero;
                    rectTransform.anchoredPosition3D = Vector3.zero;
                    gameHotUpdateMessageBox.transform.localScale = Vector3.one;
                    gameHotUpdateMessageBox.SetActive(false);
                }
                return gameHotUpdateMessageBox;
            }
        }
        
        private GameObject gameHotUpdateCircle = null;
        public GameObject GameHotUpdateCircle
        {
            get
            {
                if (gameHotUpdateCircle == null)
                {
                    GameObject prefab = Resources.Load<GameObject>("UI/PanelHotUpdateCircle");
                    gameHotUpdateCircle = Object.Instantiate(prefab);
                    gameHotUpdateCircle.transform.SetParent(AppConst.GlobalCahce.PanelRoot);
                    RectTransform rectTransform = gameHotUpdateCircle.GetComponent<RectTransform>();
                    rectTransform.offsetMin = Vector2.zero;
                    rectTransform.offsetMax = Vector2.zero;
                    rectTransform.anchoredPosition3D = Vector3.zero;
                    gameHotUpdateCircle.transform.localScale = Vector3.one;
                    gameHotUpdateCircle.SetActive(false);
                }
                return gameHotUpdateCircle;
            }
        }
        public void InitPanel()
        {
        }
    }
}