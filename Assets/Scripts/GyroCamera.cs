using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroCamera : MonoBehaviour
{
    private Gyroscope gyroscope;
    private bool GyroscopeSupported;
    private Quaternion rotFix;
    private GameObject camParent;

    [SerializeField]
    private Transform worldObj;
    private float startY;

    [SerializeField]
    private Transform zoomObj;

	// Use this for initialization
	void Start ()
    {
        GyroscopeSupported = SystemInfo.supportsGyroscope;

        camParent = new GameObject("CameraParent");
        camParent.transform.position = transform.position;
        transform.parent = camParent.transform;

        if(GyroscopeSupported)
        {
            gyroscope = Input.gyro;
            gyroscope.enabled = true;

            camParent.transform.rotation = Quaternion.Euler(90f, 180f, 0);
            rotFix = new Quaternion(0, 0, 1, 0);
        }

        ResetGyroRotation();

    }
	
	// Update is called once per frame
	void Update ()
    {
        if(GyroscopeSupported && startY == 0)
        {
            ResetGyroRotation();
        }
       
        transform.localRotation = gyroscope.attitude * rotFix;
	}

    void ResetGyroRotation()
    {
        /*
        int x = Screen.width / 2;
        int y = Screen.height / 2;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(x, y));
        RaycastHit hit;

        if(Physics.Raycast (ray, out hit, 500))
        {
            Vector3 hitPoint = hit.point;
            hitPoint.y = 0;

            float z = Vector3.Distance(Vector3.zero, hitPoint);
            zoomObj.localPosition = new Vector3(0f, zoomObj.localPosition.y, Mathf.Clamp(z, 2f, 10f));
        }
        */

        startY = transform.eulerAngles.y;
        worldObj.rotation = Quaternion.Euler(0f, startY, 0f);
    }
}
