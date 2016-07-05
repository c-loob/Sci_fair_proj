using UnityEngine;
using System.Collections;

public class renderer : MonoBehaviour {
    public Renderer rend;
	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();
        rend.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
