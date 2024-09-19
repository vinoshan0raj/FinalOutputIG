using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    public GameManager gameManager; 
    public float timeLimit = 10f; 
    public float respawnInterval = 10f; 
    public Image timerImage; 

    private float timeSinceLastSpawn;
    private bool respawnCooldown = false; 
    private float currentTimer;

    void Start()
    {
        ResetTimer(); 
    }

    void Update()
    {
        if (!respawnCooldown)
        {
            currentTimer -= Time.deltaTime; 
            UpdateTimerUI(); 

            if (currentTimer <= 0f)
            {
                Debug.Log("Time limit exceeded. Respawning new set of images.");
                gameManager.RespawnNewSetOfImages(); 
                StartRespawnCooldown(); 
            }
        }
        else
        {
            currentTimer -= Time.deltaTime; 
            UpdateTimerUI(); 

            if (currentTimer <= 0f)
            {
                Debug.Log("Cooldown ended, resetting timer.");
                ResetTimer(); 
                respawnCooldown = false;
            }
        }
    }

    public void ResetTimer()
    {
        timeSinceLastSpawn = Time.time; 
        currentTimer = timeLimit; 
        UpdateTimerUI(); 
    }

    void StartRespawnCooldown()
    {
        currentTimer = respawnInterval; 
        respawnCooldown = true; 
        UpdateTimerUI(); 
        Debug.Log("Cooldown started.");
    }

    void UpdateTimerUI()
    {
        if (timerImage != null)
        {
            float fillAmount = currentTimer / (respawnCooldown ? respawnInterval : timeLimit); 
            timerImage.fillAmount = Mathf.Clamp01(fillAmount); 
        }
    }
}
