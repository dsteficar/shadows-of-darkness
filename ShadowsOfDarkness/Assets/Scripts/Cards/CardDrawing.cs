using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor.Animations;
using System.Net;
using UnityEditor.Rendering;
using Unity.VisualScripting;

public class CardDrawing : MonoBehaviour
{
    public List<GameObject> targetSpotList;
    //public List<GameObject> handSlots;
    public GameObject playerDeck;
    //public GameObject playerHand;
    public GameObject playerCamera;
    public GameObject nextCard;

    public bool lockedDraw = false;
    //private bool lastCard = false;

    //public float cardZOffset;
    //public float cardYOffset;

    public float fieldSizeExpander = 0.03f;

    public int numberOfSlots = 0;
    public int maxNumberOfSlots = 0;

    [SerializeField]
    public List<GameObject> currentSlotList = new List<GameObject>();
    [SerializeField]
    public List<GameObject> futureSlotList = new List<GameObject>();


    public GameObject handField;
    public GameObject slotPrefab;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && playerDeck.transform.childCount != 0 && !lockedDraw && numberOfSlots < 10)
        {
            numberOfSlots++;
            DrawCard();
        }
    }

    private void DrawCard()
    {
        lockedDraw = true;
        nextCard = playerDeck.transform.GetChild(playerDeck.transform.childCount - 1).gameObject;
        Vector3 endPoint = targetSpotList[0].transform.position;
        nextCard.transform.DOMove(endPoint, 1).SetEase(Ease.InQuart).OnComplete(() => ShowCard());
    }

    private void ShowCard()
    {
        nextCard.transform.parent = handField.transform;
        nextCard.transform.position = new Vector3(targetSpotList[1].transform.position.x - 1, targetSpotList[1].transform.position.y, targetSpotList[1].transform.position.z + 2);
        nextCard.transform.rotation = targetSpotList[1].transform.rotation;
        nextCard.transform.DOJump(targetSpotList[1].transform.position, 0.5f, 1, 2, false).OnComplete(() => ScaleHand());
    }

    private void ScaleHand()
    {

        GameObject slot;
        int halfNumberOfCards = Mathf.CeilToInt((float)numberOfSlots / 2);
        float zOffset = -1;
        float yxOffset = 0.0025f;
        if (numberOfSlots % 2 == 0)
        {
            zOffset = -(fieldSizeExpander * ((Mathf.CeilToInt(((float)numberOfSlots + 1) / 2))) - (fieldSizeExpander / 2));
        }
        else
        {
            zOffset = -(fieldSizeExpander * ((numberOfSlots + 1) / 2));

        }
        for (int currentSlot = 1; currentSlot <= numberOfSlots; currentSlot++)
        {
            float currentYXOffset = (yxOffset * currentSlot); 
            slot = Instantiate(slotPrefab, handField.transform.position, handField.transform.rotation);
            slot.transform.parent = handField.transform;

            zOffset = zOffset + fieldSizeExpander;
            Vector3 slotPosition = new Vector3(currentYXOffset, currentYXOffset, zOffset);
            slot.transform.localPosition = slotPosition;
            //yxOffset = yxOffset + fieldSizeExpander/2;
            futureSlotList.Add(slot);
        }

        AddCardToHand();

    }

    private void AddCardToHand()
    {
        if (numberOfSlots != 1)
        {
            for (int i = 0; i < numberOfSlots-1; i++)
            {
                currentSlotList[i].gameObject.transform.GetChild(0).gameObject.transform.DOMove(futureSlotList[i].transform.position, 1.5f);
                currentSlotList[i].gameObject.transform.GetChild(0).transform.parent = futureSlotList[i].transform;
            }
        }

        Vector3 cardRotation = new Vector3(0,0,-45);
        nextCard.transform.DORotate(cardRotation, 1);
        nextCard.transform.DOMove(futureSlotList[numberOfSlots - 1].transform.position, 1.5f).OnComplete(() => UnlockDrawing());
    }

    private void UnlockDrawing()
    {
        DOTween.CompleteAll();

        nextCard.transform.parent = futureSlotList[numberOfSlots - 1].transform;


        foreach (var slot in currentSlotList)
        {
            Destroy(slot.gameObject);
        }
        currentSlotList.Clear();

        foreach(GameObject slot in futureSlotList)
        {
            currentSlotList.Add(slot);
        }
        futureSlotList.Clear();

        lockedDraw = false;
    }

    /*
    private void AddCardToHand(GameObject nextCard)
    {
        int numberOfCards = 0;
        foreach (GameObject slot in handSlots)
        {
            if (slot.transform.childCount > 0)
            {
                numberOfCards++;
            }
        }
        switch (numberOfCards)
        {
            case 0:
                AllignCards(0);
                GetCardInOrder(0, nextCard);
                break;
            case 1:
                AllignCards(1);
                GetCardInOrder(1, nextCard);
                break;
            case 2:
                AllignCards(2);
                GetCardInOrder(2, nextCard);
                break;
            case 3:
                AllignCards(numberOfCards);
                GetCardInOrder(3, nextCard);
                break;
            case 4:
                AllignCards(numberOfCards);
                GetCardInOrder(4, nextCard);
                break;
            case 5:
                AllignCards(numberOfCards);
                GetCardInOrder(5, nextCard);
                break;
            case 6:
                AllignCards(numberOfCards);
                GetCardInOrder(6, nextCard);
                break;
            case 7:
                AllignCards(numberOfCards);
                GetCardInOrder(7, nextCard);
                break;
            case 8:
                AllignCards(numberOfCards);
                GetCardInOrder(8, nextCard);
                break;
            case 9:
                AllignCards(numberOfCards);
                GetCardInOrder(9, nextCard);
                break;
        }
    }
  


    private void AllignCards(int numberOfCards)
    {
        int futureNumberOfCards = numberOfCards + 1;
        int halfNumberOfCards = futureNumberOfCards / 2;
        int indexOfCard = -1;
        lastCard = false;

        for (int i = 0; i < futureNumberOfCards; i++)
        {
            if (i == futureNumberOfCards)
                lastCard = true;
            //Moze i van nekud
            if (futureNumberOfCards == 1)
                break;

            indexOfCard = i + 1;
            //float fieldPosition = fieldSizeExpander * indexOfCard;

            if (futureNumberOfCards % 2 == 0)
            {
                if (indexOfCard > halfNumberOfCards)
                {
                    float zOffset = fieldSizeExpander * (indexOfCard - halfNumberOfCards);

                    SetFieldOffset(zOffset, i, futureNumberOfCards);
                }
                if (indexOfCard <= halfNumberOfCards)
                {
                    float zOffset = fieldSizeExpander * ((halfNumberOfCards + 1) - indexOfCard);

                    SetFieldOffset(-zOffset, i, futureNumberOfCards);
                }
            }
            else
            {
                if (indexOfCard > halfNumberOfCards)
                {
                    float zOffset = fieldSizeExpander * (indexOfCard - halfNumberOfCards);

                    SetFieldOffset(zOffset, i, futureNumberOfCards);
                }
                else if (indexOfCard < halfNumberOfCards)
                {
                    float zOffset = fieldSizeExpander * ((halfNumberOfCards + 1) - indexOfCard);

                    SetFieldOffset(-zOffset, i, futureNumberOfCards);
                }
                else
                {
                    SetFieldOffset(0, i, futureNumberOfCards);
                }
            }
        }
    }

    private void GetCardInOrder(int slotIndex, GameObject nextCard)
    {

        //slotPosition.z = handSlots[i].transform.position.z + (cardZOffset * (i + 1));
        //slotPosition.z = handSlots[i].transform.position.z;
        //slotPosition.y = handSlots[i].transform.position.y + cardYOffset;
        //Vector3 slotPosition = handSlots[i].transform.position;
        //handSlots[i].transform.position = slotPosition;
        nextCard.transform.DORotate(handSlots[slotIndex].transform.eulerAngles, 1);
        nextCard.transform.DOMove(handSlots[slotIndex].transform.position, 1.5f).OnComplete(() => UnlockDrawing(slotIndex));
        /*
        foreach (GameObject slot in handSlots)
        {
            if (slot.transform.childCount > 0)
            {
                slotPosition = slot.transform.position;
                slotPosition.z = slot.transform.position.z - 0.05f;
                slotPosition.y = slot.transform.position.y - cardYOffset * (i + 1) + 0.02f;
                slotPosition.x = slot.transform.position.x - 0.02f;
                slot.transform.DOMove(slotPosition, 1.5f);
            }
            //slot.transform.DORotate()
        }
        

        //nextCard.transform.DOMove(handSlots[i].transform.position, 1.5f).OnComplete(() => lockedDraw = false);


    }
    private void SetFieldOffset(float zOffset, int currentSlot, int futureNumberOfCards)
    {
        Vector3 slotPosition = handSlots[currentSlot].transform.position;
        slotPosition.z = playerCamera.transform.position.z + zOffset;
        if (currentSlot + 1 != futureNumberOfCards)
        {
            slotPosition.y = handSlots[currentSlot].transform.position.y - cardYOffset;
        }

        //slotPosition.x = handSlots[currentSlot].transform.position.x - cardYOffset + 0.01f;
        if (!lastCard)
        {
            handSlots[currentSlot].transform.DOMove(slotPosition, 1.5f);
        }
        else
        {
            handSlots[currentSlot].transform.DOMove(slotPosition, 1.5f).OnComplete(() => UnlockDrawing(currentSlot));
        }
    }
*/
}
