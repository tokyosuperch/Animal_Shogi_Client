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
    int beforex, beforey;
    int afterx, aftery;
    GameObject moving;
    GameObject mykoma, enemykoma;
    int mykomanum = 0;
    int enemykomanum = 0;
    bool myturn = true;

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
        mykoma = GameObject.Find("mykoma");
        enemykoma = GameObject.Find("enemykoma");
    }

    public void komainit() {

        for (int i = 0; i < 2; i++) {
            hiyoko[i] = GameObject.Find("hiyoko" + i);
        }
        hiyoko[0].name = "h21";
        hiyoko[1].name = "H11";
        for (int i = 0; i < 2; i++) {
            zou[i] = GameObject.Find("zou" + i);
        }
        zou[0].name = "z30";
        zou[1].name = "Z02";
        for (int i = 0; i < 2; i++) {
            kirin[i] = GameObject.Find("kirin" + i);
        }
        kirin[0].name = "k32";
        kirin[1].name = "K00";
        for (int i = 0; i < 2; i++) {
            lion[i] = GameObject.Find("lion" + i);
        }
        lion[0].name = "l31";
        lion[1].name = "L01";
    }

    public void komaclick(string komaname) {
        GameObject destroyed, temporary;
        destroyed = null;
        if (myturn) {
            if (komaname.Substring(0, 1) == "h") {
                temporary = hiyoko[int.Parse(komaname.Substring(1, 1))];
                if (temporary.name.Substring(0, 1) == "h") {
                    moving = temporary;
                    movemode = true;
                } else if (movemode) {
                    destroyed = hiyoko[int.Parse(komaname.Substring(1, 1))];
                    masuclick(destroyed.name.Substring(1, 1) + destroyed.name.Substring(2, 1));
                }
            }
            if (komaname.Substring(0, 1) == "z") {
                temporary = zou[int.Parse(komaname.Substring(1, 1))];
                if (temporary.name.Substring(0, 1) == "z") {
                    moving = temporary;
                    movemode = true;
                } else if (movemode) {
                    destroyed = zou[int.Parse(komaname.Substring(1, 1))];
                    masuclick(destroyed.name.Substring(1, 1) + destroyed.name.Substring(2, 1));
                }
            }
            if (komaname.Substring(0, 1) == "k") {
                temporary = kirin[int.Parse(komaname.Substring(1, 1))];
                if (temporary.name.Substring(0, 1) == "k") {
                    moving = temporary;
                    movemode = true;
                } else if (movemode) {
                    destroyed = kirin[int.Parse(komaname.Substring(1, 1))];
                    masuclick(destroyed.name.Substring(1, 1) + destroyed.name.Substring(2, 1));
                }
            }
            if (komaname.Substring(0, 1) == "l") {
                temporary = lion[int.Parse(komaname.Substring(1, 1))];
                if (temporary.name.Substring(0, 1) == "l") {
                    moving = temporary;
                    movemode = true;
                } else if (movemode) {
                    destroyed = lion[int.Parse(komaname.Substring(1, 1))];
                    masuclick(destroyed.name.Substring(1, 1) + destroyed.name.Substring(2, 1));
                }
            }
            if (destroyed != null) Destroy(destroyed);
        } else {
            if (komaname.Substring(0, 1) == "h") {
                temporary = hiyoko[int.Parse(komaname.Substring(1, 1))];
                if (temporary.name.Substring(0, 1) == "H") {
                    moving = temporary;
                    movemode = true;
                } else if (movemode) {
                    destroyed = hiyoko[int.Parse(komaname.Substring(1, 1))];
                    masuclick(destroyed.name.Substring(1, 1) + destroyed.name.Substring(2, 1));
                }
            }
            if (komaname.Substring(0, 1) == "z") {
                temporary = zou[int.Parse(komaname.Substring(1, 1))];
                if (temporary.name.Substring(0, 1) == "Z") {
                    moving = temporary;
                    movemode = true;
                } else if (movemode) {
                    destroyed = zou[int.Parse(komaname.Substring(1, 1))];
                    masuclick(destroyed.name.Substring(1, 1) + destroyed.name.Substring(2, 1));
                }
            }
            if (komaname.Substring(0, 1) == "k") {
                temporary = kirin[int.Parse(komaname.Substring(1, 1))];
                if (temporary.name.Substring(0, 1) == "K") {
                    moving = temporary;
                    movemode = true;
                } else if (movemode) {
                    destroyed = kirin[int.Parse(komaname.Substring(1, 1))];
                    masuclick(destroyed.name.Substring(1, 1) + destroyed.name.Substring(2, 1));
                }
            }
            if (komaname.Substring(0, 1) == "l") {
                temporary = lion[int.Parse(komaname.Substring(1, 1))];
                if (temporary.name.Substring(0, 1) == "L") {
                    moving = temporary;
                    movemode = true;
                } else if (movemode) {
                    destroyed = lion[int.Parse(komaname.Substring(1, 1))];
                    masuclick(destroyed.name.Substring(1, 1) + destroyed.name.Substring(2, 1));
                }
            }
            if (destroyed != null) Destroy(destroyed);
        }
        // beforex = int.Parse(moving.name.Substring(1, 1));
        // beforey = int.Parse(moving.name.Substring(1, 2));
    }

    public void masuclick(string mess) {
        if (movemode == true && moving.transform.root.gameObject != mykoma && moving.transform.root.gameObject != enemykoma) {
            afterx = int.Parse(mess.Substring(0, 1));
            aftery = int.Parse(mess.Substring(1, 1));
            Move();
        }
    }

    public void Move() {
        moving.transform.SetParent(masume[afterx, aftery].transform);
        moving.transform.localPosition = new Vector3(0, 0);
        string temp = moving.name;
        moving.name = temp.Substring(0, 1) + afterx + aftery;
        movemode = false;
        if (myturn) { myturn = false; } else { myturn = true; }
    }

    public void Destroy(GameObject destroyed) {
        GameObject destination;
        int okiba, angle;
        if (myturn) {
            destination = enemykoma;
            angle = 180;
            okiba = enemykomanum;
            destroyed.name = destroyed.name.Substring(0, 1).ToUpper();
            enemykomanum--;
        } else {
            destination = mykoma;
            okiba = mykomanum;
            angle = 0;
            destroyed.name = destroyed.name.Substring(0, 1).ToLower();
            mykomanum++;
        }
        destroyed.transform.SetParent(destination.transform);
        destroyed.transform.localPosition = new Vector3(okiba * 150, 0);
        destroyed.transform.rotation = new Quaternion(0, 0, angle, 0);
    }
}