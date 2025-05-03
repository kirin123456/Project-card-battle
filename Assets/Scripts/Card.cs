using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine. UI;


public class Card : MonoBehaviour
{
    public CardScriptableObject cardSO;

    public int currentHealth;
    public int attackPower, manaCost;

    public TMP_Text healthText, attackText, costText, nameText, actionDescriptionText, loreText;

    public Image characterArt, bgArt;

    private Vector3 targetPoint;
    private Quaternion targetRot;
    public float moveSpeed = 5f, rotateSpeed = 540f;

    public bool inHand;
    public int handPositon;

    private HandController theHC;

    private bool isSelected;
    public Collider theCo1;

    public LayerMask whatIsDesktop, whatIsPlacement;
    private bool justPressed;

    public CardPlacePoint assignedPlace;


    // Start is called before the first frame update
    void Start()
    {
        SetupCard();

        theHC = FindObjectOfType<HandController>();
        theCo1 = GetComponent<Collider>();

    }

    public void SetupCard()
    {
        currentHealth = cardSO.currentHealth;
        attackPower = cardSO.attackPower;
        manaCost = cardSO.manaCost;

        healthText.text = currentHealth.ToString();
        attackText.text = attackPower.ToString();
        costText.text = manaCost.ToString();

        nameText.text = cardSO.cardName;
        actionDescriptionText.text = cardSO.actionDescription;
        loreText.text = cardSO.cardLore;

        characterArt.sprite = cardSO.characterSprite;
        bgArt.sprite = cardSO.bgSprite;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);

        if (isSelected)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 100f, whatIsDesktop))
            {
                MoveToPoint(hit.point + new Vector3(0f, 2f, 0f), Quaternion.identity);

            }
            if (Input.GetMouseButtonDown(1))
            {
                ReturnToHand();
            }
            if (Input.GetMouseButtonDown(0) && justPressed == false)
            {
                if(Physics.Raycast(ray, out hit, 100f, whatIsPlacement))
                {
                    CardPlacePoint selectedPoint = hit.collider.GetComponent<CardPlacePoint>();

                    if (selectedPoint.activeCard == null && selectedPoint.isPlayerPoint)
                    {
                        selectedPoint.activeCard = this;
                        assignedPlace = selectedPoint;


                        MoveToPoint(selectedPoint.transform.position, Quaternion.identity);

                        inHand = false;

                        isSelected = false;

                        theHC.RemoveCardFromHnad(this);

                    }else
                    {
                        ReturnToHand();
                    }

                }else
                {
                    ReturnToHand();
                }
            }
        }

        justPressed = false;
    }

    public void MoveToPoint(Vector3 pointToMoveTo, Quaternion rotToWatch)
    {
        targetPoint = pointToMoveTo;
        targetRot = rotToWatch;
    }

    private void OnMouseOver()
    {
        if(inHand)
        {
            MoveToPoint(theHC.cardPositions[handPositon] + new Vector3(0f, 1f, .5f), Quaternion.identity);
        }
    }

    private void OnMouseExit()
    {
        if(inHand)
        {
            MoveToPoint(theHC.cardPositions[handPositon], theHC.minPos.rotation);
        }
    }

    private void OnMouseDown()
    {
        if(inHand)
        {
            isSelected = true;
            theCo1.enabled = false;

            justPressed = true;
        }
    }

    public void ReturnToHand()
    {
        isSelected = false;
        theCo1.enabled = true;

        MoveToPoint(theHC.cardPositions[handPositon], theHC.minPos.rotation);
    }
}
