using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerWeaponVisuals weaponVisuals;

    private void Start()
    {
        weaponVisuals = GetComponentInParent<PlayerWeaponVisuals>();
    }

    public void ReloadIsOver() => weaponVisuals.MaximizeRigWeight();

    public void ReturnRig()
    {
        weaponVisuals.MaximizeRigWeight();
        weaponVisuals.MaximizeLeftHandWeight();
    }

    public void WeaponGrabIsOver() => weaponVisuals.SetBusyGrabbingWeaponTo(false);
}
