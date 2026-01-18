using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
public class PlayerDebuffs:MonoBehaviour
{
    public TextMeshProUGUI SpeedText;
    public TextMeshProUGUI JumpText;
    public TextMeshProUGUI GravityText;
    public TextMeshProUGUI MomentumText;
    public TextMeshProUGUI HoldJumpText;
    public TextMeshProUGUI InvertText;
    public Slider DelaySlider;
    public float StartInterval=6f;
    public float MinInterval=2f;
    public float Ramp=0.95f;
    public float Delay=0.5f;
    public float HoldTime=0.35f;
    PlayerMovement move;
    float speedT;
    float jumpT;
    float gravityT;
    float momentumT;
    float holdT;
    float invertT;
    float interval;
    float timer;
    void Awake()
    {
        move=GetComponent<PlayerMovement>();
        interval=StartInterval;
        DisableAll();
        DelaySlider.gameObject.SetActive(false);
    }
    void Update()
    {
        UpdateTimers();
        SpawnDebuff();
    }
    void SpawnDebuff()
    {
        timer+=Time.deltaTime;
        if(timer<interval)return;
        timer=0;
        interval=Mathf.Max(MinInterval,interval*Ramp);
        float d=Random.Range(3f,6f)*(StartInterval/interval);
        int r=Random.Range(0,6);
        if(r==0)StartCoroutine(SpeedDebuff(d));
        if(r==1)StartCoroutine(JumpDebuff(d));
        if(r==2)StartCoroutine(GravityDebuff(d));
        if(r==3)StartCoroutine(MomentumDebuff(d));
        if(r==4)StartCoroutine(HoldJumpDebuff(d));
        if(r==5)StartCoroutine(InvertDebuff(d));
    }
    void UpdateTimers()
    {
        Tick(ref speedT,()=>{move.SetSpeed(move.GetBaseSpeed());SpeedText.gameObject.SetActive(false);});
        Tick(ref jumpT,()=>{move.SetJump(move.GetBaseJump());JumpText.gameObject.SetActive(false);});
        Tick(ref gravityT,()=>{move.SetGravity(move.GetDefaultGravity());GravityText.gameObject.SetActive(false);});
        Tick(ref momentumT,()=>{move.SetMomentumLock(false);MomentumText.gameObject.SetActive(false);});
        Tick(ref holdT,()=>{move.SetHoldJump(false,0);HoldJumpText.gameObject.SetActive(false);});
        Tick(ref invertT,()=>{move.SetInvert(false);InvertText.gameObject.SetActive(false);});
    }
    void Tick(ref float t,System.Action end)
    {
        if(t<=0)return;
        t-=Time.deltaTime;
        if(t<=0)end();
    }
    IEnumerator DelayBar()
    {
        DelaySlider.gameObject.SetActive(true);
        DelaySlider.value=0;
        float t=0;
        while(t<Delay)
        {
            t+=Time.deltaTime;
            DelaySlider.value=t/Delay;
            yield return null;
        }
        DelaySlider.gameObject.SetActive(false);
    }
    IEnumerator SpeedDebuff(float d)
    {
        SpeedText.text="SLOWED";
        SpeedText.gameObject.SetActive(true);
        yield return DelayBar();
        move.SetSpeed(move.GetBaseSpeed()*Random.Range(0.6f,0.9f));
        speedT=d;
    }
    IEnumerator JumpDebuff(float d)
    {
        JumpText.text="WEAK JUMP";
        JumpText.gameObject.SetActive(true);
        yield return DelayBar();
        move.SetJump(move.GetBaseJump()*Random.Range(0.6f,0.9f));
        jumpT=d;
    }
    IEnumerator GravityDebuff(float d)
    {
        GravityText.text="HEAVY GRAVITY";
        GravityText.gameObject.SetActive(true);
        yield return DelayBar();
        move.SetGravity(move.GetDefaultGravity()*Random.Range(1.3f,1.8f));
        gravityT=d;
    }
    IEnumerator MomentumDebuff(float d)
    {
        MomentumText.text="MOMENTUM LOCK";
        MomentumText.gameObject.SetActive(true);
        yield return DelayBar();
        move.SetMomentumLock(true);
        momentumT=d;
    }
    IEnumerator HoldJumpDebuff(float d)
    {
        HoldJumpText.text="HOLD JUMP";
        HoldJumpText.gameObject.SetActive(true);
        yield return DelayBar();
        move.SetHoldJump(true,HoldTime);
        holdT=d;
    }
    IEnumerator InvertDebuff(float d)
    {
        InvertText.text="INVERTED CONTROLS";
        InvertText.gameObject.SetActive(true);
        yield return DelayBar();
        move.SetInvert(true);
        invertT=d;
    }
    public void ResetAllDebuffs()
    {
        StopAllCoroutines();
        speedT=jumpT=gravityT=momentumT=holdT=invertT=0;
        move.SetSpeed(move.GetBaseSpeed());
        move.SetJump(move.GetBaseJump());
        move.SetGravity(move.GetDefaultGravity());
        move.SetMomentumLock(false);
        move.SetHoldJump(false,0);
        move.SetInvert(false);
        DisableAll();
        DelaySlider.gameObject.SetActive(false);
        interval=StartInterval;
    }
    void DisableAll()
    {
        SpeedText.gameObject.SetActive(false);
        JumpText.gameObject.SetActive(false);
        GravityText.gameObject.SetActive(false);
        MomentumText.gameObject.SetActive(false);
        HoldJumpText.gameObject.SetActive(false);
        InvertText.gameObject.SetActive(false);
    }
}
