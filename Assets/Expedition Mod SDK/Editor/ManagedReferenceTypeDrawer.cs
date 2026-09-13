#if UNITY_EDITOR && !ODIN_INSPECTOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Expedition.ModSdk.Editor
{
    [CustomPropertyDrawer(typeof(Items.ItemFeatureConfig), true)]
    [CustomPropertyDrawer(typeof(Items.ItemComponent), true)]
    [CustomPropertyDrawer(typeof(Peleng.PelengRequirement), true)]
    [CustomPropertyDrawer(typeof(Research.AnalyzerReadings), true)]
    public sealed class ManagedReferenceTypeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            if (!property.isExpanded || property.managedReferenceValue == null) return height;
            var child = property.Copy();
            var end = child.GetEndProperty();
            if (child.NextVisible(true))
                do
                {
                    if (SerializedProperty.EqualContents(child, end)) break;
                    height += EditorGUIUtility.standardVerticalSpacing + EditorGUI.GetPropertyHeight(child, true);
                } while (child.NextVisible(false));
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var row = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            var title = new Rect(row.x, row.y, EditorGUIUtility.labelWidth, row.height);
            property.isExpanded = EditorGUI.Foldout(title, property.isExpanded, label, true);
            var selector = new Rect(title.xMax, row.y, row.width - title.width, row.height);
            string current = property.hasMultipleDifferentValues ? "Mixed" : property.managedReferenceValue?.GetType().Name ?? "None";
            if (EditorGUI.DropdownButton(selector, new GUIContent(current), FocusType.Keyboard))
            {
                var menu = new GenericMenu();
                var targets = property.serializedObject.targetObjects;
                string path = property.propertyPath;
                Type currentType = property.managedReferenceValue?.GetType();
                menu.AddItem(new GUIContent("None"), currentType == null, () => SetType(targets, path, null));
                foreach (Type type in GetTypes(property.managedReferenceFieldTypename))
                    menu.AddItem(new GUIContent(type.FullName.Replace('.', '/')), type == currentType,
                        () => SetType(targets, path, type));
                menu.DropDown(selector);
            }
            if (property.isExpanded && property.managedReferenceValue != null)
            {
                EditorGUI.indentLevel++;
                var child = property.Copy();
                var end = child.GetEndProperty();
                if (child.NextVisible(true))
                    do
                    {
                        if (SerializedProperty.EqualContents(child, end)) break;
                        row.y = row.yMax + EditorGUIUtility.standardVerticalSpacing;
                        row.height = EditorGUI.GetPropertyHeight(child, true);
                        EditorGUI.PropertyField(row, child, true);
                    } while (child.NextVisible(false));
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndProperty();
        }

        internal static Type[] GetTypes(string fieldTypeName)
        {
            int separator = fieldTypeName.IndexOf(' ');
            Type baseType = Type.GetType(fieldTypeName.Substring(separator + 1) + ", " + fieldTypeName.Substring(0, separator), true);
            return TypeCache.GetTypesDerivedFrom(baseType)
                .Where(type => type.IsClass && !type.IsAbstract && !type.ContainsGenericParameters &&
                    type.IsDefined(typeof(SerializableAttribute), false) &&
                    !typeof(UnityEngine.Object).IsAssignableFrom(type) && type.GetConstructor(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.NonPublic, null, Type.EmptyTypes, null) != null)
                .OrderBy(type => type.FullName, StringComparer.Ordinal).ToArray();
        }

        internal static void SetType(UnityEngine.Object[] targets, string path, Type type)
        {
            Undo.SetCurrentGroupName("Change component type");
            foreach (UnityEngine.Object target in targets)
            {
                using var serialized = new SerializedObject(target);
                var property = serialized.FindProperty(path);
                if (property == null || property.propertyType != SerializedPropertyType.ManagedReference) continue;
                if (property.managedReferenceValue?.GetType() == type) continue;
                property.managedReferenceValue = type == null ? null : Activator.CreateInstance(type, true);
                property.isExpanded = type != null;
                serialized.ApplyModifiedProperties();
            }
        }
    }
}
#endif
