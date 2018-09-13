using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemScript : MonoBehaviour
{

    public GameObject slotGo;
    public Transform slotGoParent;
    public string[] nameItem = new string[] { "Bottle", "Sate", "Jamu", "Curry_rice", "Tasbih", "Water", "Milk_s", "Milk_l" };
    public bool[] isIstantiated = new bool[] { false, false, false, false, false, false, false, false };
    public Sprite[] spriteItem = new Sprite[8]; //nameItem.Length = 8
    public GameObject[] duplikatnya = new GameObject[8]; //nameItem.Length = 8
    public GameObject emptyText;
    // Use this for initialization
    void Start()
    {
        UpdateItemSlot();

    }

    void Update() {
        UpdateItemSlot();
    }

    public void UpdateItemSlot()
    {
        int itemKosong = 0;
        for (int i = 0; i < nameItem.Length; i++)
        {
            Debug.Log("[list] i = " + i + "; item = " + nameItem[i] + "; jum  = " + PlayerPrefs.GetInt("player_" + nameItem[i]));
            //duplikat item jika berisi
            if (PlayerPrefs.GetInt("player_" + nameItem[i]) > 0 && isIstantiated[i] == false)
            {
                //duplikat objek
                duplikatnya[i] = Instantiate(slotGo, slotGoParent);
                duplikatnya[i].SetActive(true);
                isIstantiated[i] = true;
                //cari icon, ganti sprite 
                Image icon = duplikatnya[i].transform.Find("Icon").gameObject.GetComponent<Image>();
                icon.sprite = spriteItem[i];
                //cari teks, ganti amount
                Text textnyaDuplikat = duplikatnya[i].transform.Find("Amount/Text").gameObject.GetComponent<Text>(); //text itu child dr amount
                textnyaDuplikat.text = PlayerPrefs.GetInt("player_" + nameItem[i]).ToString();
                //Debug.Log("if 1 = " + i);
            }
            //destroy jika item kosong
            else if (PlayerPrefs.GetInt("player_" + nameItem[i]) == 0 && isIstantiated[i] == true)
            {
                Destroy(duplikatnya[i], 0.001f);
                isIstantiated[i] = false;
                //Debug.Log("if 2 = " + i);
            }
            //jika kosong semua, tampilin tulisan "empty"
            if (isIstantiated[i] == false) 
                itemKosong += 1;
            if (itemKosong == nameItem.Length)
                emptyText.SetActive(true);
            else
                emptyText.SetActive(false);
        }
    }

    //ditaruh di item button onClick
    public void UseItem()
    {
        //get nama dr button yg di klik
        GameObject thisButton = EventSystem.current.currentSelectedGameObject;
        Image icon = thisButton.transform.Find("Icon").gameObject.GetComponent<Image>();
        String name = icon.sprite.name;
        UseThisItem(name);
    }

    private void UseThisItem(string name)
    {
        int jum = PlayerPrefs.GetInt("player_" + name);
        if (jum > 0)
        {
            Debug.Log("pakai item = " + name + " " + jum);
            PlayerPrefs.SetInt("player_" + name, jum - 1);
        }
        else {
            Debug.Log("Apanya yg dipake? wong kosong");
        }  

    }

}
