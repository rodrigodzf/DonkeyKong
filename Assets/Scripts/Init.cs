using UnityEngine;
using System.Collections;

public class Init : MonoBehaviour {

    public PhysicsMaterial2D Metal;
    [SerializeField] private string address;
    [SerializeField] private int port;

    void Awake()
    {
        Debug.Log("onAwake");
        Application.runInBackground = true;
        foreach (var i in GameObject.FindObjectsOfType<BoxCollider2D>()){
            i.GetComponent<BoxCollider2D>().sharedMaterial = Metal;
            // i.GetComponent<BoxCollider2D>().isTrigger = true;
            BoxCollider2D bc = i.gameObject.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;
            bc.isTrigger = true;
            bc.size = new Vector2(0.18f,0.1f);
        }

        OSCSender sender = new OSCSender(address, port);
        

    }

}
