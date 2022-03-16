using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAim : MonoBehaviour
{
    [Header("Rotations")]
    [Tooltip("炮塔底座负责方位角上的旋转")]
    [SerializeField] private Transform turretBase = null;

    [Tooltip("炮管负责俯仰角的旋转 ")]
    [SerializeField] private Transform barrels = null;

    [Header("Elevation")]
    [Tooltip("炮塔俯仰角固定速度")]
    public float ElevationSpeed = 30f;

    [Tooltip("最大仰角高度")]
    public float MaxElevation = 60f;

    [Tooltip("最低俯角高度")]
    public float MaxDepression = 5f;

    [Header("Traverse")]

    [Tooltip("炮塔平面旋转角固定速度")]
    public float TraverseSpeed = 60f;

    [Tooltip("可设置的左右最大旋转限制角度")]
    [SerializeField] private bool hasLimitedTraverse = false;
    [Range(0, 179)] public float LeftLimit = 120f;
    [Range(0, 179)] public float RightLimit = 120f;
    private float limitedTraverseAngle = 0f;

    [Header("Behavior")]

    [Tooltip("空闲状态，炮塔固定向前，无任何旋转表现")]
    public bool IsIdle = false;

    [Tooltip("炮塔主动瞄准的目标点，在非空闲状态的情况下，炮塔将主动跟踪该瞄准点")]
    public Vector3 AimPosition = Vector3.zero;

    [Tooltip("瞄准误差冗余角设置")]
    [SerializeField]
    private float aimedThreshold = 5f;
 

    [Header("Debug")]
    // 测试用射线，旋转范围显示
    public bool DrawDebugRay = true;
    public bool DrawDebugArcs = false;

    private float angleToTarget = 0f;
    private float elevation = 0f;

    private bool hasBarrels = false;

    private bool isAimed = false;
    private bool isBaseAtRest = false;
    private bool isBarrelAtRest = false;

    /// <summary>
    /// True when the turret cannot rotate freely in the horizontal axis.
    /// </summary>
    public bool HasLimitedTraverse { get { return hasLimitedTraverse; } }

    /// <summary>
    /// True when the turret is idle and at its resting position.
    /// </summary>
    public bool IsTurretAtRest { get { return isBarrelAtRest && isBaseAtRest; } }

    /// <summary>
    /// True when the turret is aimed at the given <see cref="AimPosition"/>. When the turret
    /// is idle, this is never true.
    /// </summary>
    public bool IsAimed { get { return isAimed; } }

    /// <summary>
    /// Angle in degress to the given <see cref="AimPosition"/>. When the turret is idle,
    /// the angle reports 999.
    /// </summary>
    public float AngleToTarget { get { return IsIdle ? 999f : angleToTarget; } }

    public Vector3 LauchDir { get { return barrels.forward; } }

    [SerializeField]
    // 镭射辅助(并入Debug模块中)
    Transform laserBeam = default;

    Vector3 laserBeamScale;

    private void Awake()
    {
        hasBarrels = barrels != null;
        if (turretBase == null)
            Debug.LogError(name + ": TurretAim requires an assigned TurretBase!");
        laserBeamScale = laserBeam.localScale;
    }

    private void Update()
    {
        if (IsIdle)
        {
            laserBeam.localScale = Vector3.zero;
            if (!IsTurretAtRest)
                RotateTurretToIdle();
            isAimed = false;
        }
        else
        {
            RotateBaseToFaceTarget(AimPosition);

            if (hasBarrels)
                RotateBarrelsToFaceTarget(AimPosition);

            // 检测当前炮塔转向是否满足瞄准冗余
            angleToTarget = GetTurretAngleToTarget(AimPosition);
            isAimed = angleToTarget < aimedThreshold;

            isBarrelAtRest = false;
            isBaseAtRest = false;
        }
    }

    private float GetTurretAngleToTarget(Vector3 targetPosition)
    {

        float angle = 999f;

        if (hasBarrels)
        {
            angle = Vector3.Angle(targetPosition - barrels.position, barrels.forward);
        }
        else
        {
            Vector3 flattenedTarget = Vector3.ProjectOnPlane(
                targetPosition - turretBase.position,
                turretBase.up);

            angle = Vector3.Angle(
                flattenedTarget - turretBase.position,
                turretBase.forward);
        }

        return angle;
    }

    /// <summary>
    /// 休眠状态归正炮塔
    /// </summary>
    private void RotateTurretToIdle()
    {
        if (hasLimitedTraverse)
        {
            //待定解释
            limitedTraverseAngle = Mathf.MoveTowards(
                limitedTraverseAngle, 0f,
                TraverseSpeed * Time.deltaTime);

           
            if (Mathf.Abs(limitedTraverseAngle) > Mathf.Epsilon)
                turretBase.localEulerAngles = Vector3.up * limitedTraverseAngle;
            else
                isBaseAtRest = true;
        }
        else
        {
            turretBase.rotation = Quaternion.RotateTowards(
                turretBase.rotation,
                transform.rotation,
                TraverseSpeed * Time.deltaTime);

            isBaseAtRest = Mathf.Abs(turretBase.localEulerAngles.y) < Mathf.Epsilon;
        }

        if (hasBarrels)
        {
            elevation = Mathf.MoveTowards(elevation, 0f, ElevationSpeed * Time.deltaTime);
            if (Mathf.Abs(elevation) > Mathf.Epsilon)
                barrels.localEulerAngles = Vector3.right * -elevation;
            else
                isBarrelAtRest = true;
        }
        else // Barrels automatically at rest if there are no barrels.
            isBarrelAtRest = true;
    }

    private void RotateBarrelsToFaceTarget(Vector3 targetPosition)
    {
        // 将世界坐标下的目标偏转向量转移到炮塔底座的本地空间
        Vector3 localTargetPos = turretBase.InverseTransformDirection(targetPosition - barrels.position);
        
        Vector3 flattenedVecForBarrels = Vector3.ProjectOnPlane(localTargetPos, Vector3.up);

        float targetElevation = Vector3.Angle(flattenedVecForBarrels, localTargetPos);
        targetElevation *= Mathf.Sign(localTargetPos.y);

        targetElevation = Mathf.Clamp(targetElevation, -MaxDepression, MaxElevation);
        elevation = Mathf.MoveTowards(elevation, targetElevation, ElevationSpeed * Time.deltaTime);

        if (Mathf.Abs(elevation) > Mathf.Epsilon)
            barrels.localEulerAngles = Vector3.right * -elevation;

        float dist = Vector3.Distance(barrels.position, localTargetPos);
        laserBeamScale.z = dist;
        laserBeam.localScale = laserBeamScale;
        laserBeam.position = barrels.position + 0.5f * laserBeam.forward* dist;

#if UNITY_EDITOR
        if (DrawDebugRay)
            Debug.DrawRay(barrels.position, barrels.forward * localTargetPos.magnitude, Color.red);
#endif
    }

    /// <summary>
    ///  驱使炮塔底座旋转向目标方向
    /// </summary>
    private void RotateBaseToFaceTarget(Vector3 targetPosition)
    {
        Vector3 turretUp = transform.up;

        Vector3 vecToTarget = targetPosition - turretBase.position;
        Vector3 flattenedVecForBase = Vector3.ProjectOnPlane(vecToTarget, turretUp);

        if (hasLimitedTraverse)
        {
            Vector3 turretForward = transform.forward;
            // 对需要旋转的角度评估计算
            float targetTraverse = Vector3.SignedAngle(turretForward, flattenedVecForBase, turretUp);
            targetTraverse = Mathf.Clamp(targetTraverse, -LeftLimit, RightLimit);
            // 保障当前旋转角度在最大限制内
            limitedTraverseAngle = Mathf.MoveTowards(
                limitedTraverseAngle,
                targetTraverse,
                TraverseSpeed * Time.deltaTime);

            // 确定最终旋转角不为无限小浮点数
            if (Mathf.Abs(limitedTraverseAngle) > Mathf.Epsilon)
                turretBase.localEulerAngles = Vector3.up * limitedTraverseAngle;
        }
        else
        {
            turretBase.rotation = Quaternion.RotateTowards(
                Quaternion.LookRotation(turretBase.forward, turretUp),
                Quaternion.LookRotation(flattenedVecForBase, turretUp),
                TraverseSpeed * Time.deltaTime);
        }

#if UNITY_EDITOR
        if (DrawDebugRay && !hasBarrels)
            Debug.DrawRay(turretBase.position,
                turretBase.forward * flattenedVecForBase.magnitude,
                Color.cyan);
#endif
    }

#if UNITY_EDITOR
    // This should probably go in an Editor script, but dealing with Editor scripts
    // is a pain in the butt so I'd rather not.
    private void OnDrawGizmosSelected()
    {
        if (!DrawDebugArcs)
            return;

        if (turretBase != null)
        {
            const float kArcSize = 10f;
            Color colorTraverse = new Color(1f, .5f, .5f, .1f);
            Color colorElevation = new Color(.5f, 1f, .5f, .1f);
            Color colorDepression = new Color(.5f, .5f, 1f, .1f);

            Transform arcRoot = barrels != null ? barrels : turretBase;

            // Red traverse arc
            UnityEditor.Handles.color = colorTraverse;
            if (hasLimitedTraverse)
            {
                UnityEditor.Handles.DrawSolidArc(
                    arcRoot.position, turretBase.up,
                    transform.forward, RightLimit,
                    kArcSize);
                UnityEditor.Handles.DrawSolidArc(
                    arcRoot.position, turretBase.up,
                    transform.forward, -LeftLimit,
                    kArcSize);
            }
            else
            {
                UnityEditor.Handles.DrawSolidArc(
                    arcRoot.position, turretBase.up,
                    transform.forward, 360f,
                    kArcSize);
            }

            if (barrels != null)
            {
                // Green elevation arc
                UnityEditor.Handles.color = colorElevation;
                UnityEditor.Handles.DrawSolidArc(
                    barrels.position, barrels.right,
                    turretBase.forward, -MaxElevation,
                    kArcSize);

                // Blue depression arc
                UnityEditor.Handles.color = colorDepression;
                UnityEditor.Handles.DrawSolidArc(
                    barrels.position, barrels.right,
                    turretBase.forward, MaxDepression,
                    kArcSize);
            }
        }
    }
#endif
}
