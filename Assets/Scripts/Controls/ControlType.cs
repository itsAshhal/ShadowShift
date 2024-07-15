using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadowShift
{
    [CreateAssetMenu(fileName = "ControlType", menuName = "Scriptables/NewControlType")]
    public class ControlType : ScriptableObject
    {
        public enum Controls
        {
            Swipe, Buttons
        }
        public Controls M_Controls;
        // they will be later on saved in JSON for permanent storage
    }

}