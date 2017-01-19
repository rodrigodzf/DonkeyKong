using UnityEngine;
using System.Net;
using System.Collections.Generic;
using UnityOSC;
using System;

public class OSCReceiver
{

	public static string throwcmd = "/throw";
	public static string notecmd = "/note";
	public OSCReceiver(string serverID, int port)
	{
		OSCHandler.Instance.CreateServer(serverID, port);
	}


	void ParseOSC(OSCServer server, OSCPacket packet)
	{
		Debug.Log("Recive Packet");
		// Debug.Log(String.Format("ADDRESS: {1} VALUE 0: {2}",
		// 											packet.Address, // OSC address
		// 											packet.Data[0].ToString())); //First data value

	}

}