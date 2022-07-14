using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmRotation : MonoBehaviour
{
    [SerializeField] int rotationOffset = 0;
    [SerializeField] CharacterController2D controller;

    GameObject arm;
    bool facingRight = true;

    void Start()
    {
        arm = transform.Find("Arm").gameObject;
    }
    void Update()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - arm.transform.position;
        difference.Normalize();
        float rotation = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        if (Mathf.Abs(rotation) > 90 && facingRight)
        {
            controller.Flip();
            rotationOffset *= -1;
            facingRight = false;
        }
        if (Mathf.Abs(rotation) < 90 && !facingRight)
        {
            controller.Flip();
            rotationOffset *= -1;
            facingRight = true;
        }
        arm.transform.rotation = Quaternion.Euler(0f, 0f, rotation + rotationOffset);
    }
}
