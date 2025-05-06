using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{

    public static UIController instance;

    private void Awake()
    {
        instance = this;
    }


    public TMP_Text playerManaText;

    public GameObject manaWarning;
    public float manaWarningTime;
    private float manaWariningCounter;

    public GameObject drawCardButton, endTurnButton;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(manaWariningCounter > 0)
        {
            manaWariningCounter -= Time.deltaTime;

            if(manaWariningCounter <= 0)
            {
                manaWarning.SetActive(false);
            }
        }
        
    }

    public void SetPlayerManaText(int manaAmount)
    {
        
        playerManaText.text = "Mana:" + manaAmount;

    }

    public void ShowManaWarning()
    {
        manaWarning.SetActive(true);
        manaWariningCounter = manaWarningTime;
    }

    public void DrawCard()
    {
        DeckController.instance.DrawCardForMana();

    }

    public void EndPlayerTurn()
    {


        BattleController.instance.EndPlayerTurn();
    }
}

