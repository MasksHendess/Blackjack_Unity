using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    //Lists
    public List<Card> Deck;
    public List<Card> PlayerHand;
    public List<Card> DealerHand;

    public Card CardPrefab;
    public Card HiddenCardPrefab;
    private Card DealersFaceDownCard;
    private Card DealersHiddenCard;
    private bool StayBtnPressed;

    public GameObject PlayerHandPosition;
    public GameObject DealerHandPosition;
    //Buttons
    public GameObject HitMeBtn;
    public GameObject StayBtn;
    public GameObject DealBtn;
    public GameObject SmallbetBtn;
    public GameObject MediumbetBtn;
    public GameObject BigbetBtn;
    //Score
    public Text PlayerScore;
    public Text DealerScore;
    public Text Anouncer;
    int PlayerExtraX;
    int DealerExtraX;
    void Start()
    {
        PlayerExtraX = 0;
        DealerExtraX = 0;
        createDeck();
        HitMeBtn.SetActive(false);
        StayBtn.SetActive(false); 

        Vector3 bonusX;
        bonusX.x = 105;
        bonusX.y = 0;
        bonusX.z = 0;

        StayBtnPressed = false;
        DealersFaceDownCard = Instantiate(HiddenCardPrefab, DealerHandPosition.transform.position + bonusX,HiddenCardPrefab.transform.rotation);
        DealersFaceDownCard.gameObject.SetActive(false);
    }
    void createDeck()
    {
        addCardsToDeck("Hearts");
        addCardsToDeck("Spades");
        addCardsToDeck("Diamonds");
        addCardsToDeck("Clubs");
    }
    void addCardsToDeck(string color)
    {
        var sprites = SpriteCollection.instance.getSprites(color);
        for (int i = 1; i < 14; i++)
        {
            var Card = Instantiate(CardPrefab);
            Card.color = color;
            Card.value = i;
            var img = Card.GetComponent<Image>();
            img.sprite = sprites[i];
            Card.gameObject.SetActive(false);
            Deck.Add(Card);
        }
    }
    void Update()
    {
        PlayerScore.text = "Your Hand\n" + calculateValueInHand(PlayerHand).ToString();
        // Hides Hidden cards value from player
        if(!StayBtnPressed && DealersHiddenCard)
        { 
            var dealerScore = calculateValueInHand(DealerHand) - DealersHiddenCard.value;
            if (dealerScore < 0)
                dealerScore = 0;
            DealerScore.text = "The House\n" + dealerScore.ToString();
        }
        else
        {
            DealerScore.text = "The House\n" + calculateValueInHand(DealerHand).ToString();
        }
    }
    public void startGame()
    {
        if (Bank.instance.bet > 0)
        {
            Anouncer.text = "";
            //Generate Player Starting Hand
            for (int i = 0; i < 2; i++)
            {
                StartCoroutine(addCardToPlayer());
            }

            // Generate Dealer Starting Hand
            addCardToDealer();
            var canvas = FindObjectOfType<Canvas>();
            Vector3 bonusX;
            bonusX.x = 0;
            bonusX.y = 0;
            bonusX.z = 0;
            // second Card
            bonusX.x = 105; 
            DealersHiddenCard = Deck[Random.Range(0, Deck.Count)];
            Deck.Remove(DealersHiddenCard);
            DealerHand.Add(DealersHiddenCard);

            DealersHiddenCard.transform.position = DealerHandPosition.transform.position + bonusX;
            DealersHiddenCard.transform.SetParent(canvas.gameObject.transform);
            if (calculateValueInHand(DealerHand) == 21)
            {
                DealersHiddenCard.gameObject.SetActive(true);
               endGame(3);
            }
            else
            {
                DealersFaceDownCard.gameObject.SetActive(true);
                DealersFaceDownCard.transform.SetParent(canvas.gameObject.transform);
                //Btns
                HitMeBtn.SetActive(true);
                StayBtn.SetActive(true);
                DealBtn.SetActive(false);
                SmallbetBtn.SetActive(false);
                MediumbetBtn.SetActive(false);
                BigbetBtn.SetActive(false);
            }
        }
    }
    public void hitMeBtn()
    {
        StartCoroutine(addCardToPlayer());
    }
    public void stayBtn()
    {
       
        if (PlayerHand.Count >= 2)
        {
            StayBtnPressed = true;
            DealersFaceDownCard.gameObject.SetActive(false);
            DealersHiddenCard.gameObject.SetActive(true);
            StartCoroutine(dealerPealer());
        }
    }
    IEnumerator addCardToPlayer()
    {
        Vector3 bonusX;
        bonusX.x = PlayerExtraX;
        bonusX.y = 0;
        bonusX.z = 0;

        //Get random Card from deck
        var randomCard = Deck[Random.Range(0, Deck.Count)];
        Deck.Remove(randomCard);
        PlayerHand.Add(randomCard);
        randomCard.gameObject.SetActive(true);
        // Set Position
        randomCard.transform.position = PlayerHandPosition.transform.position + bonusX;
        PlayerExtraX += 105;

        var canvas = FindObjectOfType<Canvas>();
        randomCard.transform.SetParent(canvas.gameObject.transform);
        //Evaluate Score
        int score = calculateValueInHand(PlayerHand);
        if (PlayerHand.Count == 2 && score == 21)
        {
            yield return new WaitForSeconds(1f);
            endGame(0);
        }
        if (score > 21)
        {
            yield return new WaitForSeconds(1f);
            endGame(3);
        }
    }
    IEnumerator dealerPealer()
    {
        yield return new WaitForSeconds(1f);
        DealerExtraX = 210;
        int playerHandvalue = calculateValueInHand(PlayerHand);
        int DealerHandvalue = calculateValueInHand(DealerHand);
        while (DealerHandvalue < playerHandvalue && DealerHandvalue != playerHandvalue)
        {
            addCardToDealer();
            yield return new WaitForSeconds(1f);
            DealerExtraX += 105;
            DealerHandvalue = calculateValueInHand(DealerHand);
        }
        // Round is over, determine winner
        int winrar = 9;
        if (calculateValueInHand(DealerHand) >= 22)
        {
            // Player Win 
            winrar = 1;
        }
        else if (calculateValueInHand(DealerHand) > playerHandvalue && calculateValueInHand(DealerHand) < 22)
        {
            winrar = 3;
            // Dealer Win
        }

        yield return new WaitForSeconds(1f);
        endGame(winrar);
    }
    void addCardToDealer()
    {
        var canvas = FindObjectOfType<Canvas>();
        // Dealer Draw
        var randomCard = Deck[Random.Range(0, Deck.Count)];
        Deck.Remove(randomCard);
        DealerHand.Add(randomCard);
        randomCard.gameObject.SetActive(true);
        // Set position
        Vector3 bonusX;
        bonusX.x = DealerExtraX;
        bonusX.y = 0;
        bonusX.z = 0;
        randomCard.transform.position = DealerHandPosition.transform.position + bonusX;
        randomCard.transform.SetParent(canvas.gameObject.transform);
    }
    void endGame(int scenario)
    {
        if (scenario == 0)//Player blackJack
        {
            Anouncer.text = "BLACKJACK!";
            Bank.instance.blackJack();
        }
        else if (scenario == 1) //Player Win
        {
            Anouncer.text = "You win!";
            Bank.instance.modifyChips(true);
        }
        else if (scenario == 3) //Player Loses
        {
            Anouncer.text = "House Wins!";
            Bank.instance.modifyChips(false);
        }
        StartCoroutine(shuffleCardsIntoDeck());
    }
    IEnumerator shuffleCardsIntoDeck()
    {
        yield return new WaitForSeconds(1f); 
        resetDeck(PlayerHand);
        resetDeck(DealerHand); 
        PlayerExtraX = 0;
        DealerExtraX = 0;
        // Reset Btns
        HitMeBtn.SetActive(false);
        StayBtn.SetActive(false);
        DealBtn.SetActive(true);
        SmallbetBtn.SetActive(true);
        MediumbetBtn.SetActive(true);
        BigbetBtn.SetActive(true);
        DealersFaceDownCard.gameObject.SetActive(false);
        StayBtnPressed = false; 
        Anouncer.text = "Place your bet.";
    }
    void resetDeck(List<Card> deck)
    {
        foreach (var card in deck)
        {
            card.gameObject.SetActive(false);
            Deck.Add(card);
        }
        deck.Clear();
    }
    int calculateValueInHand(List<Card> hand)
    {
        int playerTotal = 0;
        foreach (var card in hand)
        {
            if (card.value >= 10)
            {
                playerTotal += 10;
            }
            else if (card.value == 1)
            {
                // Skip Aces for now
            }
            else
            {
                playerTotal += card.value;
            }
        }
        //Handle Aces
        List<Card> aces = hand.Where(x => x.value == 1).ToList();
        foreach (var ace in aces)
        {
            if (playerTotal + 11 < 22)
                playerTotal += 11;
            else
                playerTotal += 1;
        }
        return playerTotal;
    }
}
