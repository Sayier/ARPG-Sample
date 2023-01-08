using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Tools
{
    public class AddressablesLoader : MonoBehaviour
    {
        public static AddressablesLoader instance;

        [Header("Graphics")]
        public AssetReferenceGameObject damagePopupPrefab;

        [Header("Player")]
        [SerializeField] private AssetReference playerData;

        [HideInInspector] public Player.PlayerData player;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            Addressables.InitializeAsync();
            StartCoroutine(PreloadReferences());
        }

        private IEnumerator PreloadReferences()
        {
            AsyncOperationHandle<Player.PlayerData> playerDataLoadHandle = playerData.LoadAssetAsync<Player.PlayerData>();

            yield return playerDataLoadHandle;
            player = playerDataLoadHandle.Result;
        }

        private void OnApplicationQuit()
        {
            playerData.ReleaseAsset();
        }
    }
}