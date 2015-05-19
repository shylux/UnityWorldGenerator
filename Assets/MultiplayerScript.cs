using UnityEngine;
using System.Collections;

public class MultiplayerScript : MonoBehaviour 
{
	private const string typeName = "ch.shylux.mazerunner";
	private const string gameName = "Game1";
	private HostData[] hostList;
	public GameObject playerPrefab;
	
	private void StartServer()
	{
		Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
	}
	
	void OnServerInitialized()
	{
		Debug.Log("Server Initializied");
		SpawnPlayer();
	}

	void Start () {
		DontDestroyOnLoad (transform.gameObject);
	}
	
	void OnGUI() {
		if (!Network.isClient && !Network.isServer) {
			if (GUILayout.Button("Start Server"))
				StartServer();

			if (GUILayout.Button("Refresh Hosts"))
				MasterServer.RequestHostList(typeName);
			
			if (hostList != null) {
				for (int i = 0; i < hostList.Length; i++) {
					if (GUILayout.Button(hostList[i].gameName))
						Network.Connect(hostList[i]);
				}
			}
		}
	}

	void OnMasterServerEvent(MasterServerEvent msEvent) {
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}
	
	void OnConnectedToServer() {
		Debug.Log("Client connected to server");
		SpawnPlayer();
	}

	private void SpawnPlayer()
	{   Network.Instantiate(playerPrefab, 
		                    new Vector3(0f,5f,0f), 
		                    Quaternion.identity, 0);
	}
}
