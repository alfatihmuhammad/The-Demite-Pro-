using System;
using UnityEngine;

public class Gun : MonoBehaviour {

    public GameObject DamageObject;
    private Assessment AccesControl;
    public float damage = 30f;
    public float range = 100f;
    public Camera camera;
    public bool onOver = true;

    TheTarget target;

    RaycastHit hit;

    

    // Update is called once per frame
    void Start()
    {
        //AccesControl = DamageObject.GetComponent<Assessment>();
    }
    void Update () {

        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, range))
        {
            //Debug.Log("AAAAAAA" + hit.transform.name);

            target = hit.transform.GetComponent<TheTarget>();
            if (target != null)
            {
                onOver = true;
                
            }
            
        }
        else
        {
            onOver = false;
        }

    }
   

    public void Shoot()
    {

        //RaycastHit hit;
        //if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, range)) {
        //    Debug.Log("AAAAAAA"+hit.transform.name);

        if (onOver == true)
        {
           target = hit.transform.GetComponent<TheTarget>();
           if (target != null)
            {
                //        onOver = true;
                AccesControl = DamageObject.GetComponent<Assessment>();
                damage = AccesControl.damage;
                 
                target.TakeDamage(damage);
            }
        }
       
        //else
        //{
        //    onOver = false;
        //}
    }

}
