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
    public TextMeshProUGUI orderText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI ratingText;
    
    public Button msgButton;
    public Button gingerButton;
    public Button garlicButton;
    public Button zestButton;
    public Button cuminButton;
    public Button cookButton;
    public Button resetButton;
    private string ingredientComboCombined = "";
    
    private int currentComboLocation = 0;

    [Header("Timer")]
    [SerializeField] private float startingTimeSeconds = 60f;
    [SerializeField] private float comboBonusSeconds = 15f;
    private float _currentTimeSeconds;
    
    [SerializeField] private float flashThreshold = 15f;
    [SerializeField] private Color normalTimerColor = Color.white;
    [SerializeField] private Color flashTimerColor = Color.red;
    [SerializeField] private float flashSpeed = 3f; // Higher is faster
    
    public float CurrentTimeSeconds // this is so other scripts can read Current Time
    {
        get { return _currentTimeSeconds; }
    }

    public int totalIngredientsCollected = 0;

    private InputAction _moveAction;
    private bool _ownsEnabledAction;
    private int _currentOrderIndex;
    private bool[] _cookedSpiceSnapshot = new bool[5];
    private bool _timerExpired;
    private bool _scoreSubmitted;

    private string[] customerOrders = new string[5]
    {
        "Humph. Let's see if you can cook without poisoning me. \nI want a rich, mouth-watering savoriness. Keep it bright and tangy, though. Oh, and add plenty of that stinky, pungent bulb so my breath keeps everyone away. Don't mess up.", // correct answer: MSG, Zest, Garlic
        "It's chilly today, so warm my old bones. \nGive me a deep, earthy, smoky flavor... like good pond mud. I need a sharp, warming bite that clears the sinuses, too. And throw in that classic stink; I haven't offended enough people today.", // correct answer: Cumin, Ginger, Garlic
        "Bah, beginner's luck. \nNow I'm craving that rich, lip-smacking savory depth. But you gotta balance that heavy, smoky earthiness with a bright pop of citrus. I need a sour kick to wake me up! Make it bland, and I toss it to the flies.", // correct answer: MSG, Cumin, Zest
        "Bad cricket gave me a tummy ache.\nMake it clean. I need a zippy acidity to cut the muck, paired with a warm, stinging spice to settle my stomach. And add the stinky stuff! I want my breath pungent enough to peel paint.", // correct answer: Ginger, Zest, Garlic
        "Still here? Let's see if you're a true chef. \nGive me that deep, musky smoke. Pair it with a sharp, sweet heat that bites back. Finally, I want that ultimate, rich umami depth that makes it impossible to stop eating. Don't expect a tip." // correct answer: Cumin, Ginger, MSG
    };

    private string[] ratingOptions = new string[4]
    {
        "amazing",
        "decent",
        "mid",
        "ass"
    };

    private int totalScore = 0;

    [Header("Prefabs")]
    [SerializeField] private GameObject ingredientPrefab;
    
    [Header("Spawn Settings")]
    [SerializeField] private Transform ingredientSpawn;

    [SerializeField] private float scatterForce = 3f;
    
    

    private void Awake()
    {
        InitializeTimer();
        InitializeOrders();
        ResolveMoveAction();
    }

    private void Update()
    {
        TickTimer();
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

    public void OnResetSelected()
    {
        Array.Clear(selectedSpices, 0, selectedSpices.Length);
        recipeDisplayText.text = " - Spices -    ";
        msgButton.interactable = true;
        gingerButton.interactable = true;
        garlicButton.interactable = true;
        zestButton.interactable = true;
        cuminButton.interactable = true;
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

        return (selectedSpiceCount < MAX_SPICES) && cookButton.interactable;
    }

    public void OnCookButtonPressed() // this is basically the setup for the cooking minigame
    {
        if (_timerExpired)
        {
            return;
        }

        if (selectedSpices[0] == false && selectedSpices[1] == false && selectedSpices[2] == false && selectedSpices[3] == false && selectedSpices[4] == false)
        {
            return;
        }
        resetButton.interactable = false;
        cookButton.interactable = false;

        Array.Copy(selectedSpices, _cookedSpiceSnapshot, selectedSpices.Length);
        if (ratingText != null)
        {
            ratingText.text = string.Empty;
        }

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
        if (_timerExpired)
        {
            return;
        }

        if (string.IsNullOrEmpty(ingredientComboCombined) || currentComboLocation >= ingredientComboCombined.Length)
        {
            return;
        }

        if (ingredientComboCombined[currentComboLocation].ToString() != pressedSymbol)
        {
            currentComboLocation = 0;
            UpdateComboText();
            return;
        }

        currentComboLocation++;
        UpdateComboText();

        if (currentComboLocation >= ingredientComboCombined.Length)
        {
            Debug.Log("Combo complete!");
            UpdateRatingText();
            AddComboBonusTime();
            AdvanceToNextOrder();
            if (_currentOrderIndex >= customerOrders.Length - 1)
            {
                SubmitScoreToSceneManager(totalScore);
            }
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
        resetButton.interactable = true;
        cookButton.interactable = true;
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

    private void InitializeTimer()
    {
        _currentTimeSeconds = Mathf.Max(0f, startingTimeSeconds);
        _timerExpired = false;
        _scoreSubmitted = false;
        UpdateTimerText();
    }

    private void HandleTimerExpired()
    {
        if (_timerExpired)
        {
            return;
        }

        _timerExpired = true;
        _currentTimeSeconds = 0f;
        UpdateTimerText();
        SubmitScoreToSceneManager(-1);
    }

    private void TickTimer()
    {
        if (_timerExpired)
        {
            return;
        }

        _currentTimeSeconds = Mathf.Max(0f, _currentTimeSeconds - Time.deltaTime);
        UpdateTimerText();

        if (_currentTimeSeconds <= 0f)
        {
            HandleTimerExpired();
        }
    }

    private void AddComboBonusTime()
    {
        _currentTimeSeconds += comboBonusSeconds;
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        if (timerText == null)
        {
            return;
        }

        int minutes = Mathf.FloorToInt(_currentTimeSeconds / 60f);
        int seconds = Mathf.FloorToInt(_currentTimeSeconds % 60f);
        timerText.text = $"{minutes}:{seconds:00}";

        // --- FLASHING LOGIC ---
        if (_currentTimeSeconds <= flashThreshold && _currentTimeSeconds > 0)
        {
            // Mathf.PingPong bounces a value back and forth between 0 and 1 over time.
            float flashLerp = Mathf.PingPong(Time.time * flashSpeed, 1f);
            
            // Color.Lerp blends the two colors based on that 0 to 1 value
            timerText.color = Color.Lerp(normalTimerColor, flashTimerColor, flashLerp);
        }
        else
        {
            // Ensure the text stays its normal color when above 15 seconds (or at 0)
            timerText.color = normalTimerColor;
        }
    }

    private void InitializeOrders()
    {
        _currentOrderIndex = 0;
        UpdateOrderText();
        if (ratingText != null)
        {
            ratingText.text = string.Empty;
        }
    }

    private void AdvanceToNextOrder()
    {
        if (customerOrders == null || customerOrders.Length == 0)
        {
            return;
        }

        _currentOrderIndex = Mathf.Min(_currentOrderIndex + 1, customerOrders.Length - 1);
        UpdateOrderText();
    }

    private void UpdateOrderText()
    {
        if (orderText == null || customerOrders == null || customerOrders.Length == 0)
        {
            return;
        }

        _currentOrderIndex = Mathf.Clamp(_currentOrderIndex, 0, customerOrders.Length - 1);
        orderText.text = customerOrders[_currentOrderIndex];
    }

    private void UpdateRatingText()
    {
        if (ratingText == null || customerOrders == null || customerOrders.Length == 0)
        {
            return;
        }

        int correctCount = CalculateCorrectSpiceCount();
        totalScore += correctCount;
        ratingText.text = GetRatingForCorrectCount(correctCount);
    }

    private void SubmitScoreToSceneManager(int score)
    {
        if (_scoreSubmitted)
        {
            return;
        }

        if (LocalSceneManager.Instance != null)
        {
            _scoreSubmitted = true;
            LocalSceneManager.Instance.ReceiveTotalScore(score);
        }
        else
        {
            Debug.LogWarning($"Score {score} was ready to send, but no LocalSceneManager instance was available to receive it.");
        }
    }

    private int CalculateCorrectSpiceCount()
    {
        if (customerOrders == null || customerOrders.Length == 0)
        {
            return 0;
        }

        bool[] correctSpices = GetCorrectSpiceMaskForOrder(_currentOrderIndex);
        int correctCount = 0;

        for (int i = 0; i < _cookedSpiceSnapshot.Length && i < correctSpices.Length; i++)
        {
            if (_cookedSpiceSnapshot[i] && correctSpices[i])
            {
                correctCount++;
            }
        }

        return correctCount;
    }

    private bool[] GetCorrectSpiceMaskForOrder(int orderIndex)
    {
        bool[] mask = new bool[5];

        switch (Mathf.Clamp(orderIndex, 0, customerOrders.Length - 1))
        {
            case 0:
                mask[0] = true;
                mask[2] = true;
                mask[3] = true;
                break;
            case 1:
                mask[1] = true;
                mask[2] = true;
                mask[4] = true;
                break;
            case 2:
                mask[0] = true;
                mask[3] = true;
                mask[4] = true;
                break;
            case 3:
                mask[1] = true;
                mask[2] = true;
                mask[3] = true;
                break;
            default:
                mask[0] = true;
                mask[1] = true;
                mask[4] = true;
                break;
        }

        return mask;
    }

    private string GetRatingForCorrectCount(int correctCount)
    {
        switch (Mathf.Clamp(correctCount, 0, 3))
        {
            case 3:
                return ratingOptions[0];
            case 2:
                return ratingOptions[1];
            case 1:
                return ratingOptions[2];
            default:
                return ratingOptions[3];
        }
    }
}
