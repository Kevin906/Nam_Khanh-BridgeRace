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
		// Tìm pool chứa prefab brick
		PoolAmount brickPool = Pool.Find(p => p.prefab != null && p.prefab.poolType == PoolType.Brick);
		if (brickPool == null || platform == null) return;

		int totalBricks = Mathf.Max(0, brickPool.amount);
		if (totalBricks == 0) return;

		// Lấy danh sách tất cả màu khả dụng (bot + player)
		var availableColors = new List<(EColorType color, Material mat)>();
		foreach (var bot in botList)
		{
			if (bot != null && bot.colorType != EColorType.Default)
				availableColors.Add((bot.colorType, colorData.GetColorMat(bot.colorType)));
		}
		if (player != null && player.colorType != EColorType.Default)
			availableColors.Add((player.colorType, colorData.GetColorMat(player.colorType)));

		if (availableColors.Count == 0) return;

		// Tính vùng platform
		Vector3 platformPos = platform.position;
		Vector3 platformScale = platform.localScale;

		float halfX = platformScale.x * 0.5f;
		float halfZ = platformScale.z * 0.5f;
		float yPos = platformPos.y + platformScale.y * 0.5f + 0.1f;

		// Xác định số hàng và cột (dựa theo số lượng brick)
		int rowCount = Mathf.CeilToInt(Mathf.Sqrt(totalBricks)); // số hàng
		int colCount = Mathf.CeilToInt((float)totalBricks / rowCount); // số cột

		// Tính khoảng cách giữa các brick
		float spacingX = (platformScale.x - 1f) / colCount; // trừ nhỏ cho khoảng biên
		float spacingZ = (platformScale.z - 1f) / rowCount;

		int brickIndex = 0;

		for (int row = 0; row < rowCount; row++)
		{
			for (int col = 0; col < colCount; col++)
			{
				if (brickIndex >= totalBricks) break;

				// Tính vị trí theo hàng cột
				float posX = platformPos.x - halfX + (col + 0.5f) * spacingX;
				float posZ = platformPos.z - halfZ + (row + 0.5f) * spacingZ;
				Vector3 spawnPos = new Vector3(posX, yPos, posZ);

				// Spawn brick
				GameUnit unit = HBPool.Spawn<GameUnit>(brickPool.prefab.poolType, spawnPos, Quaternion.identity);
				Brick brick = unit as Brick;
				if (brick != null)
				{
					// Random màu trong các màu khả dụng
					var randomOwner = availableColors[Random.Range(0, availableColors.Count)];
					brick.SetColor(randomOwner.mat, randomOwner.color);
				}

				brickIndex++;
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
