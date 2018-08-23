using System.Collections.Generic;
using UnityEngine;

public class CreateMountainPlane : MonoBehaviour
{

    public float sideLength;
    public float variance;
    private float actualVariance;
    public int nIterations;


    // Use this for initialization
    void Start()
    {
        float[,] ys = DiamondSquare(nIterations);
        for (int x = 0; x < ys.GetLength(0); x++)
        {
            for (int z = 0; z < ys.GetLength(0); z++)
            {
            }
        }
        List<Vector3> vertices = GenTriangles(ys, sideLength);
        // From Tut1

        // Add a MeshFilter component to this entity. This essentially comprises of a
        // mesh definition, which in this example is a collection of vertices, colours 
        // and triangles (groups of three vertices). 
        MeshFilter cubeMesh = this.gameObject.AddComponent<MeshFilter>();
        cubeMesh.mesh = this.CreateMountainMesh(vertices);

        // Add a MeshRenderer component. This component actually renders the mesh that
        // is defined by the MeshFilter component.
        this.gameObject.AddComponent<MeshRenderer>().material.shader = Shader.Find("Unlit/VertexColorShader");

    }

    // Method to create a cube mesh with coloured vertices
    private Mesh CreateMountainMesh(List<Vector3> vertices)
    {
        // Adapted from Tut1
        Mesh m = new Mesh
        {
            name = "Mountain",

            // Define the vertices. These are the "points" in 3D space that allow us to
            // construct 3D geometry (by connecting groups of 3 points into triangles).
            vertices = vertices.ToArray()
        };

        // Define the vertex colours
        Color[] colors = new Color[m.vertices.Length];

        // Automatically define the triangles based on the number of vertices
        int[] triangles = new int[m.vertices.Length];
        for (int i = 0; i < m.vertices.Length; i++)
        {
            triangles[i] = i;
            float c = (m.vertices[i].y);
            colors[i] = new Color(c, c, c, 1.0f);
        }

        m.triangles = triangles;
        m.colors = colors;

        return m;
    }

    private List<Vector3> GenTriangles(float[,] ys, float size) {
        List<Vector3> vertices = new List<Vector3>();
        float increment = size / ys.GetLength(0);

        for (int x = 0; x < ys.GetLength(0); x++)
        {
            for (int z = 0; z < ys.GetLength(0); z++)
            {
                if ((x + 1 < ys.GetLength(0)) && (z - 1 >= 0))
                {
                    vertices.Add(new Vector3(x * increment, ys[x, z], z * increment));
                    vertices.Add(new Vector3((x+1) * increment, ys[x+1, z], z * increment));
                    vertices.Add(new Vector3(x * increment, ys[x, z-1], (z-1) * increment));
                }
                if ((x - 1 >= 0) && (z - 1 >= 0)) {
                    vertices.Add(new Vector3(x * increment, ys[x, z], z * increment));
                    vertices.Add(new Vector3(x * increment, ys[x, z-1], (z - 1) * increment));
                    vertices.Add(new Vector3((x - 1) * increment, ys[x-1, z-1], (z - 1) * increment));
                }
            }
        }

        return vertices;
    }

    private float[,] DiamondSquare(int iterations)
    {
        int maxIndex = Power(2, iterations);
        float[,] ys = new float[maxIndex+1, maxIndex+1];



        // generate corners
        ys[0, 0] = Random.Range(-variance, variance);
        ys[maxIndex, 0] = Random.Range(-variance,variance);
        ys[0, maxIndex] = Random.Range(-variance, variance);
        ys[maxIndex, maxIndex] = Random.Range(-variance, variance);

        actualVariance = variance;

        for (int currSize = maxIndex; currSize > 1; currSize /= 2) {

            int half = currSize / 2;

            // diamond step
            for (int x = half; x < maxIndex; x += currSize)
            {
                for (int z = half; z < maxIndex; z += currSize)
                {
                    ys[x, z] = Diamond(x, z, half, ys);
                }
            }

            // square step
            int startz = half;
            for (int x = 0; x <= maxIndex; x += half)
            {
                for (int z = startz; z <= maxIndex; z += currSize)
                {
                    ys[x, z] = Square(x, z, half, ys);
                }

                startz = (startz + half) % currSize;
            }
            actualVariance /= 2;
        }

        return ys;
    }



    float Square(int x, int z, int half, float[,] ys)
    {
        List<float> relevantYs = new List<float>();
        if (z - half >= 0)
        {
            // append north
            relevantYs.Add(ys[x, z - half]);
        }
        if (z + half < ys.GetLength(0))
        {
            // append south
            relevantYs.Add(ys[x, z + half]);
        }
        if (x - half >= 0)
        {
            // append west
            relevantYs.Add(ys[x - half, z]);
        }
        if (x + half < ys.GetLength(0))
        {
            // append east
            relevantYs.Add(ys[x + half, z]);
        }
        print(relevantYs.Count);
        return AvgRandom(relevantYs);
    }

    float Diamond(int x, int z, int half, float[,] ys) {
        List<float> relevantYs = new List<float>
        {
            // add NW
            ys[x - half, z - half],
            // add NE
            ys[x + half, z - half],
            // add SW
            ys[x - half, z + half],
            // add SE
            ys[x + half, z + half]
        };
        return AvgRandom(relevantYs);
    }

    float AvgRandom(List<float> inputs) {
        float tot = 0.0f;
        foreach (float input in inputs) {
            tot += input;
        }
        return tot / inputs.Count + Random.Range(-actualVariance, actualVariance);
    }

    int Power(int b, int e)
    {
        int res = 1;
        for (int i = 0; i < e; i++)
        {
            res *= b;
        }
        return res;
    }
}

