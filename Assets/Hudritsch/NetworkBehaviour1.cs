using UnityEngine;
using System.Collections;

public class NetworkBehaviour1 : MonoBehaviour 
{
	public GameObject 	spawn; //empty spawn object with spawn code
	
	string 	remoteIP;		// IP Address of the server
	int 	remotePort;		// Port of the server (to connect)
	int 	listenPort;		// Port of the server (to listen)
	bool 	useNAT = true;	// For enabling/disabling NAT usage
	string 	myIP;			// Variables to show your IP
	string 	myPort;			// Variables to show your port
	
	public static string playerName; // static local player name
	
	// Get the name and connection parameters from the prefs
	void Start()
	{   playerName = PlayerPrefs.GetString("playerName", "PlayerName");
		remoteIP   = PlayerPrefs.GetString("remoteIP", "127.0.0.1");
		remotePort = PlayerPrefs.GetInt("remotePort", 25000);
		listenPort = PlayerPrefs.GetInt("listenPort", 25000);
	}
	
	void  OnGUI ()
	{
		// Checking if you are connected to the server or not
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			if (GUILayout.Button("Start me as Server"))
			{	
				// Init server for max. 32 clients
				if (Network.InitializeServer(32, listenPort, useNAT) == NetworkConnectionError.NoError)
				{	
					// Spawn a new player prefab by calling InstantiatePlayer
					spawn.SendMessage("InstantiatePlayer", 
					                  playerName,
					                  SendMessageOptions.DontRequireReceiver);
					PlayerPrefs.SetString("playerName", playerName);
				}
			}
			
			GUILayout.Label("or");
			
			// If not connected connect to remote server by IP & port		
			if (GUILayout.Button("Connect to Server"))
			{	if (Network.Connect(remoteIP, remotePort) == NetworkConnectionError.NoError)
				{	// Save preferences
					PlayerPrefs.SetString("playerName", playerName);
					PlayerPrefs.SetString("remoteIP", remoteIP);
					PlayerPrefs.SetInt("remotePort", remotePort);
				}
			}
			
			// Fields to insert ip address and port 
			GUILayout.Label("Remote IP:");
			remoteIP = GUILayout.TextField(remoteIP);
			GUILayout.Label("Remote Port:");
			remotePort = int.Parse(GUILayout.TextField(remotePort.ToString()));
			GUILayout.Label("Player Name:");
			playerName = GUILayout.TextField(playerName);
		}
		else
		{	// If connected show IP and peer type
			myIP   = Network.player.ipAddress;
			myPort = Network.player.port.ToString();
			GUILayout.Label("IP Adress: "+myIP+":"+myPort);
			GUILayout.Label("Peer type: " + Network.peerType.ToString());
			
			// Show ping time and No. of connections
			if (Network.peerType == NetworkPeerType.Client)
			{	GUILayout.Label("Ping to server: " + Network.GetAveragePing(Network.connections[0]) + " ms");		
			} else if (Network.peerType == NetworkPeerType.Server)
			{	GUILayout.Label("Connections: " + Network.connections.Length);
				if(Network.connections.Length > 0)
					GUILayout.Label("Ping to first player: " + Network.GetAveragePing(Network.connections[0]) + " ms");
			}
			
			// Disconnect & destroy my own player
			if (GUILayout.Button("Disconnect"))
			{	Network.Disconnect(200);
				GameObject.DestroyObject(GameObject.Find("Player_" + playerName));
			}
			
			// Let my box jump
			if (GUILayout.Button("Let my box jump"))
			{	GameObject GO = GameObject.Find("Player_" + playerName);
				Vector3 hitPos = GO.transform.position;
				hitPos += new Vector3(0.1f, 0f, 0f);
				GO.GetComponent<Rigidbody>().AddForceAtPosition(new Vector3(0,500,0), hitPos);
			}
		}
	}
	
	// OnConnectedToServer is called for each client that conntects to the server
	void  OnConnectedToServer ()
	{
		// Spawn a new player prefab for each new connection
		spawn.SendMessage("InstantiatePlayer", playerName, SendMessageOptions.DontRequireReceiver);
	}
}
