// Game and Code By RvRproduct (Roberto Reynoso)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class ObjectPoolPair
    {
        [HideInInspector] public string poolTag;
        public int MAXPOOLSIZE;
        public GameObject objectToPool;
    }

    [Header("Object Pool")]
    [SerializeField] protected List<ObjectPoolPair> objectPoolPairs;
    protected Dictionary<string, List<GameObject>> objectPool;

    protected virtual void Awake()
    {
        objectPool = new Dictionary<string, List<GameObject>>();
    }

    protected void SetUpObjectPool()
    {
        foreach (ObjectPoolPair objectPoolPair in objectPoolPairs)
        {
            for (int obj = 0; obj < objectPoolPair.MAXPOOLSIZE; obj++)
            {
                GameObject currentPoolObject =
                    (Instantiate(objectPoolPair.objectToPool, gameObject.transform));

                BasePoolObject poolObjectBase = currentPoolObject.GetComponent<BasePoolObject>();

                if (!objectPool.ContainsKey(poolObjectBase.poolTag))
                {
                    objectPool.Add(poolObjectBase.poolTag, new List<GameObject>());
                    objectPoolPair.poolTag = poolObjectBase.poolTag;
                }

                objectPool[objectPoolPair.poolTag].Add(currentPoolObject);
                objectPool[objectPoolPair.poolTag][obj].SetActive(false);
            }
        }
    }

    protected List<ObjectPoolPair> GetObjectPoolPairs()
    {
        return objectPoolPairs;
    }

    protected GameObject GetValidObjectInPool(string _poolTag)
    {
        foreach (GameObject obj in objectPool[_poolTag])
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        return null;
    }

}