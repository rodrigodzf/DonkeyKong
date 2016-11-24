using UnityEngine;
using System.Collections;

public class AnimationEvents : MonoBehaviour {

	// Event Key Triggers
	void WinTransition()
	{
		Application.LoadLevel("Win");
	}

	void LoseTransition()
	{
		if (PlayerData.Instance.Lives > 0)
		{
			Application.LoadLevel(Application.loadedLevel);
		}
		else if (PlayerData.Instance.Lives == 0)
		{
			Application.LoadLevel("Lose");
		}
	}
}
