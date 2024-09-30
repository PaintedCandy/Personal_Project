using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    Rigidbody myRB;
    Camera playerCam;
    Transform cameraHolder;

    Vector2 camRotation;



    [Header("Player Stats")]
    public bool takenDamage = false;
    public float damageCooldownTimer = .5f;
    public int health = 5;
    public int maxHealth = 10;
    public int HealthPickupAmt = 5;
    

    [Header("Weapon Stats")]
    public Transform WeaponSlot;
    public GameObject shot;
    public float shotVel = 0;
    public int weaponID = -1;
    public int fireMode = 0;
    public float fireRate = 0;
    public float currentClip = 0;
    public float clipSize = 0;
    public float maxAmmo = 0;
    public float currentAmmo = 0;
    public float reloadAmt = 0;
    public float bulletLifespan = 0;
    public bool canFire = true;

    [Header("Movement Stats")]
    public bool sprinting = false;
    public float speed = 10f;
    public float sprintMult = 1.5f;
    public float jumpHeight = 5f;
    public float groundDetection = 1.5f;


    [Header("User Setting")]
    public bool sprintToggle = false;
    public float mouseSensitivity = 2.0f;
    public float xSensitivity = 2.0f;
    public float ySensitivity = 2.0f;
    public float camRotationLimit = 90f;

    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody>();
        playerCam = transform.GetChild(0).GetComponent<Camera>();
        playerCam = Camera.main;
        cameraHolder = transform.GetChild(0);

        camRotation = Vector2.zero;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        camRotation.x += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        camRotation.y += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        camRotation.y = Mathf.Clamp(camRotation.y, -90, 90);

        playerCam.transform.position = cameraHolder.position;


        playerCam.transform.rotation = Quaternion.Euler(-camRotation.y, camRotation.x, 0);
        transform.localRotation = Quaternion.AngleAxis(camRotation.x, Vector3.up);

        if (Input.GetMouseButtonDown(0) && canFire && currentClip > 0 && weaponID >= 0)
        {
            GameObject s = Instantiate(shot, WeaponSlot.position, WeaponSlot.rotation);
            s.GetComponent<Rigidbody>().AddForce(playerCam.transform.forward * shotVel);
            Destroy(s, bulletLifespan);

            canFire = false;
            currentClip--;
            StartCoroutine("cooldownFire");
        }

        if (Input.GetKeyDown(KeyCode.R))
            reloadClip();


        if (!sprinting)
        {
            if (!sprinting && !sprintToggle && Input.GetKey(KeyCode.LeftShift))
                sprinting = true;

            if (!sprinting && sprintToggle && (Input.GetAxisRaw("Vertical") > 0) && Input.GetKey(KeyCode.LeftShift))
                sprinting = true;
        }


        Vector3 temp = myRB.velocity;

        temp.z = Input.GetAxisRaw("Horizontal") * speed;
        temp.x = Input.GetAxisRaw("Vertical") * speed;

        if (sprinting)
            temp.z *= sprintMult;

        if (sprinting && sprintToggle && (Input.GetAxisRaw("Vertical") <= 0))
            sprinting = false;

        if (sprinting && !sprintToggle && Input.GetKeyUp(KeyCode.LeftShift))
            sprinting = false;

        if (Input.GetKeyDown(KeyCode.Space) && Physics.Raycast(transform.position, -transform.up, groundDetection))
            temp.y = jumpHeight;

        myRB.velocity = (transform.forward * temp.x) + (transform.right * temp.z) + (transform.up * temp.y);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.tag == "HealthPickup") && health < maxHealth)
        {
            if (health + HealthPickupAmt > maxHealth)
                health = maxHealth;

            else
                health += HealthPickupAmt;

            Destroy(collision.gameObject);
        }

        if ((collision.gameObject.tag == "ammoPickup") && currentAmmo < maxAmmo)
        {
            if (currentAmmo + reloadAmt > maxAmmo)
                currentAmmo = maxAmmo;

            else
                currentAmmo += reloadAmt;

            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Weapon")
        {
            other.transform.SetPositionAndRotation(WeaponSlot.position, WeaponSlot.rotation);

            other.transform.SetParent(WeaponSlot);

            switch (other.gameObject.name)
            {
                case "Weapon1":
                    weaponID = 0;
                    shotVel = 10000;
                    fireMode = 0;
                    fireRate = 0.1f;
                    currentClip = 20;
                    clipSize = 20;
                    maxAmmo = 400;
                    currentAmmo = 200;
                    reloadAmt = 20;
                    bulletLifespan = .5f;
                    break;

                default:
                    break;
            }
        }
    }
        public void reloadClip()
        {
            if (currentClip >= clipSize)
                return;

            else
            {
                float reloadCount = clipSize - currentClip;

                if (currentAmmo < reloadCount)
                {
                    currentClip += currentAmmo;
                    currentAmmo = 0;
                    return;
                }

                else
                {
                    currentClip += reloadCount;
                    currentAmmo -= reloadCount;
                    return;
                }
            }
        }

    IEnumerator cooldownFire()
    {
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }

    public IEnumerator cooldownDamage()
    {
        yield return new WaitForSeconds(damageCooldownTimer);
        takenDamage = false;
    }
}
