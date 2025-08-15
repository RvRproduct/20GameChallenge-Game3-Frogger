// Game and Code By RvRproduct (Roberto Reynoso)
using UnityEngine;

public abstract class BasePoolObject : MonoBehaviour
{
    public string poolTag { get; private set; }
    protected string poolReturnTag { get; private set; }

    /// <summary>
    /// Must Return a Pool Tag string for the Object Pooling
    /// system to understand where to find the object you are
    /// trying to retrieve will using it from the pool.
    /// </summary>
    /// <returns>string</returns>
    protected abstract string ProvidePoolTag();
    protected abstract string ProvidePoolReturnTag();


    protected virtual void Awake()
    {
        SetPoolTag(ProvidePoolTag());
        SetPoolReturnTag(ProvidePoolReturnTag());
    }

    public void SetPoolTag(string _poolTag)
    {
        poolTag = ProvidePoolTag();
    }

    public void SetPoolReturnTag(string _poolReturnTag)
    {
        poolReturnTag = _poolReturnTag;
    }

    public void SetPoolReturnTagForReplay()
    {
        // For this game only if anything more was added to this game
        // This would need to be corrected
        if (poolReturnTag == PoolTags.EntityReturnTags.EntityRightReturn)
        {
            poolReturnTag = PoolTags.EntityReturnTags.EntityLeftReturn;
        }
        else
        {
            poolReturnTag = PoolTags.EntityReturnTags.EntityRightReturn;
        }
    }

    protected void BasePoolObjectTrigger(Collider2D collision)
    {
        if (collision.gameObject.tag == poolReturnTag)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BasePoolObjectTrigger(collision);
    }
}
