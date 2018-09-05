
using UnityEngine;
using System.Collections;

public class PointLight : MonoBehaviour
{

    public Color color;
    public int multiplier;

    // Rotation will be used as a speed element for the rotation (aka sunsetting and rising)
    private float rotation;

    // Multiplier value to modify rotation speed!

    public Vector3 GetWorldPosition()
    {
        return this.transform.position;
    }

    //	Initialize
    void Start()
    {
        // Set initial rotation radius
        CreateMountainPlane planeObject = GameObject.Find("Mountain").GetComponent<CreateMountainPlane>();
        gameObject.transform.position = new Vector3(0, planeObject.sideLength / 2, -planeObject.sideLength / 2);
    }


    // Perform Rotation and keep the sun facing the origin
    public void Update()
    {
        // Time.deltaTime gets the time between frames
        rotation = Time.deltaTime * multiplier; //The greater the multiplier the faster the speed.

        // Vector3.right is the same as new Vector3(1,0,0) (x axis is constant, sun rotates around z and y).
        transform.RotateAround(Vector3.zero, Vector3.right, rotation);

        // Transform at every frame to ensure light is looking at (0,0,0)
        transform.LookAt(Vector3.zero);
    }
}

