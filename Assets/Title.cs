using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour {

    public static string username = "supachan_Client";
    InputField namefield;

	// Use this for initialization
	void Start () {
        namefield = GameObject.Find("InputField").GetComponent<InputField>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void NameInput() {
        username = namefield.text;
        username = username.Replace(System.Environment.NewLine, "");
        Debug.Log(username);
    }


    public void StartButton() {
        SceneManager.LoadScene("Game");
    }
}
