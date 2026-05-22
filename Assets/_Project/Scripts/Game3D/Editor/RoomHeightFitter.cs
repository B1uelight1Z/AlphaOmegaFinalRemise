// Assets/Script/Editor/RoomHeightFitter.cs
using UnityEditor;
using UnityEngine;

public class RoomHeightFitter : EditorWindow
{
    GameObject roomObject;
    GameObject roofObject;
    GameObject floorObject;

    [MenuItem("Tools/Room Height Fitter")]
    static void Open() => GetWindow<RoomHeightFitter>("Room Height Fitter");

    void OnGUI()
    {
        GUILayout.Label("Redimensionne un objet entre un toit et un sol", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        roomObject = (GameObject)EditorGUILayout.ObjectField("Objet à redimensionner", roomObject, typeof(GameObject), true);
        roofObject = (GameObject)EditorGUILayout.ObjectField("Toit (référence haute)", roofObject, typeof(GameObject), true);
        floorObject = (GameObject)EditorGUILayout.ObjectField("Sol (référence basse)", floorObject, typeof(GameObject), true);

        EditorGUILayout.Space();

        if (GUILayout.Button("Appliquer"))
        {
            if (roomObject == null || roofObject == null || floorObject == null)
            {
                Debug.LogWarning("Assigne les 3 objets avant d'appliquer.");
                return;
            }
            FitRoomHeight();
        }
    }

    void FitRoomHeight()
    {
        // Récupère les bounds combinés (enfants inclus)
        Bounds roofBounds = GetCombinedBounds(roofObject);
        Bounds floorBounds = GetCombinedBounds(floorObject);
        Bounds roomBounds = GetCombinedBounds(roomObject);

        // Calcule la distance entre le bas du toit et le haut du sol
        float roofBottom = roofBounds.min.y;
        float floorTop = floorBounds.max.y;
        float targetHeight = roofBottom - floorTop;

        if (targetHeight <= 0)
        {
            Debug.LogWarning("Le toit est en dessous du sol — vérifie les objets assignés.");
            return;
        }

        // Calcule le scale Y nécessaire
        float currentHeight = roomBounds.size.y;
        float scaleFactor = targetHeight / currentHeight;

        Undo.RecordObject(roomObject.transform, "Fit Room Height");

        // Applique le nouveau scale
        Vector3 newScale = roomObject.transform.localScale;
        newScale.y *= scaleFactor;
        roomObject.transform.localScale = newScale;

        // Repositionne pour que le bas de l'objet touche le haut du sol
        Vector3 newPos = roomObject.transform.position;
        newPos.y = floorTop + (targetHeight / 2f);
        roomObject.transform.position = newPos;

        Debug.Log($"✅ Hauteur appliquée : {targetHeight:F3}u | Scale Y : {newScale.y:F3} | Position Y : {newPos.y:F3}");
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
}