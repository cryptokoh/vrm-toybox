using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObjectPool : MonoBehaviour
{
    public GameObject dotPrefab; // Dot prefab to be instantiated
    public int poolSize = 10; // Initial size of the pool

    private Queue<GameObject> dotPool = new Queue<GameObject>(); // The pool of dots

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject dot = Instantiate(dotPrefab);
            dot.SetActive(false);
            dotPool.Enqueue(dot);
        }
    }

    public GameObject GetDot()
    {
        if (dotPool.Count == 0)
        {
            // If pool is empty, create a new dot
            GameObject dot = Instantiate(dotPrefab);
            dot.SetActive(false);
            return dot;
        }
        else
        {
            // Otherwise, reuse a dot from the pool
            GameObject dot = dotPool.Dequeue();
            dot.SetActive(true);
            return dot;
        }
    }

    public void ReturnDot(GameObject dot)
    {
        dot.SetActive(false);
        dotPool.Enqueue(dot);
    }
}
