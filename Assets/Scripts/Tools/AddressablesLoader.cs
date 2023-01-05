using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Tools
{
    public class AddressablesLoader : MonoBehaviour
    {
        public static AddressablesLoader instance;

        [Header("Graphics")]
        public AssetReferenceGameObject damagePopupPrefab;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            Addressables.InitializeAsync();
        }
    }
}