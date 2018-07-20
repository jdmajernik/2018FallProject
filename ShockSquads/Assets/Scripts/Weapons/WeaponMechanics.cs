using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMechanics : MonoBehaviour {
	
    public bool canShoot = true;
    public GameObject creator;

    public WeaponMechanics weapon_type;

    void Start()
    {
        creator = gameObject;
    }

    #region Update() (OLD)

    /*

	public virtual void Update()
    {

        if (Input.GetMouseButton(0)) //AUTOMATIC FIRE
        {
            if (automatic)
            {
                if (canShoot)
                {
                    Fire();
                }
            }
        }
        if (Input.GetMouseButtonDown(0)) //SEMI AUTO FIRE
        {
            if (!automatic)
            {
                if (canShoot)
                {
                    Fire();
                }
            }
        }

    }

    */

    #endregion

    #region Rate of fire

    IEnumerator RofCoroutine(float value)
    {
        canShoot = false;
        yield return new WaitForSeconds(1 / value);
        canShoot = true;
    }

    public void Rof(float value)
    {
        StartCoroutine(RofCoroutine(value));
    }

    #endregion

    public virtual Vector3 AddNoiseOnAngle(float bloom)
    {
        float x_noise = Random.Range(-bloom, bloom);
        float y_noise = Random.Range(-bloom, bloom);

        Vector3 noise = new Vector3(
            Mathf.Sin(2f * Mathf.PI * x_noise / 360),
            Mathf.Sin(2f * Mathf.PI * y_noise / 360),
            0f);

        return noise;
    }

    public virtual void Fire(Vector3 aimPosition)
    {
        //base.Rof();
        print("Basic firing function");
    }
}
