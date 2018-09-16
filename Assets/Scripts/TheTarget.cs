using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CymaticLabs.Unity3D.Amqp;
using UnityEngine.SceneManagement;

public class TheTarget : MonoBehaviour {
    //SITS ON EVERY GHOST AT SCENE CATCH!!

    public float health = 100f;
    public Image healthBar;
    public Text popUpDamage;
    string id;

    void Start() {
        id = Guid.NewGuid().ToString();
        popUpDamage.gameObject.SetActive(false);
        Debug.Log("ID USER : " + PlayerPrefs.GetString("id_user"));
        CreateNewIdCatch(PlayerPrefs.GetString("id_user"), "1"); //"1" nya DIGANTI ID GHOST
    }

    public void TakeDamage(float amount) {
        popUpDamage.gameObject.SetActive(true);
        health -= amount;
        healthBar.fillAmount = health / 100;
        if (health <= 0f) {
            Die();
        }
        popUpDamage.text = "+"+amount;
        StartCoroutine(LateCall());
    }

    IEnumerator LateCall()
    {
        yield return new WaitForSeconds(0.5f);
        popUpDamage.gameObject.SetActive(false);
    }

    private void Die()
    {
        Destroy(gameObject);
        StartCoroutine(LateCall());
        SceneManager.LoadScene("Score");
    }

    #region jadi setiap fight ghost baru, generate new id catch

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
                if (type == "create_id_catch")
                {
                    Debug.Log("create_id_catch");
                    PlayerPrefs.SetString("id_catch", (string)msg["id_catch"]);
                }
            }
        }
    }

    public void CreateNewIdCatch(string id_user, string id_ghost)
    {
        AmqpControllerScript.amqpControl.exchangeSubscription.Handler = Process;

        CreateNewIdCatchRequestJson request = new CreateNewIdCatchRequestJson();
        request.id = id;
        request.type = "create_id_catch";
        request.id_user = id_user;
        request.id_ghost = id_ghost;

        string requestJson = JsonUtility.ToJson(request);
        Debug.Log(requestJson);

        AmqpClient.Publish(AmqpControllerScript.amqpControl.requestExchange, AmqpControllerScript.amqpControl.requestRoutingKey, requestJson);
    }

    [Serializable]
    public class CreateNewIdCatchRequestJson
    {
        public string id;
        public string type;
        public string id_user;
        public string id_ghost;
    }

    #endregion
}
