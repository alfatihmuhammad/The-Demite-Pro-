using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CymaticLabs.Unity3D.Amqp;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour {

    public Text ghost_name, speed, dv, max_hp, description;
    public Text word, spoken, damage;
    public Image ghostImage;
    public Sprite[] ghost = new Sprite[7];

    string id ;
    public int count;
    int limit;

    public GameObject rowGo;
    public Transform rowGoParent;

    //List<GameObject> listGO = new List<GameObject>();
    public GameObject[] duplikatnya = new GameObject[8];
    bool clicked = false;

    void Start()
    {
        id = Guid.NewGuid().ToString();
        ReadGhost();

        limit = 0;
        count = 0;
        ReadAssessment(0); //nge read baris 1 aja soalnya proses ternyata terakhir, jadi gabisa ngitung count
        limit++;
        
    }

    public void ClickReadAssessment() {  // nge read baris2 selanjutnya
        //int i = 0;
        if (clicked == false) {
            do
            {
                ReadAssessment(limit);
                limit++;
                //i++;
                Debug.Log("Lim " + limit + " COUNT " + count);
            }
            while (limit < count); 
        }        
        clicked = true;
    }

    public void ReadGhost() {
        AmqpControllerScript.amqpControl.exchangeSubscription.Handler = Process;

        GhostCatchedRequestJson request = new GhostCatchedRequestJson();
        request.id = id;
        request.id_ghost = PlayerPrefs.GetInt("id_ghost");
        request.type = "read_ghost";

        string requestToJson = JsonUtility.ToJson(request);
        AmqpClient.Publish(AmqpControllerScript.amqpControl.requestExchange, AmqpControllerScript.amqpControl.requestRoutingKey, requestToJson);

        Debug.Log("READ GHOST");
    }

    public void ReadAssessment(int limit)
    {
        AmqpControllerScript.amqpControl.exchangeSubscription.Handler = Process;

        ScoreRequestJson request = new ScoreRequestJson();
        request.id = id;
        request.type = "read_assessment";
        request.limit = limit.ToString();
        request.id_catch = PlayerPrefs.GetString("id_catch");
        //request.id_catch = "7"; //GANTI

        string requestToJson = JsonUtility.ToJson(request);
        AmqpClient.Publish(AmqpControllerScript.amqpControl.requestExchange, AmqpControllerScript.amqpControl.requestRoutingKey, requestToJson);

        Debug.Log("READ ASSESSMENT");
    }

    void Process(AmqpExchangeReceivedMessage received)
    {
        var receivedJson = System.Text.Encoding.UTF8.GetString(received.Message.Body);
        var msg = CymaticLabs.Unity3D.Amqp.SimpleJSON.JSON.Parse(receivedJson);

        if (msg != null)
        {
            string msgId = (string)msg["id"];
            if (msgId == id)
            {
                string type = (string)msg["type"];
                if (type == "read_ghost")
                {
                    Debug.Log("Process READ GHOST");
                    LoadGhostDescription(msg);
                }
                else if (type == "read_assessment")
                {
                    Debug.Log("Process READ ASSESSMENT");
                    LoadScoreAssessment(msg, limit);
                    Debug.Log("limit "+limit);
                }
            }
        }
    }

    public void LoadGhostDescription(CymaticLabs.Unity3D.Amqp.SimpleJSON.JSONNode msg)
    {
        ghost_name.text = (string)msg["data"][0];
        speed.text = (string)msg["data"][1];
        dv.text = (string)msg["data"][2];
        max_hp.text = (string)msg["data"][3];
        description.text = (string)msg["data"][4];
        ghostImage.sprite = ghost[PlayerPrefs.GetInt("id_ghost")];
        //Debug.Log(PlayerPrefs.GetString("id_user"));
    }

    public void LoadScoreAssessment(CymaticLabs.Unity3D.Amqp.SimpleJSON.JSONNode msg, int n)
    {
        count = msg["count"];
        Debug.Log("count "+count );

        duplikatnya[n] = Instantiate(rowGo, rowGoParent);
        Text text_word = duplikatnya[n].transform.Find("Word").gameObject.GetComponent<Text>();
        if(text_word == null)
            Debug.Log("NULLLLLLL ");
        text_word.text = (string)msg["data"][0];
        Text text_spoken = duplikatnya[n].transform.Find("Spoken").gameObject.GetComponent<Text>();
        text_spoken.text = (string)msg["data"][1];
        Text text_damage = duplikatnya[n].transform.Find("Damage").gameObject.GetComponent<Text>();
        text_damage.text = (string)msg["data"][2];
    }

    public void BackToMap() {
        SceneManager.LoadScene("Map");
    }

    [Serializable]
    public class ScoreRequestJson
    {
        public string id;
        public string type;
        //public string id_user;
        public string id_catch;
        public string limit;
        //public string word;
        //public string spoken;
        //public int damage;
    }

    public class GhostCatchedRequestJson
    {
        public string id;
        public string type;
        public int id_ghost;
        //public string ghost_speed;
        //public string destruct_val;
        //public string max_health;
        //public string description;
    }
}
