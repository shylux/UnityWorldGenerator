using UnityEngine;
using System.Collections;

// SpawnBehaviour controls the network instantiation of players and 
// is attached to the empty spawn gameobject
public class PlayerBehaviour1 : MonoBehaviour 
{	
	public GameObject spawnPrefab;
	
	// InstantiatePlayer function will be called for myself and once for each new player connected
	void InstantiatePlayer(string playerName)
	{		
		// Version 1: Clone prefab with Network.Instantiate
		// Disadvantage: I don't have access to the objects created on connected clients
		GameObject newPlayer = (GameObject)Network.Instantiate(spawnPrefab, 
		                                                       transform.position, 
		                                                       transform.rotation, 0);
		newPlayer.name = "Player_" + playerName;
		newPlayer.GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value);
		Debug.Log("Joined: " + playerName);
		
		// Version 2: Clone prefab with an RPC call
		// Advantage: I can pass a custom name and color but have to pass also a network viewID
//		GetComponent<NetworkView>().RPC("SpawnBoxRPC", 
//		                                RPCMode.AllBuffered,
//		                                Network.AllocateViewID(),
//		                                playerName, 
//		                                Random.value, Random.value, Random.value);
	}
	
	
	// SpawnBoxRPC does a local instantiation for networkview object
    [RPC] void SpawnBoxRPC(NetworkViewID viewID, string playerName, float r, float g, float b) 
	{
        // Instantiate object to the network in the position of spawn
		GameObject newPlayer = (GameObject)Instantiate(spawnPrefab, 
		                                               transform.position, 
		                                               transform.rotation);
		// Apply custom paramters
		newPlayer.name = "Player_" + playerName;
		newPlayer.GetComponent<Renderer>().material.color = new Color(r,g,b);
		newPlayer.GetComponent<NetworkView>().viewID = viewID;
		Debug.Log("Joined: " + playerName);
    }
	
	// OnPlayerDisconnected function will be called when player is disconnected
	void OnPlayerDisconnected (NetworkPlayer player)
	{
		// Removing player from network and scene
		Network.RemoveRPCs(player, 0);
		Network.DestroyPlayerObjects(player);
	}
}
