using System;
using UnityEngine;

public class CameraFollowBridge : MonoBehaviour
{
    public static CameraFollowBridge Instance;
    public UnityEngine.Object cameraOverride; // optional: drag your CinemachineCamera/VirtualCamera GO or Component here

    private void Awake()
    {
        Instance = this;
    }

    public void SetTarget(Transform target)
    {
        if (target == null) return;

        // 1) Ưu tiên cameraOverride nếu có
        if (TryRetargetOnObject(cameraOverride, target)) return;

        // 2) Thử tìm trên chính GameObject này
        if (TryRetargetOnObject(this, target)) return;

        // 3) Quét toàn scene tìm CinemachineCamera/CinemachineVirtualCamera
        var all = Resources.FindObjectsOfTypeAll<Component>();
        foreach (var c in all)
        {
            if (c == null) continue;
            if (!c.gameObject.scene.IsValid()) continue;
            string n = c.GetType().Name;
            if (n == "CinemachineCamera" || n == "CinemachineVirtualCamera")
            {
                if (TryRetargetOnComponent(c, target)) return;
            }
        }

        Debug.LogWarning("CameraFollowBridge: Không tìm thấy component Cinemachine trên camera này.");
    }

    private bool TryRetargetOnObject(UnityEngine.Object obj, Transform target)
    {
        if (obj == null) return false;
        if (obj is GameObject go)
        {
            var comps = go.GetComponents<Component>();
            foreach (var c in comps)
            {
                if (TryRetargetOnComponent(c, target)) return true;
            }
            return false;
        }
        if (obj is Component comp)
        {
            return TryRetargetOnComponent(comp, target);
        }
        return false;
    }

    private bool TryRetargetOnComponent(Component comp, Transform target)
    {
        if (comp == null) return false;
        var name = comp.GetType().Name;
        if (name == "CinemachineCamera")
        {
            var targetProp = comp.GetType().GetProperty("Target");
            if (targetProp == null) return false;
            var targetObj = targetProp.GetValue(comp, null);
            if (targetObj == null) return false;
            var trackingProp = targetObj.GetType().GetProperty("TrackingTarget");
            var lookAtProp = targetObj.GetType().GetProperty("LookAtTarget");
            if (trackingProp == null) return false;
            trackingProp.SetValue(targetObj, target, null);
            if (lookAtProp != null) lookAtProp.SetValue(targetObj, null, null);
            Debug.Log("CameraFollowBridge: Retargeted Cinemachine v3.");
            return true;
        }
        if (name == "CinemachineVirtualCamera")
        {
            var followProp = comp.GetType().GetProperty("Follow");
            var lookAtProp = comp.GetType().GetProperty("LookAt");
            if (followProp == null) return false;
            followProp.SetValue(comp, target, null);
            if (lookAtProp != null) lookAtProp.SetValue(comp, null, null);
            Debug.Log("CameraFollowBridge: Retargeted Cinemachine v2.");
            return true;
        }
        return false;
    }
}


