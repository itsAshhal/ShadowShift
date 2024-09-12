using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadowShift
{
    /// <summary>
    /// This script is helpful to hide certain gameObject instead of manually having reference to them in some 
    /// scripts and then hiding them.
    /// </summary>
    public class ManualDestroyer : MonoBehaviour
    {
        public float IdealHideTime = 2f;
        public GameObject[] ObjectsToHide;

        /// <summary>
        /// When this method is called, the referenced gameObjects are destroyed after the wait "IdealHideTime"
        /// </summary>
        public void StartHidingTimer()
        {
            Invoke(nameof(HideFinally), IdealHideTime);
        }

        void HideFinally()
        {
            foreach (var obj in ObjectsToHide) obj.SetActive(false);
        }
    }

}