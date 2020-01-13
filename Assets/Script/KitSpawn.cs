using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitSpawn : MonoBehaviour
{
    public GameObject[] ammoPrefab;
    public GameObject ammoParent;
    public float timeBetweenSpawn;
    private float cooldown;
    public int MaxX;
    public int MinX;
    public int MaxZ;
    public int MinZ;
    // Start is called before the first frame update
    void Start()
    {
        timeBetweenSpawn = Random.Range(3, 8);
        cooldown = Random.Range(1, 3);
    }

    // Update is called once per frame
    void Update()
    {
        if(cooldown<=0f)
        {
            SpawnPack();
            timeBetweenSpawn = Random.Range(3, 8);
            cooldown = Random.Range(1, 3);
            cooldown = timeBetweenSpawn-cooldown;
        }
        cooldown -= Time.deltaTime;
    }

    void SpawnPack()
    {
        float randomChance = Random.Range(0.1f, 1.5f);
        int randomX = Random.Range(MinX, MaxX);

        int randomZ = Random.Range(MinZ, MaxZ);

        int randomY = 0;

        if (randomChance <= 0.5f)
        {
            GameObject s = Instantiate(ammoPrefab[0], new Vector3(randomX,randomY,randomZ), Quaternion.identity);
            Destroy(s, 5.0f);
        }
        if (randomChance>= 0.6f && randomChance <=1.0f)
        {
            GameObject p = Instantiate(ammoPrefab[1], new Vector3(randomX, randomY, randomZ), Quaternion.identity);
            Destroy(p, 5.0f);
        }
        if (randomChance > 1.0f)
        {
            GameObject k = Instantiate(ammoPrefab[2], new Vector3(randomX, randomY, randomZ), Quaternion.identity);
            Destroy(k, 5.0f);
        }
    }
}
