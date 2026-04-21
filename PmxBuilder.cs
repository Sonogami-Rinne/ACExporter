using Character;
using CharacterCreation;
using Il2CppSystem.Linq;
using IllusionMods;
using PmxLib;
using RuntimeUnityEditor.Core.Utils;
using ACExporter;
using ACExporter.Utils;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
internal class PmxBuilder
{

	public string msg = "";

    public HashSet<string> ignoreList = new HashSet<string>
	{
		"Bonelyfans", "c_m_shadowcast", "Standard", "cf_m_body", "cf_m_face_00", "cf_m_tooth", "cf_m_canine", "cf_m_mayuge_00", "cf_m_noseline_00", "cf_m_eyeline_00_up",
		"cf_m_eyeline_kage", "cf_m_eyeline_down", "cf_m_sirome_00", "cf_m_hitomi_00", "cf_m_tang", "cf_m_namida_00", "cf_m_gageye_00", "cf_m_gageye_01", "cf_m_gageye_02",
		"cf_O_face_atari_M", "Highlight_cm_O_face_rend", "cm_m_body", "Highlight_o_body_a_rend", "Highlight_cf_O_face_rend", "o_shadowcaster", "o_body", "cf_O_face", "cf_O_tooth",
		"cf_O_canine", "cf_O_mayuge", "cf_O_noseline", "cf_O_eyeline", "cf_O_eyeline_low", "cf_O_namida_L", "cf_O_namida_M", "cf_O_namida_S", "cf_Ohitomi_L", "cf_Ohitomi_R",
		"cf_Ohitomi_L02", "cf_Ohitomi_R02", "cf_O_gag_eye_00", "cf_O_gag_eye_01", "cf_O_gag_eye_02", "o_tang", "cf_O_face_atari", "o_tango", "o_nail_def01", "o_nail_foot", "cf_m_body_00", "cf_m_head_00"
    };

	public string EyeMatName = "cf_m_hitomi_00";

	public HashSet<string> whitelistOffsetBones = new HashSet<string>
	{
		"cf_d_sk_top", "cf_d_sk_00_00", "cf_j_sk_00_00", "cf_j_sk_00_01", "cf_j_sk_00_02", "cf_j_sk_00_03", "cf_j_sk_00_04", "cf_j_sk_00_05", "cf_d_sk_01_00", "cf_j_sk_01_00",
		"cf_j_sk_01_01", "cf_j_sk_01_02", "cf_j_sk_01_03", "cf_j_sk_01_04", "cf_j_sk_01_05", "cf_d_sk_02_00", "cf_j_sk_02_00", "cf_j_sk_02_01", "cf_j_sk_02_02", "cf_j_sk_02_03",
		"cf_j_sk_02_04", "cf_j_sk_02_05", "cf_d_sk_03_00", "cf_j_sk_03_00", "cf_j_sk_03_01", "cf_j_sk_03_02", "cf_j_sk_03_03", "cf_j_sk_03_04", "cf_j_sk_03_05", "cf_d_sk_04_00",
		"cf_j_sk_04_00", "cf_j_sk_04_01", "cf_j_sk_04_02", "cf_j_sk_04_03", "cf_j_sk_04_04", "cf_j_sk_04_05", "cf_d_sk_05_00", "cf_j_sk_05_00", "cf_j_sk_05_01", "cf_j_sk_05_02",
		"cf_j_sk_05_03", "cf_j_sk_05_04", "cf_j_sk_05_05", "cf_d_sk_06_00", "cf_j_sk_06_00", "cf_j_sk_06_01", "cf_j_sk_06_02", "cf_j_sk_06_03", "cf_j_sk_06_04", "cf_j_sk_06_05",
		"cf_d_sk_07_00", "cf_j_sk_07_00", "cf_j_sk_07_01", "cf_j_sk_07_02", "cf_j_sk_07_03", "cf_j_sk_07_04", "cf_j_sk_07_05"
	};

    string[] ignoredShaders = { "LIF/lit_hair_overlay", "LIF/lif_main_hair_outline" };

    public bool exportAllOutfits = false;

	public bool exportWithMainCamera = false;

    public static int minCoord;

	public static int maxCoord;

	public static int nowCoordinate = -1;

	public static string savePath;

	public static string currentSavePath;

	public static Dictionary<string, int> currentMaterialList = new Dictionary<string, int>();

	public static HashSet<string> currentBonesList = new HashSet<string>();

	public Dictionary<Renderer, List<string>> currentRendererMaterialMapping = new Dictionary<Renderer, List<string>>();

	public static Pmx pmxFile;

	private List<SMRData> characterSMRData = new List<SMRData>();

	private List<MaterialDataComplete> materialDataComplete = new List<MaterialDataComplete>();

	private List<TextureData> textureData = new List<TextureData>();

	private List<CharacterInfoData> characterInfoData = new List<CharacterInfoData>();

	private List<DynamicBoneData> dynamicBonesData = new List<DynamicBoneData>();

	private List<DynamicBoneColliderData> dynamicBoneCollidersData = new List<DynamicBoneColliderData>();

	private List<BoneOffsetData> boneOffsetData = new List<BoneOffsetData>();

	private static Dictionary<int, int> instanceIDs = new Dictionary<int, int>();

	private static HashSet<string> offsetBoneCandidates = new HashSet<string>();

	private int vertexCount;

	private readonly int scale = 1;

	private Dictionary<string, int> currentBoneKeysList = new Dictionary<string, int>();

	private Dictionary<string, int> vertexCountRecord = new Dictionary<string, int>();

    private List<BoneInfo> editBoneInfo = new List<BoneInfo>();

	private List<object> recoverInfos = new List<object>();

    private List<Renderer> meshRenders = new List<Renderer>();

	private List<string> lightDarkMaterials = new List<string>();

	private Dictionary<string, List<string>> smrMaterialsCache = new Dictionary<string, List<string>>();

	private GameObject gameObjectMeshCopier;

	private GameObject gameObjectMeshContainer;// Carrier of square mesh which is used to export light textures

    private string charaName;

    public IEnumerator BuildStart()
	{
		TextureSaver.Init();
        Human human = ACExporterPlugin.selectedChara;
        this.charaName = human.fileParam.GetCharaName(false);
        CreateBaseSavePath();
        ChangeAnimations();
		yield return new WaitForSeconds(0.4f);
        CollectIgnoreList();
        CreateCharacterInfoData();
		yield return new WaitForSeconds(0.4f);
		Prepare();
        nowCoordinate = exportAllOutfits ? 0 : human.fileStatus.coordinateType;
		maxCoord = exportAllOutfits ? human.coorde._data.Coordinates.Length : nowCoordinate + 1;

        for (; nowCoordinate < maxCoord + 1; nowCoordinate++)
		{
			if (nowCoordinate < maxCoord)
			{
				human.coorde.ChangeCoordinateType((ChaFileDefine.CoordinateType)nowCoordinate);
				human.ReloadCoordinate(Human.ReloadFlags.Coorde);                
                yield return new WaitForSeconds(2f);
			}
            BuildStart_BG();
        }
		ExportAllDataLists();
		CleanUp();
		OpenFolderInExplorer(savePath);
		TextureSaver.Recycle();
		Console.WriteLine("Finished");
	}

    private void BuildStart_BG()
	{
		try
		{
			ResetPmxBuilder();
			CreateModelInfo();
			CreateInstanceIDs();
            SetSavePath();
            Directory.CreateDirectory(currentSavePath);
            Directory.CreateDirectory(currentSavePath + "/pre_light");
            Directory.CreateDirectory(currentSavePath + "/pre_dark");
            ClearMorphs();

            if (nowCoordinate < maxCoord)
            {
                CreateBoneList();
            }
            CreateMeshList();

			//if (nowCoordinate == maxCoord)
   //         {
			//	//ExportGagEyes();
			//}
            AddAccessory();
			ExportLightTexture();
			if (nowCoordinate < maxCoord)
			{
				try
				{
					CreateDynamicBonesData();
					CreateDynamicBoneCollidersData();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
			}

			CreatePmxHeader();
			Save();

		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
		}

	}
	
	private void Prepare()
	{
        gameObjectMeshCopier = new GameObject("MeshCopier");
        var smr = gameObjectMeshCopier.AddComponent<SkinnedMeshRenderer>();

		GameObject.Find("Cvs_BackGround").GetComponent<Canvas>().enabled = false;
		Camera camera;
		GameObject light = Light.FindObjectsOfType<Light>()[0].gameObject;

        recoverInfos.Clear();
        recoverInfos.Add(light.transform.rotation);
        recoverInfos.Add(light.transform.position);
        recoverInfos.Add(Camera.main.transform.position);
        recoverInfos.Add(Camera.main.transform.rotation);
        if (this.exportWithMainCamera)
		{
			camera = Camera.main;
			recoverInfos.Add(camera.clearFlags);
			recoverInfos.Add(camera.allowHDR);
			recoverInfos.Add(camera.allowMSAA);
			recoverInfos.Add(camera.cullingMask);
			recoverInfos.Add(camera.aspect);

        }
		else
		{
			camera = gameObjectMeshCopier.AddComponent<Camera>();
            camera.CopyFrom(Camera.main);
        }

		camera.orthographic = true;
		camera.aspect = 1f;
		camera.clearFlags = CameraClearFlags.SolidColor;
		camera.allowHDR = false;
		camera.allowMSAA = false;
		camera.cullingMask = 1 << 10;

		if (this.exportWithMainCamera)
		{
            var camera1 = gameObjectMeshCopier.AddComponent<Camera>();
            camera1.CopyFrom(camera);
        }

        Mesh square = new Mesh();

		Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<UnityEngine.Vector3> vertices = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<UnityEngine.Vector3>(4);
		vertices[0] = new UnityEngine.Vector3(1.001f, -0.001f, 0f);
		vertices[1] = new UnityEngine.Vector3(1.001f, 1.001f, 0f);
		vertices[2] = new UnityEngine.Vector3(-0.001f, 1.001f, 0f);
		vertices[3] = new UnityEngine.Vector3(-0.001f, -0.001f, 0f);

		square.vertices = vertices;

        Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<UnityEngine.Vector2> uvs = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<UnityEngine.Vector2>(4);
		uvs[0] = new UnityEngine.Vector2(-0.001f, -0.001f);
		uvs[1] = new UnityEngine.Vector2(-0.001f, 1.001f);
		uvs[2] = new UnityEngine.Vector2(1.001f, 1.001f);
		uvs[3] = new UnityEngine.Vector2(1.001f, -0.001f);

		square.uv = uvs;

		Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<int> triangles = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<int>(6);

		triangles[0] = 1;
		triangles[1] = 3;
		triangles[2] = 0;
		triangles[3] = 2;
		triangles[4] = 3;
		triangles[5] = 1;
        square.triangles = triangles;

        square.RecalculateBounds();
        square.RecalculateNormals();
        square.RecalculateTangents();

        square.MarkModified();

		gameObjectMeshContainer = new GameObject("test");
		gameObjectMeshContainer.AddComponent<MeshFilter>().sharedMesh = square;

        var assembly = Assembly.GetExecutingAssembly();
        using (Stream stream = assembly.GetManifestResourceStream("ACExporter.Assets.meshexporter.unity3d"))
        {
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            AssetBundle bundle = AssetBundle.LoadFromMemory(data);
            foreach (var i in bundle.LoadAllAssets<Shader>())
            {
                smr.material = new Material(i);
            }
			bundle.Unload(false);
        }

        //human.body.LoadAnimation("custom/00.unity3d", "tpose");

    }
	private void SetSavePath()
	{
        if (nowCoordinate == maxCoord)
        {
            currentSavePath = savePath;
        }
        else
        {
            currentSavePath = savePath + "Outfit " + nowCoordinate.ToString("00") + "/";
        }
    } 

    public void CleanUp()
	{
		try
		{
			GameObject.Destroy(gameObjectMeshCopier);

            editBoneInfo.Clear();
			lightDarkMaterials.Clear();

            GameObject light = Light.FindObjectsOfType<Light>()[0].gameObject;
            Camera camera = Camera.main;

            light.transform.rotation = (UnityEngine.Quaternion)recoverInfos[0];
            light.transform.position = (UnityEngine.Vector3)recoverInfos[1];

            camera.transform.position = (UnityEngine.Vector3)recoverInfos[2];
            camera.transform.rotation = (UnityEngine.Quaternion)recoverInfos[3];

			if (this.exportWithMainCamera)
			{
				camera.clearFlags = (CameraClearFlags)recoverInfos[4];
				camera.allowHDR = (bool)recoverInfos[5];
				camera.allowMSAA = (bool)recoverInfos[6];
				camera.cullingMask = (int)recoverInfos[7];
				camera.aspect = (float)recoverInfos[8];
				camera.orthographic = false;
            }

            GameObject.Find("Cvs_BackGround").GetComponent<Canvas>().enabled = true;

            recoverInfos.Clear();
			

			GameObject.Destroy(gameObjectMeshContainer);
        }
		catch(Exception ex)
		{
			Console.WriteLine("Error when cleaning up, reason:" + ex.Message);
		}
		finally
		{
			pmxFile = null;
            Human human = ACExporterPlugin.selectedChara;
			human.Reload();
		}
	}
    public void ExportLightTexture()
    {
		List<bool> renderStatus = new List<bool>();
		var renders = GameObject.Find("BodyTop").GetComponentsInChildren<Renderer>();
        for(int i = 0; i < renders.Length; i++)
		{
			renderStatus.Add(renders[i].enabled);
			renders[i].enabled = false;
        }

        string[] ignoredSMRs = { "cf_O_gag_eye_00", "cf_O_gag_eye_01", "cf_O_gag_eye_02", "cf_O_namida_L", "cf_O_namida_M", "cf_O_namida_S", "Highlight_o_body_a_rend", "Highlight_cf_O_face_rend", "o_Mask" };
		string[] multiTexShaders = { "AC/skin_head", "AC/skin_body" };
		string[] ignoredShaders = { "AC/hair_outline", "AC/sub/shadowcast" };
		GameObject light = Light.FindObjectsOfType<Light>()[0].gameObject;
		Camera camera;
		if (this.exportWithMainCamera)
		{
			camera = Camera.main;
		}
		else
		{
            camera = gameObjectMeshCopier.GetComponent<Camera>();
        }
		Camera cameraMain = Camera.main;
		Camera auxiliaryCamera = gameObjectMeshCopier.GetComponent<Camera>();
        SkinnedMeshRenderer meshCopier = gameObjectMeshCopier.GetComponent<SkinnedMeshRenderer>();
        UnityEngine.Quaternion lightRotation = new UnityEngine.Quaternion(-0.03940f, 0.95533f, -0.21508f, -0.19882f);
        UnityEngine.Quaternion darkRotation = new UnityEngine.Quaternion(0.16412f, -0.02926f, 0.00487f, 0.98600f);
        UnityEngine.Vector3 lightPosition = new UnityEngine.Vector3(0.70f, 4.46f, 2.69f);
        UnityEngine.Vector3 darkPosition = new UnityEngine.Vector3(-0.17f, 4.40f, 2.84f);
		Mesh square = gameObjectMeshContainer.GetComponent<MeshFilter>().sharedMesh;
        Mesh mesh = new Mesh();

        for (int i = 0; i < meshRenders.Count; i++)
        {
            var smr = meshRenders[i];
            string smrName = smr.name;

            bool flag = false;
            smrName = ((ignoreList.Contains(smrName, StringComparer.Ordinal) && smr.sharedMaterials.Length > 0 && ignoreList.Contains(PmxBuilder.CleanUpName(smr.sharedMaterial.name), StringComparer.Ordinal)) ? smrName : (smrName + " " + PmxBuilder.GetAltInstanceID(smr)));
            for (int j = 0; j < ignoredSMRs.Length; j++)
            {
                if (smrName.Contains(ignoredSMRs[j]))
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                continue;
            }
            Console.WriteLine("Exporting textures for " + smr.name);
			Mesh originalMesh;
            LightProbeUsage probeUsage;
            Transform probeAnchor;
            ShadowCastingMode probeCastingMode;
            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            bool receiveShadow;
			bool isSMR = false;
            
            if (smr.GetType().Name == "SkinnedMeshRenderer")
            {
                var _tmp = (SkinnedMeshRenderer)smr;
				isSMR = true;
				originalMesh = _tmp.sharedMesh;
                probeUsage = _tmp.lightProbeUsage;
                probeAnchor = _tmp.probeAnchor;
                probeCastingMode = _tmp.shadowCastingMode;
                receiveShadow = _tmp.receiveShadows;
                _tmp.GetPropertyBlock(materialPropertyBlock);

            }
			else
			{
				MeshFilter meshFilter = smr.gameObject.GetComponent<MeshFilter>();
				var _tmp = (MeshRenderer)smr;
				originalMesh = meshFilter.sharedMesh;
                probeUsage = _tmp.lightProbeUsage;
				probeAnchor = _tmp.probeAnchor;
				probeCastingMode = _tmp.shadowCastingMode;
				receiveShadow = _tmp.receiveShadows;
				_tmp.GetPropertyBlock(materialPropertyBlock);

			}
            int subMeshCount = originalMesh.subMeshCount;
            int layer = smr.gameObject.layer;// Do not forget to drawmesh in this layer

            int verticalBlockCount = 1;
			int horizontalBlockCount = 1;
			UnityEngine.Vector3 positionFront = new UnityEngine.Vector3();
			UnityEngine.Vector3 positionLookAt = new UnityEngine.Vector3();
			bool modifiedMesh = false;
			

			for (int j = 0; j < smr.sharedMaterials.Length; j++)
            {
				int submeshIndex = Math.Min(j, subMeshCount);
                if (meshRenders[i].sharedMaterials[j] == null || ignoredShaders.Contains(meshRenders[i].sharedMaterials[j].shader.name)) continue;
                Material material = new Material(meshRenders[i].sharedMaterials[j]);
				if (ignoredShaders.Contains(material.shader.name))
				{
					continue;
				}

                string matName = smrMaterialsCache[GetGameObjectPath(smr.gameObject)][j];

                lightDarkMaterials.Add(matName);
                try
                {
                    Texture mainTex = null;
                    if (material.HasProperty("_Main_texture"))
                    {
                        mainTex = material.GetTexture("_Main_texture");
                    }
                    else if (material.HasProperty("_Create_main_texture"))
                    {
                        mainTex = material.GetTexture("_Create_main_texture");
                    }
					else if (material.HasProperty("_eyebrow_texture"))
					{
						mainTex = material.GetTexture("_eyebrow_texture");
                    }
					else if (material.HasProperty("_Eyelash_up_texture"))
					{
						mainTex = material.GetTexture("_Eyelash_up_texture");
                    }
					else if (material.HasProperty("_Eyelash_dw_texture"))
					{
						mainTex = material.GetTexture("_Eyelash_dw_texture");
                    }
                    int baseLength = mainTex != null ? Math.Max(mainTex.width, mainTex.height) : 2048;

                    if (material.HasProperty("_DetailNormal"))
                    {
                        material.SetTexture("_DetailNormal", null);
                    }
                    if (material.HasProperty("_Matcap_mask"))
                    {
                        material.SetTexture("_Matcap_mask", null);
                    }
                    if (material.HasProperty("_Mat_cap_01"))
                    {
                        material.SetTexture("_Mat_cap_01", null);
                    }
                    if (material.HasProperty("_Mat_cap_02"))
                    {
                        material.SetTexture("_Mat_cap_02", null);
                    }
                    if (material.HasProperty("_Mat_cap_03"))
                    {
                        material.SetTexture("_Mat_cap_03", null);
                    }
                    if (material.HasProperty("unity_Lightmaps"))
                    {
                        material.SetTexture("unity_Lightmaps", null);
                    }
                    if (material.HasProperty("unity_LightmapsInd"))
                    {
                        material.SetTexture("unity_LightmapsInd", null);
                    }
                    if (material.HasProperty("_Normal"))
                    {
                        material.SetTexture("_Normal", null);
                    }
                    if (material.HasProperty("_Normal_map"))
                    {
                        material.SetTexture("_Normal_map", null);
                    }
                    if (material.HasProperty("_Cloth_alpha"))
                    {
                        material.SetTexture("_Cloth_alpha", null);
                    }
                    if (material.HasProperty("_Cloth_alpha_bot"))
                    {
                        material.SetTexture("_Cloth_alpha_bot", null);
                    }
					if (material.HasProperty("_Highlight_texture"))
					{
						material.SetTexture("_Highlight_texture", null);
					}

                    Color32[] lightColor;
                    Color32[] darkColor;
					Color32[] lightOverlay = new Color32[0];
					Color32[] darkOverlay = new Color32[0];
					//Texture2D image = new Texture2D(baseLength, baseLength, TextureFormat.ARGB32, false);
					Texture2D image;
					bool customSize = false;
					switch (baseLength)
					{
						case 512:
							image = TextureSaver.GetTexture2D(0);
							break;
						case 1024:
							image = TextureSaver.GetTexture2D(1);
							break;
						case 2048:
							image = TextureSaver.GetTexture2D(2);
							break;
						case 4096:
							image = TextureSaver.GetTexture2D(3);
							break;
						default:
                            image = new Texture2D(baseLength, baseLength, TextureFormat.ARGB32, false);
							customSize = true;
							break;
                    }

					bool isMultiTexShaders = multiTexShaders.Contains(material.shader.name);

                    if (isMultiTexShaders)
					{
						renderWithModifiedMesh(ref lightOverlay, ref darkOverlay, camera);
                        if (this.exportWithMainCamera)
                        {
                            Color32[] alphaOverlay = new Color32[0];
                            Color32[] _ = null;
                            renderWithModifiedMesh(ref alphaOverlay, ref _, auxiliaryCamera);
                            lightOverlay = addAlpha(lightOverlay, alphaOverlay);
                            darkOverlay = addAlpha(darkOverlay, alphaOverlay);
                        }
                    }

                    camera.orthographicSize = 0.5f;
                    camera.aspect = 1f;
                    camera.transform.position = new UnityEngine.Vector3(0.5f, 0.5f, 1f);
                    camera.transform.LookAt(new UnityEngine.Vector3(0.5f, 0.5f, 0f));
					cameraMain.transform.position = camera.transform.position;
					cameraMain.transform.LookAt(new UnityEngine.Vector3(0.5f, 0.5f, 0f));
					camera.transform.hasChanged = true;
					cameraMain.transform.hasChanged = true;

                    lightColor = render(lightRotation, lightPosition, 0, square, baseLength, baseLength, image, camera);
                    darkColor = render(darkRotation, darkPosition, 0, square, baseLength, baseLength, image, camera);
					if (this.exportWithMainCamera)
					{
                        auxiliaryCamera.orthographicSize = 0.5f;
                        auxiliaryCamera.aspect = 1f;
                        auxiliaryCamera.transform.position = new UnityEngine.Vector3(0.5f, 0.5f, 1f);
                        auxiliaryCamera.transform.LookAt(new UnityEngine.Vector3(0.5f, 0.5f, 0f));
						auxiliaryCamera.transform.hasChanged = true;
						Color32[] alphaOverlay = render(lightRotation, lightPosition, 0, square, baseLength, baseLength, image, auxiliaryCamera);
						lightColor = addAlpha(lightColor, alphaOverlay);
						darkColor = addAlpha(darkColor, alphaOverlay);
                    }
                    if (isMultiTexShaders)
					{
                        blend(lightColor, lightOverlay);
						blend(darkColor, darkOverlay);
					}
                    TextureSaver.SaveTexture(lightColor, baseLength, baseLength, currentSavePath + "/pre_light/" + matName + "_light.png");
                    TextureSaver.SaveTexture(darkColor, baseLength, baseLength, currentSavePath + "/pre_dark/" + matName + "_dark.png");
					if (customSize)
					{
                        Texture2D.DestroyImmediate(image);
                    }
                    Color32[] render(UnityEngine.Quaternion rotation, UnityEngine.Vector3 position, int subMeshIndex, Mesh mesh, int texturewidth, int textureheight, Texture2D _, Camera camera)
                    {
                        light.transform.rotation = rotation;
                        light.transform.position = position;
                        //GL.Flush();

                        RenderTexture renderTexture = new RenderTexture(texturewidth, textureheight, 24, RenderTextureFormat.ARGB32);
                        renderTexture.antiAliasing = 1;
                        renderTexture.filterMode = FilterMode.Point;
                        renderTexture.Create();

                        camera.targetTexture = renderTexture;
                        camera.backgroundColor = Color.clear;
                        Graphics.DrawMesh(mesh, Matrix4x4.identity, material, layer, camera, subMeshIndex, materialPropertyBlock, probeCastingMode, receiveShadow, probeAnchor, probeUsage);
                        camera.Render();

                        RenderTexture.active = renderTexture;
                        _.ReadPixels(new Rect(0, 0, texturewidth, textureheight), 0, 0);
                        _.Apply();
                        RenderTexture.active = null;
                        var data = _.GetPixels32();
                        camera.targetTexture = null;
                        renderTexture.Release();
                        return data;
                    }

                    void blend(Color32[] color, Color32[] blend)
                    {
                        for (int i = 0; i < color.Length; i++)
                        {
                            if (blend[i].a == 255)
                            {
                                color[i] = blend[i];
                            }
                        }
                    }

                    Color32[] shrink(Color32[] overlay)
                    {
                        if (horizontalBlockCount == 1 && verticalBlockCount == 1)
                        {
                            return overlay;
                        }

                        Color32[] result = new Color32[baseLength * baseLength];

                        int _horizonLength0 = horizontalBlockCount * baseLength;
                        int _horizonLength1 = _horizonLength0 - baseLength;

                        int _verticalLength = _horizonLength0 * baseLength;

                        int cur = 0;
                        int curresult = 0;
                        int pointer = 0;
                        bool flag;
                        for (int cx = 0; cx < baseLength; cx++)
                        {
                            for (int cy = 0; cy < baseLength; cy++)
                            {
                                pointer = cur;
                                flag = false;
                                for (int i = 0; i < verticalBlockCount; i++)
                                {
                                    for (int j = 0; j < horizontalBlockCount; j++)
                                    {
                                        if (overlay[cur].a > result[curresult].a)
                                        {
                                            result[curresult] = overlay[pointer];

                                            if (result[curresult].a == 255)
                                            {
                                                flag = true;
                                                break;
                                            }
                                        }
                                        pointer += baseLength;
                                    }
                                    if (flag) break;
                                    pointer += _verticalLength;
                                }
                                ++cur;
                                ++curresult;
                            }
                            cur += _horizonLength1;
                        }
                        return result;
                    }

					Color32[] addAlpha(Color32[] colors, Color32[] alpha)
					{
                        for (int i = 0; i < colors.Length; i++)
                        {
                            colors[i].a = alpha[i].a;
                            if (colors[i].a == 0)
                            {
                                colors[i].r = 0;
                                colors[i].g = 0;
                                colors[i].b = 0;
                            }
                        }
                        return colors;
                    }
					
					void renderWithModifiedMesh(ref Color32[] overlay1, ref Color32[] overlay2, Camera _camera)
					{
                        if (!modifiedMesh)
                        {
                            modifiedMesh = true;
                            if (isSMR)
                            {
                                var _tmp = (SkinnedMeshRenderer)smr;
                                _tmp.BakeMesh(mesh);
                            }
                            else
                            {
                                Mesh.DestroyImmediate(mesh);
                                if (smr.gameObject.GetComponent<MeshFilter>().sharedMesh.isReadable)
                                {
                                    mesh = Mesh.Instantiate(smr.gameObject.GetComponent<MeshFilter>().sharedMesh);
                                }
                                else
                                {
                                    mesh = RestoreMesh(smr.gameObject.GetComponent<MeshFilter>().sharedMesh);
                                }
                            }

                            var uvs = mesh.uv;
                            if (uvs == null || uvs.Length == 0)
                            {
                                Console.WriteLine("No uv map, ignore");
                                return;
                            }

                            int xOffset;
                            int yOffset;

                            UnityEngine.Vector3[] verts = new UnityEngine.Vector3[uvs.Length];
                            // Transform mesh into a surface according to its uv
                            float minX = float.PositiveInfinity;
                            float minY = float.PositiveInfinity;
                            float maxX = float.NegativeInfinity;
                            float maxY = float.NegativeInfinity;
                            for (int uv = 0; uv < uvs.Length; uv++)
                            {
                                verts[uv] = new UnityEngine.Vector3(-uvs[uv].x, uvs[uv].y, 0f);
                                minX = Mathf.Min(minX, uvs[uv].x);
                                maxX = Mathf.Max(maxX, uvs[uv].x);
                                minY = Mathf.Min(minY, uvs[uv].y);
                                maxY = Mathf.Max(maxY, uvs[uv].y);
                            }

                            xOffset = ((int)Math.Floor(minX));
                            yOffset = ((int)Math.Floor(minY));
                            horizontalBlockCount = (int)Math.Ceiling(maxX) - xOffset;
                            verticalBlockCount = (int)Math.Ceiling(maxY) - yOffset;

                            if (horizontalBlockCount == 0 || verticalBlockCount == 0)
                            {
                                horizontalBlockCount = 1;
                                verticalBlockCount = 1;
                                var v0 = verts[mesh.triangles[0]];
                                verts[mesh.triangles[1]].x = v0.x + 1;
                                verts[mesh.triangles[2]].y = v0.y + 1;
                            }

                            // Correct triangle if its normal direction is not positive z
                            var triangles = mesh.triangles;
                            for (int tri = 0; tri < triangles.Length; tri += 3)
                            {
                                var v0 = verts[triangles[tri]];
                                var v1 = verts[triangles[tri + 1]];
                                var v2 = verts[triangles[tri + 2]];

                                if (UnityEngine.Vector3.Cross(v1 - v0, v2 - v0).z < 0)
                                {
                                    var tmp = triangles[tri + 1];
                                    triangles[tri + 1] = triangles[tri + 2];
                                    triangles[tri + 2] = tmp;
                                }
                            }

                            uvIslandSolver(triangles, verts);

                            mesh.vertices = verts;
                            mesh.triangles = triangles;
                            mesh.RecalculateBounds();
                            mesh.RecalculateNormals();
                            mesh.RecalculateTangents();

                            positionFront = new UnityEngine.Vector3(-xOffset - horizontalBlockCount / 2.0f, yOffset + verticalBlockCount / 2.0f, 10f);
                            positionLookAt = new UnityEngine.Vector3(positionFront.x, positionFront.y, 0f);
                        }

                        _camera.orthographicSize = verticalBlockCount / 2.0f;
                        _camera.aspect = (float)horizontalBlockCount / verticalBlockCount;
                        _camera.transform.position = positionFront;
                        _camera.transform.LookAt(positionLookAt);
                        _camera.transform.hasChanged = true;
                        cameraMain.transform.hasChanged = true;
                        cameraMain.transform.position = positionFront;
                        cameraMain.transform.LookAt(positionLookAt);

                        int texturewidth = baseLength * horizontalBlockCount;
                        int textureheight = baseLength * verticalBlockCount;

                        Texture2D _overlay;
                        bool overlayCustomSize = false;
                        if (texturewidth == textureheight)
                        {
                            switch (texturewidth)
                            {
                                case 512:
                                    _overlay = TextureSaver.GetTexture2D(0);
                                    break;
                                case 1024:
                                    _overlay = TextureSaver.GetTexture2D(1);
                                    break;
                                case 2048:
                                    _overlay = TextureSaver.GetTexture2D(2);
                                    break;
                                case 4096:
                                    _overlay = TextureSaver.GetTexture2D(3);
                                    break;
                                default:
                                    _overlay = new Texture2D(texturewidth, textureheight, TextureFormat.ARGB32, false);
                                    overlayCustomSize = true;
                                    break;
                            }
                        }
                        else
                        {
                            _overlay = new Texture2D(texturewidth, textureheight, TextureFormat.ARGB32, false);
                            overlayCustomSize = true;
                        }

                        overlay1 = render(lightRotation, lightPosition, submeshIndex, mesh, texturewidth, textureheight, _overlay, _camera);
                        if (overlay2 != null)
						{
                            overlay2 = render(darkRotation, lightPosition, submeshIndex, mesh, texturewidth, textureheight, _overlay, _camera);
                        }

                        if (overlayCustomSize)
                        {
                            Texture2D.DestroyImmediate(_overlay);
                        }

                        overlay1 = shrink(overlay1);
                        if (overlay2 != null)
						{
                            overlay2 = shrink(overlay2);
                        }

                        if (material.HasProperty("_Lip_line_texture"))
                        {
                            material.SetTexture("_Lip_line_texture", null);
                        }
                        if (material.HasProperty("_Nose_texture"))
                        {
                            material.SetTexture("_Nose_texture", null);
                        }
                        if (material.HasProperty("_Nip"))
                        {
                            material.SetTexture("_Nip", null);
                        }
                        if (material.HasProperty("_Under_hair_texture"))
                        {
                            material.SetTexture("_Under_hair_texture", null);
                        }
                    }
					

				}
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (material != null)
                    {
                        Material.Destroy(material);
                    }
                }
            }
            
        }
		
		for(int i = 0;i < renders.Length; i++)
		{
			renders[i].enabled = renderStatus[i];
        }

        if (mesh != null)
        {
            Mesh.DestroyImmediate(mesh);
        }

        void uvIslandSolver(int[] triangles, UnityEngine.Vector3[] vertices)
        {
            int[] parent = new int[vertices.Length];
            float[] minX = new float[vertices.Length];
            float[] minY = new float[vertices.Length];
            float[] maxX = new float[vertices.Length];
            float[] maxY = new float[vertices.Length];

            int findAndUpdate(int index)
            {
                Stack<int> stack = new Stack<int>();
                while (parent[index] != index)
                {
                    stack.Push(index);
                    index = parent[index];
                }
                while (stack.Count > 0)
                {
                    int cur = stack.Pop();
                    parent[cur] = index;
                }
                return index;
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                parent[i] = i;
                var vertex = vertices[i];
                minX[i] = vertex.x;
                maxX[i] = vertex.x;
                minY[i] = vertex.y;
                maxY[i] = vertex.y;
            }

            for (int i = 0; i < triangles.Length; i += 3)
            {
                var parentIndex = findAndUpdate(parent[triangles[i]]);
                var _parentIndex1 = findAndUpdate(parent[triangles[i + 1]]);
                var _parentIndex2 = findAndUpdate(parent[triangles[i + 2]]);

                parent[_parentIndex1] = parentIndex;
                parent[_parentIndex2] = parentIndex;

                minX[parentIndex] = Math.Min(minX[parentIndex], minX[_parentIndex1]);
                minX[parentIndex] = Math.Min(minX[parentIndex], minX[_parentIndex2]);

                minY[parentIndex] = Math.Min(minY[parentIndex], minY[_parentIndex1]);
                minY[parentIndex] = Math.Min(minY[parentIndex], minY[_parentIndex2]);

                maxX[parentIndex] = Math.Max(maxX[parentIndex], maxX[_parentIndex1]);
                maxX[parentIndex] = Math.Max(maxX[parentIndex], maxX[_parentIndex2]);

                maxY[parentIndex] = Math.Max(maxY[parentIndex], maxY[_parentIndex1]);
                maxY[parentIndex] = Math.Max(maxY[parentIndex], maxY[_parentIndex2]);
            }
            List<List<int>> islands = new List<List<int>>();
            Dictionary<int, float> offsets = new Dictionary<int, float>();

            for (int i = 0; i < vertices.Length; i++)
            {
                int parentIndex = findAndUpdate(parent[i]);
                if (offsets.TryGetValue(parentIndex, out float offset))
                {
                    var _v = vertices[i];
                    vertices[i] = new UnityEngine.Vector3(_v.x, _v.y, offset);
                    continue;
                }
                offset = 0f;
                bool flag = false;

                for (int layer = 0; layer < islands.Count; layer++)
                {
                    flag = true;
                    var layerInfo = islands[layer];
                    for (int j = 0; j < islands[layer].Count; j++)
                    {
                        int _parentIndex = layerInfo[j];
                        if (!(minX[_parentIndex] >= maxX[parentIndex] || maxX[_parentIndex] <= minX[parentIndex] || minY[_parentIndex] >= maxY[parentIndex] || maxY[_parentIndex] <= minY[parentIndex]))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        offsets.Add(parentIndex, offset);
                        layerInfo.Add(parentIndex);
                        var _v = vertices[i];
                        vertices[i] = new UnityEngine.Vector3(_v.x, _v.y, offset);
                        break;
                    }
                    offset -= 0.0001f;
                }
                if (!flag)
                {
                    offsets.Add(parentIndex, offset);
                    islands.Add(new List<int>() { parentIndex });
                }
            }
        }
    }
    public static void ChangeAnimations()
	{
		Human human = ACExporterPlugin.selectedChara;
		HumanFace face = human.face;
		face.ChangeEyesBlinkFlag(false);
		face.ChangeEyesShaking(false);
		face.ChangeLookEyesTarget(1, null, 0f);
		face.ChangeEyesPtn(1, false);
		face.ChangeEyesPtn(0, false);
		face.ChangeMouthPtn(1, false);
		face.ChangeMouthPtn(0, false);
		face.ChangeEyebrowPtn(1, false);
		face.ChangeEyebrowPtn(0, false);
		face.ChangeEyesOpenMax(1);
		face.ChangeMouthOpenMax(0);
		
		human.body.animBody.speed = 0f;

		// Need a T-Pose mod
		human.body.animBody.Play(human.body.animBody.runtimeAnimatorController.animationClips[0].name);
	}

    public void CreateBaseSavePath()
    {
        //Use the card's name in the save path, but remove any invalid characters to prevent save issues
        //string characterName = Singleton<CustomBase>.Instance.chaCtrl.chaFile.parameter.fullname.Replace(" ", string.Empty);
        //characterName = string.Join("_", characterName.Split(Path.GetInvalidFileNameChars()));
        string exportFolder = ACExporterPlugin.basePath;
        savePath = Path.Combine(Application.dataPath, $"../{exportFolder}/{DateTime.Now:yyyyMMddHHmmss}_{charaName}/");
        Directory.CreateDirectory(savePath);
    }

    private void ResetPmxBuilder()
	{
		currentMaterialList.Clear();
		currentRendererMaterialMapping.Clear();
		List<PmxBone> boneList = new List<PmxBone>();
		Dictionary<string, Pmx.BackupBoneData> boneBackupData = new Dictionary<string, Pmx.BackupBoneData>();
		if (pmxFile != null)
		{
			boneList = pmxFile.BoneList;
			boneBackupData = pmxFile.BoneBackupData;
		}
		pmxFile = new Pmx
		{
			BoneList = boneList,
			BoneBackupData = boneBackupData
		};
		vertexCount = 0;
		vertexCountRecord.Clear();
        meshRenders.Clear();
        smrMaterialsCache.Clear();
    }

	public void CreateModelInfo()
	{
		PmxModelInfo pmxModelInfo = new PmxModelInfo
		{
			ModelName = "Aicomi",
			ModelNameE = "",
			Comment = "Exported Aicomi"
		};
		pmxModelInfo.Comment = "";
		pmxFile.ModelInfo = pmxModelInfo;
	}

	public void CreateInstanceIDs()
	{
		instanceIDs.Clear();
		Transform[] componentsInChildren = GameObject.Find("BodyTop").transform.GetComponentsInChildren<Transform>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (!(componentsInChildren[i].gameObject == null))
			{
				instanceIDs.Add(componentsInChildren[i].gameObject.GetInstanceID(), instanceIDs.Count);
			}
		}
		Component[] componentsInChildren2 = GameObject.Find("BodyTop").transform.GetComponentsInChildren<Component>(includeInactive: true);
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			if (!(componentsInChildren2[j] == null))
			{
				instanceIDs.Add(componentsInChildren2[j].GetInstanceID(), instanceIDs.Count);
			}
		}
	}

	public void CollectIgnoreList()
	{
		Human human = ACExporterPlugin.selectedChara;
		
		Renderer[] componentsInChildren = human.face.objHead.GetComponentsInChildren<Renderer>(includeInactive: true);
		foreach (Renderer renderer in componentsInChildren)
		{
			if (!ignoreList.Contains(renderer.name, StringComparer.Ordinal))
			{
				ignoreList.Add(renderer.name);
			}
			//Material[] materials = renderer.materials;
			//for (int j = 0; j < materials.Length; j++)
			//{
			//	string text = CleanUpName(materials[j].name);
			//	if (!ignoreList.Contains(text, StringComparer.Ordinal))
			//	{
			//		ignoreList.Add(text);
			//	}
			//}
		}
		var rendEye = human.face.rendEye;
		if (rendEye[0] != null && rendEye[0].material != null)
		{
			EyeMatName = CleanUpName(rendEye[0].material.name);
		}
	}

	public void CreateMeshList()
	{
		//string[] source = new string[0];
		//string[] source2 = new string[] { "cf_O_namida_L", "cf_O_namida_M", "cf_O_namida_S", "cf_O_gag_eye_00", "o_tang" };
		string[] source3 = new string[8] { "o_mnpa", "o_mnpb", "n_tang", "n_tang_silhouette", "o_dankon", "o_gomu", "o_dan_f", "cf_O_canine" };
		//string[] source4 = new string[23]
		//{
		//	"o_hit_armL", "o_hit_armR", "o_hit_footL", "o_hit_footR", "o_hit_handL", "o_hit_handR", "o_hit_hara", "o_hit_haraB", "o_hit_haraUnder", "o_hit_kneeBL",
		//	"o_hit_kneeBR", "o_hit_kneeL", "o_hit_kneeR", "o_hit_kokan", "o_hit_legBL", "o_hit_legBR", "o_hit_legL", "o_hit_legR", "o_hit_mune", "o_hit_muneB",
		//	"o_hit_siriL", "o_hit_siriR", "cf_O_face_atari"
		//};
		SkinnedMeshRenderer[] componentsInChildren = GameObject.Find("BodyTop").transform.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive:true);
        Mesh mesh = new Mesh();
        int total = 0;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (
				(
					nowCoordinate < maxCoord
					&&
					(
						(!componentsInChildren[i].enabled || !componentsInChildren[i].isVisible)
						||
						ignoreList.Contains(componentsInChildren[i].name, StringComparer.Ordinal)
						||
						componentsInChildren[i].sharedMaterials.Length == 0
					)
				)
				||
				(
					nowCoordinate == maxCoord
					&&
					!ignoreList.Contains(componentsInChildren[i].name, StringComparer.Ordinal)
				)
				||
				source3.Contains(componentsInChildren[i].name, StringComparer.Ordinal)
				||
				ignoredShaders.Contains(componentsInChildren[i].sharedMaterial?.shader.name)

            )
			{
				continue;
			}
			
			meshRenders.Add(componentsInChildren[i]);
            Console.WriteLine("Exporting: " + componentsInChildren[i].name);
			SMRData sMRData = new SMRData(this, componentsInChildren[i]);
			AddToSMRDataList(sMRData);
			smrMaterialsCache.Add(sMRData.SMRPath, sMRData.SMRMaterialNames);
			MaterialDataComplete matData = new MaterialDataComplete(this, componentsInChildren[i]);
			AddToMaterialDataCompleteList(matData);
			if (currentRendererMaterialMapping.ContainsKey(componentsInChildren[i]))
			{
				Console.WriteLine("Issue - Renderer already added to Material name cache: " + sMRData.SMRName);
			}
			else
			{
				currentRendererMaterialMapping.Add(componentsInChildren[i], sMRData.SMRMaterialNames);
			}

			
            componentsInChildren[i].BakeMesh(mesh);

			if (nowCoordinate == maxCoord)
			{
				vertexCountRecord.Add(componentsInChildren[i].name + componentsInChildren[i].sharedMaterial.name, total);
				total += mesh.vertexCount;
            }
			//_ = componentsInChildren[i].gameObject;
			BoneWeight[] boneWeights = componentsInChildren[i].sharedMesh.boneWeights;
			Transform transform = componentsInChildren[i].gameObject.transform;
			int bone = sbi(GetAltBoneName(transform), transform.GetInstanceID().ToString());
			UnityEngine.Vector2[] uv = mesh.uv;
			List<UnityEngine.Vector2[]> list = new List<UnityEngine.Vector2[]> { mesh.uv2, mesh.uv3, mesh.uv4 };
			//_ = mesh.colors;
			UnityEngine.Vector3[] normals = mesh.normals;
			UnityEngine.Vector3[] vertices = mesh.vertices;
			int vertexOffset = pmxFile.VertexList.Count;
            for (int j = 0; j < mesh.subMeshCount; j++)
			{
				int[] triangles = mesh.GetTriangles(j);
				AddFaceList(triangles, vertexCount);
				if (j < componentsInChildren[i].sharedMaterials.Length)
				{
					CreateMaterial(componentsInChildren[i].sharedMaterials[j], sMRData.SMRMaterialNames[j], triangles.Length);
				}
				else if (componentsInChildren[i].sharedMaterial != null)
				{
					CreateMaterial(componentsInChildren[i].sharedMaterial, sMRData.SMRMaterialNames[0], triangles.Length);
				}
			}
			if (string.CompareOrdinal(componentsInChildren[i].name, "cf_O_eyeline") == 0)
			{
				int[] triangles2 = mesh.GetTriangles(0);
				AddFaceList(triangles2, vertexCount);
				for (int k = 1; k < 2; k++)
				{
					CreateMaterial(componentsInChildren[i].sharedMaterials[k], sMRData.SMRMaterialNames[k], triangles2.Length);
				}
			}
			vertexCount += mesh.vertexCount;
			for (int l = 0; l < mesh.vertexCount; l++)
			{
				PmxVertex pmxVertex = new PmxVertex
				{
					UV = new PmxLib.Vector2(uv[l].x, (float)((double)(0f - uv[l].y) + 1.0))
				};
				for (int m = 0; m < list.Count; m++)
				{
					if (list[m].Length != 0)
					{
						pmxVertex.UVA[m] = new PmxLib.Vector4(list[m][l].x, (float)((double)(0f - list[m][l].y) + 1.0), 0f, 0f);
					}
				}
				if (boneWeights.Length != 0)
				{
					pmxVertex.Weight = ConvertBoneWeight(boneWeights[l], componentsInChildren[i].bones);
				}
				else
				{
					pmxVertex.Weight = new PmxVertex.BoneWeight[4];
					pmxVertex.Weight[0].Bone = bone;
					pmxVertex.Weight[0].Value = 1f;
				}
				UnityEngine.Vector3 vector = componentsInChildren[i].transform.TransformDirection(normals[l]);
				pmxVertex.Normal = new PmxLib.Vector3(0f - vector.x, vector.y, 0f - vector.z);
                UnityEngine.Vector3 vector2 = componentsInChildren[i].transform.TransformPointUnscaled(vertices[l]);
                //UnityEngine.Vector3 vector2 = componentsInChildren[i].transform.TransformPoint(vertices[l]);
                pmxVertex.Position = new PmxLib.Vector3((0f - vector2.x) * (float)scale, vector2.y * (float)scale, (0f - vector2.z) * (float)scale);
				pmxVertex.Deform = PmxVertex.DeformType.BDEF4;
				pmxFile.VertexList.Add(pmxVertex);
			}
			AddMorph(componentsInChildren[i], vertexOffset, componentsInChildren[i].sharedMesh);
        }
		Mesh.DestroyImmediate(mesh);
	}

    private PmxVertex.BoneWeight[] ConvertBoneWeight(BoneWeight unityWeight, Transform[] bones)
	{
		PmxVertex.BoneWeight[] array = new PmxVertex.BoneWeight[4];
		try
		{
			if (unityWeight.boneIndex0 >= 0 && unityWeight.boneIndex0 < bones.Length)
			{
				array[0].Bone = sbi(GetAltBoneName(bones[unityWeight.boneIndex0]), bones[unityWeight.boneIndex0].GetInstanceID().ToString());
			}
			array[0].Value = unityWeight.weight0;
			if (unityWeight.boneIndex1 >= 0 && unityWeight.boneIndex0 < bones.Length)
			{
				array[1].Bone = sbi(GetAltBoneName(bones[unityWeight.boneIndex1]), bones[unityWeight.boneIndex1].GetInstanceID().ToString());
			}
			array[1].Value = unityWeight.weight1;
			if (unityWeight.boneIndex2 >= 0 && unityWeight.boneIndex0 < bones.Length)
			{
				array[2].Bone = sbi(GetAltBoneName(bones[unityWeight.boneIndex2]), bones[unityWeight.boneIndex2].GetInstanceID().ToString());
			}
			array[2].Value = unityWeight.weight2;
			if (unityWeight.boneIndex3 >= 0 && unityWeight.boneIndex0 < bones.Length)
			{
				array[3].Bone = sbi(GetAltBoneName(bones[unityWeight.boneIndex3]), bones[unityWeight.boneIndex3].GetInstanceID().ToString());
			}
			array[3].Value = unityWeight.weight3;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
		return array;
	}

	private void ClearMorphs()
	{
        Human human = ACExporterPlugin.selectedChara;

		var fBSTarget = human.face.eyesCtrl.FBSTarget;
		for (int i = 0; i < fBSTarget.Length; i++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = fBSTarget[i].GetSkinnedMeshRenderer();
			int blendShapeCount = skinnedMeshRenderer.sharedMesh.blendShapeCount;
			for (int j = 0; j < blendShapeCount; j++)
			{
                skinnedMeshRenderer.SetBlendShapeWeight(j, 0f);
            }
		}
		fBSTarget = human.face.mouthCtrl.FBSTarget;
		for (int i = 0; i < fBSTarget.Length; i++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer2 = fBSTarget[i].GetSkinnedMeshRenderer();
			int blendShapeCount2 = skinnedMeshRenderer2.sharedMesh.blendShapeCount;
			for (int k = 0; k < blendShapeCount2; k++)
			{
				skinnedMeshRenderer2.SetBlendShapeWeight(k, 0f);
			}
		}
		fBSTarget = human.face.eyebrowCtrl.FBSTarget;
		for (int i = 0; i < fBSTarget.Length; i++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer3 = fBSTarget[i].GetSkinnedMeshRenderer();
			int blendShapeCount3 = skinnedMeshRenderer3.sharedMesh.blendShapeCount;
			for (int l = 0; l < blendShapeCount3; l++)
			{
				skinnedMeshRenderer3.SetBlendShapeWeight(l, 0f);
			}
		}
	}

	private void AddMorph(SkinnedMeshRenderer smr, int vertexCountOffset, Mesh mesh)
	{
        string[] ignoredSMRs = { "cf_O_gag_eye_00", "cf_O_gag_eye_01", "cf_O_gag_eye_02", "cf_O_namida_L", "cf_O_namida_M", "cf_O_namida_S", "Highlight_o_body_a_rend", "Highlight_cf_O_face_rend", "o_Mask" };
		if (ignoredSMRs.Contains(smr.name, StringComparer.Ordinal))
		{
			return;
		}
        Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<UnityEngine.Vector3> array = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<UnityEngine.Vector3>(mesh.vertexCount);
        Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<UnityEngine.Vector3> _deltaNormals = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<UnityEngine.Vector3>(mesh.vertexCount);
        Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<UnityEngine.Vector3> _deltaTangents = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<UnityEngine.Vector3>(mesh.vertexCount);
        Matrix4x4[] bindposes = mesh.bindposes;
        BoneWeight[] boneWeights = mesh.boneWeights;
        Matrix4x4 basisInverse = Matrix4x4.TRS(smr.transform.position, smr.transform.rotation, UnityEngine.Vector3.one).inverse;
        Transform[] bones = smr.bones;
        for (int i = 0; i < mesh.blendShapeCount; i++)
        {
            for (int j = 0; j < mesh.GetBlendShapeFrameCount(i); j++)
            {
                string name = mesh.GetBlendShapeName(i);
                if (j > 0)
                {
                    name += (" --" + j + 1);
                }
                PmxMorph morph = new PmxMorph()
                {
                    Name = name,
                    NameE = "",
                    Panel = 1,
                    Kind = PmxMorph.OffsetKind.Vertex
                };

                mesh.GetBlendShapeFrameVertices(i, j, array, _deltaNormals, _deltaTangents);
                for (int k = 0; k < array.Length; k++)
                {
                    var v = array[k];
                    BoneWeight _bw = boneWeights[k];
                    UnityEngine.Vector3 correct = UnityEngine.Vector3.zero;
                    for (int m = 0; m < 4; m++)
                    {
                        GetBoneWeightInfo(_bw, m, out var _bone, out var _weight);
                        if (_weight <= 0 || _bone < 0)
                        {
                            continue;
                        }
                        correct += _weight * (basisInverse * bones[_bone].localToWorldMatrix * bindposes[_bone]).MultiplyVector(v);
                    }
                    PmxVertexMorph pvm = new PmxVertexMorph(vertexCountOffset + k, new PmxLib.Vector3(-correct.x, correct.y, -correct.z));
                    morph.OffsetList.Add(pvm);
                }
                pmxFile.MorphList.Add(morph);
            }
        }

    }
    private static void GetBoneWeightInfo(BoneWeight bw, int index, out int bone, out float weight)
    {
        switch (index)
        {
            case 0:
                bone = bw.boneIndex0;
                weight = bw.weight0;
                return;
            case 1:
                bone = bw.boneIndex1;
                weight = bw.weight1;
                return;
            case 2:
                bone = bw.boneIndex2;
                weight = bw.weight2;
                return;
            case 3:
                bone = bw.boneIndex3;
                weight = bw.weight3;
                return;
            default:
                throw new IndexOutOfRangeException();
        }
    }

	public void CreateMaterial(Material material, string matName, int count)
	{
		PmxMaterial pmxMaterial = new PmxMaterial
		{
			Name = matName,
			NameE = matName,
			Flags = (PmxMaterial.MaterialFlags.DrawBoth | PmxMaterial.MaterialFlags.Shadow | PmxMaterial.MaterialFlags.SelfShadowMap | PmxMaterial.MaterialFlags.SelfShadow)
		};
		pmxMaterial.FaceCount = count;
		pmxFile.MaterialList.Add(pmxMaterial);
	}

	private void AddFaceList(int[] faceList, int count)
	{
		for (int i = 0; i < faceList.Length; i++)
		{
			faceList[i] += count;
			pmxFile.FaceList.Add(faceList[i]);
		}
	}

	private void AddAccessory()
	{
		MeshFilter[] componentsInChildren = GameObject.Find("BodyTop").transform.GetComponentsInChildren<MeshFilter>(includeInactive: true);

        for (int i = 0; i < componentsInChildren.Length; i++)
		{
			MeshRenderer meshRenderer = componentsInChildren[i].gameObject.GetComponent<MeshRenderer>();
			if (
				!meshRenderer.enabled 
				|| 
				(
					nowCoordinate < maxCoord 
					&& 
					ignoreList.Contains(meshRenderer.name, StringComparer.Ordinal) 
					//&& 
					//meshRenderer.sharedMaterials.Length > 0 
					//&& 
					//ignoreList.Contains(CleanUpName(meshRenderer.sharedMaterial.name), StringComparer.Ordinal)
				) 
				|| 
				(
					nowCoordinate == maxCoord 
					&& 
					!ignoreList.Contains(meshRenderer.name, StringComparer.Ordinal) 
					//&& 
					//meshRenderer.sharedMaterials.Length > 0 
					//&& 
					//!ignoreList.Contains(CleanUpName(meshRenderer.sharedMaterial.name), StringComparer.Ordinal)
				)
			)
			{
				continue;
			}
            meshRenders.Add(meshRenderer);
			Console.WriteLine("Exporting Acc: " + meshRenderer.name);
			SMRData sMRData = new SMRData(this, meshRenderer);
            AddToSMRDataList(sMRData);
            smrMaterialsCache.Add(sMRData.SMRPath, sMRData.SMRMaterialNames);
            MaterialDataComplete matData = new MaterialDataComplete(this, meshRenderer);
			AddToMaterialDataCompleteList(matData);
			if (currentRendererMaterialMapping.ContainsKey(meshRenderer))
			{
				Console.WriteLine("Issue - Renderer already added to Material name cache: " + sMRData.SMRName);
			}
			else
			{
				currentRendererMaterialMapping.Add(meshRenderer, sMRData.SMRMaterialNames);
			}
			GameObject gameObject = componentsInChildren[i].gameObject;
            Transform transform = componentsInChildren[i].gameObject.transform;
            int bone = sbi(GetAltBoneName(transform), transform.GetInstanceID().ToString());

			Mesh mesh = componentsInChildren[i].sharedMesh;
			if (!mesh.isReadable)
			{
				mesh = RestoreMesh(mesh);
			}

            UnityEngine.Vector2[] uv = mesh.uv;
            List<UnityEngine.Vector2[]> list = new List<UnityEngine.Vector2[]> { mesh.uv2, mesh.uv3, mesh.uv4 };
            UnityEngine.Vector3[] normals = mesh.normals;
            UnityEngine.Vector3[] vertices = mesh.vertices;
            for (int j = 0; j < mesh.subMeshCount; j++)
            {
                int[] triangles = mesh.GetTriangles(j);
                AddFaceList(triangles, vertexCount);
                CreateMaterial(meshRenderer.sharedMaterials[j], sMRData.SMRMaterialNames[j], triangles.Length);
            }
            vertexCount += mesh.vertexCount;
            bool uv_error_flag = false;
            for (int k = 0; k < mesh.vertexCount; k++)
            {
                PmxVertex pmxVertex = new PmxVertex();
                try
                {
                    pmxVertex.UV = new PmxLib.Vector2(uv[k].x, (float)((double)(0f - uv[k].y) + 1.0));
                    pmxVertex.Weight = new PmxVertex.BoneWeight[4];
                }
                catch
                {
                    if (uv_error_flag == false)
                    {
                        Console.WriteLine("Issue - Object did not have a UV map: " + meshRenderer.name);
                    }
                    uv_error_flag = true;
                    pmxVertex.UV = new PmxLib.Vector2();
                    pmxVertex.Weight = new PmxVertex.BoneWeight[4];
                }
                pmxVertex.Weight[0].Bone = bone;
                pmxVertex.Weight[0].Value = 1f;
                for (int l = 0; l < list.Count; l++)
                {
                    if (list[l].Length != 0)
                    {
                        pmxVertex.UVA[l] = new PmxLib.Vector4(list[l][k].x, (float)((double)(0f - list[l][k].y) + 1.0), 0f, 0f);
                    }
                }
                UnityEngine.Vector3 vector = gameObject.transform.TransformDirection(normals[k]);
                pmxVertex.Normal = new PmxLib.Vector3(0f - vector.x, vector.y, 0f - vector.z);
                UnityEngine.Vector3 vector2 = gameObject.transform.TransformPoint(vertices[k]);
                pmxVertex.Position = new PmxLib.Vector3((0f - vector2.x) * (float)scale, vector2.y * (float)scale, (0f - vector2.z) * (float)scale);
                pmxVertex.Deform = PmxVertex.DeformType.BDEF4;
                pmxFile.VertexList.Add(pmxVertex);
            }
            
        }
        //Mesh.DestroyImmediate(mesh);
    }
    private Mesh RestoreMesh(Mesh originalMesh)
    {
        Mesh mesh = new Mesh();
        UnityEngine.Vector3[] vertices = new UnityEngine.Vector3[originalMesh.vertexCount];
        UnityEngine.Vector3[] normals = new UnityEngine.Vector3[originalMesh.vertexCount];
        UnityEngine.Vector2[] uvs = new UnityEngine.Vector2[originalMesh.vertexCount];
        Color[] colors = new Color[originalMesh.vertexCount];
        HashSet<uint> builtVertices = new HashSet<uint>();
        SubMeshDescriptor[] subs = new SubMeshDescriptor[originalMesh.subMeshCount];
        int indicesCount = 0;
        int indexCount = 0;
        for (int _sub = 0; _sub < originalMesh.subMeshCount; _sub++)
        {
            indicesCount += originalMesh.GetSubMesh(_sub).indexCount;
        }
        int[] triangles = new int[indicesCount];

        for (int _sub = 0; _sub < originalMesh.subMeshCount; _sub++)
        {
            SubMeshDescriptor smd = originalMesh.GetSubMesh(_sub);
            int indexStart = indexCount;
            uint[] pixels = FetchGPUData(originalMesh, _sub, 0);
            for (int pointer = 0; pointer < pixels.Length && (pixels[pointer] != 0x01010100);)
            {
                uint vertexID = pixels[pointer++];
                if (builtVertices.Contains(vertexID))
                {
                    pointer += 15;
                }
                else
                {
                    vertices[vertexID] = new UnityEngine.Vector3(UnpackFloatFromUInt(pixels[pointer++]), UnpackFloatFromUInt(pixels[pointer++]), UnpackFloatFromUInt(pixels[pointer++]));
                    normals[vertexID] = new UnityEngine.Vector3(UnpackFloatFromUInt(pixels[pointer++]), UnpackFloatFromUInt(pixels[pointer++]), UnpackFloatFromUInt(pixels[pointer++]));
                    uvs[vertexID] = new UnityEngine.Vector2(UnpackFloatFromUInt(pixels[pointer++]), UnpackFloatFromUInt(pixels[pointer++]));
                    colors[vertexID] = new Color(UnpackFloatFromUInt(pixels[pointer++]), UnpackFloatFromUInt(pixels[pointer++]), UnpackFloatFromUInt(pixels[pointer++]), UnpackFloatFromUInt(pixels[pointer++]));
                    builtVertices.Add(vertexID);
                    pointer += 3;
                }
                triangles[indexCount++] = (int)vertexID;
            }
			SubMeshDescriptor subMeshDescriptor = new SubMeshDescriptor();
			subMeshDescriptor._indexStart_k__BackingField = indexStart;
			subMeshDescriptor._indexCount_k__BackingField = indexCount - indexStart;
            subs[_sub] = subMeshDescriptor;
        }
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.SetIndexBufferParams(triangles.Length, IndexFormat.UInt32);
		//Il2CppStructArray<int> il2cppArray = triangles;
		var il2cpparray = Il2CppSystem.Array.CreateInstance(Il2CppSystem.Type.GetType("System.Int32"), triangles.Length);
		for(int i = 0; i < triangles.Length; i++)
		{
			il2cpparray.InternalArray__set_Item(i, triangles[i]);
        }
        mesh.InternalSetIndexBufferDataFromArray(il2cpparray, 0, 0, triangles.Length, sizeof(int), MeshUpdateFlags.DontValidateIndices);
        if (originalMesh.HasVertexAttribute(VertexAttribute.TexCoord0))
        {
            mesh.uv = uvs;
        }
        else
        {
            GenerateUV(mesh);
        }
        if (originalMesh.HasVertexAttribute(VertexAttribute.Color))
        {
            mesh.colors = colors;
        }

        int uvMask = (originalMesh.HasVertexAttribute(VertexAttribute.TexCoord1) ? 0x1 : 0) |
            (originalMesh.HasVertexAttribute(VertexAttribute.TexCoord2) ? 0x2 : 0) |
            (originalMesh.HasVertexAttribute(VertexAttribute.TexCoord3) ? 0x4 : 0) |
            (originalMesh.HasVertexAttribute(VertexAttribute.TexCoord4) ? 0x8 : 0) |
            (originalMesh.HasVertexAttribute(VertexAttribute.TexCoord5) ? 0x10 : 0) |
            (originalMesh.HasVertexAttribute(VertexAttribute.TexCoord6) ? 0x20 : 0) |
            (originalMesh.HasVertexAttribute(VertexAttribute.TexCoord7) ? 0x40 : 0);

        RestoreAdditionalUVs(originalMesh, mesh, (uint)uvMask);
        mesh.subMeshCount = originalMesh.subMeshCount;

        for (int i = 0; i < subs.Length; i++)
        {
            mesh.SetSubMesh(i, subs[i], MeshUpdateFlags.DontValidateIndices);
        }
        mesh.RecalculateBounds();
        if (!originalMesh.HasVertexAttribute(VertexAttribute.Normal))
        {
            mesh.RecalculateNormals();
        }
        mesh.RecalculateTangents();
        return mesh;
    }
    private static void GenerateUV(Mesh mesh)
    {
        float minX = mesh.bounds.min.x;
        float minY = mesh.bounds.min.y;
        float minZ = mesh.bounds.min.z;
        float sizeX = mesh.bounds.size.x;
        float sizeY = mesh.bounds.size.y;
        float sizeZ = mesh.bounds.size.z;
        Func<UnityEngine.Vector3, UnityEngine.Vector2> lambda;

        if (sizeZ <= sizeX && sizeZ <= sizeY)
        {
            lambda = vec => new UnityEngine.Vector2((vec.x - minX) / sizeX, (vec.y - minY) / sizeY);
        }
        else if (sizeX <= sizeY && sizeX <= sizeZ)
        {
            lambda = vec => new UnityEngine.Vector2((vec.y - minY) / sizeY, (vec.z - minZ) / sizeZ);
        }
        else
        {
            lambda = vec => new UnityEngine.Vector2((vec.x - minX) / sizeX, (vec.z - minZ) / sizeZ);
        }
        UnityEngine.Vector2[] uvs = new UnityEngine.Vector2[mesh.vertexCount];
        var vertices = mesh.vertices;
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            uvs[i] = lambda(vertices[i]);
        }
        mesh.uv = uvs;
    }
    private static float UnpackFloatFromUInt(uint v)
    {
        return System.BitConverter.ToSingle(BitConverter.GetBytes(v), 0);
    }
    private void RestoreAdditionalUVs(Mesh originalMesh, Mesh targetMesh, uint uvMask)
    {
        UnityEngine.Vector4[][] uvs = new UnityEngine.Vector4[3][];
        uvs[0] = new UnityEngine.Vector4[targetMesh.vertexCount];
        uvs[1] = new UnityEngine.Vector4[targetMesh.vertexCount];
        uvs[2] = new UnityEngine.Vector4[targetMesh.vertexCount];

        while (uvMask > 0)
        {
            uint _uvMask = 0;
            for (int subMeshIndex = 0; subMeshIndex < targetMesh.subMeshCount; subMeshIndex++)
            {
                _uvMask = uvMask;
                int pointer = 0;
                int count = 0;
                uint[] pixels = FetchGPUData(originalMesh, subMeshIndex, 1);
                int i = 0;
                while (pixels[i] == 0x01010100)
                {
                    i += 16;
                }
                for (; i < pixels.Length && pixels[i] != 0x01010100;)
                {
                    uint vertexID = pixels[i++];
                    uvs[0][vertexID] = new UnityEngine.Vector4(UnpackFloatFromUInt(pixels[i++]), UnpackFloatFromUInt(pixels[i++]), UnpackFloatFromUInt(pixels[i++]), UnpackFloatFromUInt(pixels[i++]));
                    uvs[1][vertexID] = new UnityEngine.Vector4(UnpackFloatFromUInt(pixels[i++]), UnpackFloatFromUInt(pixels[i++]), UnpackFloatFromUInt(pixels[i++]), UnpackFloatFromUInt(pixels[i++]));
                    uvs[2][vertexID] = new UnityEngine.Vector4(UnpackFloatFromUInt(pixels[i++]), UnpackFloatFromUInt(pixels[i++]), UnpackFloatFromUInt(pixels[i++]), UnpackFloatFromUInt(pixels[i++]));
                    i += 3;
                }
                while (_uvMask > 0 && count < 3)
                {
                    if (((_uvMask >> pointer) & 0x1) != 0)
                    {
                        targetMesh.SetUVs(pointer + 1, uvs[count]);
                        count++;
                    }
                    _uvMask &= (uint)(0xFFFFFFFE << pointer);
                    pointer++;
                }
            }
            uvMask = _uvMask;
        }
    }
    private uint[] FetchGPUData(Mesh mesh, int subMeshIndex, int shaderPass)
    {
        Material mat = gameObjectMeshCopier.GetComponent<SkinnedMeshRenderer>().sharedMaterial;

        RenderTexture rd = RenderTexture.GetTemporary(2048, 2048, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
        Texture2D text = new Texture2D(2048, 2048, TextureFormat.RGBAHalf, false, true);

        rd.antiAliasing = 1;
        rd.filterMode = FilterMode.Point;
        rd.Create();
        mat.SetInt("_TextureWidth", 2048);
        mat.SetInt("_TextureHeight", 2048);
        CommandBuffer cmd = new CommandBuffer();
        cmd.SetRenderTarget(rd);
        cmd.ClearRenderTarget(true, true, Color.white);
        cmd.DrawMesh(mesh, Matrix4x4.identity, mat, subMeshIndex, shaderPass);
        Graphics.ExecuteCommandBuffer(cmd);

        RenderTexture tmp = RenderTexture.active;
        RenderTexture.active = rd;
        text.ReadPixels(new Rect(0, 0, 2048, 2048), 0, 0);
        text.Apply();
        RenderTexture.active = tmp;
        cmd.Release();
        RenderTexture.ReleaseTemporary(rd);
        Color[] colors = text.GetPixels();
        uint[] data = new uint[colors.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            data[i] = (uint)(colors[i].r * 256) | (uint)(colors[i].g * 256) << 8 | (uint)(colors[i].b * 256) << 16 | (uint)(colors[i].a * 256) << 24;
        }
        return data;
    }
    private void RestoreVertexColor(Mesh originalMesh, Mesh mesh)
    {
        Color[] colors = new Color[mesh.vertexCount];

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            uint[] ori = FetchGPUData(originalMesh, i, 2);
            int pointer = 0;
            while (ori[pointer] == 0x01010100)
            {
                pointer += 8;
            }

            for (; pointer < ori.Length && (ori[pointer] != 0x01010100);)
            {
                uint vertexIndex = ori[pointer++];
                colors[vertexIndex] = new Color(UnpackFloatFromUInt(ori[pointer++]), UnpackFloatFromUInt(ori[pointer++]), UnpackFloatFromUInt(ori[pointer++]), UnpackFloatFromUInt(ori[pointer++]));
                pointer += 3;
            }
        }
        mesh.colors = colors;
    }

    private void CreateBoneList()
	{
		offsetBoneCandidates.Clear();
		currentBonesList.Clear();
		Transform transform = GameObject.Find("BodyTop").transform;
		Dictionary<Transform, int> dictionary = new Dictionary<Transform, int>();
		Transform[] componentsInChildren = transform.GetComponentsInChildren<Transform>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GetAltBoneName(componentsInChildren[i]));
			stringBuilder.Append(" ");
			stringBuilder.Append(componentsInChildren[i].GetInstanceID().ToString());
			string text = stringBuilder.ToString();
			if (pmxFile.BoneBackupData.TryGetValue(text, out var value))
			{
				if (!whitelistOffsetBones.Contains(GetAltBoneName(componentsInChildren[i], ignoreID: true), StringComparer.Ordinal))
				{
					continue;
				}
				PmxBone pmxBone = value.PmxBone;
				if (pmxBone != null)
				{
					UnityEngine.Vector3 vector = componentsInChildren[i].transform.position * scale;
					BoneOffsetData bnOffsetData = new BoneOffsetData(offset: new PmxLib.Vector3(0f - vector.x, vector.y, 0f - vector.z) - pmxBone.Position, boneName: GetAltBoneName(componentsInChildren[i]));
					AddToBoneOffsetDataList(bnOffsetData);
					offsetBoneCandidates.Add(text);
					StringBuilder stringBuilder2 = new StringBuilder();
					stringBuilder2.Append(GetAltBoneName(componentsInChildren[i]));
					stringBuilder2.Append(" ");
					stringBuilder2.Append(componentsInChildren[i].GetInstanceID().ToString());
					text = stringBuilder2.ToString();
				}
			}
			int value2;
			if (pmxFile.BoneList.Count > 0)
			{
				StringBuilder stringBuilder3 = new StringBuilder();
				stringBuilder3.Append(GetAltBoneName(componentsInChildren[i].parent));
				stringBuilder3.Append(" ");
				stringBuilder3.Append(componentsInChildren[i].parent.GetInstanceID().ToString());
				string key = stringBuilder3.ToString();
				currentBoneKeysList.TryGetValue(key, out value2);
			}
			else
			{
				dictionary.TryGetValue(componentsInChildren[i].parent, out value2);
			}
			PmxBone pmxBone2 = new PmxBone
			{
				Name = GetAltBoneName(componentsInChildren[i]),
				NameE = componentsInChildren[i].GetInstanceID().ToString(),
				Parent = value2
			};
			UnityEngine.Vector3 vector2 = componentsInChildren[i].transform.position * scale;
			pmxBone2.Position = new PmxLib.Vector3(0f - vector2.x, vector2.y, 0f - vector2.z);
			dictionary.Add(componentsInChildren[i], i);
			pmxFile.BoneList.Add(pmxBone2);
			pmxFile.BoneBackupData.Add(text, new Pmx.BackupBoneData(pmxBone2.Name, componentsInChildren[i].GetInstanceID(), pmxBone2));
			currentBoneKeysList.Add(text, currentBoneKeysList.Count);
			currentBonesList.Add(pmxBone2.Name);
            editBoneInfo.Add(new BoneInfo(pmxBone2.Name, componentsInChildren[i]));
        }
    }

	private int sbi(string name, string id)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(name);
		stringBuilder.Append(" ");
		stringBuilder.Append(id);
		string text = stringBuilder.ToString();
		if (!currentBoneKeysList.TryGetValue(text, out var value))
		{
			Console.WriteLine("SBI Failed for: " + text);
		}
		return value;
	}

	public void ExportGagEyes()
	{
		string[] names = new string[]
		{
            "cf_t_gageye_00", "cf_t_gageye_01", "cf_t_gageye_02", "cf_t_gageye_03", "cf_t_gageye_04", "cf_t_gageye_05",
            "cf_t_gageye_06", "cf_t_gageye_07", "cf_t_gageye_08", "cf_t_gageye_09", "cf_t_gageye_10",
            "cf_t_expression_00", "cf_t_expression_01"
        };
		var loadedBundles = AssetBundle.GetAllLoadedAssetBundles().ToList();
		foreach (var bundle in loadedBundles)
		{
			if (bundle.name.Contains("mt_eye_000_00"))
			{
				foreach (var i in bundle.LoadAllAssets<Shader>())
				{
					if (names.Contains(i.name))
					{
						Texture2D texture = i.TryCast<Texture2D>();
						TextureSaver.SaveTexture(texture, savePath + i.name + ".png");
						Texture2D.Destroy(texture);
					}
					UnityEngine.Object.Destroy(i);
				}
            }
		}
    }

	public void CreateDynamicBonesData()
	{
		DynamicBone[] componentsInChildren = GameObject.Find("BodyTop").transform.GetComponentsInChildren<DynamicBone>(includeInactive: true);
		//DynamicBone_Ver01[] componentsInChildren2 = GameObject.Find("BodyTop").transform.GetComponentsInChildren<DynamicBone_Ver01>(includeInactive: true);
		DynamicBone_Ver02[] componentsInChildren3 = GameObject.Find("BodyTop").transform.GetComponentsInChildren<DynamicBone_Ver02>(includeInactive: true);
		DynamicBone[] array = componentsInChildren;
		foreach (DynamicBone dynamicBone in array)
		{
			if (dynamicBone != null)
			{
				DynamicBoneData dynamicBoneData = new DynamicBoneData(dynamicBone);
				AddToDynamicBonesDataList(dynamicBoneData);
			}
		}
		//DynamicBone_Ver01[] array2 = componentsInChildren2;
		//foreach (DynamicBone_Ver01 dynamicBone_Ver in array2)
		//{
		//	if (dynamicBone_Ver != null)
		//	{
		//		DynamicBoneData dynamicBoneData2 = new DynamicBoneData(dynamicBone_Ver);
		//		AddToDynamicBonesDataList(dynamicBoneData2);
		//	}
		//}
		DynamicBone_Ver02[] array3 = componentsInChildren3;
		foreach (DynamicBone_Ver02 dynamicBone_Ver2 in array3)
		{
			if (dynamicBone_Ver2 != null)
			{
				DynamicBoneData dynamicBoneData3 = new DynamicBoneData(dynamicBone_Ver2);
				AddToDynamicBonesDataList(dynamicBoneData3);
			}
		}
	}

	public void CreateDynamicBoneCollidersData()
	{
		DynamicBoneCollider[] componentsInChildren = GameObject.Find("BodyTop").transform.GetComponentsInChildren<DynamicBoneCollider>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			DynamicBoneColliderData dynamicBoneColliderData = new DynamicBoneColliderData(componentsInChildren[i]);
			AddToDynamicBoneCollidersDataList(dynamicBoneColliderData);
		}
	}

	public void CreateCharacterInfoData()
	{
        Human human = ACExporterPlugin.selectedChara;
        StateMiniSelection stateMiniSelection = GameObject.Find("Cvs_StateMiniWindow").GetComponent<StateMiniSelection>();
        var face = human.face;
		var eyes = face.eyesCtrl;
		var eyeMatControllerl = face.eyeLookMatCtrl[0]._eyeLR == ILLGAMES.Unity.Animations.EYE_LR.EYE_L ? face.eyeLookMatCtrl[0] : face.eyeLookMatCtrl[1];
		var eyeMatControllerr = face.eyeLookMatCtrl[0]._eyeLR == ILLGAMES.Unity.Animations.EYE_LR.EYE_R ? face.eyeLookMatCtrl[0] : face.eyeLookMatCtrl[1];

        CharacterInfoData item4 = new CharacterInfoData
		{
			Name = human.fileParam.GetCharaName(false),
            Personality = human.fileParam.personality,
			VoiceRate = human.fileParam.voiceRate,
			PupilWidth = human.fileFace.pupilWidth,
			PupilHeight = human.fileFace.pupilHeight,
			PupilY = human.fileFace.pupil[0].gradOffsetY,
			HlUpX = human.fileFace.pupil[0].highlightInfos[0].x,
			HlUpY = human.fileFace.pupil[0].highlightInfos[0].y,
			HlDownX = human.fileFace.pupil[0].highlightInfos[1].x,
			HlDownY = human.fileFace.pupil[0].highlightInfos[1].y,
			ShapeInfoFace = human.fileFace.shapeValueFace.ToList(),
			ShapeInfoBody = human.fileBody.shapeValueBody.ToList()
		};

        List<BlendShapeinfo> blendShapeinfos = new();

        var patterns = Human.lstCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.cha_eyeset);
		List<string> ptnNames = new();

		foreach (var name in patterns.Values)
		{
			ptnNames.Add(name.Name);
		}
		
        face.ChangeEyesOpenMax(1);
        face.ChangeEyesShaking(false);
        face.ChangeEyesBlinkFlag(false);
        face.ChangeEyesPtn(1, false);

        for (int j = 0; j < ptnNames.Count; j++)
        {
            BlendShapeinfo bld = new BlendShapeinfo(ptnNames[j], "Eye");
            blendShapeinfos.Add(bld);
            face.ChangeEyesPtn(j, false);
			face.fbsCtrl.OnLateUpdate(0);
            eyes.CalculateBlendShape(0);

            foreach (var __ in eyes.FBSTarget)
            {
                var smr = __.GetSkinnedMeshRenderer();
                for (int k = 0; k < smr.sharedMesh.blendShapeCount; k++)
                {
                    float weight = smr.GetBlendShapeWeight(k);
                    if (weight > 0)
                    {
                        bld.Add(smr.sharedMesh.GetBlendShapeName(k), weight);
                    }
                }
            }
        }
        face.ChangeEyesPtn(0, false);


        UnityEngine.Vector2 _vecl = eyeMatControllerl.material.GetTextureOffset("_Pipil_texture");
		UnityEngine.Vector2 _vecr = eyeMatControllerr.material.GetTextureOffset("_Pipil_texture");
		item4.pupilOffset = new List<float>() { _vecl.x, _vecl.y, _vecr.x, _vecr.y };

		_vecl = eyeMatControllerl.material.GetTextureOffset("_Highlight_texture_01");
		_vecr = eyeMatControllerr.material.GetTextureOffset("_Highlight_texture_01");
		item4.highlight01Offset = new List<float>() { _vecl.x, _vecl.y, _vecr.x, _vecr.y };

		_vecl = eyeMatControllerl.material.GetTextureOffset("_Highlight_texture_02");
		_vecr = eyeMatControllerr.material.GetTextureOffset("_Highlight_texture_02");
		item4.highlight02Offset = new List<float>() { _vecl.x, _vecl.y, _vecr.x, _vecr.y };

        _vecl = eyeMatControllerl.material.GetTextureOffset("_Highlight_texture_03");
        _vecr = eyeMatControllerr.material.GetTextureOffset("_Highlight_texture_03");
        item4.highlight03Offset = new List<float>() { _vecl.x, _vecl.y, _vecr.x, _vecr.y };

        _vecl = eyeMatControllerl.material.GetTextureScale("_Pipil_texture");
		_vecr = eyeMatControllerr.material.GetTextureScale("_Pipil_texture");
		item4.pupilScale = new List<float>() { _vecl.x, _vecl.y, _vecr.x, _vecr.y };

		_vecl = eyeMatControllerl.material.GetTextureScale("_Highlight_texture_01");
		_vecr = eyeMatControllerr.material.GetTextureScale("_Highlight_texture_01");
		item4.highlight01Scale = new List<float>() { _vecl.x, _vecl.y, _vecr.x, _vecr.y };

		_vecl = eyeMatControllerl.material.GetTextureScale("_Highlight_texture_02");
		_vecr = eyeMatControllerr.material.GetTextureScale("_Highlight_texture_02");
		item4.highlight02Scale = new List<float>() { _vecl.x, _vecl.y, _vecr.x, _vecr.y };

		_vecl = eyeMatControllerl.material.GetTextureScale("_Highlight_texture_03");
		_vecr = eyeMatControllerr.material.GetTextureScale("_Highlight_texture_03");
		item4.highlight03Scale = new List<float>() { _vecl.x, _vecl.y, _vecr.x, _vecr.y };

		//item4.eyeRotation = new List<float>() { eyeMatControllerl._material.GetFloat("_rotation"), eyeMatControllerr._material.GetFloat("_rotation") };

		characterInfoData.Add(item4);
        ExportDataToJson(characterInfoData, "AC_CharacterInfoData.json");
		ExportDataToJson(blendShapeinfos, "AC_BlendShapeInfo.json");
	}

	public void AddToSMRDataList(SMRData smrData)
	{
		if (characterSMRData.Find((SMRData data) => string.CompareOrdinal(smrData.SMRName, data.SMRName) == 0 && string.CompareOrdinal(smrData.SMRPath, data.SMRPath) == 0) == null)
		{
			characterSMRData.Add(smrData);
		}
	}
    public void AddToMaterialDataCompleteList(MaterialDataComplete smrData)
    {
        if (materialDataComplete.Find((MaterialDataComplete data) => string.CompareOrdinal(smrData.SMRName, data.SMRName) == 0 && string.CompareOrdinal(smrData.SMRPath, data.SMRPath) == 0) == null)
        {
            materialDataComplete.Add(smrData);
        }
    }

    public void AddToTextureDataList(TextureData texData)
	{
		if (texData.textureName != null && textureData.Find((TextureData data) => string.CompareOrdinal(texData.textureName, data.textureName) == 0) == null)
		{
			textureData.Add(texData);
		}
	}

	public void AddToDynamicBonesDataList(DynamicBoneData dynamicBoneData)
	{
		dynamicBonesData.Add(dynamicBoneData);
	}

	public void AddToDynamicBoneCollidersDataList(DynamicBoneColliderData dynamicBoneColliderData)
	{
		dynamicBoneCollidersData.Add(dynamicBoneColliderData);
	}

	public void AddToBoneOffsetDataList(BoneOffsetData bnOffsetData)
	{
		boneOffsetData.Add(bnOffsetData);
	}
	public void ExportDataToJson<T>(T data, string fileName)
	{
        string text = CustomJsonSerializer.Serialize(data);

        File.WriteAllText(savePath + fileName, text);
    }

	public static string GetGameObjectPath(GameObject obj)
	{
		string text = "/" + GetAltBoneName(obj, ignoreID: true);
		while (obj.transform.parent != null)
		{
			obj = obj.transform.parent.gameObject;
			text = "/" + GetAltBoneName(obj, ignoreID: true) + text;
		}
		return text;
	}

	public static int GetAltInstanceID(UnityEngine.Object _object)
	{
		if (instanceIDs.TryGetValue(_object.GetInstanceID(), out var value) && value >= 0)
		{
			int.TryParse(value.ToString() + nowCoordinate, out value);
			return value;
		}
		Console.WriteLine("No ID Found");
		return -1;
	}

	public static string GetAltBoneName(UnityEngine.Object _object, bool ignoreID = false)
	{
		string cleanedName = CleanUpName(_object.name);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(cleanedName);
		stringBuilder.Append(" ");
		stringBuilder.Append(_object.GetInstanceID().ToString());
		string text = stringBuilder.ToString();
		bool flag = nowCoordinate == minCoord;
		bool flag2 = false;
		bool flag3 = pmxFile.BoneBackupData.ContainsKey(text);
		if (flag3)
		{
			flag2 = offsetBoneCandidates.Contains(text, StringComparer.Ordinal);
		}
		if (flag && currentBonesList.Contains(cleanedName))
		{
			flag = false;
		}
		if (flag || ignoreID || (flag3 && !flag2))
		{
			return cleanedName;
		}
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.Append(cleanedName);
		stringBuilder2.Append(" ");
		stringBuilder2.Append(GetAltInstanceID(_object).ToString());
		return stringBuilder2.ToString();
	}

	public static string GetAltMaterialName(PmxBuilder pmxBuilder, string materialName)
	{
		if (currentMaterialList.TryGetValue(materialName, out var value))
		{
			if (!pmxBuilder.ignoreList.Contains(materialName))
			{
				currentMaterialList[materialName] = value + 1;
				materialName = materialName + " " + value.ToString("00");
			}
		}
		else
		{
			currentMaterialList.Add(materialName, value);
		}
		return materialName;
	}

	public void CreatePmxHeader()
	{
		PmxElementFormat pmxElementFormat = new PmxElementFormat(1f)
		{
			VertexSize = PmxElementFormat.GetUnsignedBufSize(pmxFile.VertexList.Count),
			UVACount = 3
		};
		int val = int.MinValue;
		for (int i = 0; i < pmxFile.BoneList.Count; i++)
		{
			val = Math.Max(val, Math.Abs(pmxFile.BoneList[i].IK.LinkList.Count));
		}
		int count = Math.Max(val, pmxFile.BoneList.Count);
		pmxElementFormat.BoneSize = PmxElementFormat.GetSignedBufSize(count);
		if (pmxElementFormat.BoneSize < 2)
		{
			pmxElementFormat.BoneSize = 2;
		}
		pmxElementFormat.MorphSize = PmxElementFormat.GetUnsignedBufSize(pmxFile.MorphList.Count);
		pmxElementFormat.MaterialSize = PmxElementFormat.GetUnsignedBufSize(pmxFile.MaterialList.Count);
		pmxElementFormat.BodySize = PmxElementFormat.GetUnsignedBufSize(pmxFile.BodyList.Count);
		PmxHeader pmxHeader = new PmxHeader(2.1f);
		pmxHeader.FromElementFormat(pmxElementFormat);
		pmxFile.Header = pmxHeader;
	}

	public void Save()
	{
		pmxFile.ToFile(currentSavePath + "model.pmx");
	}

	public void ExportAllDataLists()
	{
		ExportDataToJson(characterSMRData, "AC_SMRData.json");
        ExportDataToJson(materialDataComplete, "AC_MaterialDataComplete.json");
        //ExportDataToJson(materialData, "KK_MaterialData.json");
        //ExportDataToJson(textureData, "KK_TextureData.json");
        //ExportDataListToJson(clothesData, "KK_ClothesData.json");
        //ExportDataListToJson(accessoryData, "KK_AccessoryData.json");
        //ExportDataListToJson(referenceInfoData, "KK_ReferenceInfoData.json");
        ExportDataToJson(dynamicBonesData, "AC_DynamicBoneData.json");
        ExportDataToJson(dynamicBoneCollidersData, "AC_DynamicBoneColliderData.json");
		//ExportDataListToJson(accessoryStateData, "KK_AccessoryStateData.json");
		ExportDataToJson(boneOffsetData, "AC_BoneOffsetData.json");
        //ExportDataToJson(listInfoData, "KK_ListInfoData.json");
        //ExportChaFileCoordinateDataListToJson(chaFileCoordinateData, "KK_ChaFileCoordinateData.json");
        ExportDataToJson(editBoneInfo, "AC_EditBoneInfo.json");
        ExportDataToJson(lightDarkMaterials, "AC_LightDarkMaterials.json");
    }

	public void OpenFolderInExplorer(string filename)
	{
		if (filename == null)
		{
			throw new ArgumentNullException("filename");
		}
		try
		{
			filename = Path.GetFullPath(filename);
			Process.Start("explorer.exe", filename ?? "");
		}
		catch (Exception value)
		{
			Console.WriteLine(value);
		}
	}

	public static string CleanUpName(string str)
	{
		//return str.Replace("(Instance)", "").Replace("(Clone)", "").Trim();
		return str.Replace("(Instance)", "").Trim();
	}

	public static string AnimationCurveToJSON(AnimationCurve curve)
	{
		StringBuilder stringBuilder = new StringBuilder();
		BeginObject(stringBuilder);
		BeginArray(stringBuilder, "keys");
		Keyframe[] keys = curve.GetKeys();
		for (int i = 0; i < keys.Length; i++)
		{
			Keyframe keyframe = keys[i];
			BeginObject(stringBuilder);
			WriteFloat(stringBuilder, "time", keyframe.time);
			Next(stringBuilder);
			WriteFloat(stringBuilder, "value", keyframe.value);
			Next(stringBuilder);
			WriteFloat(stringBuilder, "intangent", keyframe.inTangent);
			Next(stringBuilder);
			WriteFloat(stringBuilder, "outtangent", keyframe.outTangent);
			EndObject(stringBuilder);
			if (i < keys.Length - 1)
			{
				Next(stringBuilder);
			}
		}
		EndArray(stringBuilder);
		EndObject(stringBuilder);
		return stringBuilder.ToString();
	}

	public static UnityEngine.Vector2[] GenerateUV(UnityEngine.Vector3[] vertices, int[] triangles)
	{
		UnityEngine.Vector2[] uv = new UnityEngine.Vector2[vertices.Length];
		

		return uv;
	}

	public static void BeginObject(StringBuilder sb)
	{
		sb.Append("{ ");
	}

	public static void EndObject(StringBuilder sb)
	{
		sb.Append(" }");
	}

	public static void BeginArray(StringBuilder sb, string keyname)
	{
		sb.AppendFormat("\"{0}\" : [", keyname);
	}

	public static void EndArray(StringBuilder sb)
	{
		sb.Append(" ]");
	}

	public static void WriteString(StringBuilder sb, string key, string value)
	{
		sb.AppendFormat("\"{0}\" : \"{1}\"", key, value);
	}

	public static void WriteFloat(StringBuilder sb, string key, float val)
	{
		sb.AppendFormat("\"{0}\" : {1}", key, val);
	}

	public static void WriteInt(StringBuilder sb, string key, int val)
	{
		sb.AppendFormat("\"{0}\" : {1}", key, val);
	}

	public static void WriteVector3(StringBuilder sb, string key, UnityEngine.Vector3 val)
	{
		sb.AppendFormat("\"{0}\" : {1}", key, CustomJsonSerializer.Serialize(val));
	}

	public static void Next(StringBuilder sb)
	{
		sb.Append(", ");
	}
}
