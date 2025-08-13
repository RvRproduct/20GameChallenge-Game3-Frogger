// Game and Code By RvRproduct (Roberto Reynoso)

using PoolTags;
using System.Collections;
using System.Collections.Generic;
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

    private void SetPoolTag(string _poolTag)
    {
        if (poolTag == null)
        {
            poolTag = ProvidePoolTag();
        }
        else
        {
            throw new System.NotImplementedException("Pool tag not added to the pooling object");
        }
    }

    private void SetPoolReturnTag(string _poolReturnTag)
    {
        poolReturnTag = _poolReturnTag;
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
