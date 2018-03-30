using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IncreasableList : MonoBehaviour {

    [SerializeField]
    GameObject sampleRow;
    [SerializeField]
    Transform addenRow;

	// Use this for initialization
	void Start () {
        sampleRow.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
