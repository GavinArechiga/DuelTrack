using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridObjectListSO))]
public class GridObjectImageGenerator : Editor
{
    private RenderTexture renderTexture;
    private Texture2D texture2D;
    private GameObject cameraGameObject;
    private GameObject keyLightGameObject;
    private GameObject fillLightGameObject;
    private GameObject currentPrefab;
    private Camera camera;


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
            
            PrefabCleanup();
            AssetDatabase.SaveAssets();
        }
        
    }
    
    private void GeneratePrefabImage(GridObjectData gridObjectData, GridObjectListSO gridObjectListSO)
    {
        RenderPrefab(gridObjectData.prefab);

        const string folderPath = "Assets/_UI/Object Catalogue/Generated Sprites";
        Directory.CreateDirectory(folderPath);

        string path = $"{folderPath}/{gridObjectData.name}.png";

        File.WriteAllBytes(path, texture2D.EncodeToPNG());
        AssetDatabase.ImportAsset(path);

        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.SaveAndReimport();

        gridObjectData.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

        // Tells unity that the asset needs to be refreshed
        EditorUtility.SetDirty(gridObjectListSO);
    }

    private void RenderPrefab(GameObject prefab)
    {
        currentPrefab = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        
        // reusing camera and lights to improve performance
        if (!cameraGameObject)
        {
            SetupCamera();
        }

        if (!keyLightGameObject || !fillLightGameObject)
        {
            keyLightGameObject = SetupLight(Quaternion.Euler(50f, -30f, 0f), 1.2f);
            fillLightGameObject = SetupLight(Quaternion.Euler(340f, 30f, 0f), 0.4f);
        }
        
        SetCameraPositionAndRotation();
        RenderCameraView();
        DestroyImmediate(currentPrefab);

    }
    private void SetupCamera()
    {
        cameraGameObject = new GameObject("Camera");
        camera = cameraGameObject.AddComponent<Camera>();
        
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0, 0, 0, 0);
        camera.orthographic = false;
        camera.fieldOfView = 40f;
    }
    private void SetCameraPositionAndRotation()
    {
        Bounds bounds = GetBounds(currentPrefab);
        
        Vector3 offsetInfluence = new Vector3(-1f, 0.7f, -1f).normalized; // controls how much distance affects each axis and in what direction
        float distance = bounds.size.magnitude * 1.5f; // the magnitude is used so the distance is scaled based on the prefabs size
        
        cameraGameObject.transform.position = bounds.center + offsetInfluence * distance;
        // rotate camera to look at the center of the prefab
        cameraGameObject.transform.LookAt(bounds.center);
    }
    private void PrefabCleanup()
    {
        RenderTexture.active = null;
        camera.targetTexture = null;
        
        DestroyImmediate(cameraGameObject);
        DestroyImmediate(keyLightGameObject);
        DestroyImmediate(fillLightGameObject);
        DestroyImmediate(renderTexture);
    }
    private void RenderCameraView()
    {
        const int width = 512;
        const int height = 512;
        
        // reusing textures to improve performance
        
        if (!renderTexture)
        {
            const int depth = 24;
            
            renderTexture = new RenderTexture(width, height, depth, RenderTextureFormat.ARGB32);
            camera.targetTexture = renderTexture;
            RenderTexture.active = renderTexture;
        }

        if (!texture2D)
        {
            texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);
        }
        
        //These vars are just for readability since the parameter names for ReadPixels() and Rect() can be confusing
        const int writePixelStartPosX = 0;
        const int writePixelStartPosY = 0;

        const int originX = 0;
        const int originY = 0;
        
        camera.Render();
        texture2D.ReadPixels(new Rect(originX, originY, width, height), writePixelStartPosX, writePixelStartPosY);
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
    
    private Bounds GetBounds(GameObject gameObject)
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return new Bounds(gameObject.transform.position, Vector3.one);

        Bounds bounds = renderers[0].bounds;

        foreach (Renderer renderer in renderers)
            bounds.Encapsulate(renderer.bounds);

        return bounds;
    }
}
