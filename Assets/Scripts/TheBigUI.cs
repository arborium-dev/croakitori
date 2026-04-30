using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;


public class TheBigUI : MonoBehaviour
{

    private bool[] theBigTable = new bool[5];
    public TextMeshProUGUI theBigText;
    public Button theBigGinger;
    public Button theBigMSG;
    public Button theBigGarlic;
    public Button theBigZest;
    public Button theBigCumin;
    public Button theBigCook;
    
    public void TheBigMSG()
    {
        if (TheBigCheck() == true)
        {
            theBigTable[0] = true;
            Debug.Log("The Big MSG");
            theBigText.text += " - MSG                          ";
            theBigMSG.interactable = false;
        }
    }
    
    public void TheBigGinger()
    {
        if (TheBigCheck() == true)
        {
            theBigTable[1] = true;
            Debug.Log("The Big Ginger");
            theBigText.text += " - Ginger                       ";
            theBigGinger.interactable = false;
        }
    }
    
    public void TheBigGarlic()
    {
        if (TheBigCheck() == true)
        {
            theBigTable[2] = true;
            Debug.Log("The Big Garlic");
            theBigText.text += " - Garlic                       ";
            theBigGarlic.interactable = false;
        }
    }
    
    public void TheBigZest()
    {
        if (TheBigCheck() == true)
        {
            theBigTable[3] = true;
            Debug.Log("The Big Zest");
            theBigText.text += " - Zest                         ";
            theBigZest.interactable = false;
        }
    }
    
    public void TheBigCumin()
    {
        if (TheBigCheck() == true)
        {
            theBigTable[4] = true;
            Debug.Log("The Big Cumin");
            theBigText.text += " - Cumin                        ";
            theBigCumin.interactable = false;
        }
    }

    public bool TheBigCheck()
    {
        int theBigElements = 0;
        for (int i = 0; i < theBigTable.Length; i++)
        {

            if (theBigTable[i])
            {
                theBigElements++;
            }

        }
        if (theBigElements >= 3)
        {
            return false;
        }
        else
        {
            return true;
        }
        

    }
    public void TheBigCook()
    {
        for (int i = 0; i < theBigTable.Length; i++)
        { 
            theBigTable[i] = false;
        }
        
        theBigGarlic.interactable = true;
        theBigGinger.interactable = true;
        theBigMSG.interactable = true;
        theBigZest.interactable = true;
        theBigCumin.interactable = true;
        theBigText.text = " - Spices -";
        
    }
}
