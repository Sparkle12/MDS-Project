using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //I recommend 7 for the move speed, and 1.2 for the force damping
    public Rigidbody2D rb;
    public float moveSpeed, dashSpeed;
    public float dashCooldown,dashTimer = 0, dashDuration,dashT = 0, attackCooldown, abilityCooldown,attackTimer = 0,  abilityTimer = 0;
    Vector2 forceToApply;
    Vector2 PlayerInput, PlayerInputDash;
    bool IsKnocked = false, wantDash = false;
    bool doAttack = false , doAbility = false;
    bool[] useItems = new bool[4];
    public float forceDamping;

    private void Start()
    {
        for(int i = 0;i<4;i++)
            useItems[i] = false;
        attackCooldown = 0.5f;
        abilityCooldown = 3f;
    }


    void Update()
    {
        if (!IsKnocked)
            PlayerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        else
            PlayerInput = Vector2.zero;

        if ( Input.GetKeyDown(KeyCode.Space) && dashTimer <= 0 && !IsKnocked)
        {
            PlayerInputDash = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            dashTimer = dashCooldown;
            dashT = dashDuration;
            wantDash = true;
        }
        //Left click
        if (!IsKnocked && Input.GetMouseButtonDown(0) && attackTimer <= 0)
        {
            doAttack = true;
            attackTimer = attackCooldown;
        }
        //RightClick
        if(!IsKnocked && Input.GetMouseButtonDown(1) && abilityTimer <= 0)
        {
            doAbility = true;
            abilityTimer = abilityCooldown;
        }

        for(int i = 0;i<4;i++)
            if(!IsKnocked && Input.GetKeyDown("" + (i+1)))
            {
                useItems[i] = true;
            }



        dashTimer -= Time.deltaTime;
        dashT -= Time.deltaTime;
        attackTimer -= Time.deltaTime;
        abilityTimer -= Time.deltaTime;

        if (attackTimer < 0)
            attackTimer = 0;
        if (abilityTimer < 0)
            abilityTimer = 0;
        if (dashTimer < 0)
            dashTimer = 0;
        if (dashT < 0)
            dashT = 0;
    }
    void FixedUpdate()
    {
        Vector2 moveForce = new Vector2();

        for (int i = 0; i < 4;i++)
        {
            if (useItems[i])
            {
                useItems[i] = false;
                Debug.Log("Use Item on Slot " + (i+1));
            }
        }


        if(doAttack)
        {
            doAttack = false;
            Debug.Log("Attack");
        }

        if (doAbility) 
        {
            doAbility = false;
            Debug.Log("Ability");
        }

        if (wantDash)
        {
            
            if (dashT > 0 && !IsKnocked)
            {
                moveForce = PlayerInputDash * dashSpeed;
            }
            else
            {
                wantDash = false;
            }
        }
        else
        {
            moveForce = PlayerInput * moveSpeed;
        }

        moveForce += forceToApply;
        forceToApply /= forceDamping;
        if (Mathf.Abs(forceToApply.x) <= 0.1f && Mathf.Abs(forceToApply.y) <= 0.1f)
        {
            IsKnocked = false;     
        }
        if (Mathf.Abs(forceToApply.x) <= 0.01f && Mathf.Abs(forceToApply.y) <= 0.01f)
        {
            forceToApply = Vector2.zero;
        }
        rb.velocity = moveForce;

        faceMouse();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var knockback = collision.gameObject.GetComponent<Knockback>();

        Debug.Log("Collision detected");
        
        if (knockback != null)
        {
            IsKnocked = true;
            Vector2 dir = (collision.otherCollider.transform.position - collision.transform.position).normalized;
            forceToApply += dir * knockback.force ;
            //Destroy(collision.gameObject);
        }
    }

    void faceMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 direction = new Vector2(
            mousePosition.x - transform.position.x,
            mousePosition.y - transform.position.y
        );

        transform.up = direction;
    }
}
