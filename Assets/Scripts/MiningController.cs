using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MiningController : MonoBehaviour
{
    [Header("Tool Settings")]
    [SerializeField] Transform firePoint;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float miningDistance = 10f;
    [SerializeField] float miningToolCooldown = 1f;

    [Header("Tilemaps")]
    [SerializeField] Tilemap CaveTilemap;
    [SerializeField] Tilemap DecorTilemap;
    [SerializeField] TileBase CaveTile;

    bool mineOnCooldown = false;
    AudioManager audioManager;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();    
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot()
    {
        if (!mineOnCooldown)
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.right, miningDistance);

            if (hitInfo)
            {
                lineRenderer.SetPosition(0, firePoint.position);
                lineRenderer.SetPosition(1, hitInfo.point);

                int posX = Mathf.RoundToInt(Mathf.Floor(hitInfo.point.x));
                int posY = Mathf.RoundToInt(Mathf.Floor(hitInfo.point.y));

                TileBase target = CaveTilemap.GetTile(new Vector3Int(posX, posY, 0));

                // Mathf.Floor can't get position of tiles to the left and below hitpoint, so calculate which one is more appropriate
                if (target == null)
                {
                    float xDecimal = hitInfo.point.x - posX;
                    float yDecimal = hitInfo.point.y - posY;
                    if (xDecimal >= yDecimal)
                    {
                        posY -= 1;
                        target = CaveTilemap.GetTile(new Vector3Int(posX, posY, 0));
                    }
                    else
                    {
                        posX -= 1;
                        target = CaveTilemap.GetTile(new Vector3Int(posX, posY, 0));
                    }
                }
                
                // Checks if target is mineable
                if (target == CaveTile)
                {
                    CaveTilemap.SetTile(new Vector3Int(posX, posY, 0), null);
                    DecorTilemap.SetTile(new Vector3Int(posX, posY - 1, 0), null);
                    DecorTilemap.SetTile(new Vector3Int(posX, posY + 1, 0), null);
                }
            }
            else
            {
                lineRenderer.SetPosition(0, firePoint.position);
                lineRenderer.SetPosition(1, firePoint.position + firePoint.right * miningDistance);
            }

            audioManager.Play("MiningToolLaser");

            lineRenderer.enabled = true;
            yield return new WaitForSeconds(0.02f);
            lineRenderer.enabled = false;

            StartCoroutine(StartMiningToolCooldown());
        }
    }

    IEnumerator StartMiningToolCooldown()
    {
        mineOnCooldown = true;

        yield return new WaitForSeconds(miningToolCooldown);

        mineOnCooldown = false;
    }
}
