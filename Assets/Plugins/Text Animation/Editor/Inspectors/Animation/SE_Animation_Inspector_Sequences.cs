using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

namespace I2.TextAnimation
{
	public partial class SE_Animation_Inspector
    {
        static GUIStyle boxBackground;
        static GUIStyle headerBackground;
        static GUIStyle draggingHandle;
        static GUIStyle elementBackground;
        static GUIStyle footerBackground;
        static GUIStyle preButton;
        static GUIContent buttonPlus, buttonMinus;

        float mSequences_DragOffset;
        public bool mSequence_IsDragging;

        float mTimeLine_Time;
        float mTimeLine_PlayingStartTime;

        static void InitGUIStyles()
        {
            if (boxBackground == null)
            {
                boxBackground = "RL Background";
                headerBackground = "RL Header";
                draggingHandle = "RL DragHandle";
                elementBackground = "RL Element";
                footerBackground = "RL Footer";
                preButton = "RL FooterButton";

                preButton.padding.top = -7;
                buttonPlus = EditorGUIUtility.IconContent("Toolbar Plus", "Add Sequences");
                buttonMinus = EditorGUIUtility.IconContent("Toolbar Minus", "Remove Sequences");
            }
        }
        void OnGUI_Sequences()
        {
            if (buttonPlus == null)
                InitGUIStyles();
            
            GUILayout.BeginHorizontal(headerBackground);
                GUILayout.Label("Sequences");
            GUILayout.EndHorizontal();

            float ElementHeight = 21;
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                GUILayout.Space(2+5+ElementHeight* mAnimation._Sequences.Length + ElementHeight);
            GUILayout.EndVertical();

            if (mCurrentSequenceIndex < 0 && mSequence_IsDragging)
                mSequence_IsDragging = false;


            var currentEvent = Event.current;
            var rect = GUILayoutUtility.GetLastRect();


            {
                float mouseY = currentEvent.mousePosition.y;

                if (currentEvent.type==EventType.Repaint)
                    boxBackground.Draw(rect, false, false, false, false);
                rect.yMax -= ElementHeight;

                // Render Sequences that are not selected (selected can be dragged and so is rendered AFTER everything else to show it on TOP)
                var elementRect = rect;
                for (int i = 0; i < mAnimation._Sequences.Length; ++i)
                {
                    elementRect.yMin = rect.yMin + 2 + i * ElementHeight;
                    elementRect.yMax = elementRect.yMin + ElementHeight;

                    if (mSequence_IsDragging)
                    {
                        if (i == mCurrentSequenceIndex)
                            continue;
                        if (i < mCurrentSequenceIndex && mouseY - mSequences_DragOffset < elementRect.yMax)
                            elementRect.y += Mathf.Min(ElementHeight, elementRect.yMax - (mouseY - mSequences_DragOffset));
                        else
                        if (i > mCurrentSequenceIndex && mouseY - mSequences_DragOffset + ElementHeight > elementRect.yMin)
                            elementRect.y -= Mathf.Min(ElementHeight, (mouseY - mSequences_DragOffset + ElementHeight)- elementRect.yMin);
                    }
                    OnGUI_Animation_SequenceElement(elementRect, i, false, false);
                }

                // Now draw the selected sequence so that it renders ON TOP of everything
                if (mSequence_IsDragging)
                {
                    elementRect.yMin = rect.yMin + 2 + mCurrentSequenceIndex * ElementHeight;
                    elementRect.yMax = elementRect.yMin + ElementHeight;
                    elementRect.y = mouseY - mSequences_DragOffset;

                    // Constrain the element movement to INSIDE the rect
                    elementRect.y += Mathf.Max(0, rect.yMin - elementRect.yMin);
                    elementRect.y -= Mathf.Max(0, elementRect.yMax - rect.yMax);

                    OnGUI_Animation_SequenceElement(elementRect, mCurrentSequenceIndex, false, false);
                }
            }

            if (currentEvent.isMouse && currentEvent.button == 0)
            {
                float mouseY = currentEvent.mousePosition.y - rect.yMin - 2;
                int row = Mathf.FloorToInt(mouseY / ElementHeight);

                switch (currentEvent.type)
                {
                    case EventType.MouseDown:
                        {
                            if (rect.Contains(currentEvent.mousePosition))
                            {
								if (row >= 0 && row < mAnimation._Sequences.Length)
								{
									mCurrentSequenceIndex = row;
									GUI.FocusControl(null);
								}

                                // Start dragging
                                if (mCurrentSequenceIndex >= 0 && !mSequence_IsDragging && currentEvent.button == 0 && rect.Contains(currentEvent.mousePosition))
                                {
                                    mSequence_IsDragging = true;
                                    mSequences_DragOffset = mouseY - row * ElementHeight;
                                }

                                currentEvent.Use();
                            }
                            break;
                        }
                    case EventType.MouseUp:
                        {
                            if (mSequence_IsDragging && mCurrentSequenceIndex >= 0)
                            {
                                row = Mathf.Clamp(row, 0, mAnimation._Sequences.Length - 1);

                                if (row != mCurrentSequenceIndex)
                                {
                                    var temp = mAnimation._Sequences[mCurrentSequenceIndex];
                                    if (row < mCurrentSequenceIndex)
                                    {
                                        for (int i = mCurrentSequenceIndex; i > row; --i)
                                            mAnimation._Sequences[i] = mAnimation._Sequences[i - 1];
                                    }
                                    else
                                    {
                                        for (int i = mCurrentSequenceIndex; i < row; ++i)
                                            mAnimation._Sequences[i] = mAnimation._Sequences[i + 1];
                                    }
                                    mAnimation._Sequences[row] = temp;
                                    mCurrentSequenceIndex = row;
                                    GUI.changed = true;
                                }
                                Event.current.Use();
                            }
                            mSequence_IsDragging = false;
                            break;
                        }
                    case EventType.MouseDrag:
                        {
/*                            if (mCurrentSequenceIndex>=0 && !mSequence_IsDragging && currentEvent.button == 0 && rect.Contains(currentEvent.mousePosition) )
                            {
                                mSequence_IsDragging = true;
                                mSequences_DragOffset = mouseY - row * ElementHeight;
                            }*/
                            break;
                        }
                }
            }
            else
            if (mSequence_IsDragging && currentEvent.type == EventType.KeyDown && currentEvent.keyCode==KeyCode.Escape)
            {
                mSequence_IsDragging = false;
            }

            var SegmentsMinY = rect.yMin;
            rect.yMin = rect.yMax;      rect.yMax += ElementHeight-2;

            GUI.enabled = (mTextAnimation != null);
            OnGUI_AnimationControls(rect, SegmentsMinY);
            GUI.enabled = true;

            // Plus/Minus buttons
            GUILayout.BeginHorizontal(GUILayout.Height(12));
                GUILayout.FlexibleSpace();
                GUILayout.Space(60);
            
                GUILayout.BeginHorizontal(footerBackground);

                    if (GUILayout.Button(buttonPlus, preButton, GUILayout.Width(27)))
                        OnGUI_OnAddSequence();

                    if (GUILayout.Button(buttonMinus, preButton, GUILayout.Width(27)))
                        OnGUI_OnRemoveSequence();
                GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();
            GUILayout.Space(5);
        }

        void OnGUI_AnimationControls( Rect rect, float SegmentsMinY )
        {
            var changed = GUI.changed;
            float xDivider = Mathf.Lerp(rect.xMin, rect.xMax, 0.5f);
            var rectTimeline = Rect.MinMaxRect(xDivider-4, rect.yMin, rect.xMax -7-15+2, rect.yMax);

            var currentEvent = Event.current;
            if (currentEvent.type == EventType.Repaint)
            {
                var lineX = Mathf.Lerp(rectTimeline.xMin+4, rectTimeline.xMax-4, mTimeLine_Time);
                var rectLine = Rect.MinMaxRect(lineX - 2, SegmentsMinY, lineX + 2, rect.yMin+7);
                elementBackground.Draw(rectLine, false, true, true, true);
            }

            EditorGUI.BeginChangeCheck();

            xDivider -= 10;
            bool oldPlaying = mTimeLine_PlayingStartTime > 0;
            bool newPlaying = GUI.Toggle(Rect.MinMaxRect(xDivider - 21, rect.yMin - 2, xDivider - 21 * 1 + 21, rect.yMax - 2), oldPlaying, ">", EditorStyles.miniButtonRight);
            if (newPlaying != oldPlaying)
            {
                if (newPlaying)
                    AnimationControls_StartPlaying(false);
                else
                    AnimationControls_StopPlaying(false);
            }

            if (GUI.Button(Rect.MinMaxRect(xDivider - 21 * 2, rect.yMin - 2, xDivider - 21 * 2 + 21, rect.yMax - 2), "||", EditorStyles.miniButtonMid)) AnimationControls_StopPlaying(true);
            if (GUI.Button(Rect.MinMaxRect(xDivider - 21 * 3, rect.yMin - 2, xDivider - 21 * 3 + 21, rect.yMax - 2), "X", EditorStyles.miniButtonLeft))
            {
                AnimationControls_StopPlaying(true);
                mAnimation.IsPlaying = false;
            }

            bool dirtyVertices = true;
            bool dirtyMaterials = false;

            float newTime = GUI.HorizontalSlider(rectTimeline, mTimeLine_Time, 0, 1);
            if (newTime != mTimeLine_Time)
            {
                if (mTimeLine_PlayingStartTime>0) AnimationControls_StopPlaying(false);
                mTimeLine_Time = newTime;
                mAnimation.IsPlaying = true;
                mAnimation.mRealTime = mAnimation.mTime = mTimeLine_Time * mAnimation.mTotalTime;
            }

            if (EditorGUI.EndChangeCheck() && mTextAnimation != null)
            {
                mTextAnimation.MarkWidgetAsChanged(dirtyVertices, dirtyMaterials);
            }

            GUI.changed = changed; // revert this "changed" given that the sequence was not modified
        }

        public void AnimationControls_StopPlaying(bool reset)
        {
            if (reset) 
            { 
                mTimeLine_Time = mAnimation.mRealTime = 0; 
            }
            mTimeLine_PlayingStartTime = -1;
            UnRegisterUpdatePlay();
        }

        public void AnimationControls_StartPlaying( bool reset )
        {
            if (reset) 
            { 
                mTimeLine_Time = mAnimation.mRealTime = 0; 
            }
            if (mTextAnimation == null)
                return;

            if (mTimeLine_PlayingStartTime <= 0)
                RegisterUpdatePlay();
            mTimeLine_PlayingStartTime = Time.realtimeSinceStartup;
        }

        bool mRegisteredUpdatePlay = false;
        void RegisterUpdatePlay()
        {
            if (mRegisteredUpdatePlay)
                return;
            mRegisteredUpdatePlay = true;
            EditorApplication.update += AnimationControls_UpdatePlay;
        }
        void UnRegisterUpdatePlay()
        {
            mRegisteredUpdatePlay = false;
            EditorApplication.update -= AnimationControls_UpdatePlay;
        }

        void AnimationControls_UpdatePlay()
        {
            if (mTimeLine_PlayingStartTime <= 0 || mAnimation == null || mTextAnimation == null)
            {
                UnRegisterUpdatePlay();
                mTimeLine_PlayingStartTime = -1;
                return;
            }

            //mTimeLine_Time = Mathf.Repeat( Time.realtimeSinceStartup -  mTimeLine_PlayingStartTime, 1);
            //mCurrentAnim.mTime = mTimeLine_Time * mCurrentAnim.mTotalTime;

            bool dirtyVertices = true;
            bool dirtyMaterials = false;
            mAnimation.IsPlaying = true;
            mAnimation.AnimUpdate(Time.realtimeSinceStartup - mTimeLine_PlayingStartTime, mTextAnimation, ref dirtyVertices, ref dirtyMaterials);

            if (!mAnimation.IsPlaying)
            {
                //AnimationControls_StopPlaying(true);
                //mAnimation.mRealTime = mAnimation.mTime = 0;
                mAnimation.Play(mTextAnimation);
                mTimeLine_PlayingStartTime = -1;
            }
            else
                mTimeLine_PlayingStartTime = Time.realtimeSinceStartup;

            mTimeLine_Time = mAnimation.mTime / mAnimation.mTotalTime;

            if (mTextAnimation != null)
            {
                mTextAnimation.MarkWidgetAsChanged(dirtyVertices, dirtyMaterials);
            }
        }


        void OnGUI_Animation_SequenceElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var selected = index == mCurrentSequenceIndex;
            bool isPaintEvent = Event.current.type == EventType.Repaint;
            if (isPaintEvent)
                elementBackground.Draw(rect, false, selected, selected, true);

            rect.yMin += 2; rect.yMax -= 3;
            float xDivider     = Mathf.Lerp(rect.xMin, rect.xMax, 0.5f);
            var rectDragHandle = Rect.MinMaxRect(rect.xMin+5, rect.yMin+7, rect.xMin + 15, rect.yMax-7);
            var rectType       = Rect.MinMaxRect(rectDragHandle.xMax+3, rect.yMin, 100, rect.yMax);
            var rectName       = Rect.MinMaxRect(rectType.xMax, rect.yMin, xDivider, rect.yMax);
            var rectEnabled    = Rect.MinMaxRect(rect.xMax-7-15, rect.yMin, rect.xMax - 7, rect.yMax);
            var rectTimeline   = Rect.MinMaxRect(xDivider, rect.yMin, rectEnabled.xMin-2, rect.yMax);

            var seq = mAnimation._Sequences[index];
            if (seq==null)
                return;

            var origColor = GUI.color;

            if (isPaintEvent)
            {
                if (!seq._Enabled) GUI.color = new Color(origColor.r, origColor.g, origColor.b, origColor.a * 0.5f);
                draggingHandle.Draw(rectDragHandle, false, false, false, false);

                GUI.Label(rectType, seq.GetTypeName(), EditorStyles.label);
                GUI.Label(rectName, seq._Name, I2_InspectorTools.Style_LabelItalic);
            }

            var newEnabled = GUI.Toggle(rectEnabled, seq._Enabled, "");
            if (newEnabled!=seq._Enabled)
            {
                seq._Enabled = newEnabled;
                GUI.changed = true;
            }

            if (mAnimation.mTotalTime <= 0 || !isPaintEvent)
            {
                GUI.color = origColor;
                return;
            }

            float delay     = seq.mDelay / mAnimation.mTotalTime;
            float totaltime = seq.mTotalTime / mAnimation.mTotalTime;
            var rectDuration = rectTimeline;
            rectDuration.xMin = Mathf.Lerp(rectTimeline.xMin, rectTimeline.xMax, Mathf.Clamp01(delay));
            rectDuration.xMax = Mathf.Lerp(rectTimeline.xMin, rectTimeline.xMax, Mathf.Clamp01(totaltime));
            GUI.Label(rectDuration, "", "Box");

            if (totaltime <= delay)
            {
                GUI.color = origColor;
                return;
            }
            float elementTime = seq.mElementDuration / (seq.mTotalTime - seq.mDelay);
            rectDuration.xMax = Mathf.Lerp(rectDuration.xMin, rectDuration.xMax, Mathf.Clamp01(elementTime));
            GUI.color = new Color(0,GUI.color.g,0, GUI.color.a);
            GUI.Label(rectDuration, "", "Box");
            GUI.color = origColor;
        }

        void OnGUI_OnRemoveSequence()
        {
            var list = mAnimation._Sequences.ToList();
            if (mCurrentSequenceIndex < 0 || mCurrentSequenceIndex >= list.Count)
                return;
            list.RemoveAt(mCurrentSequenceIndex);
            mAnimation._Sequences = list.ToArray();
            mCurrentSequenceIndex = Mathf.Min(list.Count - 1, mCurrentSequenceIndex);
            GUI.changed = true;
            SE_AnimationPreset_Inspector.mDirty = true;
        }

        void OnGUI_OnAddSequence()
        {
            var menu = new GenericMenu();

            if (mCurrentSequenceIndex >= 0 && mAnimation._Sequences[mCurrentSequenceIndex] != null)
                menu.AddItem(new GUIContent("Duplicate:" + mAnimation._Sequences[mCurrentSequenceIndex].GetTypeName()), false, DuplicateSequence, mAnimation._Sequences[mCurrentSequenceIndex]);

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Basic/Alpha"), false, AddBasicSequence, typeof(SE_AnimSequence_Alpha));
            menu.AddItem(new GUIContent("Basic/Position"), false, AddBasicSequence, typeof(SE_AnimSequence_Position));
            menu.AddItem(new GUIContent("Basic/Color"), false, AddBasicSequence, typeof(SE_AnimSequence_Color));
            menu.AddItem(new GUIContent("Basic/Scale"), false, AddBasicSequence, typeof(SE_AnimSequence_Scale));
            menu.AddItem(new GUIContent("Basic/Rotation"), false, AddBasicSequence, typeof(SE_AnimSequence_Rotation));

            var mAnimPath = TextAnimation_Inspector.GetI2TextAnimationPath() + "/Presets/";
            var len = mAnimPath.Length;

            foreach (var fileName in System.IO.Directory.GetFiles(mAnimPath, "*.asset", System.IO.SearchOption.AllDirectories))
            {
                var option = fileName.Substring(len, fileName.Length - len - 6).Replace('\\', '/');
                menu.AddItem(new GUIContent("Presets/" + option), false, AddPresetSequence, fileName);
            }
            menu.ShowAsContext();
        }
        void DuplicateSequence(object o)
        {
            int idx = mCurrentSequenceIndex >= 0 ? mCurrentSequenceIndex : mAnimation._Sequences.Length;

            var list = mAnimation._Sequences.ToList();
            list.Insert(idx, ((SE_AnimSequence)o).Clone());           // clone the object by duplicating in the list and Save-to-Xml and then Load it back
			mCurrentSequenceIndex++;
			mAnimation._Sequences = list.ToArray();

            mAnimation.InitTimes(mTextAnimation, true);
            SE_AnimationPreset_Inspector.mDirty = true;
        }

        void AddBasicSequence(object o)
        {
            var sequenceType = (System.Type)o;
            var sequence = System.Activator.CreateInstance(sequenceType) as SE_AnimSequence;

            int idx = mCurrentSequenceIndex >= 0 ? mCurrentSequenceIndex : mAnimation._Sequences.Length;

            var list = mAnimation._Sequences.ToList();
			list.Insert(list.Count==0 ? 0 : idx+1, sequence);
			mCurrentSequenceIndex++;
			mAnimation._Sequences = list.ToArray();
            mAnimation.InitTimes(mTextAnimation, true);
            SE_AnimationPreset_Inspector.mDirty = true;
        }

        void AddPresetSequence(object o)
        {
            var fileName = (string)o;
            var sourcePreset = AssetDatabase.LoadAssetAtPath(fileName, typeof(SE_AnimationPreset)) as SE_AnimationPreset;
            if (sourcePreset == null)
            {
                EditorUtility.DisplayDialog("Unable to add Sequence", "Asset '" + fileName + "' is not a valid SE_AnimationPreset", "Cancel");
                return;
            }
            var sourceAnim = sourcePreset.CreateAnimation();
            var sequences = sourceAnim._Sequences.Where(x => x != null).Select(x=>x.Clone());

            int idx = mCurrentSequenceIndex >= 0 ? mCurrentSequenceIndex : mAnimation._Sequences.Length;

            var list = mAnimation._Sequences.ToList();
			list.InsertRange(list.Count==0 ? 0 : idx+1, sequences);
			mCurrentSequenceIndex++;
			mAnimation._Sequences = list.ToArray();

            mAnimation.InitTimes(mTextAnimation, true);
            SE_AnimationPreset_Inspector.mDirty = true;
        }
    }
}