using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    public float speed; // the speed of flying bus
    public float rotationSpeed; // the speed of rotation
    public float boundSize = 5;
    public float skyBoundSize = 30;
    private float gap = 1f; // gap for detecting camera out of bound
    private CreateMountainPlane planeObject; // reference to mountatin plane


    // Use this for initialization
    void Start () {

        speed = 1;
        rotationSpeed = 100;
        // setting initial camera rotation
        this.transform.localRotation = Quaternion.Euler(20.0f, 0.0f, 0.0f);
        // setting initial camera position
        this.transform.localPosition = new Vector3(0.0f, 10.0f, -4f);


    }
	
	// Update is called once per frame
	void Update () {

        Quaternion delta = Quaternion.identity;
        // Pitch
        float pitch = Input.GetAxis("Mouse Y");
        delta *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime * pitch,
                                      -Vector3.right);
        // Yaw
        float yaw = Input.GetAxis("Mouse X");
        delta *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime * yaw,
                                                   Vector3.up);

        transform.rotation *= delta;

        // Movement
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.rotation * new Vector3(0, 0, speed) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += transform.rotation * new Vector3(-speed, 0, 0) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += transform.rotation * new Vector3(0, 0, -speed) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.rotation * new Vector3(speed, 0, 0) * Time.deltaTime;
        }

        restrictMovement();

    }


    private void restrictMovement()
    {
        //CreateMountainPlane planeObject = GameObject.Find("Mountain").GetComponent<CreateMountainPlane>();
        Vector3 currentPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        // Keep the camera inside the terrain's bounds
        if (currentPosition.x < -boundSize + gap)
        {
            currentPosition.x = -boundSize + gap;
        }
        if (currentPosition.z < -boundSize + gap)
        {
            currentPosition.z = -boundSize + gap;
        }
        if (currentPosition.x > boundSize - gap)
        {
            currentPosition.x = boundSize - gap;
        }
        if (currentPosition.z > boundSize - gap)
        {
            currentPosition.z = boundSize - gap;
        }

        // Keep the camera above the plane
        if (currentPosition.y < 0 + gap)
        {
            currentPosition.y = 0 + gap;
        }

        // Keep the camera below sky bound
        if (currentPosition.y > skyBoundSize - gap)
        {
            currentPosition.y = skyBoundSize - gap;
        }

        transform.position = currentPosition;


    }
}
