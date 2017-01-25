using UnityEngine;
using System.Net;
using System.Collections.Generic;
using UnityOSC;

public class OSCSender  {

	public static string PDClient = "PD";
	public static string jumpCmd = "/dk/jump";
	public static string enemyCollisionCmd = "/dk/enemyCollision";
	public static string hammerTimeCmd = "/dk/hammerTime";
	public static string movingCmd = "/dk/moving";
	public static string floorCmd = "/dk/floor";
	public static string positionCmd = "/dk/position";
	public static string hitCmd = "/dk/hit"; // f f

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
