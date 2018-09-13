using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using CymaticLabs.Unity3D.Amqp;
using System;
using System.Collections.Generic;

/* Sits on all InventorySlots. */

public class InventorySlot : MonoBehaviour
{

    public Image icon;
    Sprite spriteNew;
    public Button removeButton;
    public Text amount;
    int numSlotInv;
    public ItemShopScript iss = new ItemShopScript();
    //List<EquipmentSlot> equipmentSlot = new List<EquipmentSlot>();
    bool equip;
    GameObject go;
    Transform t;
    public GameObject me;
    public Image meImange;
    public string meImageName;
    public Dictionary<string, int> dictionary = new Dictionary<string, int>();

    void Start()
    {
        numSlotInv = 8;
        go = iss.gameObject;
        t = go.transform.Find("Icon");
        equip = false;
        //coba doang
        me = transform.Find("Icon").gameObject;
        meImange = me.GetComponent<Image>();
        meImageName = meImange.sprite.name;

        dictionary.Add("Bottle", 1);
        dictionary.Add("Sate", 2);
        dictionary.Add("Jamu", 3);
        dictionary.Add("Curry_rice", 4);
        dictionary.Add("Tasbih", 5);
        dictionary.Add("Water", 6);
        dictionary.Add("Milk_s", 7);
        dictionary.Add("Milk_l", 8);
    }

    void Update() {
        //icon.sprite = spriteNew;
        amount.text = PlayerPrefs.GetInt("player_"+iss.itemName).ToString();
    }

    public void EquipItem()
    {
        //equipmentSlot.Add(EquipmentSlot.Instantiate);
        if (PlayerPrefs.GetInt("player_" + iss.itemName) >= 0 && equip == true) //true jd false
        {
            PlayerPrefs.SetInt("equip_" + iss.itemName, 0);
            removeButton.gameObject.SetActive(true);
            equip = false;
            Debug.Log("UN EQUIPPED NOO");
        }
        else if (PlayerPrefs.GetInt("player_" + iss.itemName) > 0 && equip == false) // false jadi true
        {
            PlayerPrefs.SetInt("equip_" + iss.itemName, 1);
            removeButton.gameObject.SetActive(false);
            equip = true;
            Debug.Log("EQUIPPED YAY");
        }
    }

    public void RemoveItem() {
        PlayerPrefs.SetInt("player_" + iss.itemName, 0);
        PlayerPrefs.SetInt("equip_" + iss.itemName, 0);
        equip = false;
        UpdateInventory(PlayerPrefs.GetInt("id_user").ToString(), dictionary[iss.itemName].ToString(), "0");
    }

    string id = Guid.NewGuid().ToString();

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
                if (type == "insert_inv")
                {
                    Debug.Log("INSERT");
                }
                else if (type == "update_inv")
                {
                    Debug.Log("UPDATE");
                }
            }
        }
    }

    public void UpdateInventory(string id_user, string id_item, string item_total)
    {
        AmqpControllerScript.amqpControl.exchangeSubscription.Handler = Process;

        RequestJson request = new RequestJson();
        request.id = id;
        request.type = "update_inv";
        request.id_user = id_user;
        request.id_item = id_item;
        request.item_total = item_total;

        string requestJson = JsonUtility.ToJson(request);
        Debug.Log(requestJson);

        AmqpClient.Publish(AmqpControllerScript.amqpControl.requestExchange, AmqpControllerScript.amqpControl.requestRoutingKey, requestJson);
    }

    [Serializable]
    public class RequestJson
    {
        public string id;
        public string type;
        public string id_user;
        public string id_item;
        public string item_total;
    }
}
