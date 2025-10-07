using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerWeaponVisuals : MonoBehaviour
{
    private Animator anim;
    private Rig rig;

    [SerializeField] private Transform[] gunTransforms;

    [SerializeField] private Transform pistol;
    [SerializeField] private Transform revolver;
    [SerializeField] private Transform autoRifle;
    [SerializeField] private Transform shotgun;
    [SerializeField] private Transform rifle;

    private Transform currentGun;

    [Header("Rig")]
    [SerializeField] private float rigWeightIncreaseRate;

    [Header("Left Hand IK")]
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private float leftHandIkWeightIncreaseRate;

    private bool shouldIncreaseRigWeight, shouldIncreaseLeftHandIKWeight, isGrabbingWeapon;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();
        SwitchGun(pistol);
    }

    private void Update()
    {
        CheckWeaponSwitch();

        if (Input.GetKeyDown(KeyCode.R) && !isGrabbingWeapon)
        {
            anim.SetTrigger("Reload");
            ReduceRigWeight();
        }

        UpdateRigWeight();
        UpdateLeftHandIKWeight();
    }

    private void UpdateLeftHandIKWeight()
    {
        if (shouldIncreaseLeftHandIKWeight)
        {
            leftHandIK.weight += leftHandIkWeightIncreaseRate * Time.deltaTime;
            if (leftHandIK.weight >= 1) shouldIncreaseLeftHandIKWeight = false;
        }
    }

    private void UpdateRigWeight()
    {
        if (shouldIncreaseRigWeight)
        {
            rig.weight += rigWeightIncreaseRate * Time.deltaTime;
            if (rig.weight >= 1) shouldIncreaseRigWeight = false;
        }
    }

    private void PlayWeaponGrabAnimation(GrabType grabType)
    {
        leftHandIK.weight = 0;
        ReduceRigWeight();
        anim.SetFloat("WeaponGrabType", ((float)grabType));
        anim.SetTrigger("WeaponGrab");

        SetBusyGrabbingWeaponTo(true);
    }

    private void ReduceRigWeight() => rig.weight = 0.15f;

    public void MaximizeRigWeight() => shouldIncreaseRigWeight = true;

    public void MaximizeLeftHandWeight() => shouldIncreaseLeftHandIKWeight = true;

    public void SetBusyGrabbingWeaponTo(bool busy)
    {
        isGrabbingWeapon = busy;
        anim.SetBool("isBusyGrabbingWeapon", isGrabbingWeapon);
    }

    private void SwitchGun(Transform gunTransform)
    {
        SwitchOffGuns();
        gunTransform.gameObject.SetActive(true);
        currentGun = gunTransform;
        AttachLeftHand();
    }

    private void SwitchOffGuns()
    {
        for (int i = 0; i < gunTransforms.Length; i++)
        {
            gunTransforms[i].gameObject.SetActive(false);
        }
    }

    private void AttachLeftHand()
    {
        Transform targetTransform = currentGun.GetComponentInChildren<LeftHandTargetTransform>().transform;
        leftHandTarget.localPosition = targetTransform.localPosition;
        leftHandTarget.localRotation = targetTransform.localRotation;
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 1; i < anim.layerCount; i++) anim.SetLayerWeight(i, 0);

        anim.SetLayerWeight(layerIndex, 1);
    }

    private void SwitchWeaponTo(Transform weapon, KeyCode key, int layer, GrabType grab)
    {
        if (Input.GetKeyDown(key))
        {
            SwitchGun(weapon);
            SwitchAnimationLayer(layer);
            PlayWeaponGrabAnimation(grab);
        }
    }

    private void CheckWeaponSwitch()
    {
        SwitchWeaponTo(pistol, KeyCode.Alpha1, 1, GrabType.SideGrab);
        SwitchWeaponTo(revolver, KeyCode.Alpha2, 1, GrabType.SideGrab);
        SwitchWeaponTo(autoRifle, KeyCode.Alpha3, 1, GrabType.BackGrab);
        SwitchWeaponTo(shotgun, KeyCode.Alpha4, 2, GrabType.BackGrab);
        SwitchWeaponTo(rifle, KeyCode.Alpha5, 3, GrabType.BackGrab);
    }

    public enum GrabType
    {
        SideGrab,
        BackGrab
    };
}
