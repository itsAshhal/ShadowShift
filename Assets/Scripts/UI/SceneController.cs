using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShadowShift.UI
{
    public class SceneController : MonoBehaviour
    {
        public void ChangeScene_String(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
        public void ChangeScene_Index(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }

}