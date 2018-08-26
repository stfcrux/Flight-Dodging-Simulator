﻿using System.Collections.Generic;
using UnityEngine;

public class CreateMountainPlane : MonoBehaviour
{

    public float sideLength;
    public float roughness;
    public float initialHeightRange;
    public int nIterations;


    void Start()
    {
        // generate height maps
        float[,] ys = DiamondSquare(nIterations);

        // generate triangle verticies from the heightmap
        List<Vector3> vertices = GenTriangles(ys, sideLength);

        // create the mesh
        MeshFilter mountainMesh = this.gameObject.AddComponent<MeshFilter>();
        mountainMesh.mesh = this.CreateMountainMesh(vertices);

        // set the shader on the game object
        this.gameObject.AddComponent<MeshRenderer>().material.shader =
                Shader.Find("Unlit/VertexColorShader");
    }

    private Mesh CreateMountainMesh(List<Vector3> vertices)
    {
        Mesh m = new Mesh
        {
            name = "Mountain",
            vertices = vertices.ToArray()
        };

        Color[] colors = new Color[m.vertices.Length];
        int[] triangles = new int[m.vertices.Length];

        // assign the triangles and the color for each vertex
        for (int i = 0; i < m.vertices.Length; i++)
        {
            triangles[i] = i;
            // TODO: don't use this simple colouring
            float c = (m.vertices[i].y);
            colors[i] = new Color(c, c, c, 1.0f);
        }

        // copy the traingles and colors to the mesh
        m.triangles = triangles;
        m.colors = colors;

        return m;
    }

    private List<Vector3> GenTriangles(float[,] ys, float size) {
        List<Vector3> vertices = new List<Vector3>();
        // determine the x and z increment using the square sidelength
        float increment = size / ys.GetLength(0);

        for (int x = 0; x < ys.GetLength(0); x++)
        {
            for (int z = 0; z < ys.GetLength(0); z++)
            {
                // create a top left triangle if possible
                if ((x + 1 < ys.GetLength(0)) && (z - 1 >= 0))
                {
                    vertices.Add(new Vector3(x * increment, ys[x, z], z * increment));
                    vertices.Add(new Vector3((x+1) * increment, ys[x+1, z], z * increment));
                    vertices.Add(new Vector3(x * increment, ys[x, z-1], (z-1) * increment));
                }
                // create a bottom right triangle if possible
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

        float range = initialHeightRange;

        // generate corners randomnly
        ys[0, 0] = Random.Range(-range, range);
        ys[maxIndex, 0] = Random.Range(-range,range);
        ys[0, maxIndex] = Random.Range(-range, range);
        ys[maxIndex, maxIndex] = Random.Range(-range, range);

        for (int currSize = maxIndex; currSize > 1; currSize /= 2) {
            // reduce the range at each iteration to stop the surface from
            // looking jaggad
            range = (range / 2) * roughness;

            int half = currSize / 2;

            // diamond step
            for (int x = half; x < maxIndex; x += currSize)
            {
                for (int z = half; z < maxIndex; z += currSize)
                {
                    ys[x, z] = Diamond(x, z, half, ys, range);
                }
            }

            // square step
            int startz = half;
            for (int x = 0; x <= maxIndex; x += half)
            {
                for (int z = startz; z <= maxIndex; z += currSize)
                {
                    ys[x, z] = Square(x, z, half, ys, range);
                }

                startz = (startz + half) % currSize;
            }
        }

        return ys;
    }



    float Square(int x, int z, int half, float[,] ys, float range)
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
        return AvgRandom(relevantYs, range);
    }

    float Diamond(int x, int z, int half, float[,] ys, float range) {
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
        return AvgRandom(relevantYs, range);
    }

    float AvgRandom(List<float> inputs, float range) {
        float tot = 0.0f;
        foreach (float input in inputs) {
            tot += input;
        }
        return tot / inputs.Count + Random.Range(-range, range);
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

