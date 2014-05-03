UnityTGC Version 1.1.0
Author:Chris
Date:2013.3.20

Change Log:
1)Added signal state icon assets to project.

------------------------------------------------------------------------------------
UnityTGC Version 1.0.0
Author:Chris
Date:2013.3.13

Change Log:
1)Initialize project.

------------------------------------------------------------------------------------
Instructions

Run the Demo:
1.Drag the NeuroSkyTGCController prefab from Project view to Hierarchy view
2.Add the DisplayData.cs to main Camera
3.Play UnityEditor,it will auto connect TGC
4.You can click Disconnect button to disconnect
5.You can click Connect button to re-connect

Run your own project:
1.At the place you want to use Attention,Meditation,EEG and so on,add following code

    void Start()
    {
		
		controller = GameObject.Find("NeuroSkyTGCController").GetComponent<TGCConnectionController>();
		
		controller.UpdatePoorSignalEvent += OnUpdatePoorSignal;
		controller.UpdateAttentionEvent += OnUpdateAttention;
		controller.UpdateMeditationEvent += OnUpdateMeditation;
		
		controller.UpdateDeltaEvent += OnUpdateDelta;
		
    }
    
2.Implement every function above
3.Call Connect() function of TGCConnectionController to connect
4.Call disconnect() function of TGCConnectionController to disconnect

Note:Check the Api Compatibility Level is .NET2.0,not .NET2.0 Subset.