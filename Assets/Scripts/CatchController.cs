using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CatchController : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;

            if(Physics.Raycast(ray, out raycastHit))
            {
                if (raycastHit.collider.tag == "Pocong")
                {
                    SceneManager.LoadScene("CatchPocong");
                }

                if (raycastHit.collider.tag == "Sundel")
                {
                    SceneManager.LoadScene("CatchSundel");
                }

                if (raycastHit.collider.tag == "ButoIjo")
                {
                    SceneManager.LoadScene("CatchButoIjo");
                }

                if (raycastHit.collider.tag == "Genderuwo")
                {
                    SceneManager.LoadScene("CatchGenderuwo");
                }

                if (raycastHit.collider.tag == "Kuntianak")
                {
                    SceneManager.LoadScene("CatchKuntilanak");
                }
            }
        }
	}
}
