using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private ColorData colorData;
    [SerializeField] private Transform spawnPoints;
    [SerializeField] private int numberToSpawn;
    [SerializeField] private float spawnSpace = 1.5f;
    [SerializeField] private GameUnit pfBot;

    private void Awake()
    {
        HBPool.Preload(pfBot, numberToSpawn, this.transform);
    }
    private void Start()
    {
        SpawnRunners();
    }

    void SpawnRunners()
    {
        for (int i = 0; i < numberToSpawn; i++)
        {
            Vector3 spawnPos = spawnPoints.position + new Vector3(i * spawnSpace, 0f, 0f);
            Quaternion spawnRot = Quaternion.identity;

            Bot runner = HBPool.Spawn<Bot>(PoolType.Bot, spawnPos, spawnRot);

            EColorType colorType = (EColorType)Random.Range(1, System.Enum.GetValues(typeof(EColorType)).Length);
            Material mat = colorData.GetColorMat(colorType);

            runner.SetColor(mat, colorType);
        }
    }
}
