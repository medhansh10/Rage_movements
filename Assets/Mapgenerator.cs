using UnityEngine;
public class LevelGenerator:MonoBehaviour
{
    public Transform GenerationPoint;
    public GameObject FloorPrefab;
    public int MinPlatformLength=2;
    public int MaxPlatformLength=6;
    public int MinGap=2;
    public int MaxGap=5;
    public float HeightStep=2f;
    public int MinHeight=-2;
    public int MaxHeight=3;
    Vector2 spawnPos;
    float tileWidth;
    int currentHeight;
    void Start()
    {
        tileWidth=FloorPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
        ResetLevel();
    }
    void Update()
    {
        while(spawnPos.x<GenerationPoint.position.x)
        {
            int gap=Random.Range(MinGap,MaxGap+1);
            spawnPos.x+=gap*tileWidth;
            int heightChange=Random.Range(-1,2);
            currentHeight=Mathf.Clamp(currentHeight+heightChange,MinHeight,MaxHeight);
            spawnPos.y=currentHeight*HeightStep;
            SpawnPlatform(Random.Range(MinPlatformLength,MaxPlatformLength+1));
        }
    }
    void SpawnPlatform(int length)
    {
        for(int i=0;i<length;i++)
        {
            Instantiate(FloorPrefab,spawnPos,Quaternion.identity,transform);
            spawnPos.x+=tileWidth;
        }
    }
    public void ResetLevel()
    {
        for(int i=transform.childCount-1;i>=0;i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        spawnPos=transform.position;
        currentHeight=0;
        SpawnPlatform(Random.Range(MinPlatformLength,MaxPlatformLength+1));
    }
}
