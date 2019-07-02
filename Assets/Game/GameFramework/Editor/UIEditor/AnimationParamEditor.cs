using GameFramework.Animation;
using GameFramework.Animation.Base;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GameFramework.Editor.UIEditor
{
    [CustomEditor(typeof(AnimationParam))]
    public class AnimationParamEditor : UnityEditor.Editor
    {
        private AnimationParam param;

        void OnEnable()
        {
            param = (AnimationParam) target;
        }

        public override void OnInspectorGUI()
        {
            param.animationName = EditorGUILayout.TextField(new GUIContent("动画名称"), param.animationName);
            param.delay = EditorGUILayout.FloatField(new GUIContent("延迟时间"), param.delay);
            param.playType = (AnimationPlayType) EditorGUILayout.EnumPopup(new GUIContent("动画类型"),param.playType);
            if (param.playType != AnimationPlayType.Once)
            {
                EditorGUILayout.HelpBox("-1或者0 为永久循环", MessageType.Info);
                param.loopCount = EditorGUILayout.IntField(new GUIContent("循环次数"), param.loopCount);
            }
            param.durationTime = EditorGUILayout.FloatField(new GUIContent("动画时间"), param.durationTime);
            param.isRealTime = EditorGUILayout.Toggle(new GUIContent("是否随时间缩放"), param.isRealTime);
            
            param.isColor = EditorGUILayout.Toggle(new GUIContent("Color Animation"), param.isColor);
            if (param.isColor)
            {
                this.ShowColorParams();
            }
            param.isFading = EditorGUILayout.Toggle(new GUIContent("Fade Animation"), param.isFading);
            if (param.isFading)
            {
                this.ShowFadeParams();
            }
            param.isResizing = EditorGUILayout.Toggle(new GUIContent("Size Animation"), param.isResizing);
            if (param.isResizing)
            {
                this.ShowResizeParams();
            }
            param.isRotating = EditorGUILayout.Toggle(new GUIContent("Rotate Animation"), param.isRotating);
            if (param.isRotating)
            {
                this.ShowRotateParams();
            }
            param.isScaling = EditorGUILayout.Toggle(new GUIContent("Scale Animation"), param.isScaling);
            if (param.isScaling)
            {
                this.ShowScaleParams();
            }
            param.isMoving = EditorGUILayout.Toggle(new GUIContent("Move Animation"), param.isMoving);
            if (param.isMoving)
            {
                this.ShowMoveParams();
            }
            if (GUI.changed)
            {
                EditorUtility.SetDirty(param);
            }
            if (GUILayout.Button("播放")) {
               AnimationComponent.PlayAnimation(param.gameObject,param.animationName);
            }
            
        }

        private void ShowColorParams()
        {
            this.param.startColor = EditorGUILayout.ColorField(new GUIContent("开始颜色值"), param.startColor);
            this.param.targetColor = EditorGUILayout.ColorField(new GUIContent("结束颜色值"), param.targetColor);
            this.param.renderer = EditorGUILayout.ObjectField(new GUIContent("SpriteRenderer", "The renderer of the object."),
                this.param.renderer, typeof(SpriteRenderer), true) as SpriteRenderer;
            this.param.image = EditorGUILayout.ObjectField(new GUIContent("Image", "The image of the object."),
                this.param.image, typeof(Image), true) as Image;
            this.param.colorCurveR = EditorGUILayout.CurveField(new GUIContent("Color R"), this.param.colorCurveR);
            this.param.colorCurveG = EditorGUILayout.CurveField(new GUIContent("Color G"), this.param.colorCurveG);
            this.param.colorCurveB = EditorGUILayout.CurveField(new GUIContent("Color B"), this.param.colorCurveB);
            this.param.colorCurveA = EditorGUILayout.CurveField(new GUIContent("Color A"), this.param.colorCurveA);
        }

        private void ShowFadeParams()
        {
            this.param.startAlpha = EditorGUILayout.FloatField(new GUIContent("开始alpha值"), param.startAlpha);
            this.param.targetAlpha = EditorGUILayout.FloatField(new GUIContent("结束alpha值"), param.targetAlpha);
            this.param.canvasGroup =
                EditorGUILayout.ObjectField(new GUIContent("CanvasGroup", "The canvasGroup of the object."),
                    this.param.canvasGroup, typeof(CanvasGroup), true) as CanvasGroup;
            this.param.spriteRender =
                EditorGUILayout.ObjectField(new GUIContent("SpriteRender", "The spriteRender of the object."),
                    this.param.spriteRender, typeof(SpriteRenderer), true) as SpriteRenderer;
            this.param.fadeImage = EditorGUILayout.ObjectField(new GUIContent("FadeImage", "The image of the object."),
                this.param.fadeImage, typeof(Image), true) as Image;
            this.param.fadeCurve = EditorGUILayout.CurveField(new GUIContent("Fade"), this.param.fadeCurve);
        }
        
        private void ShowResizeParams()
        {
            this.param.startSize = EditorGUILayout.Vector2Field(new GUIContent("开始大小值"), param.startSize);
            this.param.targetSize = EditorGUILayout.Vector2Field(new GUIContent("结束大小值"), param.targetSize);
            this.param.sizeCurveX = EditorGUILayout.CurveField(new GUIContent("Size X"), this.param.sizeCurveX);
            this.param.sizeCurveY = EditorGUILayout.CurveField(new GUIContent("Size Y"), this.param.sizeCurveY);
        }
        
        private void ShowRotateParams()
        {
            this.param.startAngle = EditorGUILayout.Vector3Field(new GUIContent("开始角度值"), param.startAngle);
            this.param.targetAngle = EditorGUILayout.Vector3Field(new GUIContent("结束角度值"), param.targetAngle);
            this.param.angleInterval =
                EditorGUILayout.IntField(new GUIContent("Angle Interval"), this.param.angleInterval);
            this.param.rotateCurveX = EditorGUILayout.CurveField(new GUIContent("Rotate X"), this.param.rotateCurveX);
            this.param.rotateCurveY = EditorGUILayout.CurveField(new GUIContent("Rotate Y"), this.param.rotateCurveY);
            this.param.rotateCurveZ = EditorGUILayout.CurveField(new GUIContent("Rotate Z"), this.param.rotateCurveZ);
        }
        
        private void ShowScaleParams()
        {
            this.param.startScale = EditorGUILayout.Vector3Field(new GUIContent("开始缩放值"), param.startScale);
            this.param.targetScale = EditorGUILayout.Vector3Field(new GUIContent("结束缩放值"), param.targetScale);
            this.param.scaleCurveX = EditorGUILayout.CurveField(new GUIContent("Scale X"), this.param.scaleCurveX);
            this.param.scaleCurveY = EditorGUILayout.CurveField(new GUIContent("Scale Y"), this.param.scaleCurveY);
            this.param.scaleCurveZ = EditorGUILayout.CurveField(new GUIContent("Scale Z"), this.param.scaleCurveZ);
        }
        
        private void ShowMoveParams()
        {
            this.param.startPosition = EditorGUILayout.Vector3Field(new GUIContent("开始位置值"), param.startPosition);
            this.param.targetPosition = EditorGUILayout.Vector3Field(new GUIContent("结束位置值"), param.targetPosition);
            this.param.positionSpace = (enPositionSpace) EditorGUILayout.EnumPopup(new GUIContent("参考坐标系"),param.positionSpace);
            this.param.moveCurveX = EditorGUILayout.CurveField(new GUIContent("Move X"), this.param.moveCurveX);
            this.param.moveCurveY = EditorGUILayout.CurveField(new GUIContent("Move Y"), this.param.moveCurveY);
            this.param.moveCurveZ = EditorGUILayout.CurveField(new GUIContent("Move Z"), this.param.moveCurveZ);
        }
    }
}