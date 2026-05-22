using UnityEditor;
using UnityEngine;

public class PrecisionSceneTransformTool : EditorWindow
{
    enum TransformMode { Move, ResizeScale, ResizeBounds }
    enum Axis { X, Y, Z }

    const string StepKey = "AOI.PrecisionSceneTransform.Step";
    const string ShortcutsKey = "AOI.PrecisionSceneTransform.Shortcuts";
    const string ModeKey = "AOI.PrecisionSceneTransform.Mode";

    float step = 0.1f;
    bool shortcutsEnabled = true;
    TransformMode mode = TransformMode.Move;

    [MenuItem("Tools/Precision Scene Transform")]
    static void Open() => GetWindow<PrecisionSceneTransformTool>("Precision Transform");

    void OnEnable()
    {
        step = EditorPrefs.GetFloat(StepKey, 0.1f);
        shortcutsEnabled = EditorPrefs.GetBool(ShortcutsKey, true);
        mode = (TransformMode)EditorPrefs.GetInt(ModeKey, (int)TransformMode.Move);
        SceneView.duringSceneGui += DuringSceneGui;
    }

    void OnDisable()
    {
        EditorPrefs.SetFloat(StepKey, step);
        EditorPrefs.SetBool(ShortcutsKey, shortcutsEnabled);
        EditorPrefs.SetInt(ModeKey, (int)mode);
        SceneView.duringSceneGui -= DuringSceneGui;
    }

    void OnGUI()
    {
        GUILayout.Label("Ajustements precis dans la Scene View", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        step = Mathf.Max(0.001f, EditorGUILayout.FloatField("Pas", step));
        shortcutsEnabled = EditorGUILayout.Toggle("Raccourcis Scene View", shortcutsEnabled);
        mode = (TransformMode)EditorGUILayout.EnumPopup("Mode raccourcis", mode);
        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetFloat(StepKey, step);
            EditorPrefs.SetBool(ShortcutsKey, shortcutsEnabled);
            EditorPrefs.SetInt(ModeKey, (int)mode);
            SceneView.RepaintAll();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Appliquer ce pas au snap Unity"))
            ApplyUnityMoveSnap();

        EditorGUILayout.HelpBox(
            "Dans la Scene View: Alt + fleches = X/Z, Alt + PageUp/PageDown = Y. " +
            "Shift = pas fin, Ctrl = pas large. Le mode choisi controle si le raccourci deplace, scale ou redimensionne par bounds.",
            MessageType.Info);

        EditorGUILayout.Space();
        GUILayout.Label("Nudge position", EditorStyles.boldLabel);
        DrawAxisButtons("Deplacer", ApplyMove);

        EditorGUILayout.Space();
        GUILayout.Label("Scale local", EditorStyles.boldLabel);
        DrawAxisButtons("Scale", ApplyLocalScale);

        EditorGUILayout.Space();
        GUILayout.Label("Redimensionner par bounds", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Utile pour un mur: agrandit ou reduit la dimension visible tout en gardant le bord oppose en place.",
            MessageType.None);
        DrawBoundsResizeButtons();
    }

    void DrawAxisButtons(string label, System.Action<Axis, float> action)
    {
        EditorGUILayout.BeginHorizontal();
        DrawAxisPair(label, Axis.X, action);
        DrawAxisPair(label, Axis.Y, action);
        DrawAxisPair(label, Axis.Z, action);
        EditorGUILayout.EndHorizontal();
    }

    void DrawAxisPair(string label, Axis axis, System.Action<Axis, float> action)
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(58));
        GUILayout.Label(axis.ToString(), EditorStyles.miniBoldLabel);

        if (GUILayout.Button("-")) action(axis, -step);
        if (GUILayout.Button("+")) action(axis, step);

        EditorGUILayout.EndVertical();
    }

    void DrawBoundsResizeButtons()
    {
        DrawBoundsResizeRow(Axis.X);
        DrawBoundsResizeRow(Axis.Y);
        DrawBoundsResizeRow(Axis.Z);
    }

    void DrawBoundsResizeRow(Axis axis)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(axis.ToString(), GUILayout.Width(20));
        if (GUILayout.Button("- depuis MIN")) ApplyBoundsResize(axis, -step, true);
        if (GUILayout.Button("+ depuis MIN")) ApplyBoundsResize(axis, step, true);
        if (GUILayout.Button("- depuis MAX")) ApplyBoundsResize(axis, -step, false);
        if (GUILayout.Button("+ depuis MAX")) ApplyBoundsResize(axis, step, false);
        EditorGUILayout.EndHorizontal();
    }

    void DuringSceneGui(SceneView sceneView)
    {
        DrawSceneOverlay();

        if (!shortcutsEnabled || Selection.transforms.Length == 0)
            return;

        Event current = Event.current;
        if (current.type != EventType.KeyDown || !current.alt)
            return;

        if (!TryGetShortcutAxis(current.keyCode, out Axis axis, out float direction))
            return;

        float activeStep = GetActiveStep(current);
        float amount = direction * activeStep;

        switch (mode)
        {
            case TransformMode.Move:
                ApplyMove(axis, amount);
                break;
            case TransformMode.ResizeScale:
                ApplyLocalScale(axis, amount);
                break;
            case TransformMode.ResizeBounds:
                ApplyBoundsResize(axis, amount, amount >= 0f);
                break;
        }

        current.Use();
    }

    void DrawSceneOverlay()
    {
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(12f, 12f, 260f, 92f), "Precision Transform", EditorStyles.helpBox);
        GUILayout.Label($"Pas: {step:0.###} | Mode: {mode}", EditorStyles.miniBoldLabel);
        GUILayout.Label("Alt+fleches: X/Z | Alt+PgUp/PgDn: Y", EditorStyles.miniLabel);
        GUILayout.Label("Shift: fin | Ctrl: large", EditorStyles.miniLabel);
        GUILayout.EndArea();
        Handles.EndGUI();
    }

    bool TryGetShortcutAxis(KeyCode keyCode, out Axis axis, out float direction)
    {
        axis = Axis.X;
        direction = 0f;

        switch (keyCode)
        {
            case KeyCode.LeftArrow:
                axis = Axis.X;
                direction = -1f;
                return true;
            case KeyCode.RightArrow:
                axis = Axis.X;
                direction = 1f;
                return true;
            case KeyCode.DownArrow:
                axis = Axis.Z;
                direction = -1f;
                return true;
            case KeyCode.UpArrow:
                axis = Axis.Z;
                direction = 1f;
                return true;
            case KeyCode.PageDown:
                axis = Axis.Y;
                direction = -1f;
                return true;
            case KeyCode.PageUp:
                axis = Axis.Y;
                direction = 1f;
                return true;
        }

        return false;
    }

    float GetActiveStep(Event current)
    {
        if (current.shift)
            return step * 0.1f;

        if (current.control || current.command)
            return step * 10f;

        return step;
    }

    void ApplyUnityMoveSnap()
    {
        EditorSnapSettings.move = new Vector3(step, step, step);
        Debug.Log($"Snap Unity de deplacement regle a {step:0.###} sur X/Y/Z.");
    }

    void ApplyMove(Axis axis, float amount)
    {
        Transform[] transforms = Selection.transforms;
        if (transforms.Length == 0)
            return;

        Undo.RecordObjects(transforms, "Precision Move");
        foreach (Transform selected in transforms)
        {
            Vector3 position = selected.position;
            SetAxis(ref position, axis, GetAxis(position, axis) + amount);
            selected.position = position;
            EditorUtility.SetDirty(selected);
        }
    }

    void ApplyLocalScale(Axis axis, float amount)
    {
        Transform[] transforms = Selection.transforms;
        if (transforms.Length == 0)
            return;

        Undo.RecordObjects(transforms, "Precision Scale");
        foreach (Transform selected in transforms)
        {
            Vector3 scale = selected.localScale;
            SetAxis(ref scale, axis, Mathf.Max(0.001f, GetAxis(scale, axis) + amount));
            selected.localScale = scale;
            EditorUtility.SetDirty(selected);
        }
    }

    void ApplyBoundsResize(Axis axis, float amount, bool keepMinSide)
    {
        Transform[] transforms = Selection.transforms;
        if (transforms.Length == 0)
            return;

        Undo.RecordObjects(transforms, "Precision Bounds Resize");
        foreach (Transform selected in transforms)
            ResizeBounds(selected, axis, amount, keepMinSide);
    }

    void ResizeBounds(Transform selected, Axis axis, float amount, bool keepMinSide)
    {
        Bounds before = GetCombinedBounds(selected.gameObject);
        float currentSize = Mathf.Max(0.001f, GetAxis(before.size, axis));
        float targetSize = Mathf.Max(0.001f, currentSize + amount);
        float factor = targetSize / currentSize;
        float anchor = keepMinSide ? GetAxis(before.min, axis) : GetAxis(before.max, axis);

        Vector3 scale = selected.localScale;
        SetAxis(ref scale, axis, Mathf.Max(0.001f, GetAxis(scale, axis) * factor));
        selected.localScale = scale;

        Bounds after = GetCombinedBounds(selected.gameObject);
        float newAnchor = keepMinSide ? GetAxis(after.min, axis) : GetAxis(after.max, axis);
        float correction = anchor - newAnchor;

        Vector3 position = selected.position;
        SetAxis(ref position, axis, GetAxis(position, axis) + correction);
        selected.position = position;
        EditorUtility.SetDirty(selected);
    }

    Bounds GetCombinedBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(obj.transform.position, Vector3.one);

        Bounds combined = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            combined.Encapsulate(renderers[i].bounds);

        return combined;
    }

    float GetAxis(Vector3 value, Axis axis)
    {
        switch (axis)
        {
            case Axis.X: return value.x;
            case Axis.Y: return value.y;
            default: return value.z;
        }
    }

    void SetAxis(ref Vector3 value, Axis axis, float newValue)
    {
        switch (axis)
        {
            case Axis.X:
                value.x = newValue;
                break;
            case Axis.Y:
                value.y = newValue;
                break;
            case Axis.Z:
                value.z = newValue;
                break;
        }
    }
}
