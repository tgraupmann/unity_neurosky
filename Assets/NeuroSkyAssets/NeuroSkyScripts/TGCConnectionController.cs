using System;
using System.Threading;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MindWave.LitJson;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace MindWave
{
    public class TGCConnectionController : MonoBehaviour
    {
        private TcpClient client;
        private Stream stream;
        private byte[] buffer;

        public delegate void UpdateIntValueDelegate(int value);

        public delegate void UpdateFloatValueDelegate(float value);

        public event UpdateIntValueDelegate UpdatePoorSignalEvent;
        public event UpdateIntValueDelegate UpdateAttentionEvent;
        public event UpdateIntValueDelegate UpdateMeditationEvent;
        public event UpdateIntValueDelegate UpdateRawdataEvent;
        public event UpdateIntValueDelegate UpdateBlinkEvent;

        public event UpdateFloatValueDelegate UpdateDeltaEvent;
        public event UpdateFloatValueDelegate UpdateThetaEvent;
        public event UpdateFloatValueDelegate UpdateLowAlphaEvent;
        public event UpdateFloatValueDelegate UpdateHighAlphaEvent;
        public event UpdateFloatValueDelegate UpdateLowBetaEvent;
        public event UpdateFloatValueDelegate UpdateHighBetaEvent;
        public event UpdateFloatValueDelegate UpdateLowGammaEvent;
        public event UpdateFloatValueDelegate UpdateHighGammaEvent;

        private bool m_waitForExit = true;

        private void Start()
        {
            ThreadStart ts = new ThreadStart(Connect);
            Thread thread = new Thread(ts);
            thread.Start();
        }

        public void Disconnect()
        {
            stream.Close();
        }

        public void Connect()
        {
            client = new TcpClient("127.0.0.1", 13854);
            stream = client.GetStream();
            buffer = new byte[1024];
            byte[] myWriteBuffer = Encoding.ASCII.GetBytes(@"{""enableRawOutput"": true, ""format"": ""Json""}");
            stream.Write(myWriteBuffer, 0, myWriteBuffer.Length);

            while (m_waitForExit)
            {
                ParseData();
                Thread.Sleep(100);
            }
        }

        public class PowerData
        {
            public float delta = 0;
            public float theta = 0;
            public float lowAlpha = 0;
            public float highAlpha = 0;
            public float lowBeta = 0;
            public float highBeta = 0;
            public float lowGamma = 0;
            public float highGamma = 0;
            public PowerData()
            {
            }
        }

        public class SenseData
        {
            public int attention = 0;
            public int meditation = 0;
            public PowerData eegPower = null;
            public SenseData()
            {
            }
        }

        public class PackatData
        {
            public string status = string.Empty;
            public int poorSignalLevel = 0;
            public int rawEeg = 0;
            public int blinkStrength = 0;
            public SenseData eSense = null;
            public PackatData()
            {
            }
        }

        int GetObjectCount(String json)
        {
            int level = 0;
            int count = 0;
            for (int i = 0; i < json.Length; ++i)
            {
                if (json[i].Equals('{'))
                {
                    if (level == 0)
                    {
                        ++count;
                    }
                    ++level;
                }
                if (json[i].Equals('}'))
                {
                    --level;
                }
            }
            return count;
        }

        private void ParseData()
        {
            if (stream.CanRead)
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    List<PackatData> packets = new List<PackatData>();

                    String packet = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    if (!string.IsNullOrEmpty(packet))
                    {
                        Debug.Log(packet);
                        if (packet.Contains("}"))
                        {
                            int count = GetObjectCount(packet);
                            if (count == 1)
                            {
                                PackatData data = JsonMapper.ToObject<PackatData>(packet);
                                packets.Add(data);
                            }
                            else if (count > 1)
                            {
                                PackatData[] data = JsonMapper.ToObject<PackatData[]>(packet);
                                for (int index = 0; index < data.Length; ++index)
                                {
                                    packets.Add(data[index]);
                                }
                            }
                        }
                    }

                    foreach (PackatData data in packets)
                    {
                        if (null == data)
                        {
                            continue;
                        }
                        if (data.poorSignalLevel != 0)
                        {
                            Debug.Log("data.poorSignalLevel: " + data.poorSignalLevel);
                            if (null != UpdatePoorSignalEvent)
                            {
                                UpdatePoorSignalEvent.Invoke(data.poorSignalLevel);
                            }

                            if (null != data.eSense)
                            {
                                if (UpdateAttentionEvent != null)
                                {
                                    UpdateAttentionEvent(data.eSense.attention);
                                }
                                if (UpdateMeditationEvent != null)
                                {
                                    UpdateMeditationEvent(data.eSense.meditation);
                                }

                                if (null != data.eSense.eegPower)
                                {
                                    if (UpdateDeltaEvent != null)
                                    {
                                        UpdateDeltaEvent(data.eSense.eegPower.delta);
                                    }
                                    if (UpdateThetaEvent != null)
                                    {
                                        UpdateThetaEvent(data.eSense.eegPower.theta);
                                    }
                                    if (UpdateLowAlphaEvent != null)
                                    {
                                        UpdateLowAlphaEvent(data.eSense.eegPower.lowAlpha);
                                    }
                                    if (UpdateHighAlphaEvent != null)
                                    {
                                        UpdateHighAlphaEvent(data.eSense.eegPower.highAlpha);
                                    }
                                    if (UpdateLowBetaEvent != null)
                                    {
                                        UpdateLowBetaEvent(data.eSense.eegPower.lowBeta);
                                    }
                                    if (UpdateHighBetaEvent != null)
                                    {
                                        UpdateHighBetaEvent(data.eSense.eegPower.highBeta);
                                    }
                                    if (UpdateLowGammaEvent != null)
                                    {
                                        UpdateLowGammaEvent(data.eSense.eegPower.lowGamma);
                                    }
                                    if (UpdateHighGammaEvent != null)
                                    {
                                        UpdateHighGammaEvent(data.eSense.eegPower.highGamma);
                                    }
                                }


                            }
                        }
                        else if (data.rawEeg != 0)
                        {
                            if (null != UpdateRawdataEvent)
                            {
                                UpdateRawdataEvent(data.rawEeg);
                            }
                        }
                        else if (data.blinkStrength != 0)
                        {
                            if (null != UpdateRawdataEvent)
                            {
                                UpdateBlinkEvent(data.blinkStrength);
                            }
                        }
                    }
                }
                catch (IOException e)
                {
                    Debug.Log("IOException " + e);
                }
                catch (System.Exception e)
                {
                    Debug.Log("Exception " + e);
                }
            }

        } // end ParseData

        void OnDisable()
        {
            m_waitForExit = false;
            Disconnect();
        }

        private void OnApplicationQuit()
        {
            m_waitForExit = false;
            Disconnect();
        }


    }
}