using UnityEngine;
using System.Collections;

namespace MindWave
{
    public class PreserveGameObject : MonoBehaviour
    {

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}