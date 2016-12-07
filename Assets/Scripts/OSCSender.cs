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

	OSCHandler instance;


	public OSCSender (string address, int port){
		OSCHandler.Instance.CreateClient("PD", IPAddress.Parse(address), port);
		// _entities = RDUtils.SceneManager.Entities;
		// internalEntity = _entities[0].GetComponent<RDUtils.Entity>();
	}
}
