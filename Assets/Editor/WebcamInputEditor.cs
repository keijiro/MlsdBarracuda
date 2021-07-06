using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WebcamInput))]
sealed class WebcamInputEditor : Editor
{
    static readonly GUIContent SelectLabel = new GUIContent("Select");

    SerializedProperty _deviceName;
    SerializedProperty _captureSize;
    SerializedProperty _cropSize;
    SerializedProperty _frameRate;
    SerializedProperty _dummyImage;

    void OnEnable()
    {
        _deviceName = serializedObject.FindProperty("_deviceName");
        _captureSize = serializedObject.FindProperty("_captureSize");
        _cropSize = serializedObject.FindProperty("_cropSize");
        _frameRate  = serializedObject.FindProperty("_frameRate");
        _dummyImage = serializedObject.FindProperty("_dummyImage");
    }

    void ShowDeviceSelector(Rect rect)
    {
        var menu = new GenericMenu();

        foreach (var device in WebCamTexture.devices)
            menu.AddItem(new GUIContent(device.name), false,
                         () => { serializedObject.Update();
                                 _deviceName.stringValue = device.name;
                                 serializedObject.ApplyModifiedProperties(); });

        menu.DropDown(rect);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginDisabledGroup(Application.isPlaying);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PropertyField(_deviceName);

        var rect = EditorGUILayout.GetControlRect(false, GUILayout.Width(60));
        if (EditorGUI.DropdownButton(rect, SelectLabel, FocusType.Keyboard))
            ShowDeviceSelector(rect);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(_captureSize);
        EditorGUILayout.PropertyField(_cropSize);
        EditorGUILayout.PropertyField(_frameRate);
        EditorGUILayout.PropertyField(_dummyImage);

        EditorGUI.EndDisabledGroup();

        serializedObject.ApplyModifiedProperties();
    }
}
