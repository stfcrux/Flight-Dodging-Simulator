
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    public float speed; // the speed of flying bus
    public float rotationSpeed; // the speed of rotation
    public float boundSize = 5; // the length of our terrain is 10 so we take half
    public float skyBoundSize = 25; // the top of where the camera can go to
    private float gap = 0.5f; // offset for detecting if camera is out of bounds
    public CreateMountainPlane planeObject; // reference to mountatin plane
    public Rigidbody rigid; // reference to rigid body to check for collisions

    float pitch;
    float yaw;

    // Use this for initialization
    void Start () {

        Cursor.lockState = CursorLockMode.Locked;
	
	//speed of movement
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

        // Movement based on w a s d keys
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += -transform.right * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += -transform.forward * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * Time.deltaTime * speed;
        }

        //pitch and yaw to change with movement of mouse
        yaw += rotationSpeed * Input.GetAxis("Mouse X");
        pitch -= rotationSpeed * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        // restricting movement out of bounds of the map
        restrictMovement();

    }


    private void restrictMovement()
    {
        
        Vector3 currentPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        // Keep the camera inside the terrain's bounds
        if (currentPosition.x < -boundSize + gap)
        {
            currentPosition.x = -boundSize + gap;
        }
	if (currentPosition.x > boundSize - gap)
        {
            currentPosition.x = boundSize - gap;
        }
        if (currentPosition.z < -boundSize + gap)
        {
            currentPosition.z = -boundSize + gap;
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
	rigid.velocity = Vector3.zero;     
        rigid.angularVelocity = Vector3.zero; 
        


    }

}

