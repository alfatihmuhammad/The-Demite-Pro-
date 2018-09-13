using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour {

    public Image iconSprite;
    public string itemName;
    int isEquipped;
    public GameObject demit;
    public Sprite sprite;
    Text amountText;
    Image icon;
    //public string[] equipmentInSlot = new string[5];
    //public Transform t;

    // Use this for initialization
    void Awake() {
        gameObject.SetActive(true);
    }

    void Start () {
        amountText = transform.Find("Amount").GetComponentInChildren<Text>();
        icon = transform.Find("Icon").GetComponent<Image>();
        Debug.Log(icon.sprite.name);
        PlayerPrefs.SetInt("equip_Bottle",0);
        /*cara 1 biar clone nya ke parent
        Transform clone = Instantiate(t,transform);*/
        /*cara 2
        Transform clone = Instantiate(t);
        clone.transform.parent = gameObject.transform;*/
    }
	
	// Update is called once per frame
	void Update () {
        
        isEquipped = PlayerPrefs.GetInt("equip_Bottle");
        if (isEquipped == 0)
        {
            icon.sprite = null;
            amountText.text = "0";
        }
        else if (isEquipped == 1)
        {
            icon.sprite = sprite;
            amountText.text = "" + PlayerPrefs.GetInt("player_Bottle");
        }
    }

    public void UseItem()
    {
        if (isEquipped == 1 && PlayerPrefs.GetInt("player_Bottle")>0) {
            PlayerPrefs.SetInt("player_Bottle", PlayerPrefs.GetInt("player_Bottle") - 1);
            Destroy(demit);
        }
        if (PlayerPrefs.GetInt("player_Bottle") == 0) {
            isEquipped = 0;
        }
    } 

}
