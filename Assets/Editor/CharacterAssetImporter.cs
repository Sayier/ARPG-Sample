using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterAssetImporter : AssetPostprocessor
{
    private static readonly string PROCESSOR_FOLDER = "Characters";
    private static readonly string MATERIALS_FOLDER = "Materials";
    private static readonly string TEXTURES_FOLDER = "Textures";

    private static string[] TEXTURE_TYPES = new string[]
    {
        "__diffuse",
        "__normal",
        "__specular"
    };

    private static Dictionary<string, Avatar> avatarsPerModelFile = new Dictionary<string, Avatar>();
    private static int incompleteAssets = 0;

    private static bool ShouldProcessModel(string assetPath)
    {
        if (!assetPath.Contains(Path.Combine("Imports", PROCESSOR_FOLDER)))
        { 
            return false; 
        }

        if (!assetPath.EndsWith(".fbx"))
        {
            return false;
        }

        Debug.Log("Importing" + assetPath);

        return true;
    }

    private static string GetCharacterFolder(string assetPath)
    {
        return Path.GetFileName(Path.GetDirectoryName(assetPath));
    }

    private static string GetModelFilePath(string assetPath)
    {
        string[] assetPaths = Directory.GetFiles(Path.GetDirectoryName(assetPath));

        foreach(string path in assetPaths)
        {
            if (Path.GetFileName(path).StartsWith("_"))
            {
                return path;
            }
        }

        return "";
    }

    void OnPreprocessModel()
    {
        if(ShouldProcessModel(assetPath) != true)
        {
            return;
        }

        ModelImporter modelImporter = assetImporter as ModelImporter;

        modelImporter.bakeAxisConversion = true;

        if (Path.GetFileName(assetPath).StartsWith("_"))
        {
            modelImporter.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            modelImporter.optimizeGameObjects = true;

            modelImporter.ExtractTextures(Path.Combine(
                "Assets", TEXTURES_FOLDER,
                PROCESSOR_FOLDER, GetCharacterFolder(assetPath)));
        }

        else
        {
            modelImporter.avatarSetup = ModelImporterAvatarSetup.CopyFromOther;

            string modelFilePath = GetModelFilePath(assetPath);
            if(modelFilePath != "")
            {
                Avatar avatar;
                if(avatarsPerModelFile.TryGetValue(modelFilePath, out avatar) != true)
                {
                    avatar = (Avatar)AssetDatabase
                        .LoadAllAssetsAtPath(modelFilePath)
                        .First(model => model.GetType() == typeof(Avatar));
                    avatarsPerModelFile[modelFilePath] = avatar;
                }

                if(avatar != null)
                {
                    modelImporter.sourceAvatar = avatar;
                }
                else
                {
                    incompleteAssets++;
                }
            }

            modelImporter.materialImportMode = ModelImporterMaterialImportMode.None;
        }
    }

    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths) 
    {
        string materialsRootFolder = Path.Combine("Assets", MATERIALS_FOLDER, PROCESSOR_FOLDER);    //Get reference to the Materials/Characters
        string materialRefFolder, materialAssetDir;

        foreach(string path in importedAssets)
        {
            materialRefFolder = GetCharacterFolder(path);   //Get specific Character folder name
            materialAssetDir = Path.Combine(materialsRootFolder, materialRefFolder);    //Get reference to the full path of that Characters Materials folder

            if (ShouldProcessModel(path))   //Checks if the import is a model we want to process
            {
                if(Directory.Exists(materialAssetDir) != true)  //Checks if materials folder already exists or if it needs to be created
                {
                    AssetDatabase.CreateFolder(materialsRootFolder, materialAssetDir);
                }

                IEnumerable<Object> materials = AssetDatabase   //Preps being able to iterate on all found materials
                    .LoadAllAssetsAtPath(path)
                    .Where(Object => Object.GetType() == typeof(Material));

                string materialAssetPath, error;

                foreach (Object material in materials)
                {
                    materialAssetPath = Path.Combine(materialAssetDir, $"{material.name}.mat"); //Creates reference to path of specific material 

                    error = AssetDatabase.ExtractAsset(material, materialAssetPath);    //Attempts extraction

                    if(error != "")
                    {
                        Debug.LogWarning($"Could not extract material '{material.name}': {error}", material); //If extraction fails log error, else finish importing
                    }
                    else
                    {
                        AssetDatabase.WriteImportSettingsIfDirty(path);
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    }
                }
            }

            else if(isTexture(path) && ShouldProcessTexture(path))
            {
                Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(path);
                if(texture == null)
                {
                    Debug.LogWarning($"Could not find texture '{path}' - no auto-linking of the texture");
                    return;
                }

                (string materialName, string mapType) = ParseTexturePath(path);
                Material material = AssetDatabase.LoadAssetAtPath<Material>(Path.Combine(materialAssetDir, $"{materialName}.mat"));

                if(material == null)
                {
                    Debug.LogWarning($"Could not find material '{materialName}' - no auto-linking of the textures");
                    return;
                }

                if(mapType == "__diffuse")
                {
                    material.SetTexture("_MainTex", texture);
                }
                else if(mapType == "__normal")
                {
                    material.SetTexture("_BumpMap", texture);
                }
                else if(mapType == "__specular")
                {
                    material.SetTexture("_MetallicGlossMap", texture);
                }
            }
        }

        int localIncompleteAssets = incompleteAssets;
        incompleteAssets = 0;

        if (localIncompleteAssets > 0)
        {
            AssetDatabase.ForceReserializeAssets();
        }
    }

    void OnPreprocessAnimation()
    {
        if(ShouldProcessModel(assetPath)!= true)
        {
            return;
        }

        string animation = Path.GetFileNameWithoutExtension(assetPath);
        ModelImporter modelImporter = assetImporter as ModelImporter;
        ModelImporterClipAnimation[] animations = modelImporter.defaultClipAnimations;

        if(animations!= null && animations.Length > 0)
        {
            for(int i = 0; i < animations.Length; i++)
            {
                animations[i].name = animation.EndsWith("@") ? animation.Substring(0, animation.Length - 1) : animation;
                if (animation.EndsWith("@"))
                {
                    animations[i].loopTime = true;
                }
            }

            modelImporter.clipAnimations = animations;
        }
    }

    private static bool isTexture(string assetPath)
    {
        string lowerPath = assetPath.ToLower();

        return lowerPath.EndsWith(".jpg") || lowerPath.EndsWith(".png") || lowerPath.EndsWith(".jpeg") || lowerPath.EndsWith(".tga");
    }

    private static bool ShouldProcessTexture(string assetPath)
    {
        if (assetPath.Contains(Path.Combine(TEXTURES_FOLDER, PROCESSOR_FOLDER)) != true)
        {
            return false;
        }

        return true;
    }

    private static (string, string) ParseTexturePath(string texturePath)
    {
        foreach(string type in TEXTURE_TYPES)
        {
            if (texturePath.Contains(type))
            {
                string materialName = Path.GetFileNameWithoutExtension(texturePath.Replace(type, ""));

                return (materialName, type);
            }
        }

        return ("", "Unknown");
    }
}
