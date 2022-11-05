using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Knife.Effects;

public class RaycastWeapon : MonoBehaviour
{

    class Bullet
    {
        public float time;
        public Vector3 initialPosition;
        public Vector3 initialVelocity;
        public TrailRenderer tracer;
        public int bounce;
    }

    [HideInInspector]
    public bool isFiring = false;


    public ThirdPersonShooterController.WeaponSlot weaponSlot;

    public float damage;
    public int currentAmmo;
    public int maxAmmo;

    public float killForce = 10f;
    public bool isFullAuto;
    public float bulletSpeed = 1000f;
    public float bulletDrop = 0.0f;

    public int maxBounces = 0;
    [Tooltip("Bullets Per Second")]
    public int fireRate = 25;

    //public ParticleGroupEmitter[] muzzleFlashes;
    public ParticleSystem[] muzzleFlashes;
    public ParticleSystem hitEffect;

    public TrailRenderer tracerEffect;

    public Transform raycastOrigin;
    public Transform raycastDestination;

    public string weaponName;

    public GameObject weaponMag;

    public ThirdPersonShooterController playerController;

    Ray ray;
    RaycastHit hitInfo;

    float accumulatedTime;
    List<Bullet> bullets = new List<Bullet>();
    public float maxLifetime = 3.0f;

    [HideInInspector]
    public WeaponRecoil recoil;

    public float nextTimeToFire = 0f;
    private void Awake()
    {
        recoil = GetComponent<WeaponRecoil>();
    }
    Vector3 GetPosition(Bullet bullet)
    {
        //position + velocity * time + 0.5 * gravity * time^2
        Vector3 gravity = Vector3.down * bulletDrop;
        return (bullet.initialPosition) + (bullet.initialVelocity * bullet.time) + (0.5f * gravity * bullet.time * bullet.time);
    }

    Bullet CreateBullet(Vector3 position, Vector3 velocity)
    {
        Bullet bullet = new Bullet();
        bullet.initialPosition = position;
        bullet.initialVelocity = velocity;
        bullet.time = 0.0f;
        bullet.bounce = maxBounces;
        bullet.tracer = Instantiate(tracerEffect, position, Quaternion.identity);
        bullet.tracer.AddPosition(position);
        return bullet;

    }

    //public void StartFiring()
    //{
    //    isFiring = true;
    //    accumulatedTime = 0.0f;
    //    FireBullet();
    //}


    //public void UpdateFiring(float deltaTime)
    //{
    //    accumulatedTime += deltaTime;
    //    float fireInterval = 1.0f / fireRate;
    //    while (accumulatedTime >= 0)
    //    {
    //        FireBullet();
    //        accumulatedTime -= fireInterval;
    //    }
    //}


    public void TryFire()
    {
        if(Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + (1 / (float)fireRate);
            FireBullet();
        }
    }
    public void UpdateBullets(float deltaTime)
    {
        SimulateBullets(deltaTime);
        DestroyBullets();
    }

    void SimulateBullets(float deltaTime)
    {
        bullets.ForEach(bullet =>
        {
            Vector3 p0 = GetPosition(bullet);
            bullet.time += deltaTime;
            Vector3 p1 = GetPosition(bullet);
            RaycastSegment(p0, p1, bullet);
        });
    }

    void DestroyBullets()
    {
        bullets.RemoveAll(bullet => bullet.time >= maxLifetime);
        foreach(Bullet bullet in bullets)
        {
            if(bullet.time >= maxLifetime)
            {
                bullets.Remove(bullet);
                Destroy(bullet.tracer);
            }
        }
    }
    void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
    {
        Vector3 direction = end - start;

        float distance = direction.magnitude;

        ray.origin = start;
        ray.direction = direction;

        //Transform hitTransform = null;


        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            //Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 1f);

            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);
            //hitTransform = hitInfo.transform;
            bullet.tracer.transform.position = hitInfo.point;
            bullet.time = maxLifetime;

            if(bullet.bounce > 0)
            {
                bullet.time = 0;
                bullet.initialPosition = hitInfo.point;
                bullet.initialVelocity = Vector3.Reflect(bullet.initialVelocity, hitInfo.normal);
                bullet.bounce--;
            }

            var rb2d = hitInfo.collider.GetComponent<Rigidbody>();
            if (rb2d)
            {
                rb2d.AddForceAtPosition(ray.direction * 20, hitInfo.point, ForceMode.Impulse);
            }

            var hitBox = hitInfo.transform.root.GetComponent<EnemyHitbox>();
            if (hitBox)
            {
                bool isHeadshot = false;
                if(hitInfo.collider.tag == "Head")
                {
                    isHeadshot = true;
                }
                hitBox.OnRaycastHit(this, ray.direction, isHeadshot,killForce);
            }
        }
        else
        {
            bullet.tracer.transform.position = end;
        }

        //Debug.Log("Shot");
        //if (hitTransform != null)
        //{
        //    //If hit enemy take damage
        //}

    }
    private void FireBullet()
    {
        if (currentAmmo <= 0) return;

        currentAmmo--;

        playerController.ChangeAmmo(currentAmmo);

        foreach (var particle in muzzleFlashes)
        {
            particle.Emit(1);
        }

        Vector3 velocity = (raycastDestination.position - raycastOrigin.position).normalized * bulletSpeed;
        var bullet = CreateBullet(raycastOrigin.position, velocity);

        bullets.Add(bullet);

        recoil.GenerateRecoil(weaponName);
    }

    //public void StopFiring()
    //{
    //    isFiring = false;
    //}
}
