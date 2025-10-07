using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Player player;
    private PlayerControls controls;

    [Header("Aim Visuals")]
    [SerializeField] private LineRenderer aimLaser;

    [Header("Aim Control")]
    [SerializeField] private Transform aim;
    [SerializeField] private bool isAimingPrecisely;
    [SerializeField] private bool isLockedOnTarget;

    [Header("Camera Control")]
    [SerializeField] private Transform cameraTarget;
    [Range(0.5f, 1.0f)][SerializeField] private float minCameraDistance = 1.5f;
    [Range(1.0f, 5.0f)][SerializeField] private float maxCameraDistance = 4f;
    [Range(3.0f, 5.0f)][SerializeField] private float cameraSensitivity = 5f;
    [Space]
    [SerializeField] private LayerMask aimLayerMask;

    private Vector2 mouseInput;
    private RaycastHit lastKnownMouseHit;

    private void Start()
    {
        player = GetComponent<Player>();
        controls = player.controls;

        AssignInputEvents();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) isAimingPrecisely = !isAimingPrecisely;
        if (Input.GetKeyDown(KeyCode.L)) isLockedOnTarget = !isLockedOnTarget;

        UpdateAimVisuals();
        UpdateAimPosition();
        UpdateCameraPosition();
    }

    private void UpdateAimVisuals()
    {
        float gunDistance = 4f, laserTipLength = 0.5f;
        Transform gunPoint = player.weapon.GunPoint();
        Vector3 laserDirection = player.weapon.BulletDirection(), endPoint = gunPoint.position + laserDirection * gunDistance;
        if (Physics.Raycast(gunPoint.position, laserDirection, out RaycastHit hit, gunDistance))
        {
            endPoint = hit.point;
            laserTipLength = 0f;
        }
        aimLaser.SetPosition(0, gunPoint.position);
        aimLaser.SetPosition(1, endPoint);
        aimLaser.SetPosition(2, endPoint + laserDirection * laserTipLength);
    }

    public Transform Target()
    {
        Transform target = null;
        if (GetMouseHitInfo().transform.GetComponent<Target>()) target = GetMouseHitInfo().transform;
        return target;
    }

    private void UpdateAimPosition()
    {
        Transform target = Target();
        if (target && isLockedOnTarget)
        {
            aim.position = target.position;
            return;
        }

        aim.position = GetMouseHitInfo().point;
        if (!isAimingPrecisely) aim.position = new Vector3(aim.position.x, transform.position.y + 1, aim.position.z);
    }

    private void UpdateCameraPosition()
    {
        cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesiredCameraPosition(), cameraSensitivity * Time.deltaTime);
    }

    public Transform Aim() => aim;

    public bool CanAimPrecisely() => isAimingPrecisely;

    private Vector3 DesiredCameraPosition()
    {
        bool movingDownwards = player.movement.moveInput.y < -0.5f;
        float actualMaxCameraDistance = movingDownwards ? minCameraDistance : maxCameraDistance;

        Vector3 desiredCameraPosition = GetMouseHitInfo().point;
        Vector3 aimDirection = (desiredCameraPosition - transform.position).normalized;
        float distanceToDesiredPosition = Vector3.Distance(transform.position, desiredCameraPosition);
        float clampedDistance = Mathf.Clamp(distanceToDesiredPosition, minCameraDistance, actualMaxCameraDistance);

        desiredCameraPosition = transform.position + aimDirection * clampedDistance;
        desiredCameraPosition.y = transform.position.y + 1;

        return desiredCameraPosition;
    }

    private void AssignInputEvents()
    {
        controls.Character.Aim.performed += context => mouseInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => mouseInput = Vector2.zero;
    }

    public RaycastHit GetMouseHitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseInput);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
        {
            lastKnownMouseHit = hitInfo;
            return hitInfo;
        }
        return lastKnownMouseHit;
    }
}
