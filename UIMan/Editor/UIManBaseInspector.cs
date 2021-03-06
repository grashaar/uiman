﻿using UnityEditor;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
#endif

namespace UnuGames
{
    [CustomEditor(typeof(UIManBase), true)]
    public class UIManBaseInspector :
#if ODIN_INSPECTOR
        OdinEditor
#else
        Editor
#endif
    {
        private readonly GUIContent animator = new GUIContent("Animator", "Animator component to do custom animation");
        private readonly GUIContent show = new GUIContent("Show", "Animation to do when UI is show");
        private readonly GUIContent showTime = new GUIContent("Time", "Total time to do built-in or custom script animation");
        private readonly GUIContent hide = new GUIContent("Hide", "Animation to do when UI is hide");
        private readonly GUIContent hideTime = new GUIContent("Time", "Total time to do built-in or custom script animation");
        private readonly GUIContent idle = new GUIContent("Idle", "Animation to do when UI is idle");
        private readonly GUIContent position = new GUIContent("Show Position", "Target position to show UI");
        private readonly GUIContent cover = new GUIContent("Use Cover", "Show gray cover after dialog to prevent click behind elements");
        private readonly GUIContent background = new GUIContent("Use Background", "Setting background image behine your screen elements");

        private Color orgBgColor;

        public override void OnInspectorGUI()
        {
            var uiManBase = (UIManBase)this.target;
            this.orgBgColor = GUI.backgroundColor;

            GUI.backgroundColor = CommonColor.GetOrange();
            GUILayout.BeginHorizontal("Box");
            LabelHelper.HeaderLabel(string.Format("UIMan View Model ({0})", uiManBase.GetUIBaseType()));
            GUILayout.EndHorizontal();

            GUI.backgroundColor = this.orgBgColor;

            LineHelper.Draw(CommonColor.GetGray());

            EditorGUILayout.Space();
            LabelHelper.HeaderLabel("General");
            GUILayout.BeginVertical("Box");

            if (uiManBase is UIManDialog dialog)
            {
                EditorGUI.BeginChangeCheck();

                var useCover = EditorGUILayout.Toggle(this.cover, dialog.useCover);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(dialog, nameof(useCover));
                    dialog.useCover = useCover;
                }
            }
            else if (uiManBase is UIManScreen screen)
            {
                EditorGUI.BeginChangeCheck();

                var useBackground = EditorGUILayout.Toggle(this.background, screen.useBackground);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(screen, nameof(useBackground));
                    screen.useBackground = useBackground;
                }

                if (screen.useBackground)
                {
                    EditorGUI.BeginChangeCheck();

                    var background = EditorGUILayout.TextField(screen.background);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(screen, nameof(background));
                        screen.background = background;
                    }
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(this.show, GUILayout.MaxWidth(50f));

            EditorGUI.BeginChangeCheck();

            var motionShow = (UIMotion)EditorGUILayout.EnumPopup(uiManBase.motionShow);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(uiManBase,nameof(motionShow));
                uiManBase.motionShow = motionShow;
            }

            GUILayout.Space(25f);
            EditorGUILayout.LabelField(this.showTime, GUILayout.MaxWidth(50f));

            EditorGUI.BeginChangeCheck();

            var animShowTime = EditorGUILayout.FloatField(uiManBase.animShowTime);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(uiManBase,nameof(animShowTime));
                uiManBase.animShowTime = animShowTime;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(this.hide, GUILayout.MaxWidth(50f));
            EditorGUI.BeginChangeCheck();

            var motionHide = (UIMotion)EditorGUILayout.EnumPopup(uiManBase.motionHide);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(uiManBase,nameof(motionHide));
                uiManBase.motionHide = motionHide;
            }

            GUILayout.Space(25f);
            EditorGUILayout.LabelField(this.hideTime, GUILayout.MaxWidth(50f));
            EditorGUI.BeginChangeCheck();

            var animHideTime = EditorGUILayout.FloatField(uiManBase.animHideTime);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(uiManBase,nameof(animHideTime));
                uiManBase.animHideTime = animHideTime;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(this.idle, GUILayout.MaxWidth(50f));
            EditorGUI.BeginChangeCheck();

            var motionIdle = (UIMotion)EditorGUILayout.EnumPopup(uiManBase.motionIdle);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(uiManBase,nameof(motionIdle));
                uiManBase.motionIdle = motionIdle;
            }

            EditorGUILayout.EndHorizontal();

            var motions = new UIMotion[3] { uiManBase.motionShow, uiManBase.motionHide, uiManBase.motionIdle };
            var haveMecanimAnim = false;
            var haveTweenAnim = false;

            foreach (UIMotion m in motions)
            {
                if (m == UIMotion.CustomMecanimAnimation)
                    haveMecanimAnim = true;
                else
                    haveTweenAnim = true;
            }

            if (uiManBase.motionShow != UIMotion.None &&
                uiManBase.motionShow != UIMotion.CustomScriptAnimation &&
                uiManBase.motionShow != UIMotion.CustomMecanimAnimation)
            {
                EditorGUILayout.Space();
                EditorGUI.BeginChangeCheck();

                var showPosition = EditorGUILayout.Vector3Field(this.position, uiManBase.showPosition);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(uiManBase, nameof(showPosition));
                    uiManBase.showPosition = showPosition;
                }
            }

            if (haveTweenAnim && haveMecanimAnim)
            {
                GUILayout.BeginHorizontal("Box");
                EditorGUILayout.HelpBox("Motion types are not matched with each other and may cause unexpected error.\nPlease choose Mecanim if you want to animate manually using Unity animator.", MessageType.Error);
                GUILayout.EndHorizontal();
            }

            if (uiManBase.motionIdle != UIMotion.CustomMecanimAnimation && uiManBase.motionIdle != UIMotion.None)
            {
                GUILayout.BeginHorizontal("Box");
                EditorGUILayout.HelpBox("Idle motion can only support Mecanim animation.", MessageType.Error);
                GUILayout.EndHorizontal();
            }

            if (uiManBase.motionShow == UIMotion.CustomMecanimAnimation ||
                uiManBase.motionHide == UIMotion.CustomMecanimAnimation)
            {

                EditorGUILayout.Space();

                if (uiManBase.gameObject != null)
                {
                    uiManBase.animRoot = uiManBase.gameObject.GetComponent<Animator>();
                }

                EditorGUI.BeginChangeCheck();

                var animRoot = EditorGUILayout.ObjectField(this.animator, uiManBase.animRoot, typeof(Animator), true) as Animator;

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(uiManBase, nameof(animRoot));
                    uiManBase.animRoot = animRoot;
                }

                if (uiManBase.animRoot == null || uiManBase.animRoot.runtimeAnimatorController == null)
                {
                    if (GUILayout.Button("Generate Animator"))
                    {
                        AnimationEditorUtils.GenerateAnimator(uiManBase.gameObject, UIManDefine.ANIM_SHOW, UIManDefine.ANIM_HIDE, UIManDefine.ANIM_IDLE);
                    }
                }
            }

            GUILayout.Space(2f);
            GUILayout.EndVertical();
            LineHelper.Draw(CommonColor.GetGray());

            EditorGUILayout.Space();
            LabelHelper.HeaderLabel("Custom fields");

#if ODIN_INSPECTOR
            EditorGUILayout.Space();
            base.OnInspectorGUI();
#else
            GUILayout.BeginVertical("Box");
            DrawDefaultInspector();
            GUILayout.EndVertical();
#endif

            EditorGUILayout.Space();

            if (ColorButton.Draw("Edit Handler (View Logic)", CommonColor.LightGreen, GUILayout.Height(25)))
            {
                var handler = UIManCodeGenerator.GetScriptPathByType(this.target.GetType());
                handler = handler.Replace(".cs", ".Handler.cs");

                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(handler, 1, 1);
            }

            if (ColorButton.Draw("Edit View", CommonColor.LightGreen, GUILayout.Height(25)))
            {
                AssetDatabase.OpenAsset(this.target);
            }

            if (ColorButton.Draw("Edit View (Within Context)", CommonColor.LightGreen, GUILayout.Height(25)))
            {
                GameObject prefabInstance;
                Object obj = FindObjectOfType(uiManBase.Type);

                if (obj != null)
                {
                    prefabInstance = ((MonoBehaviour)obj).gameObject;
                }
                else
                {
                    var isDialog = uiManBase.GetUIBaseType() == UIBaseType.Dialog;
                    prefabInstance = PrefabUtility.InstantiatePrefab(uiManBase.gameObject) as GameObject;

                    if (isDialog)
                        prefabInstance.transform.SetParent(UIMan.Instance.dialogRoot, false);
                    else
                        prefabInstance.transform.SetParent(UIMan.Instance.screenRoot, false);
                }
                Selection.activeGameObject = prefabInstance;
            }
        }
    }
}