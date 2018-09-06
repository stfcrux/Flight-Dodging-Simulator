using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {

    public int vertsPerSide;
    public PointLight pointLight;
    public float sideLength;

    private Vector3[] vertices;
    private int[] triangles;

    // Use this for initialization
    void Start () {
        // generate height maps
        float[,] ys = new float[vertsPerSide, vertsPerSide];

        // generate triangle verticies from the heightmap
        SetTriangles(ys);

        // create the mesh
        MeshFilter mountainMesh = this.gameObject.AddComponent<MeshFilter>();
        mountainMesh.mesh = this.CreateWaterMesh();

        // set the shader on the game object
        MeshRenderer meshRenderer = this.gameObject.AddComponent<MeshRenderer>();

        meshRenderer.material.shader =
                Shader.Find("Unlit/WaveShader");

        // Add MeshCollider for mesh object to avoid camera object
        gameObject.AddComponent<MeshCollider>();
	}
	
	// Update is called once per frame
	void Update () {
        // Update point light (sun)
        pointLight.Update();

        // Get renderer component (in order to pass params to shader)
        MeshRenderer renderer = this.gameObject.GetComponent<MeshRenderer>();

        // Pass updated light positions to shader
        renderer.material.SetColor("_PointLightColor", this.pointLight.color);
        renderer.material.SetVector("_PointLightPosition", this.pointLight.GetWorldPosition());

    }

    private Mesh CreateWaterMesh()
    {
        Mesh m = new Mesh
        {
            name = "Mountain",
            vertices = vertices
        };

        // copy the traingles and colors to the mesh
        m.triangles = this.triangles;
        m.RecalculateNormals();

        return m;
    }

    private void SetTriangles(float[,] ys)
    {
        int ss = ys.GetLength(0);
        vertices = new Vector3[ss * ss];
        triangles = new int[(ys.Length - ss - 1) * 2 * 3];

        // determine the x and z increment using the square sidelength
        float increment = sideLength / (ys.GetLength(0) - 1);

        int i = 0;
        for (int x = 0; x < ys.GetLength(0); x++)
        {
            for (int z = 0; z < ys.GetLength(0); z++)
            {
                vertices[i++] = new Vector3(x * increment, ys[x, z], z * increment);
            }
        }

        // gen left triangles
        i = 0;
        for (int j = 0; j < ys.Length - ss - 1; j++)
        {
            if (j % ss == ss - 1)
            {
                continue;
            }
            triangles[i++] = j;
            triangles[i++] = j + 1;
            triangles[i++] = j + ss;
        }
        // gen right triangles
        for (int j = 1; j < ys.Length - ss; j++)
        {
            if (j % ss == 0)
            {
                continue;
            }
            triangles[i++] = j;
            triangles[i++] = j + ss;
            triangles[i++] = j + ss - 1;
        }
    }}
