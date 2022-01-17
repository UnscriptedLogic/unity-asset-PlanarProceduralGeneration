using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        int meshSimplifiyIncrement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
        int vertsPerLine = (width - 1) / meshSimplifiyIncrement + 1;

        //Centering purposes
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        MeshData meshData = new MeshData(vertsPerLine, vertsPerLine);
        int vertexIndex = 0;

        for (int y = 0; y < height; y += meshSimplifiyIncrement)
        {
            for (int x = 0; x < width; x += meshSimplifiyIncrement)
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier,topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    //Adding them in the clockwise orientation
                    meshData.AddTriangle(vertexIndex, vertexIndex + vertsPerLine + 1, vertexIndex + vertsPerLine);
                    meshData.AddTriangle(vertexIndex + vertsPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex += 1;
            }
        }

        return meshData;
    }
}

public class MeshData{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    private int meshDimensions;
    private int triangleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        meshDimensions = meshWidth * meshHeight;

        vertices = new Vector3[meshDimensions];
        uvs = new Vector2[meshDimensions];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }
}