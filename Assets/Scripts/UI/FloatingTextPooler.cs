using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingTextPooler : MonoBehaviour
{
    public static FloatingTextPooler Instance { get; private set; }

    [SerializeField]
    private GameObject floatingTextPrefab;
    private Queue<GameObject> floatingTextPool = new Queue<GameObject>();
    private Queue<GameObject> floatingTextInUse = new Queue<GameObject>();
    public int maxPoolSize = 100;

    private void Awake()
    {
        Instance = this;

        // Instantiate initial pool
        for (int i = 0; i < maxPoolSize; i++)
        {
            GameObject textObj = Instantiate(floatingTextPrefab);
            textObj.SetActive(false);
            floatingTextPool.Enqueue(textObj);
        }
    }

    public GameObject Get()
    {
        if (floatingTextInUse.Count >= maxPoolSize)
        {
            // If maximum pool size is reached, deactivate the oldest object
            // and return it to the pool.
            GameObject oldestTextObj = floatingTextInUse.Dequeue();
            oldestTextObj.SetActive(false);
            floatingTextPool.Enqueue(oldestTextObj);
        }

        // Get an object from the pool
        GameObject textObj = floatingTextPool.Dequeue();
        textObj.SetActive(true);
        floatingTextInUse.Enqueue(textObj);
        return textObj;
    }

    public void ReturnToPool(GameObject textObj)
    {
        TMP_Text floatingText = textObj.GetComponent<TMP_Text>();
        if (floatingText != null)
        {
            floatingText.color = Color.white;
        }

        textObj.SetActive(false);
        floatingTextPool.Enqueue(textObj);
    }

}
