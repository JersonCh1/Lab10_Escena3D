#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;

public class ConstructorLab10
{
    [MenuItem("Lab10/Construir Escena Completa")]
    public static void ConstruirDesdeMenu()
    {
        ConstruirEscena();
        Debug.Log("Lab10: escena construida. Guarda con Ctrl+S.");
    }

    public static void ConstruirBatch()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            AssetDatabase.CreateFolder("Assets", "Scenes");
        var escena = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        ConstruirEscena();
        EditorSceneManager.SaveScene(escena, "Assets/Scenes/Escena3D_Lab10.unity");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Lab10 BATCH: escena construida y guardada en Assets/Scenes/Escena3D_Lab10.unity");
    }

    static Color Hex(string h)
    {
        ColorUtility.TryParseHtmlString("#" + h.Replace("#", ""), out Color c);
        return c;
    }

    static void EnsureFolder(string parent, string name)
    {
        if (!AssetDatabase.IsValidFolder(parent + "/" + name))
            AssetDatabase.CreateFolder(parent, name);
    }

    static Material CrearMat(string nombre, string hexBase, float smoothness, float metallic)
    {
        var m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        m.SetColor("_BaseColor", Hex(hexBase));
        m.SetFloat("_Smoothness", smoothness);
        m.SetFloat("_Metallic", metallic);
        AssetDatabase.CreateAsset(m, "Assets/Materials/" + nombre + ".mat");
        return m;
    }

    static Material CrearSkybox(string nombre, float sunDisk, float sunSize, float sunConv,
                               float atmos, string skyTint, string ground, float exposure)
    {
        var m = new Material(Shader.Find("Skybox/Procedural"));
        m.SetFloat("_SunDisk", sunDisk);
        m.SetFloat("_SunSize", sunSize);
        m.SetFloat("_SunSizeConvergence", sunConv);
        m.SetFloat("_AtmosphereThickness", atmos);
        m.SetColor("_SkyTint", Hex(skyTint));
        m.SetColor("_GroundColor", Hex(ground));
        m.SetFloat("_Exposure", exposure);
        AssetDatabase.CreateAsset(m, "Assets/Skyboxes/" + nombre + ".mat");
        return m;
    }

    static void AssignMat(GameObject go, Material m)
    {
        var r = go.GetComponent<Renderer>();
        if (r != null) r.sharedMaterial = m;
    }

    public static void ConstruirEscena()
    {
        EnsureFolder("Assets", "Materials");
        EnsureFolder("Assets", "Textures");
        EnsureFolder("Assets", "Skyboxes");
        EnsureFolder("Assets", "Scripts");
        EnsureFolder("Assets", "Scenes");

        // ----- Contenedor -----
        var contenedor = new GameObject("Escena_Contenedor");
        contenedor.transform.position = Vector3.zero;

        // ----- Suelo -----
        var suelo = GameObject.CreatePrimitive(PrimitiveType.Plane);
        suelo.name = "Suelo_Principal";
        suelo.transform.SetParent(contenedor.transform);
        suelo.transform.localScale = new Vector3(3, 1, 3);

        // ----- Plataforma -----
        var plat = GameObject.CreatePrimitive(PrimitiveType.Cube);
        plat.name = "Plataforma_Central";
        plat.transform.SetParent(contenedor.transform);
        plat.transform.position = new Vector3(0, 0.5f, 0);
        plat.transform.localScale = new Vector3(4, 1, 4);

        // ----- Caja -----
        var caja = GameObject.CreatePrimitive(PrimitiveType.Cube);
        caja.name = "Prop_Caja_01";
        caja.transform.SetParent(contenedor.transform);
        caja.transform.position = new Vector3(-1, 1.5f, -1);
        caja.transform.rotation = Quaternion.Euler(0, 45, 0);
        caja.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

        // ----- Esfera -----
        var esfera = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        esfera.name = "Prop_Esfera_01";
        esfera.transform.SetParent(contenedor.transform);
        esfera.transform.position = new Vector3(1, 1.8f, 1);
        esfera.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

        // ----- Columna -----
        var col = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        col.name = "Prop_Columna_01";
        col.transform.SetParent(contenedor.transform);
        col.transform.position = new Vector3(0, 2, 0);
        col.transform.localScale = new Vector3(0.3f, 1.5f, 0.3f);

        // ----- Cámara -----
        var cam = Camera.main;
        if (cam != null)
        {
            cam.transform.position = new Vector3(-4, 4, -6);
            cam.transform.rotation = Quaternion.Euler(20, 25, 0);
        }

        // ----- Materiales -----
        var matSuelo = CrearMat("Mat_Suelo", "4A4A4A", 0.2f, 0f);
        var matPlat = CrearMat("Mat_Plataforma", "2E4057", 0.4f, 0.1f);
        var matCaja = CrearMat("Mat_Caja_Metal", "8B7355", 0.6f, 0.8f);
        var matEsfera = CrearMat("Mat_Esfera_Emision", "00C2FF", 0.9f, 0f);
        matEsfera.EnableKeyword("_EMISSION");
        matEsfera.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        matEsfera.SetColor("_EmissionColor", Hex("00C2FF") * Mathf.Pow(2f, 1.5f));
        EditorUtility.SetDirty(matEsfera);
        var matColumna = CrearMat("Mat_Columna", "C0C0C0", 0.75f, 0.9f);

        AssignMat(suelo, matSuelo);
        AssignMat(plat, matPlat);
        AssignMat(caja, matCaja);
        AssignMat(esfera, matEsfera);
        AssignMat(col, matColumna);

        // ----- Skyboxes -----
        var skyAtardecer = CrearSkybox("Skybox_Atardecer", 2, 0.05f, 8f, 1.2f, "FF6B35", "3D2B1F", 1.2f);
        var skyNoche = CrearSkybox("Skybox_Noche", 0, 0.04f, 8f, 0.3f, "0A0A2E", "0D0D0D", 0.6f);

        // ----- Iluminación -----
        RenderSettings.skybox = skyAtardecer;
        RenderSettings.ambientMode = AmbientMode.Skybox;
        RenderSettings.ambientIntensity = 1.0f;

        Light dir = null;
        foreach (var l in Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
            if (l.type == LightType.Directional) { dir = l; break; }
        if (dir != null)
        {
            dir.color = Hex("FFB347");
            dir.intensity = 1.5f;
            dir.transform.rotation = Quaternion.Euler(45, -30, 0);
            RenderSettings.sun = dir;
        }
        DynamicGI.UpdateEnvironment();

        // ----- GameManager + SkyboxController -----
        var gm = new GameObject("GameManager");
        var sc = gm.AddComponent<SkyboxController>();
        sc.skyboxDia = skyAtardecer;
        sc.skyboxNoche = skyNoche;
        sc.esDeNoche = false;
        EditorUtility.SetDirty(gm);
    }

    // Ejecutar DESPUES de soltar las texturas de Polyhaven en Assets/Textures
    [MenuItem("Lab10/Aplicar Texturas Polyhaven al Suelo")]
    public static void AplicarTexturas()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Textures" });
        Texture2D albedo = null, normal = null;
        foreach (var g in guids)
        {
            string p = AssetDatabase.GUIDToAssetPath(g);
            string n = System.IO.Path.GetFileNameWithoutExtension(p).ToLower();
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(p);
            if (n.Contains("nor")) { normal = tex; SetNormal(p); }
            else if (n.Contains("diff") || n.Contains("albedo") || n.Contains("col")) albedo = tex;
        }
        var matSuelo = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Suelo.mat");
        if (matSuelo != null)
        {
            if (albedo) matSuelo.SetTexture("_BaseMap", albedo);
            if (normal)
            {
                matSuelo.EnableKeyword("_NORMALMAP");
                matSuelo.SetTexture("_BumpMap", normal);
                matSuelo.SetFloat("_BumpScale", 1f);
            }
            matSuelo.SetTextureScale("_BaseMap", new Vector2(3, 3));
            matSuelo.SetFloat("_Smoothness", 0f);
            EditorUtility.SetDirty(matSuelo);
            AssetDatabase.SaveAssets();
            Debug.Log($"Lab10: texturas aplicadas al suelo. albedo={albedo != null}, normal={normal != null}");
        }
    }

    static void SetNormal(string path)
    {
        var ti = AssetImporter.GetAtPath(path) as TextureImporter;
        if (ti != null && ti.textureType != TextureImporterType.NormalMap)
        {
            ti.textureType = TextureImporterType.NormalMap;
            ti.SaveAndReimport();
        }
    }
}
#endif
