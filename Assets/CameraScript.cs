
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    public float speed; // the speed of flying bus
    public float rotationSpeed; // the speed of rotation
    public float boundSize = 5;
    public float skyBoundSize = 20;
    private float gap = 0.5f; // gap for detecting camera out of bound
    public CreateMountainPlane planeObject; // reference to mountatin plane
    public Rigidbody rigid;

    float pitch;
    float yaw;

    // Use this for initialization
    void Start () {

        Cursor.lockState = CursorLockMode.Locked;

        speed = 5;
        rotationSpeed = 2;
        // setting initial camera rotation
        pitch = 70;
        yaw = -60;
        // setting initial camera position
        this.transform.localPosition = new Vector3(4.0f, 20.0f, -4f);

      
    }
	
	// Update is called once per frame
	void Update () {

        // Movement, in the case of object colliding with something in that direction, it does not move
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += -transform.right * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += -transform.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * speed * Time.deltaTime;
        }

        //Changes pitch and yaw by moving mouse
        yaw += rotationSpeed * Input.GetAxis("Mouse X");
        pitch -= rotationSpeed * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        // restricting movement out of bounds of the map
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
	    
	//Avoid forces to be applied to camera
        rigid.angularVelocity = Vector3.zero; 
        rigid.velocity = Vector3.zero; 


    }

}

