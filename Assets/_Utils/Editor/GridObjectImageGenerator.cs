using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridObjectListSO))]
public class GridObjectImageGenerator : Editor
{
    private RenderTexture renderTexture;
    private GameObject cameraGameObject;
    private GameObject keyLightGameObject;
    private GameObject fillLightGameObject;
    private GameObject currentPrefab;
    private Camera currentCamera;


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        GUILayout.Space(10);

        if (GUILayout.Button("Generate Prefab Images", GUILayout.Height(30)))
        {
            var gridObjectListSO = (GridObjectListSO)target;

            foreach (GridObjectData gridObject in gridObjectListSO.gridObjects)
            {
                GeneratePrefabImage(gridObject, gridObjectListSO);
            }
            
            AssetDatabase.SaveAssets();
        }
        
    }
    
    private void GeneratePrefabImage(GridObjectData gridObjectData, GridObjectListSO gridObjectListSO)
    {
        Texture2D texture = RenderPrefab(gridObjectData.prefab);

        const string folderPath = "Assets/_UI/Object Catalogue/Generated Sprites";
        Directory.CreateDirectory(folderPath);

        string path = $"{folderPath}/{gridObjectData.name}.png";

        File.WriteAllBytes(path, texture.EncodeToPNG());
        AssetDatabase.ImportAsset(path);

        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.SaveAndReimport();

        gridObjectData.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

        EditorUtility.SetDirty(gridObjectListSO);
    }

    private Texture2D RenderPrefab(GameObject prefab)
    {
        currentPrefab = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        
        SetupCamera();
        
        keyLightGameObject = SetupLight(Quaternion.Euler(50f, -30f, 0f), 1.2f);
        fillLightGameObject = SetupLight(Quaternion.Euler(340f, 30f, 0f), 0.4f);
        
        SetCameraPosition();
        RenderCameraView(currentCamera, out Texture2D texture2D);
        
        RenderPrefabCleanup();

        return texture2D;

    }
    private void SetupCamera()
    {

        cameraGameObject = new GameObject("Camera");
        currentCamera = cameraGameObject.AddComponent<Camera>();
        
        currentCamera.clearFlags = CameraClearFlags.SolidColor;
        currentCamera.backgroundColor = new Color(0, 0, 0, 0);
        currentCamera.orthographic = false;
        currentCamera.fieldOfView = 40f;
    }
    private void SetCameraPosition()
    {
        Bounds bounds = GetBounds(currentPrefab);
        
        Vector3 direction = new Vector3(-1f, 0.7f, -1f).normalized;
        float distance = bounds.size.magnitude * 1.5f;
        
        cameraGameObject.transform.position = bounds.center + direction * distance;
        cameraGameObject.transform.LookAt(bounds.center);
    }
    private void RenderPrefabCleanup()
    {
        RenderTexture.active = null;
        currentCamera.targetTexture = null;
        
        DestroyImmediate(renderTexture);
        DestroyImmediate(cameraGameObject);
        DestroyImmediate(keyLightGameObject);
        DestroyImmediate(fillLightGameObject);
        DestroyImmediate(currentPrefab);
    }
    private void RenderCameraView(Camera camera, out Texture2D texture2D)
    {
        renderTexture = new RenderTexture(512, 512, 24, RenderTextureFormat.ARGB32);
        camera.targetTexture = renderTexture;
        camera.Render();
        RenderTexture.active = renderTexture;

        texture2D = new Texture2D(512, 512, TextureFormat.RGBA32, false);
        texture2D.ReadPixels(new Rect(0, 0, 512, 512), 0, 0);
        texture2D.Apply();
    }

    private GameObject SetupLight(Quaternion rotation, float intensity)
    {
        var lightGameObject = new GameObject();
        var light = lightGameObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = intensity;
        lightGameObject.transform.rotation = rotation;
        return lightGameObject;
    }
    
    private Bounds GetBounds(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return new Bounds(go.transform.position, Vector3.one);

        Bounds bounds = renderers[0].bounds;

        foreach (Renderer r in renderers)
            bounds.Encapsulate(r.bounds);

        return bounds;
    }
}
