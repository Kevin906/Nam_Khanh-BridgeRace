using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PoolControler : MonoBehaviour
{
	[Header("---- POOL CONTROLER TO INIT POOL ----")]
	public List<PoolAmount> Pool;

	[Space]
	[Header("Brick Management")]
	[SerializeField] private List<Bot> botList = new List<Bot>();
	[SerializeField] private Player player;

	[Space]
	[Header("Bot Spawner")]
	public float spawnSpace = 1.5f;
	public Transform spawnPoint;
	[SerializeField] private ColorData colorData;

	[Header("Platform Reference")]
	[SerializeField] private Transform platform;


	private void Awake()
	{
		foreach (var pool in Pool)
		{
			if (pool.prefab != null)
				HBPool.Preload(pool.prefab, pool.amount, pool.root);
		}
	}

	private void Start()
	{
		SpawnBots();
		SpawnBricks();
	}

	private void SpawnBots()
	{
		botList.Clear();

		PoolAmount botPool = Pool.Find(p => p.prefab != null && p.prefab.poolType == PoolType.Bot);
		if (botPool == null) return;

		int botCount = Mathf.Max(0, botPool.amount);
		int playerCount = player != null ? 1 : 0;

		List<EColorType> availableColors = System.Enum.GetValues(typeof(EColorType)).Cast<EColorType>().Where(c => c != EColorType.Default).ToList();

		Shuffle(availableColors);
		Queue<EColorType> colorQueue = new Queue<EColorType>(availableColors);

		//bot color
		for (int i = 0; i < botCount; i++)
		{
			Vector3 spawnPos = spawnPoint.position + new Vector3(i * spawnSpace, 5f, -12f);
			Quaternion spawnRot = Quaternion.identity;

			GameUnit unit = HBPool.Spawn<GameUnit>(botPool.prefab.poolType, spawnPos, spawnRot);
			Bot bot = unit as Bot;
			if (bot != null)
			{
				EColorType assignColor = colorQueue.Count > 0 ? colorQueue.Dequeue() : EColorType.Default;
				if (assignColor != EColorType.Default && colorData != null)
				{
					Material mat = colorData.GetColorMat(assignColor);
					bot.SetColor(mat, assignColor);
				}
				else
				{
					bot.SetColor(null, EColorType.Default);
				}

				botList.Add(bot);
			}
		}

		//player color
		if (player != null)
		{
			EColorType pColor = colorQueue.Count > 0 ? colorQueue.Dequeue() : EColorType.Default;
			if (pColor != EColorType.Default && colorData != null)
			{
				Material mat = colorData.GetColorMat(pColor);
				player.SetColor(mat, pColor);
			}
			else
			{
				player.SetColor(null, EColorType.Default);
			}
		}
	}

	private void SpawnBricks()
	{
		PoolAmount brickPool = Pool.Find(p => p.prefab && p.prefab.poolType == PoolType.Brick);
		if (brickPool == null || platform == null || colorData == null)
		{
			return;
		}

		int total = Mathf.Max(1, brickPool.amount);
		List<(EColorType, Material)> colors = new List<(EColorType, Material)>();

		// Lấy màu của bot + player
		foreach (Bot b in botList)
		{
			if (b != null && b.colorType != EColorType.Default)
			{
				colors.Add((b.colorType, colorData.GetColorMat(b.colorType)));
			}
		}

		if (player != null && player.colorType != EColorType.Default)
		{
			colors.Add((player.colorType, colorData.GetColorMat(player.colorType)));
		}

		if (colors.Count == 0)
		{
			return;
		}

		// Tính lưới spawn
		Vector3 pos = platform.position;
		Vector3 scale = platform.localScale;
		float y = pos.y + scale.y * 0.5f + 0.1f;
		int row = Mathf.CeilToInt(Mathf.Sqrt(total));
		int col = Mathf.CeilToInt((float)total / row);
		float dx = scale.x / col;
		float dz = scale.z / row;
		Vector3 start = pos - new Vector3(scale.x, 0, scale.z) * 0.5f;

		// Spawn brick
		for (int i = 0; i < total; i++)
		{
			int r = i / col;
			int c = i % col;
			Vector3 p = new Vector3(start.x + (c + 0.5f) * dx, y, start.z + (r + 0.5f) * dz);

			GameUnit unit = HBPool.Spawn<GameUnit>(brickPool.prefab.poolType, p, Quaternion.identity);
			Brick brick = unit as Brick;

			if (brick != null)
			{
				(EColorType colorType, Material mat) = colors[Random.Range(0, colors.Count)];
				brick.SetColor(mat, colorType);
			}
		}
	}




	private void Shuffle<T>(List<T> list)
	{
		for (int i = list.Count - 1; i > 0; i--)
		{
			int rnd = Random.Range(0, i + 1);
			T tmp = list[i];
			list[i] = list[rnd];
			list[rnd] = tmp;
		}
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

	public PoolAmount(Transform root, GameUnit prefab, int amount, bool collect)
	{
		this.root = root;
		this.prefab = prefab;
		this.amount = amount;
		this.collect = collect;
	}
}

public enum PoolType
{
	None,
	Bot,
	Brick
}
