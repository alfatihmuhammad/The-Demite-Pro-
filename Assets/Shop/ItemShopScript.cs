using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemShopScript : MonoBehaviour {

    public int numItem, price;
    
    public Text priceText, sumPriceText;
    Text amountText;
    public Button plus, minus;
    public string itemNamePublic; //diisi diluar
    public string itemName; //for playerPref
    ShopScript shopScript = new ShopScript();
    int money;
    [HideInInspector]
    public int sumPrice /*sum for 1 item*/, totalPrice/*for all items*/;

    void Start () {
        amountText = transform.Find("Amount").GetComponentInChildren<Text>();

        numItem = 0;
        sumPrice = 0;
        totalPrice = 0;
        priceText.text = "" + price;
        itemName = itemNamePublic;
        money = PlayerPrefs.GetInt("MoneyAmount");
        ShowPrice();
    }
	
    public void AddToCart()
    {
        if (numItem >= 10 || PlayerPrefs.GetInt("MoneyAmount") < (PlayerPrefs.GetInt("TotalPrice")+price))
        {
            //do nothing
            ShowPrice();
            Debug.Log("price"+ (PlayerPrefs.GetInt("TotalPrice") + price));
            Debug.Log("money"+ PlayerPrefs.GetInt("MoneyAmount"));
        }
        else
        {
            numItem++;
            sumPrice += price;
            ShowPrice();
            int priceBefore = PlayerPrefs.GetInt("TotalPrice");
            PlayerPrefs.SetInt("TotalPrice", priceBefore + price);
        }        
    }

    public void RemoveFromCart()
    {
        if (numItem <= 0)
        {
            //do nothing
            ShowPrice();
        }
        else
        {
            numItem--;
            sumPrice -= price;
            ShowPrice();
            int priceBefore = PlayerPrefs.GetInt("TotalPrice");
            PlayerPrefs.SetInt("TotalPrice", priceBefore - price);
        }
    }

    public void ShowPrice() { //show and calculate total price
        amountText.text = "" + numItem;
        sumPriceText.text = "" + sumPrice;
        //PlayerPrefs.SetInt("buy_" + itemName, numItem);  
    }
}
