using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrainGenerator : MonoBehaviour
{
    public const float maxViewDist = 450f;
    public Transform viewer;

    public static Vector2 viewPos;
    private int chunkSize;
    private int chunksVisible;

    private Dictionary<Vector2, TerrainChunk> terrainDictionary = new Dictionary<Vector2, TerrainChunk>();
    private List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    private void Start()
    {
        chunkSize = MapGenerator.chunkSize - 1;
        chunksVisible = Mathf.RoundToInt(maxViewDist / chunkSize);
    }

    private void Update()
    {
        viewPos = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();

        Debug.Log(viewPos);

    }

    private void UpdateVisibleChunks()
    {
        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
        {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        int currChunkCoordX = Mathf.RoundToInt(viewPos.x / chunkSize);
        int currChunkCoordY = Mathf.RoundToInt(viewPos.y / chunkSize);

        for (int yOffset = -chunksVisible; yOffset <= chunksVisible; yOffset++)
        {
            for (int xOffset = -chunksVisible; xOffset <= chunksVisible; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currChunkCoordX + xOffset, currChunkCoordY + yOffset);

                if (terrainDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    if (terrainDictionary[viewedChunkCoord].isVisible())
                    {
                        terrainChunksVisibleLastUpdate.Add(terrainDictionary[viewedChunkCoord]);
                    }
                } else
                {
                    terrainDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform));
                }
            }
        }
    }

    public class TerrainChunk
    {
        GameObject meshObj;
        Vector2 position;
        Bounds bounds;

        public TerrainChunk(Vector2 coord, int size, Transform parent)
        {
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 posV3 = new Vector3(position.x, 0, position.y);

            meshObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObj.transform.position = posV3;
            meshObj.transform.localScale = Vector3.one * size / 10f;
            meshObj.transform.parent = parent;

            SetVisible(false);
        }

        //Enables and Disables the chunk based on the viewer position;
        public void UpdateTerrainChunk()
        {
            float viewDistFromEdge = Mathf.Sqrt(bounds.SqrDistance(viewPos));
            bool visible = viewDistFromEdge <= maxViewDist;

            SetVisible(visible);
        }

        public void SetVisible(bool visible)
        {
            meshObj.SetActive(visible);
        }

        public bool isVisible() { return meshObj.activeSelf; }
    }
}
