using PoolTags;
using UnityEngine;

public class EntityMovingManager : MonoBehaviour
{
    static public EntityMovingManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // I dont like this
    public void EntityMoveCommander(GameObject _entity, 
        string _entityTag, Vector3 _spawnPoint)
    {
        Entity entity = _entity.GetComponent<Entity>();


    }
}
