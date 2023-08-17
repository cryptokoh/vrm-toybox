using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(ObjectPool))]
public class ObjectPoolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ObjectPool objectPool = (ObjectPool)target;

        EditorGUILayout.Space();

        // Draw a label and a button to refresh the list
        EditorGUILayout.LabelField("Pooled Objects");
        if (GUILayout.Button("Refresh"))
        {
            RefreshPooledObjects(objectPool);
        }

        // Display the pooled objects
        foreach (var kvp in objectPool.pooledObjectsDictionary)
        {
            EditorGUILayout.LabelField($"Tag: {kvp.Key}");

            EditorGUI.indentLevel++;

            foreach (PooledObject pooledObject in kvp.Value)
            {
                EditorGUILayout.ObjectField(pooledObject.gameObject, typeof(GameObject), true);
            }

            EditorGUI.indentLevel--;
        }
    }

    private void RefreshPooledObjects(ObjectPool objectPool)
{
    foreach (ObjectPool.Pool pool in objectPool.pools)
    {
        List<PooledObject> pooledObjects = new List<PooledObject>();

        foreach (GameObject obj in ObjectPool.Instance.poolDictionary[pool.tag])
        {
            PooledObject pooledObject = obj.GetComponent<PooledObject>();
            if (pooledObject != null)
            {
                pooledObject.Tag = pool.tag;
                pooledObjects.Add(pooledObject);
            }
        }

        objectPool.pooledObjectsDictionary.Add(pool.tag, pooledObjects);
    }

    objectPool.pooledObjectsDictionary.Clear(); // Move this line here

    // Mark the target object as dirty to trigger inspector refresh
    EditorUtility.SetDirty(objectPool);
}

}
