using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class PoolControler : MonoBehaviour
{
    [Header("---- POOL CONTROLER TO INIT POOL ----")]
    //[Header("Put object pool to list Pool or Resources/Pool")]
    //[Header("Preload: Init Poll")]
    //[Header("Spawn: Take object from pool")]
    //[Header("Despawn: return object to pool")]
    //[Header("Collect: return objects type to pool")]
    //[Header("CollectAll: return all objects to pool")]

    [Space]
    [Header("Pool")]
    public List<PoolAmount> Pool;

    [Header("Particle")]
    public ParticleAmount[] Particle;


	public float spawnSpace = 1.5f;
	public Transform spawnPoint;
	[SerializeField] private ColorData colorData;

	public void Awake()
    {
		foreach (var pool in Pool)
		{
			HBPool.Preload(pool.prefab, pool.amount, pool.root);
		}
	}

	private void Start()
	{
		SpawnBots();
	}

    private void SpawnBots()
    {
		foreach (var p in Pool)
		{
			for (int i = 0; i < p.amount; i++)
			{
				Vector3 spawnPos = spawnPoint.position + new Vector3(i * spawnSpace, 0.93f, -0.27f);
				Quaternion spawnRot = Quaternion.identity;

				GameUnit unit = HBPool.Spawn<GameUnit>(p.prefab.poolType, spawnPos, spawnRot);

				Bot bot = unit as Bot;
				if (bot != null && colorData != null)
				{
					EColorType colorType = (EColorType)Random.Range(1, System.Enum.GetValues(typeof(EColorType)).Length);
					Material mat = colorData.GetColorMat(colorType);
					bot.SetColor(mat, colorType);
				}
			}
		}
	}
    private void SpawnBricks()
    {

    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(PoolControler))]
public class PoolControlerEditor : Editor
{
    PoolControler pool;

    private void OnEnable()
    {
        pool = (PoolControler)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Create Quick Root"))
        {
            for (int i = 0; i < pool.Pool.Count; i++)
            {
                if (pool.Pool[i].root == null)
                {
                    Transform tf = new GameObject(pool.Pool[i].prefab.poolType.ToString()).transform;
                    tf.parent = pool.transform;
                    pool.Pool[i].root = tf; 
                }
            }
        }

        if (GUILayout.Button("Get Prefab Resource"))
        {
            GameUnit[] resources = Resources.LoadAll<GameUnit>("Pool");

            for (int i = 0; i < resources.Length; i++)
            {
                bool isDuplicate = false;
                for (int j = 0; j < pool.Pool.Count; j++)
                {
                    if (resources[i].poolType == pool.Pool[j].prefab.poolType)
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (!isDuplicate)
                {
                    Transform root = new GameObject(resources[i].name).transform;

                    PoolAmount newPool = new PoolAmount(root, resources[i], SimplePool.DEFAULT_POOL_SIZE, true);

                    pool.Pool.Add(newPool);
                }
            }
        }
    }
}

#endif

[System.Serializable]
public class PoolAmount
{
    [Header("-- Pool Amount --")]
    public Transform root;
    public GameUnit prefab;
    public int amount;
    public bool collect;

    public PoolAmount (Transform root, GameUnit prefab, int amount, bool collect)
    {
        this.root = root;
        this.prefab = prefab;
        this.amount = amount;
        this.collect = collect;
    }
}


[System.Serializable]
public class ParticleAmount
{
    public Transform root;
    public ParticleSystem prefab;
    public int amount;
}

public enum PoolType
{
    None,
    Bot,
    Brick
}


