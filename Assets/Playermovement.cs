using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D),typeof(CapsuleCollider2D))]
public class PlayerController:MonoBehaviour
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

    public TextMeshProUGUI SpeedDebuffText;
    public TextMeshProUGUI JumpDebuffText;
    public TextMeshProUGUI GravityDebuffText;
    public TextMeshProUGUI SlipperyDebuffText;
    public TextMeshProUGUI HoldJumpDebuffText;
    public TextMeshProUGUI InvertInputDebuffText;

    public float DebuffStartInterval=6f;
    public float DebuffMinInterval=2f;
    public float DifficultyRamp=0.95f;
    public float DebuffDelay=0.5f;

    public float HoldJumpTime=0.35f;

    Rigidbody2D rb;
    CapsuleCollider2D col;
    bool grounded;
    float inputX;

    float moveSpeed;
    float jumpForce;
    float defaultGravity;

    float speedTimer;
    float jumpTimer;
    float gravityTimer;
    float slipperyTimer;
    float holdJumpTimer;
    float invertTimer;

    bool slippery;
    bool holdJumpRequired;
    bool invertInput;

    float jumpHoldCounter;

    float debuffInterval;
    float debuffTimer;

    void Awake()
    {
        rb=GetComponent<Rigidbody2D>();
        col=GetComponent<CapsuleCollider2D>();
        rb.freezeRotation=true;
        moveSpeed=BaseMoveSpeed;
        jumpForce=BaseJumpForce;
        defaultGravity=rb.gravityScale;
        debuffInterval=DebuffStartInterval;
        DisableAllTexts();
    }

    void Update()
    {
        HandleDebuffTimers();
        HandleRandomDebuffs();
        float raw=Input.GetAxisRaw("Horizontal");
        if(invertInput)raw=-raw;
        inputX=raw;
        HandleJumpInput();
        HandleBetterJump();
        CheckRespawn();
        
    }

    void FixedUpdate()
    {
        CheckGrounded();
        if(slippery)
        {
            rb.linearVelocity=Vector2.Lerp(rb.linearVelocity,new Vector2(inputX*moveSpeed,rb.linearVelocity.y),0.05f);
        }
        else
        {
            rb.linearVelocity=new Vector2(inputX*moveSpeed,rb.linearVelocity.y);
        }
    }

    void HandleJumpInput()
    {
        if(!grounded)
        {
            jumpHoldCounter=0;
            return;
        }
        if(holdJumpRequired)
        {
            if(Input.GetButton("Jump"))
            {
                jumpHoldCounter+=Time.deltaTime;
                if(jumpHoldCounter>=HoldJumpTime)
                {
                    rb.linearVelocity=new Vector2(rb.linearVelocity.x,jumpForce);
                    jumpHoldCounter=0;
                }
            }
            else
            {
                jumpHoldCounter=0;
            }
        }
        else
        {
            if(Input.GetButtonDown("Jump"))
            {
                rb.linearVelocity=new Vector2(rb.linearVelocity.x,jumpForce);
            }
        }
    }

    void HandleBetterJump()
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

    void HandleRandomDebuffs()
    {
        debuffTimer+=Time.deltaTime;
        if(debuffTimer<debuffInterval)return;
        debuffTimer=0;
        debuffInterval=Mathf.Max(DebuffMinInterval,debuffInterval*DifficultyRamp);
        int roll=Random.Range(0,6);
        float dur=Random.Range(3f,6f)*(DebuffStartInterval/debuffInterval);
        if(roll==0)StartCoroutine(DelayedSpeedDebuff(Random.Range(0.6f,0.9f),dur));
        if(roll==1)StartCoroutine(DelayedJumpDebuff(Random.Range(0.6f,0.9f),dur));
        if(roll==2)StartCoroutine(DelayedGravityDebuff(Random.Range(1.3f,1.8f),dur));
        if(roll==3)StartCoroutine(DelayedSlipperyDebuff(dur));
        if(roll==4)StartCoroutine(DelayedHoldJumpDebuff(dur));
        if(roll==5)StartCoroutine(DelayedInvertInputDebuff(dur));
    }

    void HandleDebuffTimers()
    {
        UpdateTimer(ref speedTimer,()=>{moveSpeed=BaseMoveSpeed;SpeedDebuffText.gameObject.SetActive(false);});
        UpdateTimer(ref jumpTimer,()=>{jumpForce=BaseJumpForce;JumpDebuffText.gameObject.SetActive(false);});
        UpdateTimer(ref gravityTimer,()=>{rb.gravityScale=defaultGravity;GravityDebuffText.gameObject.SetActive(false);});
        UpdateTimer(ref slipperyTimer,()=>{slippery=false;SlipperyDebuffText.gameObject.SetActive(false);});
        UpdateTimer(ref holdJumpTimer,()=>{holdJumpRequired=false;HoldJumpDebuffText.gameObject.SetActive(false);});
        UpdateTimer(ref invertTimer,()=>{invertInput=false;InvertInputDebuffText.gameObject.SetActive(false);});
    }

    void UpdateTimer(ref float t,System.Action end)
    {
        if(t<=0)return;
        t-=Time.deltaTime;
        if(t<=0)end();
    }

    
    void CheckRespawn()
    {
        if(transform.position.y<KillY)
        {
            rb.linearVelocity=Vector2.zero;
            transform.position=RespawnPoint.position;
            Generator.ResetLevel();
            FindObjectOfType<CameraFollowX>().SnapToTarget();
            ResetAllDebuffs();
            debuffInterval=DebuffStartInterval;
        }
    }

    void ResetAllDebuffs()
    {
        StopAllCoroutines();
        moveSpeed=BaseMoveSpeed;
        jumpForce=BaseJumpForce;
        rb.gravityScale=defaultGravity;
        slippery=false;
        holdJumpRequired=false;
        invertInput=false;
        speedTimer=jumpTimer=gravityTimer=slipperyTimer=holdJumpTimer=invertTimer=0;
        DisableAllTexts();
    }

    void DisableAllTexts()
    {
        SpeedDebuffText.gameObject.SetActive(false);
        JumpDebuffText.gameObject.SetActive(false);
        GravityDebuffText.gameObject.SetActive(false);
        SlipperyDebuffText.gameObject.SetActive(false);
        HoldJumpDebuffText.gameObject.SetActive(false);
        InvertInputDebuffText.gameObject.SetActive(false);
    }

    IEnumerator DelayedSpeedDebuff(float m,float d)
    {
        SpeedDebuffText.text="SLOWED";
        SpeedDebuffText.gameObject.SetActive(true);
        yield return new WaitForSeconds(DebuffDelay);
        moveSpeed=BaseMoveSpeed*m;
        speedTimer=d;
    }

    IEnumerator DelayedJumpDebuff(float m,float d)
    {
        JumpDebuffText.text="WEAK JUMP";
        JumpDebuffText.gameObject.SetActive(true);
        yield return new WaitForSeconds(DebuffDelay);
        jumpForce=BaseJumpForce*m;
        jumpTimer=d;
    }

    IEnumerator DelayedGravityDebuff(float m,float d)
    {
        GravityDebuffText.text="HEAVY GRAVITY";
        GravityDebuffText.gameObject.SetActive(true);
        yield return new WaitForSeconds(DebuffDelay);
        rb.gravityScale=defaultGravity*m;
        gravityTimer=d;
    }

    IEnumerator DelayedSlipperyDebuff(float d)
    {
        SlipperyDebuffText.text="SLIPPERY";
        SlipperyDebuffText.gameObject.SetActive(true);
        yield return new WaitForSeconds(DebuffDelay);
        slippery=true;
        slipperyTimer=d;
    }

    IEnumerator DelayedHoldJumpDebuff(float d)
    {
        HoldJumpDebuffText.text="HOLD JUMP";
        HoldJumpDebuffText.gameObject.SetActive(true);
        yield return new WaitForSeconds(DebuffDelay);
        holdJumpRequired=true;
        holdJumpTimer=d;
    }

    IEnumerator DelayedInvertInputDebuff(float d)
    {
        InvertInputDebuffText.text="INVERTED CONTROLS";
        InvertInputDebuffText.gameObject.SetActive(true);
        yield return new WaitForSeconds(DebuffDelay);
        invertInput=true;
        invertTimer=d;
    }
}
