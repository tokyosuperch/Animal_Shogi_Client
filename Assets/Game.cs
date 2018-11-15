using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Game : MonoBehaviour {
    
    GameObject[,] masume = new GameObject[4, 3];
    GameObject[] hiyoko = new GameObject[2];
    GameObject[] zou = new GameObject[2];
    GameObject[] kirin = new GameObject[2];
    GameObject[] lion = new GameObject[2];
    public bool movemode = false;
    public bool[,] ismasuclick = new bool[4, 3];
    GameObject moving;

	// Use this for initialization
	void Start () {
        for (int x = 0; x < 4; x++){
            for(int y = 0; y < 3; y++) {
                masume[x, y] = GameObject.Find(x + "," + y);
                ismasuclick[x, y] = false;
            }
        }
        for (int i = 0; i < 2; i++) {
            hiyoko[i] = GameObject.Find("hiyoko" + i);
        }
        for (int i = 0; i < 2; i++) {
            zou[i] = GameObject.Find("zou" + i);
        }
        for (int i = 0; i < 2; i++) {
            kirin[i] = GameObject.Find("kirin" + i);
        }
        for (int i = 0; i < 2; i++) {
            lion[i] = GameObject.Find("lion" + i);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (movemode == true) {
            for (int x = 0; x < 4; x++) {
                for (int y = 0; y < 3; y++) {
                    if (ismasuclick[x, y] == true) {
                        moving.transform.parent = masume[x, y].transform;
                        moving.transform.localPosition = new Vector3(0, 0);
                        movemode = false;
                        ismasuclick[x, y] = false;
                    }
                }
            }
        }
    }

    public void komaclick(string komaname) {
        if (komaname.Substring(0,1) == "h") {
            moving = hiyoko[int.Parse(komaname.Substring(1, 1))];
        }
        if (komaname.Substring(0, 1) == "z") {
            moving = zou[int.Parse(komaname.Substring(1, 1))];
        }
        if (komaname.Substring(0, 1) == "k") {
            moving = kirin[int.Parse(komaname.Substring(1, 1))];
        }
        if (komaname.Substring(0, 1) == "l") {
            moving = lion[int.Parse(komaname.Substring(1, 1))];
        }
        movemode = true;
    }

    public void masuclick(string mess) {
        int x = int.Parse(mess.Substring(0, 1));
        int y = int.Parse(mess.Substring(1, 1));
        if (movemode == true) {
            ismasuclick[x, y] = true;
        }
    }

    private void Move() {

    }
}