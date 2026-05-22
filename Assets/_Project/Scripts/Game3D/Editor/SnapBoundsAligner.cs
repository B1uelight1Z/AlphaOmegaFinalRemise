// Assets/Editor/SnapToNeighbor.cs
using UnityEditor;
using UnityEngine;

public class SnapBoundsAligner : EditorWindow
{
    enum Axis { X, Y, Z }
    Axis alignAxis = Axis.X;
    bool alignMin = true;

    [MenuItem("Tools/Snap Bounds Aligner")]
    static void Open() => GetWindow<SnapBoundsAligner>("Snap Bounds");

    void OnGUI()
    {
        GUILayout.Label("Aligne les bords des objets sélectionnés", EditorStyles.boldLabel);
        alignAxis = (Axis)EditorGUILayout.EnumPopup("Axe", alignAxis);
        alignMin = EditorGUILayout.Toggle("Aligner bord MIN (sinon MAX)", alignMin);

        if (GUILayout.Button("Aligner sur le 1er objet sélectionné"))
            AlignBounds();
    }

    void AlignBounds()
    {
        GameObject[] selected = Selection.gameObjects;
        if (selected.Length < 2) { Debug.LogWarning("Sélectionne au moins 2 objets."); return; }

        Bounds refBounds = GetCombinedBounds(selected[0]);
        if (refBounds.size == Vector3.zero) { Debug.LogWarning("L'objet de référence n'a pas de Renderer."); return; }

        float target = alignMin ? GetMin(refBounds, alignAxis) : GetMax(refBounds, alignAxis);

        for (int i = 1; i < selected.Length; i++)
        {
            Bounds b = GetCombinedBounds(selected[i]);
            if (b.size == Vector3.zero) { Debug.LogWarning($"{selected[i].name} ignoré : pas de Renderer."); continue; }

            Undo.RecordObject(selected[i].transform, "Snap Bounds");
            float current = alignMin ? GetMin(b, alignAxis) : GetMax(b, alignAxis);
            float delta = target - current;

            Vector3 pos = selected[i].transform.position;
            switch (alignAxis)
            {
                case Axis.X: pos.x += delta; break;
                case Axis.Y: pos.y += delta; break;
                case Axis.Z: pos.z += delta; break;
            }
            selected[i].transform.position = pos;
        }
    }

    Bounds GetCombinedBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds();

        Bounds combined = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            combined.Encapsulate(renderers[i].bounds);

        return combined;
    }

    float GetMin(Bounds b, Axis a) => a == Axis.X ? b.min.x : a == Axis.Y ? b.min.y : b.min.z;
    float GetMax(Bounds b, Axis a) => a == Axis.X ? b.max.x : a == Axis.Y ? b.max.y : b.max.z;
}