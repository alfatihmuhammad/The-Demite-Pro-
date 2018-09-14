using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CymaticLabs.Unity3D.Amqp;
using System;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public Text ghost_name, speed, dv, max_hp;
    public Text word, spoken, damage;

    string id ;

    void Start()
    {
        id = Guid.NewGuid().ToString();
        ReadGhost();
        ReadAssessment();

        int count = 2;

        for (int i = 0; i < count; i++) {
            //ReadAssessment();
        }
    }
    
    public void ReadGhost() {
        AmqpControllerScript.amqpControl.exchangeSubscription.Handler = Process;

        GhostCatchedRequestJson request = new GhostCatchedRequestJson();
        request.id = id;
        request.type = "read_ghost";

        string requestToJson = JsonUtility.ToJson(request);
        AmqpClient.Publish(AmqpControllerScript.amqpControl.requestExchange, AmqpControllerScript.amqpControl.requestRoutingKey, requestToJson);

        Debug.Log("READ GHOST");
    }

    public void ReadAssessment()
    {
        AmqpControllerScript.amqpControl.exchangeSubscription.Handler = Process;

        ScoreRequestJson request = new ScoreRequestJson();
        request.id = id;
        request.type = "read_assessment";
        //request.id_user = PlayerPrefs.GetString("id_user");
        request.id_catch = "1";

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
                }else if (type == "read_assessment")
                {
                    Debug.Log("Process READ ASSESSMENT");
                    LoadScoreAssessment(msg);
                }
            }
        }
    }

    void LoadGhostDescription(CymaticLabs.Unity3D.Amqp.SimpleJSON.JSONNode msg)
    {
        ghost_name.text = (string)msg["data"][0];
        speed.text = (string)msg["data"][1];
        dv.text = (string)msg["data"][2];
        max_hp.text = (string)msg["data"][3];
        //Debug.Log(PlayerPrefs.GetString("id_user"));
    }

    void LoadScoreAssessment(CymaticLabs.Unity3D.Amqp.SimpleJSON.JSONNode msg)
    {
        word.text = (string)msg["data"][0];
        spoken.text = (string)msg["data"][1];
        damage.text = (string)msg["data"][2];
    }

    [Serializable]
    public class ScoreRequestJson
    {
        public string id;
        public string type;
        //public string id_user;
        public string id_catch;
        public string word;
        public string spoken;
        public int damage;
    }

    public class GhostCatchedRequestJson
    {
        public string id;
        public string type;
        public string ghost_name;
        public string ghost_speed;
        public string destruct_val;
        public string max_health;
    }
}
