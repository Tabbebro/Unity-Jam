using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

[CustomEditor(typeof(Upgrade))]
public class VariableMonitorEditor : Editor
{
    SerializedProperty _variablesToMonitor;
    MonoBehaviour _parentScript;
    void OnEnable()
    {
        _variablesToMonitor = serializedObject.FindProperty("_variablesToUpdate");
        _parentScript = (MonoBehaviour)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Variables to Update", EditorStyles.boldLabel);

        for (int i = 0; i < _variablesToMonitor.arraySize; i++)
        {
            var element = _variablesToMonitor.GetArrayElementAtIndex(i);
            var scriptProp = element.FindPropertyRelative("Script");
            var variableNameProp = element.FindPropertyRelative("VariableName");
            var variableUpgradeAmount = element.FindPropertyRelative("UpgradeAmount");
            var variableMultiplier = element.FindPropertyRelative("UpgradeMultiplier");
            var variableBoolean = element.FindPropertyRelative("UpgradeBool");

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.PropertyField(scriptProp);
            //EditorGUILayout.PropertyField(variableUpgradeAmount);
            MonoBehaviour script = (MonoBehaviour)scriptProp.objectReferenceValue;
            if (script != null)
            {
                // Collect all field and property names
                var members = new List<string>();
                var type = script.GetType();
                foreach (var f in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    members.Add(f.Name);
                    //members.Add($"{f.Name}: {f.FieldType.Name}");
                /* foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    members.Add(p.Name); */

                int currentIndex = Mathf.Max(0, members.IndexOf(variableNameProp.stringValue));
                int newIndex = EditorGUILayout.Popup("Variable", currentIndex, members.ToArray());

                if (newIndex >= 0 && newIndex < members.Count)
                {
                    variableNameProp.stringValue = members[newIndex];
                    var field = type.GetField(variableNameProp.stringValue, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    //PropertyInfo prop = type.GetProperty(variableNameProp.stringValue, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (field != null)
                    {
                        object currentValue = field.GetValue(script);
                        if (currentValue is float f)
                        {
                            EditorGUILayout.PropertyField(variableMultiplier);
                        }
                        else if (currentValue is int intt)
                        {
                            EditorGUILayout.PropertyField(variableUpgradeAmount);

                        }
                        else if (currentValue is bool b)
                        {
                            EditorGUILayout.PropertyField(variableBoolean);
                        }
                        else
                        {

                            //Debug.LogError("Was somethg else");
                        }
                        //continue;
                    }
                    /* else if(prop != null)
                    {
                        object currentValue = prop.GetValue(script);
                        if (currentValue is float f)
                        {
                            EditorGUILayout.PropertyField(variableMultiplier);
                        }
                        else if (currentValue is int intt)
                        {
                            EditorGUILayout.PropertyField(variableUpgradeAmount);

                        }
                        else if (currentValue is bool b)
                        {
                            EditorGUILayout.PropertyField(variableBoolean);
                        }
                        else
                        {

                            Debug.LogError("Was somethg else");
                        }
                    }
                    else
                    {
                        continue;
                    } */
                    
                }

            }
            else
            {
                EditorGUILayout.HelpBox("Assign a script first.", MessageType.Info);
            }

            if (GUILayout.Button("Remove"))
            {
                _variablesToMonitor.DeleteArrayElementAtIndex(i);
            }

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Add Variable"))
        {
            int newIndex = _variablesToMonitor.arraySize;
            _variablesToMonitor.InsertArrayElementAtIndex(newIndex);

            // Get the newly added element
            var element = _variablesToMonitor.GetArrayElementAtIndex(newIndex);
        
            // Assign default values manually
            var scriptProp = element.FindPropertyRelative("Script");
            var nameProp = element.FindPropertyRelative("VariableName");
            var upgradeVariable = element.FindPropertyRelative("UpgradeAmount");
        
            scriptProp.objectReferenceValue = null;
            nameProp.stringValue = "";
            upgradeVariable.floatValue = 0;

            serializedObject.ApplyModifiedProperties();
        }
        
        serializedObject.ApplyModifiedProperties();
        }
    }