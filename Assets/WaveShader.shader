// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Original Cg/HLSL code stub copyright (c) 2010-2012 SharpDX - Alexandre Mutel
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// Adapted for COMP30019 by Jeremy Nicholson, 10 Sep 2012
// Adapted further by Chris Ewin, 23 Sep 2013
// Adapted further (again) by Alex Zable (port to Unity), 19 Aug 2016
// Adapted further by Trent Branson & Jin Wei Loh, 2018

Shader "Unlit/WaveShader"
{
    Properties
    {
        _PointLightColor("Point Light Color", Color) = (0, 0, 0)
        _PointLightPosition("Point Light Position", Vector) = (0.0, 0.0, 0.0)
        _WaterColor("Water Color", Color) = (0, 0, 1, 1)

        _normalSmoothness("Normal Smoothness", Float) = 0.1
        _waveMagnitude("Wave Magnitude", Float) = 0.1
        _xFrequency("x Frequency", Float) = 6
        _zFrequency("y Frequency", Float) = 3
        _timeFrequency("Time Frequency", Float) = 1.5
        _epsilon("Epsilon", Float) = 0.05

        _Ka("Ka", Float) = 0.3
        _Ks("Ks", Float) = 0.3
        _Kd("Kd", Float) = 0.8
        _fAtt("fAtt", Float) = 1
        _specN("specN", Float) = 1
    }
        SubShader
    {
        Pass
    {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag

#include "UnityCG.cginc"

    uniform float3 _PointLightColor;
    uniform float3 _PointLightPosition;
    uniform float4 _WaterColor;
    uniform float _normalSmoothness;
    uniform float _waveMagnitude;
    uniform float _xFrequency;
    uniform float _zFrequency;
    uniform float _timeFrequency;
    uniform float _epsilon;
    uniform float _Ka;
    uniform float _Ks;
    uniform float _Kd;
    uniform float _fAtt;
    uniform float _specN;

    struct vertIn
    {
        float4 vertex : POSITION;
        float4 normal : NORMAL;
        float4 color : COLOR;
    };

    struct vertOut
    {
        float4 vertex : SV_POSITION;
        float4 color : COLOR;
        float4 worldVertex : TEXCOORD0;
        float3 worldNormal : TEXCOORD1;
    };

    // Implementation of the vertex shader
    vertOut vert(vertIn v)
    {

        vertOut o;
        float epsilon = _epsilon;

        // create some immaginary neighbours so we can calculate a normal
        float3 v1 = v.vertex + float3(epsilon, 0.0f, 0.0f);
        float3 v2 = v.vertex + float3(0.0f, 0.0f, epsilon);

        // calculate the ys for all three verts
        v.vertex.y += _waveMagnitude * sin(_xFrequency * v.vertex.x + 
                                    _zFrequency * v.vertex.z +
                                    _timeFrequency * _Time.y);

        v1.y += _waveMagnitude * sin(_xFrequency * v1.x + 
                                    _zFrequency * v1.z +
                                    _timeFrequency * _Time.y);

        v2.y += _waveMagnitude * sin(_xFrequency * v2.x + 
                                    _zFrequency * v2.z +
                                    _timeFrequency * _Time.y);


        // By reducing delta y we can make a smoother effect on  the water;
        v1.y -= (v1.y - v.vertex.y) * _normalSmoothness;
        v2.y -= (v2.y - v.vertex.y) * _normalSmoothness;

        // calculate the normal from the vertex and the two imaginary vertices
        float3 normal = cross(v2-v.vertex, v1-v.vertex);

        // Convert Vertex position and corresponding normal into world coords.
        // Note that we have to multiply the normal by the transposed inverse of the world 
        // transformation matrix (for cases where we have non-uniform scaling; we also don't
        // care about the "fourth" dimension, because translations don't affect the normal) 
        float4 worldVertex = mul(unity_ObjectToWorld, v.vertex);
        float3 worldNormal = normalize(mul(transpose((float3x3)unity_WorldToObject), normal));

        // Transform vertex in world coordinates to camera coordinates, and pass colour
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.color = _WaterColor;

        // Pass out the world vertex position and world normal to be interpolated
        // in the fragment shader (and utilised)
        o.worldVertex = worldVertex;
        o.worldNormal = worldNormal;

        return o;
    }

    // Implementation of the fragment shader
    fixed4 frag(vertOut v) : SV_Target
    {



        // Our interpolated normal might not be of length 1
        float3 interpNormal = normalize(v.worldNormal);

        // Calculate ambient RGB intensities
        float Ka = _Ka;
        float3 amb = v.color.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb * Ka;

        // Calculate diffuse RBG reflections, we save the results of L.N because we will use it again
        // (when calculating the reflected ray in our specular component)
        float fAtt = _fAtt;
        float Kd = _Kd;
        float3 L = normalize(_PointLightPosition - v.worldVertex.xyz);
        float LdotN = dot(L, interpNormal);
        float3 dif = fAtt * _PointLightColor.rgb * Kd * v.color.rgb * saturate(LdotN);

        // Calculate specular reflections
        float Ks = _Ks;
        float specN = _specN; // Values>>1 give tighter highlights
        float3 V = normalize(_WorldSpaceCameraPos - v.worldVertex.xyz);
        // Using classic reflection calculation:
        float3 R = normalize((2.0 * LdotN * interpNormal) - L);
        float3 spe = fAtt * _PointLightColor.rgb * Ks * pow(saturate(dot(V, R)), specN);
        // Using Blinn-Phong approximation:
        //specN = 25; // We usually need a higher specular power when using Blinn-Phong
        //float3 H = normalize(V + L);
        //float3 spe = fAtt * _PointLightColor.rgb * Ks * pow(saturate(dot(interpNormal, H)), specN);

        // Combine Phong illumination model components
        float4 returnColor = float4(0.0f, 0.0f, 0.0f, 0.0f);
        returnColor.rgb = amb.rgb + dif.rgb + spe.rgb;
        returnColor.a = v.color.a;

        return returnColor;
    }
        ENDCG
    }
    }
}

