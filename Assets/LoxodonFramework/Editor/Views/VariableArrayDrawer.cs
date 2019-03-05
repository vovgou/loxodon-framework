using Loxodon.Framework.Views.Variables;
using UnityEditor;
using UnityEngine;

namespace Loxodon.Framework.Editors
{
    [CustomPropertyDrawer(typeof(VariableArray))]
    public class VariableArrayDrawer : PropertyDrawer
    {
        private const float HORIZONTAL_GAP = 5;
        private const float VERTICAL_GAP = 5;
        private const float INDENTATION = 10;

        private bool foldOut = true;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = base.GetPropertyHeight(property, label) + 60;
            var variables = property.FindPropertyRelative("variables");
            for (int i = 0; i < variables.arraySize; i++)
            {
                height += EditorGUI.GetPropertyHeight(variables.GetArrayElementAtIndex(i)) + VERTICAL_GAP;
            }
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var variables = property.FindPropertyRelative("variables");
            float height = base.GetPropertyHeight(property, label);

            float y = position.y;
            float x = position.x;

            Rect foldOutRect = new Rect(x, y, position.width, height);           
            foldOut = EditorGUI.Foldout(foldOutRect, foldOut, new GUIContent(property.displayName, ""));
            if (foldOut)
            {

                y += height + VERTICAL_GAP;
                Rect buttonLeftRect = new Rect(x, y, 50, height);
                Rect buttonRightRect = new Rect(buttonLeftRect.xMax, y, 50, height);

                if (GUI.Button(buttonLeftRect, new GUIContent("+"), EditorStyles.miniButtonLeft))
                {
                    int index = variables.arraySize > 0 ? variables.arraySize : 0;
                    this.DrawContextMenu(variables, index);
                }
                if (GUI.Button(buttonRightRect, new GUIContent("-"), EditorStyles.miniButtonRight))
                {
                    int index = variables.arraySize > 0 ? variables.arraySize - 1 : -1;
                    RemoveVariable(variables, index);
                }

                x += INDENTATION;

                float width = Mathf.Max(position.xMax - x - 50, 320);
                if (variables.arraySize > 0)
                {
                    y += height + VERTICAL_GAP;

                    Rect nameRect = new Rect(x, y, Mathf.Min(200, width * 0.4f), height);
                    Rect typeRect = new Rect(nameRect.xMax + HORIZONTAL_GAP, y, Mathf.Min(120, width * 0.2f), height);
                    Rect valueRect = new Rect(typeRect.xMax + HORIZONTAL_GAP, y, position.xMax - typeRect.xMax - HORIZONTAL_GAP, height);

                    EditorGUI.LabelField(nameRect, new GUIContent("Name", ""));
                    EditorGUI.LabelField(typeRect, new GUIContent("Type", ""));
                    EditorGUI.LabelField(valueRect, new GUIContent("Value", ""));

                    y += height + VERTICAL_GAP;

                    Handles.BeginGUI();
                    Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                    Handles.DrawLine(new Vector3(x, y), new Vector3(position.xMax, y));
                    Handles.EndGUI();
                }

                y += VERTICAL_GAP;
                for (int i = 0; i < variables.arraySize; i++)
                {
                    var variable = variables.GetArrayElementAtIndex(i);
                    Rect variableRect = new Rect(x, y, width, EditorGUI.GetPropertyHeight(variable));
                    EditorGUI.PropertyField(variableRect, variable, GUIContent.none);

                    buttonLeftRect = new Rect(variableRect.xMax + HORIZONTAL_GAP, y, 20, variableRect.height);
                    buttonRightRect = new Rect(buttonLeftRect.xMax, y, 20, variableRect.height);

                    if (GUI.Button(buttonLeftRect, new GUIContent("+"), EditorStyles.miniButtonLeft))
                    {
                        DuplicateVariable(variables, i);
                    }
                    if (GUI.Button(buttonRightRect, new GUIContent("-"), EditorStyles.miniButtonRight))
                    {
                        RemoveVariable(variables, i);
                    }

                    y += variableRect.height + VERTICAL_GAP;
                }
            }

            EditorGUI.EndProperty();
        }

        protected virtual void DrawContextMenu(SerializedProperty variables, int index)
        {
            GenericMenu menu = new GenericMenu();
            foreach (VariableType variableType in System.Enum.GetValues(typeof(VariableType)))
            {
                var type = variableType;
                menu.AddItem(new GUIContent(variableType.ToString()), false, context =>
                {
                    AddVariable(variables, index, type);
                }, null);
            }
            menu.ShowAsContext();
        }

        protected virtual void AddVariable(SerializedProperty variables, int index, VariableType type)
        {
            if (index < 0 || index > variables.arraySize)
                return;
            variables.serializedObject.Update();
            variables.InsertArrayElementAtIndex(index);
            SerializedProperty variableProperty = variables.GetArrayElementAtIndex(index);
            variableProperty.FindPropertyRelative("variableType").enumValueIndex = (int)type;

            variableProperty.FindPropertyRelative("name").stringValue = "";
            variableProperty.FindPropertyRelative("objectValue").objectReferenceValue = null;
            variableProperty.FindPropertyRelative("dataValue").stringValue = "";

            variables.serializedObject.ApplyModifiedProperties();
            GUI.FocusControl(null);
        }

        protected virtual void DuplicateVariable(SerializedProperty variables, int index)
        {
            if (index < 0 || index > variables.arraySize - 1)
                return;
            variables.serializedObject.Update();
            variables.InsertArrayElementAtIndex(index);
            SerializedProperty variableProperty = variables.GetArrayElementAtIndex(index+1);

            variableProperty.FindPropertyRelative("name").stringValue = "";
            variableProperty.FindPropertyRelative("objectValue").objectReferenceValue = null;
            variableProperty.FindPropertyRelative("dataValue").stringValue = "";

            variables.serializedObject.ApplyModifiedProperties();
            GUI.FocusControl(null);
        }

        protected virtual void RemoveVariable(SerializedProperty variables, int index)
        {
            if (index < 0)
                return;

            variables.serializedObject.Update();
            variables.DeleteArrayElementAtIndex(index);
            variables.serializedObject.ApplyModifiedProperties();
            GUI.FocusControl(null);
        }
    }
}