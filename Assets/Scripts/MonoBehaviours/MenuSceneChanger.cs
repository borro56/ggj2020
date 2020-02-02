using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MonoBehaviours
{
    public class MenuSceneChanger : MonoBehaviour
    {
        public float timeUntilSceneChange;
        public string scene;

        private bool finished = false;
        private void Update()
        {
            if (Time.time > timeUntilSceneChange && !finished)
            {
                finished = true;
                SceneManager.LoadScene(scene, LoadSceneMode.Single);
            }
        }
    }
}
