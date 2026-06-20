using System.Reflection;
using UnityEditor;
using UnityEngine;


public static class InspectorButtonDrawer
{
    public static void Draw(object obj)
    {
        MethodInfo[] methods = obj.GetType().GetMethods(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (MethodInfo method in methods)
        {
            InspectorButtonAttribute button =
                method.GetCustomAttribute<InspectorButtonAttribute>();

            if (button == null) continue;
            if (method.GetParameters().Length != 0) continue;

            string label = string.IsNullOrEmpty(button.Label)
                ? ObjectNames.NicifyVariableName(method.Name)
                : button.Label;

            if (GUILayout.Button(label))
                method.Invoke(obj, null);
        }
    }
}