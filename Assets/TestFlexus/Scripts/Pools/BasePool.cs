using System;
using System.Collections.Generic;
using UnityEngine;

namespace TestFlexus.Scripts.Pools
{
    [Serializable]
    public abstract class BasePool<T> : ScriptableObject where T : MonoBehaviour
    {

        [SerializeField] private int maxSize;
        [SerializeField] private T behaviour;
        
        private int _currentSize;
        private Queue<T> _pool = new();
        
        public void ClearPool()
        {
            _pool.Clear();
            _currentSize = 0;
        }

        public T Get()
        {
            T obj;
            if (_pool.Count > 0)
            {
                obj = _pool.Dequeue();
            }
            else
            {
                if (_currentSize >= maxSize)
                {
                    Debug.LogError("pool is full");
                    return null;
                }
                else
                {
                    _currentSize++;
                    obj = Instantiate(behaviour).GetComponent<T>();
                }
            }
            
            obj.gameObject.SetActive(true);
            
            return obj;
        }

        public void Release(T obj)
        {
            if (_pool.Count > maxSize)
            {
                Destroy(obj);
            }
            else
            {
                _currentSize--;
                obj.gameObject.SetActive(false);
                _pool.Enqueue(obj);
            }
        }
    }
}