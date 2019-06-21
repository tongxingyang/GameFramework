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
            param.loop = EditorGUILayout.Toggle(new GUIContent("是否循环"), param.loop);
            param.startTime = EditorGUILayout.FloatField(new GUIContent("起始时间"), param.startTime);
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
            EditorGUILayout.LabelField(new GUIContent("曲线时长"), new GUIContent(param.colorDuration.ToString()));
            this.param.renderer = EditorGUILayout.ObjectField(new GUIContent("Renderer", "The renderer of the object."),
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
            EditorGUILayout.LabelField(new GUIContent("曲线时长"), new GUIContent(param.fadeDuration.ToString()));
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
            EditorGUILayout.LabelField(new GUIContent("曲线时长"), new GUIContent(param.resizeDuration.ToString()));
            this.param.sizeCurveX = EditorGUILayout.CurveField(new GUIContent("Size X"), this.param.sizeCurveX);
            this.param.sizeCurveY = EditorGUILayout.CurveField(new GUIContent("Size Y"), this.param.sizeCurveY);
        }
        
        private void ShowRotateParams()
        {
            EditorGUILayout.LabelField(new GUIContent("曲线时长"), new GUIContent(param.rotateDuration.ToString()));
            this.param.angleInterval =
                EditorGUILayout.IntField(new GUIContent("Angle Interval"), this.param.angleInterval);
            this.param.rotateCurveX = EditorGUILayout.CurveField(new GUIContent("Rotate X"), this.param.rotateCurveX);
            this.param.rotateCurveY = EditorGUILayout.CurveField(new GUIContent("Rotate Y"), this.param.rotateCurveY);
            this.param.rotateCurveZ = EditorGUILayout.CurveField(new GUIContent("Rotate Z"), this.param.rotateCurveZ);
        }
        
        private void ShowScaleParams()
        {
            EditorGUILayout.LabelField(new GUIContent("曲线时长"), new GUIContent(param.scaleDuration.ToString()));
            this.param.scaleCurveX = EditorGUILayout.CurveField(new GUIContent("Scale X"), this.param.scaleCurveX);
            this.param.scaleCurveY = EditorGUILayout.CurveField(new GUIContent("Scale Y"), this.param.scaleCurveY);
            this.param.scaleCurveZ = EditorGUILayout.CurveField(new GUIContent("Scale Z"), this.param.scaleCurveZ);
        }
        
        private void ShowMoveParams()
        {
            EditorGUILayout.LabelField(new GUIContent("曲线时长"), new GUIContent(param.moveDuration.ToString()));
            this.param.startPosition = EditorGUILayout.Vector3Field(new GUIContent("开始位置"), param.startPosition);
            this.param.targetPosition = EditorGUILayout.Vector3Field(new GUIContent("目标位置"), param.targetPosition);
            this.param.positionSpace = (enPositionSpace) EditorGUILayout.EnumPopup(new GUIContent("参考坐标系"),param.positionSpace);
            this.param.moveCurveX = EditorGUILayout.CurveField(new GUIContent("Move X"), this.param.moveCurveX);
            this.param.moveCurveY = EditorGUILayout.CurveField(new GUIContent("Move Y"), this.param.moveCurveY);
            this.param.moveCurveZ = EditorGUILayout.CurveField(new GUIContent("Move Z"), this.param.moveCurveZ);
        }
    }
}