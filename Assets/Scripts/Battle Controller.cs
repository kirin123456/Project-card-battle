using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    public static BattleController instance;

    private void Awake()
    {
        instance = this;
    }


    public int startingMana = 4, maxMana = 12;
    public int playerMana, enemyMana;
    private int currentPlayerMaxMana, currentEnemyMaxMana, currentMaxMana;



    public int startingCardsAmount = 5;
    public int cardsToDrawPerTurn = 2;

    public enum TurnOrder { playerActive, playerCardAttacks, enemyActive, enemyCardAttacts}
    public TurnOrder currentPhase;

    public Transform discardPoint;

    public int playerHealth, enemyHealth;

    // Start is called before the first frame update
    void Start()
    {
        //playerMana = startingMana;
        //UIController.instance.SetPlayerManaText(playerMana);
        currentPlayerMaxMana = startingMana;
        FillPlayerManan();

        DeckController.instance.DrawMultipleCards(startingCardsAmount);
        UIController.instance.SetPlayerHealthText(playerHealth);
        UIController.instance.SetEnemyHealthText(enemyHealth);

        currentEnemyMaxMana = startingMana;
        FillEnemyManan();

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            AdvanceTurn();
        }
        
    }

    public void SpendPlayerMana(int amountToSpend)
    {
        playerMana = playerMana - amountToSpend;

        if(playerMana < 0)
        {
            playerMana = 0;
        }


        UIController.instance.SetPlayerManaText(playerMana);

    }

    public void FillPlayerManan()
    {
        //playerMana = startingMana;
        playerMana = currentPlayerMaxMana;
        UIController.instance.SetPlayerManaText(playerMana);
    }

    public void SpendEnemyMana(int amountToSpend)
    {
        enemyMana -= amountToSpend;

        if (enemyMana < 0)
        {
            enemyMana = 0;
        }


        UIController.instance.SetEnemyManaText(enemyMana);

    }

    public void FillEnemyManan()
    {
        enemyMana = currentEnemyMaxMana;
        UIController.instance.SetEnemyManaText(enemyMana);
    }

    public void AdvanceTurn()
    {
        currentPhase++;

        if((int)currentPhase >= System.Enum.GetValues(typeof(TurnOrder)).Length)
        {
            currentPhase = 0;
        }


        switch (currentPhase)
        {
            case TurnOrder.playerActive:


                UIController.instance.endTurnButton.SetActive(true);
                UIController.instance.drawCardButton.SetActive(true);

                if(currentPlayerMaxMana < maxMana)
                {
                    currentPlayerMaxMana++;
                }

                FillPlayerManan();

                DeckController.instance.DrawMultipleCards(cardsToDrawPerTurn);

                break;

               

            case TurnOrder.playerCardAttacks:

                //Debug.Log("skipping player card attacks");
                //AdvanceTurn();

                CardPointsController.instance.PlayerAttack();
                break;

               

            case TurnOrder.enemyActive:

                //Debug.Log("skipping enemy actions");
                //AdvanceTurn();

                if (currentEnemyMaxMana < maxMana)
                {
                    currentEnemyMaxMana++;
                }

                FillEnemyManan();

                EnemyController.instance.StartAction();

                break;

            case TurnOrder.enemyCardAttacts:

                //Debug.Log("skipping enemy card attacks");
                //AdvanceTurn();

                CardPointsController.instance.EnemyAttack();
                break;

        }
    }

    public void EndPlayerTurn()
    {
        UIController.instance.endTurnButton.SetActive(false);
        UIController.instance.drawCardButton.SetActive(false);
        AdvanceTurn();
    }

    public void DamagePlayer(int damageAmount)
    {
        if(playerHealth > 0)
        {
            playerHealth -= damageAmount;

            if(playerHealth <= 0)
            {
                playerHealth = 0;

                //End Battle
            }

            UIController.instance.SetPlayerHealthText(playerHealth);

            UIDamageindicator damageClone = Instantiate(UIController.instance.playerDamage, UIController.instance.playerDamage.transform.parent);
            damageClone.damageText.text = damageAmount.ToString();
            damageClone.gameObject.SetActive(true);
        }
    }

    public void DamageEnemy(int damageAmount)
    {
        if (enemyHealth > 0)
        {
            enemyHealth -= damageAmount;

            if (enemyHealth <= 0)
            {
                enemyHealth = 0;

                //End Battle
            }

            UIController.instance.SetEnemyHealthText(enemyHealth);

            UIDamageindicator damageClone = Instantiate(UIController.instance.enemyDamage, UIController.instance.enemyDamage.transform.parent);
            damageClone.damageText.text = damageAmount.ToString();
            damageClone.gameObject.SetActive(true);
        }
    }
}
