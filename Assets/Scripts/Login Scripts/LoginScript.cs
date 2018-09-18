using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CymaticLabs.Unity3D.Amqp;

public class LoginScript : MonoBehaviour {

    Button login;
    Text warning;
    string id;
    public Dictionary<int, string> dictionary = new Dictionary<int, string>();

    // Use this for initialization
    void Start () {
        id = Guid.NewGuid().ToString();

        warning = GameObject.Find("Warning_Text").GetComponent<Text>();

        login = this.GetComponent<Button>();
        login.onClick.AddListener(LoginGame);

        dictionary.Add(1, "Bottle");
        dictionary.Add(2, "Sate");
        dictionary.Add(3, "Jamu");
        dictionary.Add(4, "Curry_rice");
        dictionary.Add(5, "Tasbih");
        dictionary.Add(6, "Water");
        dictionary.Add(7, "Milk_s");
        dictionary.Add(8, "Milk_l");
    }
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}

    void ProcessLogin(AmqpExchangeReceivedMessage received)
    {
        var receivedJson = System.Text.Encoding.UTF8.GetString(received.Message.Body);
        var msg = CymaticLabs.Unity3D.Amqp.SimpleJSON.JSON.Parse(receivedJson);
        

        if(msg != null)
        {
            string msgId = (string)msg["id"];
            if(msgId == id)
            {
                string type = (string)msg["type"];
                if(type == "login")
                {
                    int count = (int)msg["count"]; // dr server LoginResponseJson 
                    if (count == 1)
                    {
                        LoadStatus(msg);
                        Time.timeScale = 1;
                        SceneManager.LoadScene("Map");
                    }
                    else
                    {
                        warning.text = "username or password not found";
                    }
                }
            }
        }
    }

    void LoginGame()
    {
        warning.text = "";

        InputField usernameField = GameObject.Find("InputField_username").GetComponent<InputField>();
        InputField passwordField = GameObject.Find("InputField_password").GetComponent<InputField>();

        string username = usernameField.text;
        string password = passwordField.text;

        if(username == "")
        {
            warning.text = "username cannot be empty";
        }

        if(password == "")
        {
            warning.text = "password cannot be empty";
        }

        Debug.Log(username + " & " + password);

        if(username != "" && password != "")
        {
            AmqpControllerScript.amqpControl.exchangeSubscription.Handler = ProcessLogin;

            LoginRequestJson request = new LoginRequestJson();
            request.id = id;
            request.type = "login";
            request.username = username;
            request.password = password;

            string requestToJson = JsonUtility.ToJson(request);
            AmqpClient.Publish(AmqpControllerScript.amqpControl.requestExchange, AmqpControllerScript.amqpControl.requestRoutingKey, requestToJson);
        }
    }

    void LoadStatus(CymaticLabs.Unity3D.Amqp.SimpleJSON.JSONNode msg)
    {

        PlayerPrefs.SetString("username", (string)msg["username"]);
        PlayerPrefs.SetString("id_user", (string)msg["id_user"]);
        for (int i = 0; i < 8; i++)
        {
            int j = 1 + i;
            Debug.Log("player_" + dictionary[j] + " diisi " + (int)msg["data"][i]);
            PlayerPrefs.SetInt("player_" + dictionary[j], (int)msg["data"][i]);
            Debug.Log(PlayerPrefs.GetInt("player_" + dictionary[j]));
        }

        PlayerPrefs.SetString("petName", (string)msg["username"]);
    }

    //data request
    [Serializable]
    public class LoginRequestJson
    {
        public string id;
        public string type;
        public string username;
        public string password;
        public int id_user;
    }
}
