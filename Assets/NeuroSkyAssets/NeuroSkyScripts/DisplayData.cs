using UnityEngine;
using System.Collections;

namespace MindWave
{
    public class DisplayData : MonoBehaviour
    {
        public Texture2D[] signalIcons;

        private int indexSignalIcons = 1;

        private TGCConnectionController controller;

        private int poorSignal1;
        private int attention1;
        private int meditation1;

        private float delta;

        private void Start()
        {

            controller = GameObject.Find("NeuroSkyTGCController").GetComponent<TGCConnectionController>();

            controller.UpdatePoorSignalEvent += OnUpdatePoorSignal;
            controller.UpdateAttentionEvent += OnUpdateAttention;
            controller.UpdateMeditationEvent += OnUpdateMeditation;

            controller.UpdateDeltaEvent += OnUpdateDelta;

        }

        private void OnUpdatePoorSignal(int value)
        {
            poorSignal1 = value;
            if (value < 25)
            {
                indexSignalIcons = 0;
            }
            else if (value >= 25 && value < 51)
            {
                indexSignalIcons = 4;
            }
            else if (value >= 51 && value < 78)
            {
                indexSignalIcons = 3;
            }
            else if (value >= 78 && value < 107)
            {
                indexSignalIcons = 2;
            }
            else if (value >= 107)
            {
                indexSignalIcons = 1;
            }
        }

        private void OnUpdateAttention(int value)
        {
            attention1 = value;
        }

        private void OnUpdateMeditation(int value)
        {
            meditation1 = value;
        }

        private void OnUpdateDelta(float value)
        {
            delta = value;
        }


        private void OnGUI()
        {
            GUILayout.BeginHorizontal();


            if (GUILayout.Button("Connect"))
            {
                controller.Connect();
            }
            if (GUILayout.Button("DisConnect"))
            {
                controller.Disconnect();
                indexSignalIcons = 1;
            }

            GUILayout.Space(Screen.width - 250);
            GUILayout.Label(signalIcons[indexSignalIcons]);

            GUILayout.EndHorizontal();


            GUILayout.Label("PoorSignal1:" + poorSignal1);
            GUILayout.Label("Attention1:" + attention1);
            GUILayout.Label("Meditation1:" + meditation1);
            GUILayout.Label("Delta:" + delta);

        }
    }
}