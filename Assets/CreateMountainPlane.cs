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
        Vector3 tl = new Vector3(-size / 2, 0.0f, size / 2);
        Vector3 tr = new Vector3(size / 2, 0.0f, size / 2);
        Vector3 bl = new Vector3(-size / 2, 0.0f, -size / 2);
        Vector3 br = new Vector3(size / 2, 0.0f, -size / 2);

    }

    HashSet<Vector3> diamondSquare(int nIterations, Vector3 tl, Vector3 tr, Vector3 bl, Vector3 br) {
        Vector3 mm = diamond(tl, tr, bl, br);
        Vector3 ml = square(mm, tl, bl);
        Vector3 mr = square(mm, tr, br);
        Vector3 tm = square(mm, tl, tr);
        Vector3 bm = square(mm, bl, br);
        if (nIterations == 1) {
            HashSet<Vector3> res = new HashSet<Vector3>();
            res.Add(tl);
            res.Add(tm);
            res.Add(tr);
            res.Add(ml);
            res.Add(mm);
            res.Add(mr);
            res.Add(bl);
            res.Add(bm);
            res.Add(br);
            return res;
        } else {
            HashSet<Vector3> res = diamondSquare(nIterations - 1, tl, tm, ml, mm);
            res.UnionWith(diamondSquare(nIterations - 1, ml, mm, bl, bm)); 
            res.UnionWith(diamondSquare(nIterations - 1, tm, tr, mm, mr));
            res.UnionWith(diamondSquare(nIterations - 1, mm, mr, bm, br));
            return res;
        }
    }

    Vector3 square(Vector3 middle, Vector3 other1, Vector3 other2) {
        return new Vector3(other1.x, avgRandom(middle, other1, other2), other1.z);
    }

    Vector3 diamond(Vector3 tl, Vector3 tr, Vector3 bl, Vector3 br) {
        return new Vector3(tr.x - tl.x, avgRandom(tl, tr, bl, br), tr.z - br.z);
    }

    float avgRandom(params Vector3[] inputs) {
        float tot = 0.0f;
        foreach (Vector3 input in inputs) {
            tot += input.y;
        }
        return tot / inputs.Length + Random.value * variance;
    }
	
}
