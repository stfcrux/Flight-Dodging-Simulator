using UnityEngine;
using System.Collections;

public class PointLight : MonoBehaviour
{
    // The color of the point light
    public Color color = Color.white;

    // the speed of the point light's rotation
    public int rotationSpeed;

    // the position of the rotation
    private float rotation;

    public Vector3 GetWorldPosition()
    {
        return this.transform.position;
    }

    // Perform Rotation of point light and making sure that the point light is facing the origin
    public void Update()
    {
        // setting the position of the point light
        rotation = Time.deltaTime * rotationSpeed;

        // Vector3.right is the vector facing the world right
        // x axis is always constant and the sun rotates around y and z
        transform.RotateAround(Vector3.zero, Vector3.right, rotation);

        // Transform at every frame to ensure light is looking at origin
        transform.LookAt(Vector3.zero);
    }
}
