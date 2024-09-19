using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    public GameManager gameManager; // Reference to the GameManager script
    public float timeLimit = 10f; // Time limit for player to select the correct image
    public float respawnInterval = 10f; // Interval between respawns
    public Image timerImage; // Reference to the UI Image for the timer

    private float timeSinceLastSpawn;
    private bool respawnCooldown = false; // Track whether we're in the cooldown period
    private float currentTimer; // Current time counting down

    void Start()
    {
        ResetTimer(); // Initialize the timer
    }

    void Update()
    {
        if (!respawnCooldown)
        {
            currentTimer -= Time.deltaTime; // Decrease the timer each frame
            UpdateTimerUI(); // Sync the timer image fill

            if (currentTimer <= 0f)
            {
                Debug.Log("Time limit exceeded. Respawning new set of images.");
                gameManager.RespawnNewSetOfImages(); // Call the respawn function from GameManager
                StartRespawnCooldown(); // Start the cooldown
            }
        }
        else
        {
            currentTimer -= Time.deltaTime; // Decrease during cooldown
            UpdateTimerUI(); // Sync the timer image fill

            if (currentTimer <= 0f)
            {
                Debug.Log("Cooldown ended, resetting timer.");
                ResetTimer(); // Reset the timer after the cooldown
                respawnCooldown = false; // End the cooldown
            }
        }
    }

    // Call this method when new images are spawned to reset the timer
    public void ResetTimer()
    {
        timeSinceLastSpawn = Time.time; // Reset the timer
        currentTimer = timeLimit; // Reset the countdown to the full time
        UpdateTimerUI(); // Reset the UI fill amount
    }

    // Starts the cooldown period after a respawn
    void StartRespawnCooldown()
    {
        currentTimer = respawnInterval; // Start counting down for cooldown
        respawnCooldown = true; // Enter cooldown mode
        UpdateTimerUI(); // Update UI for cooldown
        Debug.Log("Cooldown started.");
    }

    // Syncs the UI Image with the current timer
    void UpdateTimerUI()
    {
        if (timerImage != null)
        {
            float fillAmount = currentTimer / (respawnCooldown ? respawnInterval : timeLimit); // Normalize to 0-1
            timerImage.fillAmount = Mathf.Clamp01(fillAmount); // Clamp between 0 and 1
        }
    }
}
