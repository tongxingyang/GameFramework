using System.Collections.Generic;
using GameFramework.Animation.UIAnimation;
using GameFramework.Pool;
using UnityEngine;

namespace GameFramework.UI.UIExtension.HUD
{
    public class HUDManager
    {
        private Dictionary<string ,MotionTextModel> motionTextModels = new Dictionary<string, MotionTextModel>();
        private UnityObjectPool _HudTextPool = null;
        private List<HUDText> currentShowHudTexts = new List<HUDText>();
        private List<HUDText> removeHUDList = new List<HUDText>();
        private bool isInit = false;
        private static string path = "UI/HUD/";
        public void Init()
        {
            isInit = true;
            var hudtext = Resources.Load<GameObject>(path + "HUDText");
            _HudTextPool = new UnityObjectPool(hudtext, 10);
            HUDText.OnComplete = RecycleText;
            motionTextModels.Clear();
        }
        
        public void OnLateUpdate()
        {
            if (AppConst.GlobalCahce.Main3DCamera == null || Time.timeScale == 0.0f || currentShowHudTexts.Count <=0 )           
            {
                return;
            }
            UpdateHUDText();
        }
        
        void UpdateHUDText()
        {
            removeHUDList.Clear();
            foreach (HUDText currentShowHudText in currentShowHudTexts)
            {
                if (currentShowHudText.IsFollow)
                {
                    
                    if (currentShowHudText.FollowGameObject == null)
                    {
                        removeHUDList.Add(currentShowHudText);
                    }
                    else
                    {
                        UpdateHUDTextAnim(currentShowHudText);
                        var pos = currentShowHudText.FollowGameObject.transform.position;
                        pos.y += GetModelHeight(currentShowHudText.FollowGameObject, 1, false);
                        if (currentShowHudText.IsPlayEndHide)
                        {
                            currentShowHudText.currentTime += Time.deltaTime;
                            if (currentShowHudText.currentTime >= currentShowHudText.TotalTime)
                            {
                                currentShowHudText.OnMotionEnd();
                                removeHUDList.Add(currentShowHudText);
                            }
                        }
                    }
                }
                else
                {
                    UpdateHUDTextAnim(currentShowHudText);
                    if (currentShowHudText.IsPlayEndHide)
                    {
                        currentShowHudText.currentTime += Time.deltaTime;
                        if (currentShowHudText.currentTime >= currentShowHudText.TotalTime)
                        {
                            currentShowHudText.OnMotionEnd();
                            removeHUDList.Add(currentShowHudText);
                        }
                    }
                }
            }
            foreach (HUDText hudText in removeHUDList)
            {
                currentShowHudTexts.Remove(hudText);
            }
            
        }
        
        private void UpdateHUDTextAnim(HUDText text)
        {
            if (text.currentModel.Models.Count > 0)
            {
                foreach (MotionModel motion in text.currentModel.Models)
                {
                    if (motion == null) continue; 
                    var duration = motion.duration;
                    var dalay = motion.delay;
                    var motionType = motion.GetMotionType();
                    if (motionType == enHUDMotionType.Scale)
                    {
                        var model = motion as MotionScaleModel;
                        text.currentTime += Time.deltaTime;
                        if (model != null)
                        {
                            Vector3 startValue = model.startScale;
                            Vector3 endValue = model.endScale;
                            if (text.currentTime > dalay)
                            {
                                float animTime = text.currentTime - dalay;
                                float t = animTime / duration;
                                float ease = UIAnimationEquations.GetEaseFloat(t, model.easeType, model.Parameters);
                                Vector3 value = UnClampedLerp(startValue, endValue, ease);
                                text.RectTrans.localScale = value;
                            }
                        }
                    }else if (motionType == enHUDMotionType.Alpha)
                    {
                        var model = motion as MotionAlphaModel;
                        text.currentTime += Time.deltaTime;
                        if (model != null)
                        {
                            float startValue = model.startAlpha;
                            float endValue = model.endAlpha;
                            if (text.currentTime > dalay)
                            {
                                float animTime = text.currentTime - dalay;
                                float t = animTime / duration;
                                float ease = UIAnimationEquations.GetEaseFloat(t, model.easeType, model.Parameters);
                                float value = UnClampedLerp(startValue, endValue, ease);
                                text.CanvasGrp.alpha = value;
                            }
                        }
                        
                    }else if (motionType == enHUDMotionType.Linear)
                    {
                        var model = motion as MotionLinearModel;
                        text.currentTime += Time.deltaTime;
                        if (model != null)
                        {
                            Vector3 startValue = model.startOffset;
                            Vector3 endValue = model.endOffset;
                            if (text.currentTime > dalay)
                            {
                                float animTime = text.currentTime - dalay;
                                float t = animTime / duration;
                                float ease = UIAnimationEquations.GetEaseFloat(t, model.easeType, model.Parameters);
                                Vector3 value = UnClampedLerp(startValue, endValue, ease);
                                if (text.IsFollow)
                                {
                                    var pos = text.FollowGameObject.transform.position;
                                    pos.y += GetModelHeight(text.FollowGameObject, 1, false);
                                    Vector3 uipos = TranslatePosToUIPos(pos+text.OffectVector3 + value);
                                    text.RectTrans.position = uipos;
                                }
                                else
                                {
                                    Vector3 uipos = TranslatePosToUIPos(text.StartPos + value);
                                    text.RectTrans.position = uipos;
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public HUDText ShowText(string text, Vector3 pos, string configId, Sprite image, bool isPlayEndHide = true)
        {
            if (!isInit)
            {
                Init();
            }
            if (string.IsNullOrEmpty(text) || AppConst.GlobalCahce.Main3DCamera == null || !AppConst.GlobalCahce.Main3DCamera.enabled )
                return null;
                    
            MotionTextModel model = null;
            configId = path + configId;
            if (!motionTextModels.TryGetValue(configId, out model))
            {
                model = Resources.Load<MotionTextModel>(configId);
                motionTextModels.Add(configId, model);
            }

            if (model != null)
            {
                var hudtext = _HudTextPool.Get() as GameObject;
                if (hudtext != null)
                {
                    var hudTextComp = hudtext.GetComponent<HUDText>();
                    if (hudTextComp != null)
                    {
                        hudTextComp.FollowGameObject = null;
                        hudTextComp.IsFollow = false;
                        hudTextComp.IsPlayEndHide = isPlayEndHide;
                        hudTextComp.RectTrans.SetParent(
                            model.isOnTopLayer ? AppConst.GlobalCahce.PanelHUD.GetChild(1) : AppConst.GlobalCahce.PanelHUD.GetChild(0), false);
                        pos.x += Random.Range(-model.randomNum, model.randomNum);
                        pos.y += Random.Range(-model.randomNum, model.randomNum);
                        pos.z += Random.Range(-model.randomNum, model.randomNum);
                        pos.x += model.offect.x;
                        pos.y += model.offect.y;
                        pos.z += model.offect.z;
                        hudTextComp.StartPos = pos;
                        Vector3 uipos = TranslatePosToUIPos(pos);
                        hudTextComp.Show(text, uipos, model);
                        currentShowHudTexts.Add(hudTextComp);
                        return hudTextComp;
                    }
                }
            }
            return null;
        }
    
        private void RecycleText(HUDText hudtext)
        {
            if (hudtext == null) return;
            hudtext.RectTrans.anchoredPosition3D = new Vector3(10000, 0, 0);
            var go = hudtext.gameObject;
            _HudTextPool?.Release(go);
        }
    
        private Vector3 TranslatePosToUIPos(Vector3 pos)
        {
            Vector2 a = RectTransformUtility.WorldToScreenPoint(AppConst.GlobalCahce.Main3DCamera, pos);
            return a;
        }

        public HUDText ShowText(string text, GameObject target ,string configId, Sprite image ,bool isPlayEndHide = true)
        { 
            GameObject hurtobj = target;
            var pos = hurtobj.transform.position;
            pos.y += GetModelHeight(target, 1, true);
            return ShowText(text, pos, configId,image ,isPlayEndHide);
        }

        public HUDText ShowFollowText(string text, GameObject follow, string configId, Sprite image ,bool isPlayEndHide = true)
        {
            if (!isInit)
            {
                Init();
            }
            if (string.IsNullOrEmpty(text) || AppConst.GlobalCahce.Main3DCamera == null || !AppConst.GlobalCahce.Main3DCamera.enabled || follow == null )
                return null;
            
            MotionTextModel model = null;
            configId = path + configId;
            if (!motionTextModels.TryGetValue(configId, out model))
            {
                model = Resources.Load<MotionTextModel>(configId);
                motionTextModels.Add(configId, model);
            }
            if (model != null)
            {
                var hudtext = _HudTextPool.Get() as GameObject;
                if (hudtext != null)
                {
                    var hudTextComp = hudtext.GetComponent<HUDText>();
                    if (hudTextComp != null)
                    {
                        hudTextComp.FollowGameObject = follow;
                        hudTextComp.IsFollow = true;
                        hudTextComp.IsPlayEndHide = isPlayEndHide;
                        hudTextComp.RectTrans.SetParent(
                            model.isOnTopLayer ? AppConst.GlobalCahce.PanelHUD.GetChild(1) : AppConst.GlobalCahce.PanelHUD.GetChild(0), false);
                        var pos = follow.transform.position;
                        pos.y += GetModelHeight(follow, 1, false);
                        Vector3 offect = Vector3.zero;
                        offect.x += Random.Range(-model.randomNum, model.randomNum);
                        offect.y += Random.Range(-model.randomNum, model.randomNum);
                        offect.z += Random.Range(-model.randomNum, model.randomNum);
                        offect.x += model.offect.x;
                        offect.y += model.offect.y;
                        offect.z += model.offect.z;
                        hudTextComp.OffectVector3 = offect;
                        Vector3 uipos = TranslatePosToUIPos(pos+offect);
                        hudTextComp.Show(text, uipos, model);
                        currentShowHudTexts.Add(hudTextComp);
                        return hudTextComp;
                    }
                }
            }
            return null;
        }
        
        public void Clear()
        {
            motionTextModels.Clear();
            _HudTextPool.ReleaseAll();
        }

        public static float GetModelHeight(GameObject go, float fScale, bool isColliderInChild = false)
        {
            if (go == null) return 0;
            
            if (!isColliderInChild)
            {
                var cc = go.GetComponent<CapsuleCollider>();

                if (cc != null)
                {
                    var halfHeight = cc.height / 2;
                    if (halfHeight < cc.radius) halfHeight = cc.radius;
                    return cc.center.y * fScale + halfHeight * cc.transform.localScale.y * fScale;
                }
                var bc = go.GetComponent<BoxCollider>();
                if (bc != null)
                {
                    return (bc.center.y + bc.size.y / 2) * bc.transform.localScale.y * fScale;
                }
                var sc = go.GetComponent<SphereCollider>();
                if (sc != null)
                    return (sc.center.y + sc.radius) * sc.transform.localScale.y * fScale;
            }
            else
            {
                var cc = go.GetComponentInChildren<CapsuleCollider>();
                if (cc != null)
                {
                    return (cc.center.y + cc.height / 2) * cc.transform.localScale.y * fScale;
                }
                var bc = go.GetComponentInChildren<BoxCollider>();
                if (bc != null)
                {
                    return (bc.center.y + bc.size.y / 2) * bc.transform.localScale.y * fScale;
                }
                var sc = go.GetComponentInChildren<SphereCollider>();
                if (sc != null)
                    return (sc.center.y + sc.radius) * sc.transform.localScale.y * fScale;
            }

            return 0;
        }
        
        public float UnClampedLerp(float a, float b, float t)
        {
            return t * b + (1 - t) * a;
        }
        
        public Vector3 UnClampedLerp(Vector3 a, Vector3 b, float t)
        {
            return new Vector3(UnClampedLerp(a.x, b.x, t), UnClampedLerp(a.y, b.y, t), UnClampedLerp(a.z, b.z, t));
        }
    }
}