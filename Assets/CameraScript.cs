using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    public float speed; // the speed of flying bus
    public float rotationSpeed; // the speed of rotation
    public float boundSize = 5;
    public float skyBoundSize = 30;
    private float gap = 1f; // gap for detecting camera out of bound
    public CreateMountainPlane planeObject; // reference to mountatin plane


    // Use this for initialization
    void Start () {


        Cursor.lockState = CursorLockMode.Locked;


        speed = 5;
        rotationSpeed = 2;
        // setting initial camera rotation
        this.transform.localRotation = Quaternion.Euler(20.0f, 0.0f, 0.0f);
        // setting initial camera position
        this.transform.localPosition = new Vector3(0.0f, 10.0f, -4f);


    }
	
	// Update is called once per frame
	void Update () {

        // implementation of physics objects to check if there is anything infront for the
        // four translations
        Ray forward = new Ray(transform.position, transform.forward);
        Ray right = new Ray(transform.position, transform.right);
        Ray back = new Ray(transform.position, -transform.forward);
        Ray left = new Ray(transform.position, -transform.right);

        Quaternion delta = Quaternion.identity;
        // Pitch
        float pitch = Input.GetAxis("Mouse Y");
        delta *= Quaternion.AngleAxis(rotationSpeed * pitch,
                                      -Vector3.right);
        // Yaw
        float yaw = Input.GetAxis("Mouse X");
        delta *= Quaternion.AngleAxis(rotationSpeed * yaw,
                                                   Vector3.up);

        transform.rotation *= delta;

        // Movement, in the case of object colliding with something in that direction, it does not move
        if (Input.GetKey(KeyCode.W) && !Physics.SphereCast(forward, gap, gap))
        {
            transform.position += transform.rotation * new Vector3(0, 0, speed) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A) && !Physics.SphereCast(left, gap, gap))
        {
            transform.position += transform.rotation * new Vector3(-speed, 0, 0) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) && !Physics.SphereCast(back, gap, gap))
        {
            transform.position += transform.rotation * new Vector3(0, 0, -speed) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) && !Physics.SphereCast(right, gap, gap))
        {
            transform.position += transform.rotation * new Vector3(speed, 0, 0) * Time.deltaTime;
        }

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


    }


}
