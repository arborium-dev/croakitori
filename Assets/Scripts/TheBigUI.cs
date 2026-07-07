using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;


public class TheBigUI : MonoBehaviour
{
    private const int MAX_SPICES = 3;
    private bool[] selectedSpices = new bool[5];
    private string[] spiceButtonCombo = new string[5] { "↑↑→→", "→↓↑", "→←→←", "↑↓→←", "↑→↓←"};
    
    [Header("UI Elements")]
    public TextMeshProUGUI recipeDisplayText;
    public TextMeshProUGUI buttonComboText;
    
    public Button msgButton;
    public Button gingerButton;
    public Button garlicButton;
    public Button zestButton;
    public Button cuminButton;
    public Button cookButton;
    private string ingredientComboCombined = "";
    
    [Header("Prefabs")]
    [SerializeField] private GameObject ingredientPrefab;
    
    [Header("Spawn Settings")]
    [SerializeField] private Transform ingredientSpawn;

    [SerializeField] private float scatterForce = 3f;
    
    public void OnMsgSelected()
    {
        if (CanAddMoreSpices())
        {
            selectedSpices[0] = true;
            Debug.Log("MSG added");
            recipeDisplayText.text += " - MSG                          ";
            msgButton.interactable = false;
        }
    }
    
    public void OnGingerSelected()
    {
        if (CanAddMoreSpices())
        {
            selectedSpices[1] = true;
            Debug.Log("Ginger added");
            recipeDisplayText.text += " - Ginger                       ";
            gingerButton.interactable = false;
        }
    }
    
    public void OnGarlicSelected()
    {
        if (CanAddMoreSpices())
        {
            selectedSpices[2] = true;
            Debug.Log("Garlic added");
            recipeDisplayText.text += " - Garlic                       ";
            garlicButton.interactable = false;
        }
    }
    
    public void OnZestSelected()
    {
        if (CanAddMoreSpices())
        {
            selectedSpices[3] = true;
            Debug.Log("Zest added");
            recipeDisplayText.text += " - Zest                         ";
            zestButton.interactable = false;
        }
    }
    
    public void OnCuminSelected()
    {
        if (CanAddMoreSpices())
        {
            selectedSpices[4] = true;
            Debug.Log("Cumin added");
            recipeDisplayText.text += " - Cumin                        ";
            cuminButton.interactable = false;
        }
    }

    public bool CanAddMoreSpices()
    {
        int selectedSpiceCount = 0;
        for (int i = 0; i < selectedSpices.Length; i++)
        {
            if (selectedSpices[i])
            {
                selectedSpiceCount++;
            }
        }

        return selectedSpiceCount < MAX_SPICES;
    }

    public void OnCookButtonPressed()
    {
        
        for (int i = 0; i < selectedSpices.Length; i++)
        {
            if (selectedSpices[i])
            {
                ingredientComboCombined += spiceButtonCombo[i];
            }

            selectedSpices[i] = false;
        }

        buttonComboText.text = ingredientComboCombined;
        //     
        // garlicButton.interactable = true;
        // gingerButton.interactable = true;
        // msgButton.interactable = true;
        // zestButton.interactable = true;
        // cuminButton.interactable = true;
        recipeDisplayText.text = " - Spices -";
        SummonIngredient();
        SummonIngredient();
        SummonIngredient();
    }

    public void SummonIngredient()
    {
        if (ingredientPrefab != null)
        {
            Vector3 spawnPosition = ingredientSpawn != null ? ingredientSpawn.position : Vector3.zero;
            GameObject newIngredient = Instantiate(ingredientPrefab, spawnPosition, Quaternion.identity);
            Rigidbody2D rb = newIngredient.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                
                rb.AddForce(randomDirection * scatterForce, ForceMode2D.Impulse);
            }
            
            Debug.Log("Ingredient Spawned");
        }
        else
        {
            Debug.LogWarning("Ingredient prefab is not assigned in the inspector.");
        }
    }
}
