using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TheTarget : MonoBehaviour {

    public float health = 100f;
    public Image healthBar;
    public Text popUpDamage;

    void Start() {
        popUpDamage.gameObject.SetActive(false);
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
    }
}
