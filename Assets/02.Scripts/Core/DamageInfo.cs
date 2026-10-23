using UnityEngine;

public struct DamageInfo
{
    public float Value;
    public Vector3 HitPoint;
    public Vector3 HitNormal;

    public DamageInfo(float value, Vector3 hitPoint, Vector3 hitNormal)
    {
        Value = value;
        HitPoint = hitPoint;
        HitNormal = hitNormal;
    }

    public DamageInfo(float value)
    {
        Value = value;
        HitPoint = Vector3.zero;
        HitNormal = Vector3.zero;
    }
}