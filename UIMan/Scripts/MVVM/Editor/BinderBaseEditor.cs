﻿using System.Reflection;
using UnityEditor;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
#endif

namespace UnuGames.MVVM
{
    [CustomEditor(typeof(BinderBase), true)]
    public class BinderBaseEditor :
#if ODIN_INSPECTOR
        OdinEditor
#else
        Editor
#endif
    {
        public BinderBase binder;

        public override void OnInspectorGUI()
        {
#if ODIN_INSPECTOR
            base.OnInspectorGUI();
#else
            DrawDefaultInspector();
#endif

            this.binder = this.target as BinderBase;

            GUILayout.Space(4);
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Space(4);

            var context = EditorGUILayout.ObjectField(new GUIContent("Data Context"), this.binder.dataContext,
                                                      typeof(DataContext), true) as DataContext;

            if (context == null)
            {
                GUILayout.Space(4);

                if (GUILayout.Button(BindingDefine.FIND_CONTEXT))
                {
                    context = this.binder.FindDataContext();
                }
            }

            if (this.binder.dataContext != context)
            {
                Undo.RecordObject(this.target, "Select Data Context");
                this.binder.dataContext = context;
            }

            if (this.binder.dataContext == null)
            {
                GUILayout.Space(4);
                GUILayout.EndVertical();
                return;
            }

            GUILayout.Space(4);

            BindingField[] arrBindingFields = this.binder.GetBindingFields();
            Converter[] arrConverters = this.binder.GetConverters();
            TwoWayBinding[] arrTwoWayBindings = this.binder.GetTwoWayBindings();

            if (arrBindingFields.Length > 0)
            {
                EditorGUILayout.LabelField("Binders", EditorStyles.boldLabel);
                GUILayout.BeginVertical();

                for (var i = 0; i < arrBindingFields.Length; i++)
                {
                    DrawBindingField(arrBindingFields[i]);
                }

                GUILayout.EndVertical();
                GUILayout.Space(4);
            }

            if (arrConverters.Length > 0)
            {
                EditorGUILayout.LabelField("Converters", EditorStyles.boldLabel);
                GUILayout.BeginVertical();

                for (var i = 0; i < arrConverters.Length; i++)
                {
                    DrawConverter(arrConverters[i]);
                }

                GUILayout.EndVertical();
                GUILayout.Space(4);
            }

            if (arrTwoWayBindings.Length > 0)
            {
                EditorGUILayout.LabelField("Two-way Bindings", EditorStyles.boldLabel);
                GUILayout.BeginVertical();

                for (var i = 0; i < arrTwoWayBindings.Length; i++)
                {
                    DrawTwoWayBinding(arrTwoWayBindings[i]);
                }

                GUILayout.EndVertical();
                GUILayout.Space(4);
            }

            GUILayout.EndVertical();

            if (Event.current.type == EventType.Repaint)
            {
                FilterPopup.SetPopupRect(GUILayoutUtility.GetLastRect());
            }
        }

        public void DrawConverter(Converter converter, string prefix = "", string suffix = "")
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel(new GUIContent($"{prefix}{converter.label}{suffix}"));

            var adapter = converter.GetAdapter();
            var adapterType = converter.GetAdapterType();

            EditorGUI.BeginChangeCheck();
            adapter = EditorGUILayout.ObjectField(adapter, adapterType, false) as Adapter;

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(this.target, "Set Adapter");
                converter.SetAdapter(adapter);
                Apply();
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        public void DrawTwoWayBinding(TwoWayBinding twoWayBinding)
        {
            if (twoWayBinding.value)
                GUILayout.BeginVertical("Box");
            else
                GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel(new GUIContent(twoWayBinding.label));

            EditorGUI.BeginChangeCheck();
            var value = EditorGUILayout.Toggle(twoWayBinding.value);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(this.target, "Set Two-way Binding");
                twoWayBinding.value = value;

                Apply();
            }

            GUILayout.EndHorizontal();

            if (twoWayBinding.value)
            {
                Converter[] arrConverters = twoWayBinding.GetConverters();

                if (arrConverters.Length > 0)
                {
                    GUILayout.BeginVertical();

                    if (arrConverters.Length < 2)
                    {
                        DrawConverter(arrConverters[0], suffix: " Converter");
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Converters", EditorStyles.boldLabel);

                        for (var i = 0; i < arrConverters.Length; i++)
                        {
                            DrawConverter(arrConverters[i]);
                        }
                    }

                    GUILayout.EndVertical();
                    GUILayout.Space(4);
                }
            }

            GUILayout.EndVertical();
        }

        public void DrawBindingField(BindingField field)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel(new GUIContent(field.label));
            var curMemberName = field.member;
            if (string.IsNullOrEmpty(curMemberName))
            {
                curMemberName = BindingDefine.SELECT_MEMBER;
            }

            var viewMembers = this.binder.GetMembers(false, true, false, false, MemberTypes.Field, MemberTypes.Property);
            var dataMembers = this.binder.GetMembers(false, false, false, false, MemberTypes.Field, MemberTypes.Property);

            if (dataMembers == null)
            {
                EditorGUILayout.LabelField("No target context found!", EditorGUIHelper.RichText(color: CommonColor.GetRed()));
                GUILayout.EndHorizontal();
            }
            else
            {
                ArrayUtility.Insert(ref dataMembers, 0, "<None>");
                ArrayUtility.Insert(ref viewMembers, 0, "<None>");

                var selectedIndex = 0;

                for (var i = 0; i < dataMembers.Length; i++)
                {
                    if (curMemberName == dataMembers[i])
                    {
                        selectedIndex = i;
                        break;
                    }
                }

                GUILayout.Space(-7);
                EditorGUILayout.BeginVertical();
                GUILayout.Space(5);
                var newSelectedIndex = EditorGUILayout.Popup(selectedIndex, viewMembers);
                if (newSelectedIndex != selectedIndex)
                {
                    Undo.RecordObject(this.target, "Select Binder Member");
                    selectedIndex = newSelectedIndex;
                    field.member = dataMembers[selectedIndex];
                    Apply();
                }

                EditorGUILayout.EndVertical();

                if (EditorGUIHelper.QuickPickerButton())
                {
                    ContextBrowser.Browse(this, field, selectedIndex, true, true, false);
                }

                GUILayout.EndHorizontal();

                var curMember = this.binder.GetMemberInfo(dataMembers[selectedIndex], MemberTypes.Property, MemberTypes.Field);

                if (curMember != null)
                {
                    var attributes = curMember.GetCustomAttributes(typeof(UIManPropertyAttribute), false);

                    if (attributes == null || attributes.Length == 0)
                    {
                        GUILayout.BeginHorizontal("Box");
                        EditorGUILayout.PrefixLabel(" ");
                        GUILayout.Label("Binding field must be decorated with either [UIManProperty] or [UIManAutoProperty] attribute!", EditorGUIHelper.RichText(true, CommonColor.GetRed()));
                        GUILayout.EndHorizontal();
                    }
                }
            }

            GUILayout.EndVertical();
        }

        public void Apply()
        {
            EditorUtility.SetDirty(this.target);
        }
    }
}