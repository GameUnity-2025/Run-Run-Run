using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Players list giống ShopManager.players")]
    public List<PlayerData> allPlayers;

    [Header("Spawn settings")]
    public Transform spawnPoint;
    public bool dontDestroyOnLoad = false;
    public bool parentUnderSpawnPoint = true;

    [Header("Visibility overrides (tùy chọn)")]
    public bool applySortingOverrides = false;
    public string sortingLayerName = "Default";
    public int sortingOrder = 10;
    public bool forceDefaultLayer = true;

    [Header("Cinemachine retarget")]
    public bool retargetCinemachine = true;
    public Transform cameraTargetOverride;
    public UnityEngine.Object cinemachineCameraReference; // Drag GO or Component (CinemachineCamera/VirtualCamera)
    public Transform fixedCameraTargetAnchor; // Set this as Cinemachine Tracking Target in Inspector

    private void Start()
    {
        string selected = PlayerPrefs.GetString("SelectedCharacter", "PlayerDefault");
        int count = allPlayers != null ? allPlayers.Count : 0;
        Debug.Log($"PlayerSpawner: SelectedCharacter='{selected}', allPlayers.Count={count}");
        if (allPlayers != null)
        {
            for (int i = 0; i < allPlayers.Count; i++)
            {
                var pd = allPlayers[i];
                string name = pd != null ? pd.playerName : "<null>";
                Debug.Log($"PlayerSpawner: allPlayers[{i}]='{name}' prefab={(pd!=null && pd.characterPrefab!=null)}");
            }
        }

        PlayerData chosen = allPlayers != null ? allPlayers.Find(p => p.playerName == selected) : null;

        if (chosen == null && allPlayers != null)
        {
            // Fallback về PlayerDefault nếu không tìm thấy lựa chọn
            chosen = allPlayers.Find(p => p.playerName == "PlayerDefault");
            if (chosen != null)
            {
                Debug.LogWarning($"PlayerSpawner: Không tìm thấy '{selected}', fallback về 'PlayerDefault'.");
            }
        }

        if (chosen != null && chosen.characterPrefab != null)
        {
            Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
            pos.z = 0f; // đảm bảo trong mặt phẳng camera 2D
            Quaternion rot = spawnPoint != null ? spawnPoint.rotation : transform.rotation;
            GameObject player = Instantiate(chosen.characterPrefab, pos, rot);
            if (parentUnderSpawnPoint && spawnPoint != null)
            {
                player.transform.SetParent(spawnPoint, true);
            }
            player.transform.localScale = Vector3.one;
            player.SetActive(true);

            if (forceDefaultLayer)
            {
                SetLayerRecursively(player, LayerMask.NameToLayer("Default"));
            }

            var sr = player.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                Debug.Log($"PlayerSpawner: SpriteRenderer sortingLayer='{sr.sortingLayerName}', order={sr.sortingOrder}");
                if (applySortingOverrides)
                {
                    sr.sortingLayerName = sortingLayerName;
                    sr.sortingOrder = sortingOrder;
                    Debug.Log($"PlayerSpawner: Applied sorting override -> layer='{sortingLayerName}', order={sortingOrder}");
                }
            }
            else
            {
                Debug.LogWarning("PlayerSpawner: Không tìm thấy SpriteRenderer trong prefab (sẽ không thấy nếu không có renderer).");
            }
            Debug.Log($"PlayerSpawner: Spawned '{chosen.playerName}' at {player.transform.position}");
            if (retargetCinemachine)
            {
                Transform camTarget = cameraTargetOverride != null ? cameraTargetOverride : player.transform;
                if (fixedCameraTargetAnchor != null)
                {
                    fixedCameraTargetAnchor.SetParent(camTarget, false);
                    fixedCameraTargetAnchor.localPosition = Vector3.zero;
                    Debug.Log("PlayerSpawner: Reparented fixed camera target anchor under spawned player.");
                }
                StartCoroutine(RetryRetarget(camTarget));
            }
            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(player);
            }
        }
        else
        {
            Debug.LogWarning($"PlayerSpawner: Không tìm thấy PlayerData hoặc prefab hợp lệ cho '{selected}'.");
        }
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    private System.Collections.IEnumerator RetryRetarget(Transform camTarget)
    {
        // Thử nhiều frame để tránh vấn đề thứ tự khởi tạo (tối đa ~1s ở 60fps)
        for (int i = 0; i < 60; i++)
        {
            if (CameraFollowBridge.Instance != null)
            {
                CameraFollowBridge.Instance.SetTarget(camTarget);
                Debug.Log("PlayerSpawner: Retarget via CameraFollowBridge.");
                yield break;
            }
            if (TryRetargetCinemachineV3(camTarget) || TryRetargetCinemachineV2(camTarget))
            {
                Debug.Log("PlayerSpawner: Retarget camera success.");
                yield break;
            }
            yield return null; // đợi 1 frame
        }
        Debug.LogWarning("PlayerSpawner: Không tìm thấy Cinemachine (v2/v3) trong scene để retarget.");
    }

    private bool TryRetargetCinemachineV3(Transform camTarget)
    {
        // Tìm Cinemachine v3 theo tên lớp để tránh khác namespace/assembly
        Type v3Type = GetTypeByName("Unity.Cinemachine.CinemachineCamera")
            ?? GetTypeBySimpleName("CinemachineCamera");
        Component vcam = FindVcamComponent(v3Type, "CinemachineCamera");
        if (vcam == null) return false;

        var targetProp = v3Type.GetProperty("Target");
        if (targetProp == null) return false;
        var targetObj = targetProp.GetValue(vcam, null);
        if (targetObj == null) return false;

        var trackingProp = targetObj.GetType().GetProperty("TrackingTarget");
        var lookAtProp = targetObj.GetType().GetProperty("LookAtTarget");
        if (trackingProp == null) return false;
        trackingProp.SetValue(targetObj, camTarget, null);
        if (lookAtProp != null) lookAtProp.SetValue(targetObj, null, null);
        Debug.Log("PlayerSpawner: Cinemachine v3 retargeted to spawned player.");
        return true;
    }

    private bool TryRetargetCinemachineV2(Transform camTarget)
    {
        // Tìm Cinemachine v2 theo tên lớp
        Type v2Type = GetTypeByName("Cinemachine.CinemachineVirtualCamera")
            ?? GetTypeBySimpleName("CinemachineVirtualCamera");
        Component vcam = FindVcamComponent(v2Type, "CinemachineVirtualCamera");
        if (vcam == null) return false;

        var followProp = v2Type.GetProperty("Follow");
        var lookAtProp = v2Type.GetProperty("LookAt");
        if (followProp == null) return false;
        followProp.SetValue(vcam, camTarget, null);
        if (lookAtProp != null) lookAtProp.SetValue(vcam, null, null);
        Debug.Log("PlayerSpawner: Cinemachine v2 retargeted to spawned player.");
        return true;
    }

    private Type GetTypeByName(string fullName)
    {
        // Quét tất cả assemblies đã load để tìm type theo tên đầy đủ
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                var t = asm.GetType(fullName, false);
                if (t != null) return t;
            }
            catch { }
        }
        return null;
    }

    private Type GetTypeBySimpleName(string name)
    {
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types;
            try { types = asm.GetTypes(); }
            catch { continue; }
            foreach (var t in types)
            {
                if (t.Name == name) return t;
            }
        }
        return null;
    }

    private Component FindVcamComponent(Type preferredType, string fallbackTypeName)
    {
        // 1) Nếu người dùng kéo vào một GameObject, thử tìm component phù hợp trên đó
        if (cinemachineCameraReference is GameObject go)
        {
            if (preferredType != null)
            {
                var c = go.GetComponent(preferredType);
                if (c != null) return c;
            }
            // fallback theo tên lớp
            var all = go.GetComponents<Component>();
            foreach (var c in all)
            {
                if (c != null && c.GetType().Name == fallbackTypeName)
                    return c;
            }
        }
        // 2) Nếu người dùng kéo sẵn Component
        if (cinemachineCameraReference is Component comp)
        {
            if (preferredType == null || preferredType.IsInstanceOfType(comp))
                return comp;
        }
        // 3) Tự tìm trong scene theo type hoặc theo tên lớp
        if (preferredType != null)
        {
            var found = FindObjectOfType(preferredType) as Component;
            if (found != null) return found;
        }
        var allComps = Resources.FindObjectsOfTypeAll<Component>();
        foreach (var c in allComps)
        {
            if (c == null) continue;
            if (c.gameObject.scene.IsValid() && c.GetType().Name == fallbackTypeName)
                return c;
        }
        return null;
    }
}


