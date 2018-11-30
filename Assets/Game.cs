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
    bool senteturn = true;
    bool komauchimode = false;

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
        GameObject destroyed = null;
        GameObject temporary = null;
        if (komaname.StartsWith("h", false, null)) {
            temporary = hiyoko[int.Parse(komaname.Substring(1, 1))];
        } else if (komaname.StartsWith("z", false, null)) {
            temporary = zou[int.Parse(komaname.Substring(1, 1))];
        } else if (komaname.StartsWith("k", false, null)) {
            temporary = kirin[int.Parse(komaname.Substring(1, 1))];
        } else if (komaname.StartsWith("l", false, null)){
            temporary = lion[int.Parse(komaname.Substring(1, 1))];
        }
        if (senteturn) {
            char[] initial = temporary.name.Substring(0, 1).ToCharArray();
            if (char.IsLower(initial[0])) {
                moving = temporary;
                movemode = true;
            } else if (movemode == true && komauchimode == false && temporary.name.Length > 1) {
                destroyed = temporary;
                masuclick(destroyed.name.Substring(1, 1) + destroyed.name.Substring(2, 1));
            }
        } else {
            char[] initial = temporary.name.Substring(0, 1).ToCharArray();
            if (char.IsUpper(initial[0])) {
                moving = temporary;
                movemode = true;
            } else if (movemode == true && komauchimode == false && temporary.name.Length > 1) {
                destroyed = temporary;
                masuclick(destroyed.name.Substring(1, 1) + destroyed.name.Substring(2, 1));
            }
        }
        if (moving.name.Length == 1) { komauchimode = true; } else { komauchimode = false; }
        if (destroyed != null) Destroy(destroyed);
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
        if (senteturn) { senteturn = false; } else { senteturn = true; }
    }

    public void Destroy(GameObject destroyed) {
        GameObject destination;
        int okiba, angle;
        if (senteturn) {
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