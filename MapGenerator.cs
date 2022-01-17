using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NoiseMap,
        ColourMap,
        Mesh
    }

    public const int chunkSize = 241;

    [Range(0, 6)]
    public int levelOfDetail;

    public float scale;

    public int octaves;
    [Range(0f, 1f)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public MapDisplay mapDisplay;
    public bool autoUpdate;
    public DrawMode drawMode;
    public TerrainTypes[] regions;

    public void GenerateMap()
    {
        float[,] noisemap = Noise.GenerateNoiseMap(chunkSize, chunkSize, seed, scale, octaves, persistance, lacunarity, offset);

        Color[] colourmap = new Color[chunkSize * chunkSize];
        for (int y = 0; y < chunkSize; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                float currentHeight = noisemap[x, y];

                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colourmap[y * chunkSize + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }

        switch (drawMode)
        {
            case DrawMode.NoiseMap:
                mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(noisemap));
                break;
            case DrawMode.ColourMap:
                mapDisplay.DrawTexture(TextureGenerator.TextureFromColourMap(colourmap, chunkSize, chunkSize));
                break;
            case DrawMode.Mesh:
                mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(noisemap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColourMap(colourmap, chunkSize, chunkSize));
                break;
            default:
                break;
        }
    }

    private void OnValidate()
    {
        if (lacunarity < 1) 
        {
            lacunarity = 1;
        }

        if (octaves < 0)
        {
            octaves = 0;
        }
    }

    [System.Serializable]
    public struct TerrainTypes
    {
        public string name;
        public float height;
        public Color colour;
    }
}
