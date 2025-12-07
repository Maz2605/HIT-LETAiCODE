using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace pooling
{
    public static class PoolingManager
    {
        private static Dictionary<int, Pool> listPools;

        private static void Init(GameObject prefab = null)
        {
            if (listPools == null)
            {
                listPools = new Dictionary<int, Pool>();
            }

            if (prefab != null)
            {
                int key = prefab.GetInstanceID();
                if (!listPools.ContainsKey(key))
                    listPools[key] = new Pool(prefab);
            }
        }

        // ---------------- Spawn ----------------
        public static GameObject Spawn(GameObject prefab)
        {
            Init(prefab);
            return listPools[prefab.GetInstanceID()].Spawn(Vector3.zero, Quaternion.identity, null);
        }

        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion quaternion, Transform parent = null)
        {
            Init(prefab);
            return listPools[prefab.GetInstanceID()].Spawn(position, quaternion, parent);
        }

        public static T Spawn<T>(T prefab) where T : Component
        {
            if (prefab == null) return null;
            Init(prefab.gameObject);
            return listPools[prefab.gameObject.GetInstanceID()].Spawn<T>(Vector3.zero, Quaternion.identity, null);
        }

        public static T Spawn<T>(T prefab, Vector3 position, Quaternion quaternion, Transform parent = null) where T : Component
        {
            if (prefab == null) return null;
            Init(prefab.gameObject);
            return listPools[prefab.gameObject.GetInstanceID()].Spawn<T>(position, quaternion, parent);
        }

        // ---------------- Despawn ----------------
        // Despawn by GameObject instance
        public static void Despawn(GameObject instance)
        {
            if (instance == null) return;
            if (listPools == null || listPools.Count == 0)
            {
                Object.Destroy(instance);
                return;
            }

            Pool found = null;
            foreach (var pool in listPools.Values)
            {
                if (pool.ContainsInstance(instance))
                {
                    found = pool;
                    break;
                }
            }

            if (found == null)
            {
                // not from pool -> destroy
                Object.Destroy(instance);
            }
            else
            {
                found.Despawn(instance);
            }
        }

        // Despawn by Component instance
        public static void Despawn(Component instance)
        {
            if (instance == null) return;
            Despawn(instance.gameObject);
        }

        // Clear and destroy all pooled objects
        public static void ClearPool()
        {
            if (listPools == null) return;

            foreach (var p in listPools.Values)
            {
                p.DestroyAll();
            }

            listPools.Clear();
        }
    }

    public class Pool
    {
        private readonly Queue<GameObject> pools;
        private readonly List<GameObject> allObjects;
        public readonly HashSet<int> idObject;
        private readonly GameObject prefabObject;
        private int id = 0;

        public Pool(GameObject gameObject)
        {
            prefabObject = gameObject;
            pools = new Queue<GameObject>();
            allObjects = new List<GameObject>();
            idObject = new HashSet<int>();
        }

        public GameObject Spawn(Vector3 position, Quaternion quaternion, Transform parent = null)
        {
            GameObject newObject = null;

            // Reuse if available
            while (pools.Count > 0)
            {
                var candidate = pools.Dequeue();
                if (candidate == null) continue;
                newObject = candidate;
                break;
            }

            if (newObject == null)
            {
                newObject = Object.Instantiate(prefabObject, position, quaternion, parent);
                id++;
                idObject.Add(newObject.GetInstanceID());
                allObjects.Add(newObject);
                newObject.name = prefabObject.name + "_" + id;
            }
            else
            {
                newObject.transform.SetParent(parent);
                newObject.transform.SetPositionAndRotation(position, quaternion);
                newObject.SetActive(true);
            }

            return newObject;
        }

        public T Spawn<T>(Vector3 position, Quaternion quaternion, Transform parent = null) where T : Component
        {
            var go = Spawn(position, quaternion, parent);
            return go != null ? go.GetComponent<T>() : null;
        }

        public void Despawn(GameObject gameObject)
        {
            if (gameObject == null) return;

            if (!gameObject.activeSelf)
            {
                // already despawned
                return;
            }

            gameObject.SetActive(false);

            // Put back to queue for reuse
            pools.Enqueue(gameObject);
        }

        public bool ContainsInstance(GameObject instance)
        {
            if (instance == null) return false;
            return idObject.Contains(instance.GetInstanceID());
        }

        /// <summary>
        /// Destroy all instances created by this pool (both active and pooled)
        /// </summary>
        public void DestroyAll()
        {
            // Destroy pooled + active
            if (allObjects != null)
            {
                for (int i = allObjects.Count - 1; i >= 0; i--)
                {
                    var go = allObjects[i];
                    if (go != null)
                    {
                        Object.Destroy(go);
                    }
                }

                allObjects.Clear();
            }

            // clear queue and ids
            pools.Clear();
            idObject.Clear();
        }
    }
}
