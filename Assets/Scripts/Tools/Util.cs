using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public static class Util
    {
        public static IEnumerator WaitingForCurrentAnimation(
            Animator animator,
            System.Action callback,
            float earlyExit = 0f,
            bool stopAfterAnim = false)
        {
            if (stopAfterAnim)
            {
                yield return new WaitForEndOfFrame();
                yield return new WaitForSeconds(animator.GetAnimatorTransitionInfo(0).duration);
                yield return new WaitForEndOfFrame();
                yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
            }
            else
            {
                yield return new WaitForEndOfFrame();
                yield return new WaitForSeconds(animator.GetAnimatorTransitionInfo(0).duration);
                yield return new WaitForEndOfFrame();
                yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - earlyExit);
            }

            callback();
        }
    }
}