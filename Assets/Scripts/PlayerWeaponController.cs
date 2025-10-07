using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform gunPoint;

    [SerializeField] private Transform weaponHolder;

    private void Start()
    {
        player = GetComponent<Player>();

        player.controls.Character.Fire.performed += context => Shoot();
    }

    private void Shoot()
    {
        GameObject newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(gunPoint.forward));
        newBullet.GetComponent<Rigidbody>().velocity = BulletDirection() * bulletSpeed;
        Destroy(newBullet, 2);

        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }

    public Vector3 BulletDirection()
    {
        Vector3 direction = (player.aim.Aim().position - gunPoint.position).normalized;
        if (!player.aim.CanAimPrecisely() && player.aim.Target() == null) direction.y = 0;
        //weaponHolder.LookAt(aim);
        //gunPoint.LookAt(aim);
        return direction;
    }

    public Transform GunPoint() => gunPoint;
}
