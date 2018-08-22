using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMountainPlane : MonoBehaviour {

    public int seed;
    public float size;
    public float variance;
    public int iterations;


    // Use this for initialization
    void Start() {
        Vector3 tl = new Vector3(-1.0f, 0.0f, 1.0f);
        Vector3 tr = new Vector3(1.0f, 0.0f, 1.0f);
        Vector3 bl = new Vector3(-1.0f, 0.0f, -1.0f);
        Vector3 br = new Vector3(1.0f, 0.0f, -1.0f);
        List < Vector3 > vertices = diamondSquare(iterations, tl, tr, bl, br);
        vertices.ForEach((Vector3 obj) => print(obj.ToString()));
        // From Tut1

        // Add a MeshFilter component to this entity. This essentially comprises of a
        // mesh definition, which in this example is a collection of vertices, colours 
        // and triangles (groups of three vertices). 
        MeshFilter cubeMesh = this.gameObject.AddComponent<MeshFilter>();
        cubeMesh.mesh = this.CreateMountainMesh(vertices);

        // Add a MeshRenderer component. This component actually renders the mesh that
        // is defined by the MeshFilter component.
        MeshRenderer renderer = this.gameObject.AddComponent<MeshRenderer>();
        renderer.material.shader = Shader.Find("Unlit/VertexColorShader");

    }

    // Method to create a cube mesh with coloured vertices
    Mesh CreateMountainMesh(List<Vector3> vertices)
    {
        // Adapted from Tut1
        Mesh m = new Mesh();
        m.name = "Mountain";

        // Define the vertices. These are the "points" in 3D space that allow us to
        // construct 3D geometry (by connecting groups of 3 points into triangles).
        m.vertices = vertices.ToArray();

        // Define the vertex colours
        Color[] colors = new Color[m.vertices.Length];

        // Automatically define the triangles based on the number of vertices
        int[] triangles = new int[m.vertices.Length];
        for (int i = 0; i < m.vertices.Length; i++)
        {
            triangles[i] = i;
            float c = 0.5f * (m.vertices[i].y + variance);
            colors[i] = new Color(c, c, c, 1.0f);
        }

        m.triangles = triangles;
        m.colors = colors;

        return m;
    }

    List<Vector3> diamondSquare(int nIterations, Vector3 tl, Vector3 tr, Vector3 bl, Vector3 br) {
        Vector3 mm = diamond(tl, tr, bl, br);
        Vector3 ml = square(mm, tl, bl);
        Vector3 mr = square(mm, tr, br);
        Vector3 tm = square(mm, tl, tr);
        Vector3 bm = square(mm, bl, br);
        if (nIterations == 1) {
            List<Vector3> res = new List<Vector3>();
            res.Add(tl);
            res.Add(tm);
            res.Add(mm);

            res.Add(tl);
            res.Add(mm);
            res.Add(ml);

            res.Add(tm);
            res.Add(tr);
            res.Add(mr);

            res.Add(tm);
            res.Add(mr);
            res.Add(mm);

            res.Add(ml);
            res.Add(mm);
            res.Add(bm);

            res.Add(ml);
            res.Add(bm);
            res.Add(bl);

            res.Add(mm);
            res.Add(mr);
            res.Add(br);

            res.Add(mm);
            res.Add(br);
            res.Add(bm);

            return res;
        } else {
            List<Vector3> res = diamondSquare(nIterations - 1, tl, tm, ml, mm);
            res.AddRange(diamondSquare(nIterations - 1, ml, mm, bl, bm)); 
            res.AddRange(diamondSquare(nIterations - 1, tm, tr, mm, mr));
            res.AddRange(diamondSquare(nIterations - 1, mm, mr, bm, br));
            return res;
        }
    }

    Vector3 square(Vector3 middle, Vector3 other1, Vector3 other2) {
        if (Mathf.Abs(other1.z - other2.z) < 0.00001) {
            return new Vector3((other1.x + other2.x) / 2, avgRandom(middle, other1, other2), other1.z);
        } else {
            return new Vector3(other1.x, avgRandom(middle, other1, other2), (other1.z + other2.z) / 2);
        }
    }

    Vector3 diamond(Vector3 tl, Vector3 tr, Vector3 bl, Vector3 br) {
        return new Vector3((tr.x + tl.x) / 2, avgRandom(tl, tr, bl, br), (tr.z + br.z) / 2);
    }

    float avgRandom(params Vector3[] inputs) {
        float tot = 0.0f;
        foreach (Vector3 input in inputs) {
            tot += input.y;
        }
        return tot / inputs.Length + Random.Range(-variance, variance);
    }
}

