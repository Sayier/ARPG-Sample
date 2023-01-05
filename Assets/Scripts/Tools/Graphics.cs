using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;

namespace Tools
{
    public class Graphics
    {
        private static Transform cameraTransform;

        public static void CreateDamagePopups(float damageAmount, Vector3 worldPosition)
        {
            if (cameraTransform == null)
            {
                cameraTransform = Camera.main.transform;
            }

            AddressablesLoader.instance.damagePopupPrefab.InstantiateAsync(
                worldPosition, Quaternion.identity).Completed +=
                async (AsyncOperationHandle<GameObject> obj) =>
                {
                    GameObject gameObject = obj.Result;
                    gameObject.transform.rotation = Quaternion.LookRotation(
                        cameraTransform.forward,
                        cameraTransform.up);
                    TextMeshPro TMPobject = gameObject.GetComponent<TextMeshPro>();
                    TMPobject.text = ((int)damageAmount).ToString();
                };
        }
    }
}