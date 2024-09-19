using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;
public class GameManager : MonoBehaviour
{
    public List<Sprite> firstImagePool;
    public List<Sprite> secondImagePool; 
    public GameObject imagePrefab;       
    public Transform firstSpawnParent;   
    public Transform secondSpawnParent;  
    public Canvas mainCanvas;
    public float firstSetSpacing = 200f;
    public float secondSetSpacing = 150f;
    public TimerManager timerManager;
    public TMP_Text scoreText;

    private List<int> selectedIndices = new List<int>();  
    private GameObject highlightedImage;                  
    private int wrongSelectionCount = 0;
    private int score = 0;


    void Start()
    {   
        SpawnRandomImagesFromFirstPool();
        SpawnImagesFromSecondPool();
        timerManager.ResetTimer();
        UpdateScoreText();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DetectImageClick();
        }
    }

   
    void SpawnRandomImagesFromFirstPool()
    {
        ClearPreviousImages(firstSpawnParent);
        selectedIndices.Clear();
        HashSet<int> selected = new HashSet<int>();
        float totalWidth = (3 - 1) * firstSetSpacing;
        float startX = -totalWidth / 2;
        for (int i = 0; i < 3; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, firstImagePool.Count);
            } while (selected.Contains(randomIndex));
            selected.Add(randomIndex);
            selectedIndices.Add(randomIndex);
            GameObject newImage = Instantiate(imagePrefab, firstSpawnParent);
            newImage.GetComponent<Image>().sprite = firstImagePool[randomIndex];
            RectTransform imageRect = newImage.GetComponent<RectTransform>();      
            imageRect.anchoredPosition = new Vector2(startX + i * firstSetSpacing, 0);
        }      
        int highlightIndex = Random.Range(0, 3);
        highlightedImage = firstSpawnParent.GetChild(highlightIndex).gameObject;
        Outline outline = highlightedImage.AddComponent<Outline>();
        outline.effectColor = Color.red;
        outline.effectDistance = new Vector2(5, 5);
    }
    void SpawnImagesFromSecondPool()
    {
        // Clear previous images
        foreach (Transform child in secondSpawnParent)
        {
            Destroy(child.gameObject);
        }

        // Create a list with the same indices as in the first pool
        List<int> indicesToSpawn = new List<int>();

        // Repeat the indices if there are fewer than 5 images
        while (indicesToSpawn.Count < 5)
        {
            indicesToSpawn.AddRange(selectedIndices); // Add the selected indices multiple times
        }

        // Make sure the list has exactly 5 elements
        indicesToSpawn = indicesToSpawn.GetRange(0, 5);

        // Shuffle the list to randomize the order
        ShuffleList(indicesToSpawn);

        // Spawn 5 images, using the indices from the first pool
        float totalWidth = (5 - 1) * secondSetSpacing;
        float startX = -totalWidth / 2;

        for (int i = 0; i < indicesToSpawn.Count; i++)
        {
            int index = indicesToSpawn[i];
            GameObject newImage = Instantiate(imagePrefab, secondSpawnParent);
            newImage.GetComponent<Image>().sprite = secondImagePool[index]; // Use the second image pool
            RectTransform imageRect = newImage.GetComponent<RectTransform>();
            imageRect.anchoredPosition = new Vector2(startX + i * secondSetSpacing, 0);
        }

        Debug.Log("Randomized set of 5 images from the second pool spawned with the same indices as the first.");
    }


    void ShuffleList(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }


    void DetectImageClick()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        GraphicRaycaster raycaster = mainCanvas.GetComponent<GraphicRaycaster>();
        raycaster.Raycast(pointerData, results);

        if (results.Count > 0)
        {
            GameObject clickedImage = results[0].gameObject;
            Image clickedImageComponent = clickedImage.GetComponent<Image>();
            Image highlightedImageComponent = highlightedImage.GetComponent<Image>();

            if (clickedImageComponent.sprite.name == highlightedImageComponent.sprite.name)
            {
                Destroy(clickedImage);
                wrongSelectionCount = 0;  // Reset wrong selection count if correct image is clicked
                Debug.Log("Correct image clicked! Destroying...");
                score += 10;
                UpdateScoreText();
            }
            else
            {
                wrongSelectionCount++;
                Debug.Log("Wrong image clicked. Wrong selection count: " + wrongSelectionCount);

                // Trigger the screen shake on wrong image selection
                FindObjectOfType<ScreenShake>().TriggerShake();

                if (wrongSelectionCount > 1)
                {
                    Debug.Log("More than 2 wrong selections. Respawning new set...");
                    wrongSelectionCount = 0;
                    SpawnImagesFromSecondPool();
                }
            }
        }
    }

    void UpdateScoreText()
    {
        if(scoreText.text != null)
        {
            scoreText.text = score.ToString();
        }
    }


    void ClearPreviousImages(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    public void RespawnNewSetOfImages()
    {
        // Your existing respawn logic
        foreach (Transform child in secondSpawnParent)
        {
            Destroy(child.gameObject);
        }

        SpawnImagesFromSecondPool();

        Debug.Log("New set of images spawned due to time limit.");
        timerManager.ResetTimer(); // Reset the timer after respawn
    }
}
