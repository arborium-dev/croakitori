using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;


public class TheBigUI : MonoBehaviour
{
    private const int MAX_SPICES = 3;
    private bool[] selectedSpices = new bool[5];
    private string[] spiceButtonCombo = new string[5] { "↑↑→→", "→↓↑", "→←→←", "↑↓→←", "↑→↓←"};
    private const string HighlightColor = "#FFD54A";
    private const string NormalColor = "#FFFFFF";
    
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
    
    private int currentComboLocation = 0;

    public int totalIngredientsCollected = 0;

    private InputAction _moveAction;
    private bool _ownsEnabledAction;



    [Header("Prefabs")]
    [SerializeField] private GameObject ingredientPrefab;
    
    [Header("Spawn Settings")]
    [SerializeField] private Transform ingredientSpawn;

    [SerializeField] private float scatterForce = 3f;

    private void Awake()
    {
        ResolveMoveAction();
    }

    private void OnEnable()
    {
        ResolveMoveAction();

        if (_moveAction != null)
        {
            _moveAction.performed += OnMovePerformed;

            if (_ownsEnabledAction)
            {
                _moveAction.Enable();
            }
        }
    }

    private void OnDisable()
    {
        if (_moveAction != null)
        {
            _moveAction.performed -= OnMovePerformed;

            if (_ownsEnabledAction)
            {
                _moveAction.Disable();
                _ownsEnabledAction = false;
            }
        }
    }
    
    public void OnMsgSelected()
    {
        if (CanAddMoreSpices())
        {
            selectedSpices[0] = true;
            Debug.Log("MSG added");
            recipeDisplayText.text += "- MSG\n";
            msgButton.interactable = false;
        }
    }
    
    public void OnGingerSelected()
    {
        if (CanAddMoreSpices())
        {
            selectedSpices[1] = true;
            Debug.Log("Ginger added");
            recipeDisplayText.text += "- Ginger\n";
            gingerButton.interactable = false;
        }
    }
    
    public void OnGarlicSelected()
    {
        if (CanAddMoreSpices())
        {
            selectedSpices[2] = true;
            Debug.Log("Garlic added");
            recipeDisplayText.text += "- Garlic\n";
            garlicButton.interactable = false;
        }
    }
    
    public void OnZestSelected()
    {
        if (CanAddMoreSpices())
        {
            selectedSpices[3] = true;
            Debug.Log("Zest added");
            recipeDisplayText.text += "- Zest\n";
            zestButton.interactable = false;
        }
    }
    
    public void OnCuminSelected()
    {
        if (CanAddMoreSpices())
        {
            selectedSpices[4] = true;
            Debug.Log("Cumin added");
            recipeDisplayText.text += "- Cumin\n";
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

    public void OnCookButtonPressed() // this is basically the setup for the cooking minigame
    {
        ingredientComboCombined = string.Empty;
        currentComboLocation = 0;
        
        for (int i = 0; i < selectedSpices.Length; i++) // this creates the button string
        {
            if (selectedSpices[i])
            {
                ingredientComboCombined += spiceButtonCombo[i];
            }

            selectedSpices[i] = false;
        }

        UpdateComboText();
        
        for (int i = 0; i < 3; i++)
        {
            if (totalIngredientsCollected <= 0)
            {
                break;
            }
            SummonIngredient();
            totalIngredientsCollected--;
        }
        
    }

    public void OnComboButtonPressed(string pressedSymbol)
    {
        if (string.IsNullOrEmpty(ingredientComboCombined) || currentComboLocation >= ingredientComboCombined.Length)
        {
            return;
        }

        if (ingredientComboCombined[currentComboLocation].ToString() != pressedSymbol)
        {
            return;
        }

        currentComboLocation++;
        UpdateComboText();

        if (currentComboLocation >= ingredientComboCombined.Length)
        {
            Debug.Log("Combo complete!");
            ResetMinigame();
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 move = context.ReadValue<Vector2>();
        string pressedSymbol = GetDirectionSymbol(move);

        if (!string.IsNullOrEmpty(pressedSymbol))
        {
            OnComboButtonPressed(pressedSymbol);
        }
    }
    

    private void ResetMinigame() // resets the cooking menu to defaults
    {
        ingredientComboCombined = string.Empty;
        currentComboLocation = 0;
        buttonComboText.text = string.Empty;
        recipeDisplayText.text = " - Spices -    ";
        garlicButton.interactable = true;
        gingerButton.interactable = true;
        msgButton.interactable = true;
        zestButton.interactable = true;
        cuminButton.interactable = true;
    }

    private void UpdateComboText()
    {
        if (buttonComboText == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(ingredientComboCombined))
        {
            buttonComboText.text = string.Empty;
            return;
        }

        int highlightedIndex = Mathf.Clamp(currentComboLocation, 0, ingredientComboCombined.Length - 1);
        string before = ingredientComboCombined.Substring(0, highlightedIndex);
        string highlighted = ingredientComboCombined[highlightedIndex].ToString();
        string after = highlightedIndex + 1 < ingredientComboCombined.Length
            ? ingredientComboCombined.Substring(highlightedIndex + 1)
            : string.Empty;

        buttonComboText.text = $"<color={NormalColor}>{before}</color><color={HighlightColor}><b>{highlighted}</b></color><color={NormalColor}>{after}</color>";
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
                Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
                
                rb.AddForce(randomDirection * scatterForce, ForceMode2D.Impulse);
            }
            
            Debug.Log("Ingredient Spawned");
        }
        else
        {
            Debug.LogWarning("Ingredient prefab is not assigned in the inspector.");
        }
    }

    private void ResolveMoveAction()
    {
        _moveAction = null;
        _ownsEnabledAction = false;

       
        _moveAction = InputSystem.actions.FindAction("move");

        

        if (_moveAction != null)
        {
            _moveAction.Enable();
            _ownsEnabledAction = true;
        }
    }

    private static string GetDirectionSymbol(Vector2 move)
    {
        if (move.sqrMagnitude < 0.01f)
        {
            return string.Empty;
        }

        if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
        {
            return move.x > 0f ? "→" : "←";
        }

        return move.y > 0f ? "↑" : "↓";
    }
}
