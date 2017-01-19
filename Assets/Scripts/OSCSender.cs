using UnityEngine;
using System.Net;
using System.Collections.Generic;
using UnityOSC;

public class OSCSender  {

	public static string PDClient = "PD";
	public static string jumpCmd = "/unity/jump";
	public static string enemyCollisionCmd = "/unity/enemyCollision";
	public static string hammerTimeCmd = "/unity/hammerTime";
	public static string movingCmd = "/unity/moving";
	public static string floorCmd = "/unity/floor";
	public static string positionCmd = "/unity/position";
	public static string hitCmd = "/unity/hit";

	static bool bConnected;
	OSCHandler instance;


	public OSCSender (string address, int port){
		OSCHandler.Instance.CreateClient("PD", IPAddress.Parse(address), port);
		bConnected = true;
		
		// _entities = RDUtils.SceneManager.Entities;
		// internalEntity = _entities[0].GetComponent<RDUtils.Entity>();
	}

	public static void SendMessage<T>(string clientId, string address, T values)
	{
		if (!bConnected) return;
		OSCHandler.Instance.SendMessageToClient(clientId, address, values);
	}

	public static void SendMessage<T>(string clientId, string address, List<T> values)
	{
		if (!bConnected) return;
		OSCHandler.Instance.SendMessageToClient(clientId, address, values);	
	}


}
