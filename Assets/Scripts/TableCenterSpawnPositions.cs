using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit
{
    public class TableCenterSpawnPositions : MonoBehaviour
    {
        [SerializeField] public GameObject SpawnObject;
        [SerializeField] public bool CheckOverlaps = true;
        [SerializeField] public LayerMask LayerMask = -1;
        [SerializeField] public float SurfaceClearanceDistance = 0.1f;

        private void Start()
        {
            if (MRUK.Instance)
            {
                MRUK.Instance.RegisterSceneLoadedCallback(SpawnOnTables);
            }
        }

        public void SpawnOnTables()
        {
            if (SpawnObject == null)
            {
                Debug.LogError("TableCenterSpawnPositions: No spawn object assigned!");
                return;
            }

            foreach (var room in MRUK.Instance.Rooms)
            {
                SpawnOnTablesInRoom(room);
            }
        }

        private void SpawnOnTablesInRoom(MRUKRoom room)
        {
            var prefabBounds = Utilities.GetPrefabBounds(SpawnObject);
            float objectHeight = 0f;
            if (prefabBounds.HasValue)
            {
                objectHeight = prefabBounds.Value.size.y;
            }

            var labelFilter = new LabelFilter(MRUKAnchor.SceneLabels.TABLE);

            foreach (var anchor in room.Anchors)
            {
                if (!labelFilter.PassesFilter(anchor.Label))
                {
                    continue;
                }

                if (!anchor.PlaneRect.HasValue)
                {
                    continue;
                }

                // Get the center of the table in local space
                Vector3 centerLocal = anchor.PlaneRect.Value.center;

                // Convert to world space
                Vector3 spawnPosition = anchor.transform.TransformPoint(centerLocal);
                
                // For tables, we expect the normal to be pointing up
                Vector3 surfaceNormal = anchor.transform.up;

                // Offset the position by half the object's height plus a small buffer
                spawnPosition += surfaceNormal * (objectHeight * 0.5f + 0.001f);

                // Calculate base rotation to align with the table surface
                Quaternion baseRotation = Quaternion.LookRotation(
                    anchor.transform.forward,
                    surfaceNormal
                );

                // Add 90-degree X rotation
                Quaternion spawnRotation = baseRotation * Quaternion.Euler(90, 0, 0);

                if (!room.IsPositionInRoom(spawnPosition))
                {
                    continue;
                }

                if (CheckOverlaps && prefabBounds.HasValue)
                {
                    if (Physics.CheckBox(
                        spawnPosition,
                        prefabBounds.Value.extents,
                        spawnRotation,
                        LayerMask,
                        QueryTriggerInteraction.Ignore))
                    {
                        continue;
                    }
                }

                if (SpawnObject.gameObject.scene.path == null)
                {
                    var instance = Instantiate(
                        SpawnObject,
                        spawnPosition,
                        spawnRotation,
                        transform
                    );
                    instance.name = $"{SpawnObject.name} - {anchor.name}";
                }
                else
                {
                    SpawnObject.transform.position = spawnPosition;
                    SpawnObject.transform.rotation = spawnRotation;
                    return;
                }
            }
        }
    }
}