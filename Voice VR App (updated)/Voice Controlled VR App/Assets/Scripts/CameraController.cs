using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 2.0f;
    private float X;
    private float Y;
    private CameraControllerManager cameraManager;

    // Start is called before the first frame update
    void Start()
    {
        cameraManager = GetComponentInChildren<CameraControllerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * speed, -Input.GetAxis("Mouse X") * speed, 0));
            X = transform.rotation.eulerAngles.x;
            Y = transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(X, Y, 0);

            // If turning, stop
            cameraManager.StopTurn();
        }
    }
}
