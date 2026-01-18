using UnityEngine;
using TMPro;

public class DistanceScore:MonoBehaviour
{
    public Transform Player;
    public TextMeshProUGUI ScoreText;
    public GameObject ExplanationPanel;

    Vector3 startPos;
    bool paused;

    void Start()
    {
        startPos=Player.position;
        ExplanationPanel.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleExplanation();
        }
        if(paused)return;
        float distance=Player.position.x-startPos.x;
        if(distance<0)distance=0;
        ScoreText.text=Mathf.FloorToInt(distance).ToString();
    }

    void ToggleExplanation()
    {
        paused=!paused;
        ExplanationPanel.SetActive(paused);
        Time.timeScale=paused?0f:1f;
    }
}
