using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Net;

public class Game : MonoBehaviour {
    GameObject[,] masume = new GameObject[4, 3];
    GameObject[] hiyoko = new GameObject[2];
    bool[] hiyokonari = new bool[2];
    GameObject[] zou = new GameObject[2];
    GameObject[] kirin = new GameObject[2];
    GameObject[] lion = new GameObject[2];
    bool movemode = false;
    int beforex, beforey;
    int afterx, aftery;
    GameObject moving = null;
    GameObject mykoma, enemykoma;
    int mykomanum = 0;
    int enemykomanum = 0;
    bool senteturn = true;
    bool komauchimode = false;
    public Sprite himage;
    public Sprite nimage;
    GameObject destroyed;
    bool? iamsente = null;
    float timeElapsed;
    // メッセージを管理するリスト
    private List<string> messages = new List<string>();
    private string currentMessage = string.Empty;
    // Server
    NetworkStream stream = null;
    bool isStopReading = false;
    byte[] readbuf;

    // Use this for initialization
    private IEnumerator Start() {
        masuinit();
        komainit();
        keepaliver();
        Debug.Log("START START");
        readbuf = new byte[1024];
        while (true) {
            if (!isStopReading) { StartCoroutine(ReadMessage()); }
            yield return null;
        }
    }
    private IEnumerator SendMessage(string message) {
        Debug.Log("START SendMessage:" + message);

        if (stream == null) {
            stream = GetNetworkStream();
        }
        //サーバーにデータを送信する
        Encoding enc = Encoding.UTF8;
        byte[] sendBytes = enc.GetBytes(message + "\n");
        //データを送信する
        stream.Write(sendBytes, 0, sendBytes.Length);
        yield break;
    }

    private IEnumerator ReadMessage() {
        stream = GetNetworkStream();
        // 非同期で待ち受けする
        stream.BeginRead(readbuf, 0, readbuf.Length, new AsyncCallback(ReadCallback), null);
        isStopReading = false;
        yield return null;
    }

    public void ReadCallback(IAsyncResult ar) {
        Encoding enc = Encoding.UTF8;
        stream = GetNetworkStream();
        int bytes = stream.EndRead(ar);
        string message = enc.GetString(readbuf, 0, bytes);
        // message = message.Replace("\r", "").Replace("\n", "");
        isStopReading = false;
        messages.Add(message);
        Debug.Log(message);
        if (message.IndexOf("BEGIN Game_Summary") != -1 || message.IndexOf("Game_ID") != -1) playerinit(message);
    }

    private NetworkStream GetNetworkStream() {
        if (stream != null && stream.CanRead) {
            return stream;
        }

        string ipOrHost = "shogi.keio.app";
        int port = 80;

        //TcpClientを作成し、サーバーと接続する
        TcpClient tcp = new TcpClient(ipOrHost, port);
        Debug.Log("success conn server");

        //NetworkStreamを取得する
        return tcp.GetStream();
    }
    
    private void keepaliver() {
        StartCoroutine(KeepAlive());
    }

    private IEnumerator KeepAlive() {
        while (true) {
            StartCoroutine(SendMessage("Keep-Alive"));
            yield return new WaitForSeconds(15);
        }
    }

    // Update is called once per frame
    void Update() {

    }

    public void playerinit(string message) {
        if (message.IndexOf("Your_Turn:") != -1) {
            if (message.Substring(message.IndexOf("Your_Turn:") + 10, 1) == "+") {
                Debug.Log("先手");
                iamsente = true;
            } else if (message.Substring(message.IndexOf("Your_Turn:") + 10, 1) == "-") {
                Debug.Log("後手");
                iamsente = false;
            }
        }
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
            hiyokonari[i] = false;
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
        StartCoroutine(SendMessage("LOGIN:supachan_Client(Debug)"));
        destroyed = null;
        GameObject temporary = null;
        if (komaname.StartsWith("h", false, null)) {
            temporary = hiyoko[int.Parse(komaname.Substring(1, 1))];
        } else if (komaname.StartsWith("z", false, null)) {
            temporary = zou[int.Parse(komaname.Substring(1, 1))];
        } else if (komaname.StartsWith("k", false, null)) {
            temporary = kirin[int.Parse(komaname.Substring(1, 1))];
        } else if (komaname.StartsWith("l", false, null)) {
            temporary = lion[int.Parse(komaname.Substring(1, 1))];
        }
        if (iamsente == null) {
        } else if (senteturn && iamsente == true) {
            char[] initial = temporary.name.Substring(0, 1).ToCharArray();
            if (char.IsLower(initial[0])) {
                moving = temporary;
                if (moving.name.Length >= 3) {
                    beforex = int.Parse(moving.name.Substring(1, 1));
                    beforey = int.Parse(moving.name.Substring(2, 1));
                } else {
                    beforex = -1;
                    beforey = -1;
                }
                movemode = true;
            } else if (movemode == true && komauchimode == false && temporary.name.Length > 1) {
                destroyed = temporary;
                masuclick(destroyed.name.Substring(1, 1) + destroyed.name.Substring(2, 1));
            }
        } else if (!senteturn && iamsente == false) {
            char[] initial = temporary.name.Substring(0, 1).ToCharArray();
            if (char.IsUpper(initial[0])) {
                moving = temporary;
                if (moving.name.Length >= 3) {
                    beforex = int.Parse(moving.name.Substring(1, 1));
                    beforey = int.Parse(moving.name.Substring(2, 1));
                } else {
                    beforex = -1;
                    beforey = -1;
                }
                movemode = true;
            } else if (movemode == true && komauchimode == false && temporary.name.Length > 1) {
                destroyed = temporary;
                masuclick(destroyed.name.Substring(1, 1) + destroyed.name.Substring(2, 1));
            }
        }
        if (moving != null) {
            if (moving.name.Length == 1) { komauchimode = true; } else { komauchimode = false; }
        }
    }

    public void masuclick(string mess) {
        if (movemode == true && moving.transform.root.gameObject != mykoma && moving.transform.root.gameObject != enemykoma) {
            afterx = int.Parse(mess.Substring(0, 1));
            aftery = int.Parse(mess.Substring(1, 1));
            if (canmove()) {
                Move();
                if (destroyed != null) Destroy(destroyed);
            }
        }
    }

    public void Move() {
        moving.transform.SetParent(masume[afterx, aftery].transform);
        moving.transform.localPosition = new Vector3(0, 0);
        string temp = moving.name;
        moving.name = temp.Substring(0, 1) + afterx + aftery;
        if (senteturn == true && afterx == 0 && temp.StartsWith("h", false, null)) {
            for (int i = 0; i <= 1; i++) {
                if (moving == hiyoko[i]) {
                    Image img = hiyoko[i].GetComponent<Image>();
                    img.sprite = nimage;
                    hiyokonari[i] = true;
                }
            }
        }
        if (senteturn == false && afterx == 3 && temp.StartsWith("H", false, null)) {
            for (int i = 0; i <= 1; i++) {
                if (moving == hiyoko[i]) {
                    Image img = hiyoko[i].GetComponent<Image>();
                    img.sprite = nimage;
                    hiyokonari[i] = true;
                }
            }
        }
        movemode = false;
        if (senteturn) { senteturn = false; } else { senteturn = true; }
    }

    public bool canmove() {
        if (komauchimode) return true;
        if (afterx <= beforex + 1 && afterx >= beforex - 1 && aftery <= beforey + 1 && aftery >= beforey - 1) {
            if (moving.name.StartsWith("h", false, null)) {
                for (int i = 0; i <= 1; i++) {
                    if (moving == hiyoko[i]) {
                        if (hiyokonari[i]) {
                            if (!(afterx == beforex + 1 && (aftery == beforey - 1 || aftery == beforey + 1))) return true;
                        } else {
                            if (afterx == beforex - 1 && aftery == beforey) return true;
                        }
                    }
                }
            }
            if (moving.name.StartsWith("H", false, null)) {
                for (int i = 0; i <= 1; i++) {
                    if (moving == hiyoko[i]) {
                        if (hiyokonari[i]) {
                            if (!(afterx == beforex - 1 && (aftery == beforey - 1 || aftery == beforey + 1))) return true;
                        } else {
                            if (afterx == beforex + 1 && aftery == beforey) return true;
                        }
                    }
                }
            }
            if (moving.name.StartsWith("z", true, null)) {
                if (!(afterx == beforex || aftery == beforey)) return true;
            }
            if (moving.name.StartsWith("k", true, null)) {
                if (afterx == beforex || aftery == beforey) return true;
            }
            if (moving.name.StartsWith("l", true, null)) return true;
            return false;
        } else {
            return false;
        }
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
        if (destroyed.name.StartsWith("h", true, null)) {
            for (int i = 0; i <= 1; i++) {
                if (destroyed == hiyoko[i] && hiyokonari[i] == true) {
                    Image img = hiyoko[i].GetComponent<Image>();
                    hiyokonari[i] = false;
                    img.sprite = himage;
                }
            }
        }
        destroyed.transform.SetParent(destination.transform);
        destroyed.transform.localPosition = new Vector3(okiba * 150, 0);
        destroyed.transform.rotation = new Quaternion(0, 0, angle, 0);
    }
}