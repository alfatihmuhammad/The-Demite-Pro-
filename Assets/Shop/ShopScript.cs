using UnityEngine;
using UnityEngine.UI;
using CymaticLabs.Unity3D.Amqp;
using System;
using System.Collections.Generic;

public class ShopScript : MonoBehaviour {
    InventorySlot invslot;
	public Text moneyText; // di game
	public static int moneyAmount;

    public Text moneyAmountText; //di shop
    public Text totalPriceText;
    public int numItemInShop; //jumlah barang yg dijual di shop

    public Button buyButton;
    
    public ItemShopScript[] iss = new ItemShopScript[8]; // iss = ItemShopScript
    public int totalPrice = 0; // total price for all item 1, ... n

    public Dictionary<string, int> dictionary = new Dictionary<string, int>();

    void Start () {
        numItemInShop = 8;
        PlayerPrefs.SetInt("MoneyAmount", 100);
        PlayerPrefs.SetInt("TotalPrice", 0);
        moneyAmount = PlayerPrefs.GetInt ("MoneyAmount");
        for (int i = 0; i < numItemInShop; i++) {
            //PlayerPrefs.SetInt("player_" + iss[i].itemName,0);
            PlayerPrefs.GetInt("player_" + iss[i].itemName);
        }

        totalPrice = 0;
        buyButton.gameObject.SetActive(true);
        
        dictionary.Add("Bottle", 1);
        dictionary.Add("Sate", 2);
        dictionary.Add("Jamu", 3);
        dictionary.Add("Curry_rice", 4);
        dictionary.Add("Tasbih", 5);
        dictionary.Add("Water", 6);
        dictionary.Add("Milk_s", 7);
        dictionary.Add("Milk_l", 8);
    }
	
	void Update () {

        moneyText.text = "Money: " + moneyAmount.ToString() + "$";

        moneyAmountText.text = "Money: " + moneyAmount.ToString() + "$";

        totalPriceText.text = "Total: " + PlayerPrefs.GetInt("TotalPrice") + "$";

    }

    public void buyItems()
    {
        moneyAmount = moneyAmount - PlayerPrefs.GetInt("TotalPrice");
        PlayerPrefs.SetInt("MoneyAmount", moneyAmount);
        for (int i = 0; i < numItemInShop; i++)
        {
            int jumlahLama = PlayerPrefs.GetInt("player_" + iss[i].itemName);
            int jumlahbaru = iss[i].numItem + jumlahLama;
            PlayerPrefs.SetInt("player_" + iss[i].itemName, jumlahbaru);
            Debug.Log("buyed " + iss[i].itemName +" sebanyak  "+ iss[i].numItem+" jadi total " + PlayerPrefs.GetInt("player_" + iss[i].itemName));
            UpdateInventory(PlayerPrefs.GetInt("id_user").ToString(), dictionary[iss[i].itemName].ToString(), jumlahbaru.ToString());
        }
        resetShopCart();
    }

    public void resetShopCart()
    {
        for (int i = 0; i < numItemInShop; i++)
        {
            iss[i].numItem = 0;
            iss[i].sumPrice = 0;
            iss[i].ShowPrice();
        }
        PlayerPrefs.SetInt("TotalPrice", 0);
        Debug.Log("total"+ PlayerPrefs.GetInt("TotalPrice"));
        totalPriceText.text = "Total: " + PlayerPrefs.GetInt("TotalPrice") + "$";
        
        //PlayerPrefs.DeleteAll();
    }

    //#region duplikat2, ga dipake
    //public GameObject ygMauDiDuplikat; //didup
    //public Transform parentYgMauDiDuplikat;
    //public RectTransform rtdidup;
    //public void Duplicat()
    //{
    //    //diduplikat
    //    GameObject duplikatnya = Instantiate(ygMauDiDuplikat, parentYgMauDiDuplikat);
    //    rtdidup = duplikatnya.GetComponent<RectTransform>();
    //    rtdidup.anchorMin = new Vector2(0.5f, 0.5f);
    //    rtdidup.anchorMax = new Vector2(0.5f, 0.5f);
    //    rtdidup.pivot = new Vector2(0.5f, 0.5f);
    //    rtdidup.anchoredPosition = new Vector3(0, 0, 0);
    //    rtdidup.sizeDelta = new Vector2(1000, 100);
    //    GameObject itemParentnyaDuplikat = duplikatnya.transform.Find("ItemParent").gameObject;
    //    GridLayoutGroup glg = itemParentnyaDuplikat.GetComponent<GridLayoutGroup>();
    //    glg.constraint = GridLayoutGroup.Constraint.FixedRowCount;  //**
    //    glg.constraintCount = 1;
    //}
    //#endregion
    
    #region coba_db
    string id = Guid.NewGuid().ToString();
    
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
                }else if (type == "update_inv")
                {
                    Debug.Log("UPDATE");
                }
            }
        }
    }

    public void InsertInventory(string id_user, string id_item, string item_total)
    {
            AmqpControllerScript.amqpControl.exchangeSubscription.Handler = Process;

            RequestJson request = new RequestJson();
            request.id = id;
            request.type = "insert_inv";
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

    #endregion
}
