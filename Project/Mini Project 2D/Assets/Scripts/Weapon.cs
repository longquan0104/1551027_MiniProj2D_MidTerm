using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public static Weapon instance;

    public float fireRate = 0;
    public float damage = 10;
    public LayerMask whatTohit;

    public Sprite machineGun;

    public Transform BulletTrailPrefab;
    public Transform MuzzleFlashPrefab;
    public Transform HitPrefab;

    float timeToSpawnEffect = 0;
    float timeToFire = 0;
    public float effectSpawnRate = 10;
    public float camShakeAmt = 0.1f;
    public float camShakeLength = 0.1f;
    CameraShake camShake;

    public string weaponShootSound = "DefaultShot";

    Transform firePoint;

    AudioManager audioManager;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        firePoint = transform.Find("FirePoint");
        if (this.fireRate > 10)
        {
            this.GetComponent<SpriteRenderer>().sprite = machineGun;
        }
        if (firePoint == null)
        {
            Debug.LogError("no firepoint. Wot the fuck");
        }
    }
    private void Start()
    {
        camShake = GameMaster.gm.GetComponent<CameraShake>();
        if(camShake == null)
        {
            Debug.LogError("No CamShake");
        }
        audioManager = AudioManager.instance;
        if(audioManager == null)
        {
            Debug.LogError("Weapon: Audiomanager not found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.fireRate > 10)
        {
            this.GetComponent<SpriteRenderer>().sprite = machineGun;
        }
        if (fireRate == 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButton("Fire1") && Time.time > timeToFire)
            {
                timeToFire = Time.time + 1 / fireRate;
                Shoot();
            }
        }
    }

    void Shoot()
    {
        Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        Vector2 firePointPosition = new Vector2(firePoint.position.x,firePoint.position.y);
        RaycastHit2D hit = Physics2D.Raycast(firePointPosition,mousePosition-firePointPosition,100, whatTohit);


        //Debug.DrawLine(firePointPosition, (mousePosition-firePointPosition)*100,Color.cyan);
        if (hit.collider != null)
        {
            //Debug.DrawLine(firePointPosition, hit.point, Color.red);
            Debug.Log("We hit "+hit.collider.name+ " and did " + damage + "damages");
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.DamageEnemy(damage);
                Debug.Log("We hit " + hit.collider.name + " and did " + damage + "damages");
            }
        }
        if (Time.time >= timeToSpawnEffect)
        {
            Vector3 hitPos;
            Vector3 hitNormal;
            if (hit.collider == null)
            {
                hitPos = (mousePosition - firePointPosition)* 30;
                hitNormal = new Vector3(9999, 9999, 9999);
            }
            else
            {
                hitPos = hit.point;
                hitNormal = hit.normal;
            }
            Effect(hitPos,hitNormal);
            timeToSpawnEffect = Time.time + 1 / effectSpawnRate;
        }
    }

    void Effect(Vector3 hitPos,Vector3 hitNormal)
    {
        Transform trail =  Instantiate(BulletTrailPrefab,firePoint.position,firePoint.rotation) as Transform;
        LineRenderer lr = trail.GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.SetPosition(0, firePoint.position);
            lr.SetPosition(1, hitPos);
        }

        Destroy(trail.gameObject, 0.04f);

        if(hitNormal != new Vector3(9999, 9999, 9999))
        {
            Transform hitParticles= Instantiate(HitPrefab,hitPos,Quaternion.FromToRotation(Vector3.right,hitNormal)) as Transform;
            Destroy(hitParticles.gameObject, 1f);
            camShake.Shake(camShakeAmt, camShakeLength);
        }

        Transform clone =  Instantiate(MuzzleFlashPrefab, firePoint.position, firePoint.rotation) as Transform;
        clone.parent = firePoint;
        float size = Random.Range(0.6f, 0.9f);
        clone.localScale = new Vector3(size, size, size);
        Destroy(clone.gameObject,0.02f);

        audioManager.PlaySound(weaponShootSound);

    }
}
