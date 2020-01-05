using System;
using UnityEngine;
//using UniRx;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BaseSystem
{
    /// インスペクタ上での編集を無効にする属性
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }

    //[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false))]
    //public class ReadOnlyReactivePropertyAttribute : PropertyAttribute
    //{
    //}


#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public sealed class ReadOnlyAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup (true);
            EditorGUI.PropertyField (position, property, label);
            EditorGUI.EndDisabledGroup ();
        }
    }

	//[CustomPropertyDrawer (typeof (ReadOnlyReactivePropertyAttribute))]
	//public sealed class ReadOnlyReactivePropertyAttributeDrawer : UniRx.InspectorDisplayDrawer
	//{
	//    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	//    {
	//        EditorGUI.BeginDisabledGroup (true);
	//        base.OnGUI (position, property, label);
	//        EditorGUI.EndDisabledGroup ();
	//    }
	//}
#endif
}

/// @file
/// インスペクタ上での編集を無効にする属性
