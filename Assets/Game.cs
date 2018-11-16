using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Game : MonoBehaviour {
    GameObject[,] masume = new GameObject[4, 3];
    // koma : 味方の駒の頭文字を小文字で格納(zなど) 敵の駒は大文字で格納(Zなど)
    char[,] koma = new char[4, 3];
    GameObject[] hiyoko = new GameObject[2];
    GameObject[] zou = new GameObject[2];
    GameObject[] kirin = new GameObject[2];
    GameObject[] lion = new GameObject[2];
    public bool movemode = false;
    GameObject moving;

    // Use this for initialization
    void Start() {
        masuinit();
        komainit();
    }

    // Update is called once per frame
    void Update() {

    }

    public void masuinit() {
        for (int x = 0; x < 4; x++) {
            for (int y = 0; y < 3; y++) {
                masume[x, y] = GameObject.Find(x + "," + y);
            }
        }
    }

    public void komainit() {
        for (int x = 0; x < 2; x++) {
            for (int y = 0; y < 3; y++) {
                koma[x, y] = (char)0;
            }
        }
        koma[2, 0] = (char)0;
        koma[2, 1] = 'h';
        koma[2, 2] = (char)0;

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

    public void komaclick(string komaname) {
        if (komaname.Substring(0, 1) == "h") {
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
        if (movemode == true) {
            int x = int.Parse(mess.Substring(0, 1));
            int y = int.Parse(mess.Substring(1, 1));
            Move(x, y);
        }
    }

    public void Move(int x, int y) {
        moving.transform.SetParent(masume[x, y].transform);
        moving.transform.localPosition = new Vector3(0, 0);
        movemode = false;
    }
}