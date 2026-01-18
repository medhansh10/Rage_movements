using UnityEngine;
[RequireComponent(typeof(Rigidbody2D),typeof(CapsuleCollider2D))]
public class PlayerMovement:MonoBehaviour
{
    public float BaseMoveSpeed=8f;
    public float BaseJumpForce=16f;
    public float FallMultiplier=2.5f;
    public float LowJumpMultiplier=2f;
    public float KillY=-10f;
    public LayerMask GroundLayer;
    public float GroundCheckOffset=0.05f;
    public Transform RespawnPoint;
    public LevelGenerator Generator;
    Rigidbody2D rb;
    CapsuleCollider2D col;
    bool grounded;
    float inputX;
    float moveSpeed;
    float jumpForce;
    float defaultGravity;
    bool invertInput;
    bool holdJump;
    float holdJumpTime;
    float holdTimer;
    bool momentumLock;
    float lockedInputX;
    void Awake()
    {
        rb=GetComponent<Rigidbody2D>();
        col=GetComponent<CapsuleCollider2D>();
        rb.freezeRotation=true;
        moveSpeed=BaseMoveSpeed;
        jumpForce=BaseJumpForce;
        defaultGravity=rb.gravityScale;
    }
    void Update()
    {
        float raw=Input.GetAxisRaw("Horizontal");
        if(invertInput)raw=-raw;
        if(momentumLock&&!grounded)
        {
            inputX=lockedInputX;
        }
        else
        {
            inputX=raw;
            if(grounded)lockedInputX=raw;
        }
        HandleJump();
        BetterJump();
        CheckRespawn();
    }
    void FixedUpdate()
    {
        CheckGrounded();
        rb.linearVelocity=new Vector2(inputX*moveSpeed,rb.linearVelocity.y);
    }
    void HandleJump()
    {
        if(!grounded)
        {
            holdTimer=0;
            return;
        }
        if(holdJump)
        {
            if(Input.GetButton("Jump"))
            {
                holdTimer+=Time.deltaTime;
                if(holdTimer>=holdJumpTime)
                {
                    rb.linearVelocity=new Vector2(rb.linearVelocity.x,jumpForce);
                    holdTimer=0;
                }
            }
            else holdTimer=0;
        }
        else
        {
            if(Input.GetButtonDown("Jump"))
            {
                rb.linearVelocity=new Vector2(rb.linearVelocity.x,jumpForce);
            }
        }
    }
    void BetterJump()
    {
        if(rb.linearVelocity.y<0)
        {
            rb.linearVelocity+=Vector2.up*Physics2D.gravity.y*(FallMultiplier-1)*Time.deltaTime;
        }
        else if(rb.linearVelocity.y>0&&!Input.GetButton("Jump"))
        {
            rb.linearVelocity+=Vector2.up*Physics2D.gravity.y*(LowJumpMultiplier-1)*Time.deltaTime;
        }
    }
    void CheckGrounded()
    {
        Vector2 p=(Vector2)col.bounds.center+Vector2.down*(col.bounds.extents.y+GroundCheckOffset);
        grounded=Physics2D.OverlapCapsule(p,col.size,col.direction,0,GroundLayer);
    }
    void CheckRespawn()
    {
        if(transform.position.y<KillY)
        {
            rb.linearVelocity=Vector2.zero;
            transform.position=RespawnPoint.position;
            Generator.ResetLevel();
            FindObjectOfType<CameraFollowX>().SnapToTarget();
            GetComponent<PlayerDebuffs>().ResetAllDebuffs();
        }
    }
    public void SetSpeed(float v){moveSpeed=v;}
    public void SetJump(float v){jumpForce=v;}
    public void SetGravity(float v){rb.gravityScale=v;}
    public void SetInvert(bool v){invertInput=v;}
    public void SetHoldJump(bool v,float t){holdJump=v;holdJumpTime=t;}
    public void SetMomentumLock(bool v){momentumLock=v;}
    public float GetBaseSpeed(){return BaseMoveSpeed;}
    public float GetBaseJump(){return BaseJumpForce;}
    public float GetDefaultGravity(){return defaultGravity;}
}
