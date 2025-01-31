using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mika
{
    [DisallowMultipleComponent]
    public sealed class SpawnManager : MonoBehaviour
    {
        #region Singleton
        public static SpawnManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }
        #endregion
        [Header("Spawn Settings")]
        [SerializeField] private float spawnDelay;
        [SerializeField] private float spawnInterval;
        [SerializeField] private float asteroidMinSpeed, asteroidMaxSpeed;
        private WaitForSeconds waitDelay, waitInterval;
        [SerializeField] Collider bgCollider;
        private Bounds bounds;


        private void Start()
        {
            waitDelay = new WaitForSeconds(spawnDelay);
            waitInterval = new WaitForSeconds(spawnInterval);
            // utilise les bounds du mesh collider du background
            bounds = bgCollider.bounds;
            StartCoroutine(SpawnAsteroids());
        }

        private IEnumerator SpawnAsteroids()
        {
            yield return waitDelay;
            while (true)
            {
                bool spawnOnXSide = OnChance(); // choisit x ou y pour l'endroit d'apparition
                Vector3 wantedPos;
                // choisit un c�t� (min/max) et une position al�atoire le long de l'autre axe
                if (spawnOnXSide)
                {
                    wantedPos = new Vector3(OnChance() ? bounds.min.x : bounds.max.x, GetRandomYInsideBounds(), 0f);
                }
                else
                {
                    wantedPos = new Vector3(GetRandomXInsideBounds(), OnChance() ? bounds.min.y : bounds.max.y, 0f);
                }
                // d�finit un nouveau centre, �tant par d�faut (0,0,0), pour cr�er le vecteur direction
                Vector3 newCenter = spawnOnXSide ? new Vector3(0f, GetRandomYInsideBounds(), 0f) : new Vector3(GetRandomXInsideBounds(), 0f, 0f);
                Vector3 direction = newCenter - wantedPos;
                SpawnAsteroid(wantedPos, direction.normalized * Random.Range(asteroidMinSpeed, asteroidMaxSpeed));
                yield return waitInterval;
            }
        }

        private float GetRandomXInsideBounds()
        {
            return Random.Range(bounds.min.x, bounds.max.x);
        }

        private float GetRandomYInsideBounds()
        {
            return Random.Range(bounds.min.y, bounds.max.y);
        }

        private void SpawnAsteroid(Vector3 pos, Vector3 velocity)
        {
            GameObject o = AsteroidPool.Instance.Get();
            if (o.TryGetComponent(out AsteroidMove asteroidMoveScript))
            {
                asteroidMoveScript.SetPositionAndVelocity(pos, velocity);
            }
        }

        public bool OnChance()
        {
            return Random.Range(0, 2) == 0;
        }
    }
}