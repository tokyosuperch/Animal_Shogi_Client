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
    string destroyedname;
    bool? iamsente = null;
    float timeElapsed;
    string komatype = "";
    bool loginmode = false;
    bool enemymove = false;
    bool nameconvert = false;
    bool msgshow = false;
    string usermsg;
    // メッセージを管理するリスト
    private List<string> messages = new List<string>();
    private string currentMessage = string.Empty;
    // Server
    NetworkStream stream = null;
    bool isStopReading = false;
    byte[] readbuf;
    GameObject textObject;
    Text statusText;

    // Use this for initialization
    private IEnumerator Start() {
        MasuInit();
        KomaInit();
        TextInit();
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
        MessageHandler(message);
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
        // メインスレッドでしか呼べない関数対策
        if (msgshow) {
            if (usermsg == "") {
                textObject.SetActive(false);
                statusText.text = usermsg;
            } else {
                textObject.SetActive(true);
                statusText.text += usermsg;
            }
            msgshow = false;
        }
        if (nameconvert) {
            enemymove = true;
            KomaClick(NameConverter(komatype, beforex, beforey));
            if (NameConverter("", afterx, aftery) != null) {
                KomaClick(NameConverter("", afterx, aftery));
            } else {
                MasuClick(afterx.ToString() + aftery.ToString());
            }
            enemymove = false;
            nameconvert = false;
        }
    }

    public void PlayerInit(string message) {
        if (message.Substring(message.IndexOf("Your_Turn:") + 10, 1) == "+") {
            iamsente = true;
        } else if (message.Substring(message.IndexOf("Your_Turn:") + 10, 1) == "-") {
            iamsente = false;
        }
        usermsg = "";
        msgshow = true;
    }

    public void TextInit() {
        textObject = GameObject.Find("Text");
        statusText = textObject.GetComponent<Text>();
    }

    public void MasuInit() {
        for (int x = 0; x < 4; x++) {
            for (int y = 0; y < 3; y++) {
                masume[x, y] = GameObject.Find(x + "," + y);
            }
        }
        mykoma = GameObject.Find("mykoma");
        enemykoma = GameObject.Find("enemykoma");
    }

    public void KomaInit() {
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

    public void MessageHandler (string msg) {
        string[] line = msg.Split('\n');
        bool showaction = false;
        for (int i = 0; i < line.Length; i++) {
            if (line[i] != "") Debug.Log(line[i]);
            if (line[i].IndexOf("Your_Turn:") != -1) {
                PlayerInit(line[i]);
                loginmode = true;
            }
            if ((line[i].StartsWith("+") && iamsente == false) || (line[i].StartsWith("-") && iamsente == true)) {
                if (line[i].Substring(2, 1) == "*") {
                    if (iamsente == null) {
                        usermsg = "内部エラー";
                        showaction = true;
                    } else if (iamsente == true) {
                        komatype = line[i].Substring(1, 1).ToUpper();
                    } else {
                        komatype = line[i].Substring(1, 1).ToLower();
                    }
                    beforex = -1;
                    beforey = -1;
                } else {
                    komatype = "";
                    beforex = AxisConverter(line[i].Substring(2, 1));
                    beforey = int.Parse(line[i].Substring(1, 1)) - 1;
                }
                afterx = AxisConverter(line[i].Substring(4,1));
                aftery = int.Parse(line[i].Substring(3,1)) - 1;
                // メインスレッドしか名前変換ができないため
                nameconvert = true;
            }
            if (line[i].StartsWith("#")) {
                usermsg += line[i] + "\n";
                showaction = true;
            }
        }
        if (showaction) {
            msgshow = true;
            showaction = false;
        }
    }

    public int AxisConverter(string letter) {
        if (letter == "a") return 0;
        if (letter == "b") return 1;
        if (letter == "c") return 2;
        if (letter == "d") return 3;
        return -1;
    }

    public string ExternalAxis(int x) {
        if (x == 0) return "a";
        if (x == 1) return "b";
        if (x == 2) return "c";
        if (x == 3) return "d";
        return null;
    }

    public string NameConverter(string type, int x, int y) {
        if (x == -1 && y == -1) {
            komatype = type;
            for (int i = 0; i < 2; i++) {
                if (hiyoko[i].name.Length < 3 && hiyoko[i].name.Substring(0,1) == type.Substring(0, 1)) return "h" + i;
                if (zou[i].name.Length < 3 && zou[i].name.Substring(0, 1) == type.Substring(0, 1)) return "z" + i;
                if (kirin[i].name.Length < 3 && kirin[i].name.Substring(0, 1) == type.Substring(0 ,1)) return "k" + i;
                if (lion[i].name.Length < 3 && lion[i].name.Substring(0, 1) == type.Substring(0, 1)) return "l" + i;
                if (hiyoko[i].name.Length < 3 && hiyoko[i].name.Substring(0, 1) == type.Substring(0, 1)) return "h" + i;
                if (zou[i].name.Length < 3 && zou[i].name.Substring(0, 1) == type.Substring(0, 1)) return "z" + i;
                if (kirin[i].name.Length < 3 && kirin[i].name.Substring(0, 1) == type.Substring(0, 1)) return "k" + i;
                if (lion[i].name.Length < 3 && lion[i].name.Substring(0, 1) == type.Substring(0, 1)) return "l" + i;
            }
        } else {
            for (int i = 0; i < 2; i++) {
                if (hiyoko[i].name == "h" + x + y) {
                    komatype = "h";
                    return "h" + i;
                }
                if (zou[i].name == "z" + x + y) {
                    komatype = "z";
                    return "z" + i;
                }
                if (kirin[i].name == "k" + x + y) {
                    komatype = "k";
                    return "k" + i;
                }
                if (lion[i].name == "l" + x + y) {
                    komatype = "l";
                    return "l" + i;
                }
                if (hiyoko[i].name == "H" + x + y) {
                    komatype = "H";
                    return "h" + i;
                }
                if (zou[i].name == "Z" + x + y) {
                    komatype = "Z";
                    return "z" + i;
                }
                if (kirin[i].name == "K" + x + y) {
                    komatype = "K";
                    return "k" + i;
                }
                if (lion[i].name == "L" + x + y) {
                    komatype = "L";
                    return "l" + i;
                }
            }
        }
        return null;
    }

    public void KomaClick(string komaname) {
        if(loginmode) {
            StartCoroutine(SendMessage("LOGIN:supachan_Client"));
            loginmode = false;
        }
        destroyedname = "";
        GameObject temporary = null;
        temporary = ObjectReturner(komaname.Substring(0, 1), int.Parse(komaname.Substring(1, 1)));
        if (temporary == null) { // エラー時の暴走防止
            usermsg = "内部エラー";
            msgshow = true;
            nameconvert = false;
        }
        if (iamsente == null) {
            usermsg = "内部エラー";
            msgshow = true;
        } else {
            char[] initial = temporary.name.Substring(0, 1).ToCharArray();
            if (senteturn && iamsente == true) {
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
                    destroyedname = temporary.name;
                    MasuClick(temporary.name.Substring(1, 1) + temporary.name.Substring(2, 1));
                }
            } else if (!senteturn && iamsente == false) {
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
                    destroyedname = temporary.name;
                    MasuClick(temporary.name.Substring(1, 1) + temporary.name.Substring(2, 1));
                }
            } else if (enemymove && iamsente == true && !senteturn) {
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
                    destroyedname = temporary.name;
                    MasuClick(temporary.name.Substring(1, 1) + temporary.name.Substring(2, 1));
                }
            } else if (enemymove && iamsente == false && senteturn) {
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
                    destroyedname = temporary.name;
                    MasuClick(temporary.name.Substring(1, 1) + temporary.name.Substring(2, 1));
                }
            }
        }
        if (moving != null) {
            if (moving.name.Length < 3) {
                komauchimode = true;
                komatype = moving.name.Substring(0, 1);
            } else {
                komauchimode = false;
            }
        }
    }

    public void MasuClick(string mess) {
        if (movemode == true) {
            afterx = int.Parse(mess.Substring(0, 1));
            aftery = int.Parse(mess.Substring(1, 1));
            if (CanMove()) {
                if (destroyedname != "") Destroy(destroyedname);
                Move();
            }
        }
    }

    public void Move() {
        bool narimove = false;
        moving.transform.SetParent(masume[afterx, aftery].transform);
        moving.transform.localPosition = new Vector3(0, 0);
        string temp = moving.name;
        moving.name = temp.Substring(0, 1) + afterx + aftery;
        if ((senteturn == true && afterx == 0 && temp.StartsWith("h", false, null) || senteturn == false && afterx == 3 && temp.StartsWith("H", false, null)) && komauchimode == false) {
            for (int i = 0; i <= 1; i++) {
                if (moving == hiyoko[i]) {
                    Image img = hiyoko[i].GetComponent<Image>();
                    img.sprite = nimage;
                    narimove = true;
                }
            }
        }
        movemode = false;
        string extmsg = "";
        if (senteturn && iamsente == true) extmsg = "+";
        if (!senteturn && iamsente == false) extmsg = "-";
        if (komauchimode == true) {
            extmsg += komatype.ToLower() + "*" + (aftery + 1) + ExternalAxis(afterx);
        } else {
            extmsg += (beforey + 1) + ExternalAxis(beforex) + (aftery + 1) + ExternalAxis(afterx);
        }
        for (int i = 0; i <= 1; i++) {
            if (moving == hiyoko[i]) {
                if (hiyokonari[i] == false && narimove) {
                    extmsg += "+";
                    hiyokonari[i] = true;
                }
            }
        }
        StartCoroutine(SendMessage(extmsg));
        senteturn = !senteturn;
    }

    public bool CanMove() {
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

    public GameObject ObjectReturner(string type, int num) {
        if (type.StartsWith("h", true, null)) return hiyoko[num];
        if (type.StartsWith("z", true, null)) return zou[num];
        if (type.StartsWith("k", true, null)) return kirin[num];
        if (type.StartsWith("l", true, null)) return lion[num];
        return null;
    }

    public void Destroy(string name) {
        string converted = NameConverter("", int.Parse(name.Substring(1, 1)), int.Parse(name.Substring(2, 1)));
        GameObject destroyed = ObjectReturner(converted.Substring(0,1), int.Parse(converted.Substring(1,1)));
        GameObject destination;
        int okiba, angle;
        if (senteturn) {
            destination = mykoma;
            okiba = mykomanum;
            angle = 0;
            destroyed.name = name.Substring(0, 1).ToLower() + converted.Substring(1, 1);
            mykomanum++;
        } else {
            destination = enemykoma;
            angle = 180;
            okiba = enemykomanum;
            destroyed.name = name.Substring(0, 1).ToUpper() + converted.Substring(1, 1);
            enemykomanum--;
        }
        if (name.StartsWith("h", true, null)) {
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