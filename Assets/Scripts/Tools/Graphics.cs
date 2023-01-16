using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;

using DG.Tweening;

namespace Tools
{
    public class Graphics
    {
        private static Transform cameraTransform;

        #region Variables: Popup Damage
        private static readonly float DAMAGE_POPUP_TIME = 2f;
        private static readonly float DAMAGE_POPUP_TEXT_BASE_SIZE = 6f;
        #endregion

        public static void CreateDamagePopups(float damageAmount, Vector3 worldPosition)
        {
            if (cameraTransform == null)
            {
                cameraTransform = Camera.main.transform;
            }

            float damageMultiplier = damageAmount / AddressablesLoader.instance.player.attackDamage;

            AddressablesLoader.instance.damagePopupPrefab.InstantiateAsync(
                worldPosition, Quaternion.identity).Completed +=
                async (AsyncOperationHandle<GameObject> obj) =>
                {
                    GameObject damageTextObject = obj.Result;
                    damageTextObject.transform.rotation = Quaternion.LookRotation(
                        cameraTransform.forward,
                        cameraTransform.up);
                    
                    TextMeshPro TMPComponent = damageTextObject.GetComponent<TextMeshPro>();
                    TMPComponent.text = ((int)damageAmount).ToString();
                    TMPComponent.fontSize = 1f + 2f * Mathf.Log(DAMAGE_POPUP_TEXT_BASE_SIZE * damageMultiplier);
                    TMPComponent.color = new Color(
                        1f,
                        Mathf.Clamp01(1f - damageMultiplier * 0.1f),
                        0f,
                        1f);

                    damageTextObject.transform.DOMoveY(
                        (worldPosition + damageTextObject.transform.up * 5f).y,
                        DAMAGE_POPUP_TIME)
                    .SetEase(Ease.Linear);

                    DOTween.ToAlpha(
                        () => TMPComponent.color,
                        (Color color) => { TMPComponent.color = color; },
                        0f,
                        DAMAGE_POPUP_TIME);

                    await System.Threading.Tasks.Task.Delay((int)(DAMAGE_POPUP_TIME * 1000 + 100));
                    Addressables.ReleaseInstance(damageTextObject);
                };
        }
    }
}