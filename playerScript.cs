using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;
using UnityEngine.EventSystems;


public class playerScript : MonoBehaviour
{
    //player object
    //-- forward movement
    [SerializeField] private Rigidbody selfrb;
    [SerializeField] private float acceleration;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float currentVelocity;
    //-- jumping
    [SerializeField] private float sidewaysJumpForce;
    [SerializeField] private float jumpForce;
    [SerializeField] private float inairjumpforce;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool doubleJump;
    [SerializeField] private bool jumpTriggered = false;
    //-- rotating
    [SerializeField] private Quaternion targetRotation;
    [SerializeField] private bool rotating = false;
    [SerializeField] private float rotatingSpeed;
    [SerializeField] private float zVar;
    //attack
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private float atkSpeed;
    [SerializeField] private Transform atkPivot;
    [SerializeField] private Quaternion atkrotation;
    //user input
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private InputAction jumpAction;
    [SerializeField] private InputAction moveAction;
    [SerializeField] private InputAction tapAction;
    [SerializeField] private InputAction attackAction;
    [SerializeField] private float tapXInput;
    [SerializeField] private bool tapQueued = false;
    [SerializeField] private Vector2 queuedTapPos;
    //other
    [SerializeField] private int laneNo = 2;
    [SerializeField] private bool ded = false;
    public ParticleSystem explosionParticles;
    //event dispatchers
    public static event Action<float> died;
    
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ded = false;
        atkPivot.localPosition = new Vector3(0.676f,0,0);
        atkPivot.rotation = new Quaternion(0,180,0,0);
    }

    void OnEnable()
    {
        tapAction.performed += ctx =>
        {
            queuedTapPos = Touchscreen.current.primaryTouch.position.ReadValue();
            tapQueued = true;
        };

        tapAction.Enable();
        moveAction.Enable();
        jumpAction.Enable();
        attackAction.Enable();
        jumpAction.performed += jump;
        attackAction.performed += attack;
    }

    void OnDisable()
    {
        moveAction.Disable();
        jumpAction.performed -= jump;
        jumpAction.Disable();
        attackAction.performed -= attack;
        attackAction.Disable();
        tapAction.Disable();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isJumping)
        {
            selfrb.AddForce(Vector3.right * acceleration, ForceMode.Acceleration); //moving along the x-axis
        }

        moveInput = moveAction.ReadValue<Vector2>();
        if (tapXInput != 0)
        {
            moveInput.x = tapXInput;
        }

        if (moveInput.x != 0 && !jumpTriggered)
        {
            jumpTriggered = true;
            moveInput.x = Mathf.Round(moveInput.x);
            laneNo -= (int)moveInput.x;

            selfrb.constraints &= ~RigidbodyConstraints.FreezePositionZ;

            if (!isJumping)
            {
                selfrb.AddForce(new Vector3(0, sidewaysJumpForce, moveInput.x * sidewaysJumpForce));
                isJumping = true;
            }
            else
            {
                if (doubleJump)
                {
                    selfrb.AddForce(new Vector3(0f, 0f, moveInput.x * sidewaysJumpForce));
                    isJumping = true;
                    doubleJump = false;
                }
                else
                {
                    selfrb.AddForce(new Vector3(0f, 0f, moveInput.x * inairjumpforce));
                    isJumping = true;
                    doubleJump = false;
                }
            }
            
            targetRotation = Quaternion.AngleAxis(90f * moveInput.x, Vector3.right) * transform.rotation;
            StartCoroutine(delayRotation());
            
            if (laneNo < 1)
            {
                laneNo = 1;
            }
            else if (laneNo > 3)
            {
                laneNo = 3;
            } 
        }

        if (transform.position.z % 3 != 0 && !isJumping)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Round(transform.position.z / 3) * 3);
        }

        if (rotating)
        {
            selfrb.constraints &= ~RigidbodyConstraints.FreezeRotationX;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotatingSpeed * Time.deltaTime
            );

            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                Quaternion currentEuler = transform.localRotation;
                float normalisedZ = Mathf.Round(currentEuler.z / 90f) * 90f;
                targetRotation = Quaternion.Euler(currentEuler.x, currentEuler.y, normalisedZ);
                transform.rotation = targetRotation;
                selfrb.constraints |= RigidbodyConstraints.FreezePositionZ;
                selfrb.constraints |= RigidbodyConstraints.FreezeRotationX;
                rotating = false;
            }
        }

        if (isAttacking)
        {
            atkPivot.rotation = Quaternion.Slerp(
                atkPivot.rotation,
                atkrotation,
                atkSpeed * Time.deltaTime
            );
        }
        else
        {
            atkPivot.localPosition = new Vector3(1f,0,0);
            atkPivot.rotation = new Quaternion(0,180,0,0);
        }

        currentVelocity = selfrb.linearVelocity.magnitude;
        if (selfrb.linearVelocity.magnitude > maxVelocity && !isJumping)
        {
            selfrb.linearVelocity = selfrb.linearVelocity.normalized * maxVelocity;
        }
    }

    IEnumerator delayRotation()
    {
        yield return new WaitForSeconds(0.1f);
        rotating = true;
    }

    void jump(InputAction.CallbackContext callback)
    {     
        if (!isJumping)
        {
            isJumping = true;
            selfrb.AddForce(new Vector3(1, jumpForce, 0));
            //targetRotation = Quaternion.Euler(currentEuler.x,0f,currentEuler.z - 90f);
            targetRotation = Quaternion.AngleAxis(-90f, Vector3.forward) * transform.rotation;
            rotating = true;
        }
        else if (doubleJump)
        {
            doubleJump = false;
            selfrb.AddForce(new Vector3(2, jumpForce/1.5f, 0));
            //targetRotation = Quaternion.Euler(currentEuler.x,0f,normalisedZ - 90f);
            targetRotation = Quaternion.AngleAxis(-90f, Vector3.forward) * transform.rotation;
            rotating = true;
        }
    }

    public void mobileJump()
    {     
        Debug.Log("jump triggered");
        isJumping = false;
        if (!isJumping)
        {
            Debug.Log("just checking");
            isJumping = true;
            selfrb.AddForce(new Vector3(1, jumpForce, 0));
            //targetRotation = Quaternion.Euler(currentEuler.x,0f,currentEuler.z - 90f);
            targetRotation = Quaternion.AngleAxis(-90f, Vector3.forward) * transform.rotation;
            rotating = true;
            Debug.Log("jump logic fired!");
        }
        else if (doubleJump)
        {
            doubleJump = false;
            selfrb.AddForce(new Vector3(2, jumpForce/1.5f, 0));
            //targetRotation = Quaternion.Euler(currentEuler.x,0f,normalisedZ - 90f);
            targetRotation = Quaternion.AngleAxis(-90f, Vector3.forward) * transform.rotation;
            rotating = true;
            Debug.Log("double jump logic fired!");
        }
    }

    void attack(InputAction.CallbackContext callback)
    {
        
        if(!isAttacking)
        {
            isAttacking = true;
            Debug.Log("RANDOM BULLSHIT GO!");
            atkPivot.gameObject.SetActive(true);
            atkrotation = Quaternion.AngleAxis(0f, new Vector3(0,1,0));
            StartCoroutine(delayAttack());
        }
    }

    public void mobileAttack()
    {
        
        if(!isAttacking)
        {
            isAttacking = true;
            Debug.Log("RANDOM BULLSHIT GO!");
            atkPivot.gameObject.SetActive(true);
            atkrotation = Quaternion.AngleAxis(0f, new Vector3(0,1,0));
            StartCoroutine(delayAttack());
        }
    }

    IEnumerator delayAttack()
    {
        yield return new WaitForSeconds(0.2f);
        atkrotation = Quaternion.AngleAxis(-180f, new Vector3(0,1,0));
        atkPivot.rotation = atkrotation;
        atkPivot.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        isAttacking = false;
    }

    void Update()
    {
        if (transform.position.z > 5 || transform.position.z < -5)
        {
            die();
        }
        else if (transform.position.y < 0)
        {
            die();
        }

        if (!tapQueued) return;

        tapQueued = false;
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (queuedTapPos.x < Screen.width * 0.5f)
        {
            tapXInput = 1f;
        }
        else
        {
            tapXInput = -1f;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        foreach (var thing in col.contacts)
        {
            if (thing.thisCollider.name == "player(Clone)")
            {
                if (col.gameObject.tag == "floor")
                {
                    jumpReset();
                }
                else if (col.gameObject.tag == "ceiling")
                {
                    foreach (ContactPoint c in col.contacts)
                    {
                        if (c.normal.y > 0)
                        {
                            //Debug.Log("floor");
                            jumpReset();
                        }
                        else
                        {
                            die();
                        }
                    }
                }
                else if (col.gameObject.tag == "wall")
                {
                    die();
                }
                else if (col.gameObject.tag == "enemy")
                {
                    die();
                }
            }
        }
        
    }

    void die()
    {
        ParticleSystem newParticles = Instantiate(explosionParticles, transform.position, Quaternion.identity);
        if (!ded)
        {
            died?.Invoke(transform.position.x);
            ded = true;
        }
        Destroy(gameObject);
    }

    void jumpReset()
    {
        isJumping = false;
        doubleJump = true;
        jumpTriggered = false;
        tapXInput = 0;

        transform.position = new Vector3 (transform.position.x, transform.position.y, Mathf.Round(transform.position.z));

        if (transform.rotation != targetRotation)
        {
            selfrb.constraints &= ~RigidbodyConstraints.FreezePositionZ;
            selfrb.constraints &= ~RigidbodyConstraints.FreezeRotationX;

            transform.rotation = targetRotation;

            selfrb.constraints |= RigidbodyConstraints.FreezePositionZ;
            selfrb.constraints |= RigidbodyConstraints.FreezeRotationX;
        }
    }
}
