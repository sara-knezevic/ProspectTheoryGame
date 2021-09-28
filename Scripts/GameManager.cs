using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using MongoDB.Driver;
using MongoDB.Bson;

public class GameManager : MonoBehaviour
{
    public GameObject firstPersonController;
    public GameObject gate;
    public GameObject destination;

    public GameObject[] taskObjects;

    public GameObject money;
    public GameObject healthBar;
    public float moneyCount = 12;
    public GameObject updateAlert;

    public GameObject blackoutPanel;
    private Color blackoutColor;
    private float fadeAmount;

    public static int solvedCount = 0;

    UserAnswer activeUser = new UserAnswer();
    int taskNum;
    private bool savedAnswers = false;

    public Dropdown genderDropdown;
    public Dropdown ageDropdown;
    public Dropdown educationDropdown;

    public Button playButton;
    public GameObject failPanel;

    MongoClientSettings settings;
    MongoClient client;

    IMongoDatabase database;
    IMongoCollection<BsonDocument> collection;

    void Start()
    {
        var connectionString = "#####";
        try
        {
            var mongoUrl = new MongoUrl(connectionString);
            client = new MongoClient(mongoUrl);
            database = client.GetDatabase("ProspectTheory");
            collection = database.GetCollection<BsonDocument>("AnswersV2");

            client.ListDatabaseNames();
        } catch (Exception e)
        {
            playButton.interactable = false;
            failPanel.SetActive(true);
        }

        List<string> ageList = new List<string>();

        for (int i = 18; i < 66; i++)
        {
            ageList.Add(i.ToString());
        }

        ageDropdown.AddOptions(ageList);

        activeUser.gender = genderDropdown.options[genderDropdown.value].text;
        activeUser.age = 0;
        activeUser.education = educationDropdown.options[educationDropdown.value].text;
        activeUser.answers = new List<int> { 0, 0, 0, 0, 0, 0, 0 };

        blackoutColor = blackoutPanel.GetComponent<Image>().color;

        money.transform.GetChild(0).gameObject.GetComponent<Text>().text = (moneyCount).ToString("0");
    }

    public void SetGender()
    {
        activeUser.gender = genderDropdown.options[genderDropdown.value].text;
    }

    public void SetAge()
    {
        activeUser.age = int.Parse(ageDropdown.options[ageDropdown.value].text);
    }

    public void SetEducation()
    {
        activeUser.education = educationDropdown.options[educationDropdown.value].text;
    }

    IEnumerator ShowAlert(string textToDisplay)
    {
        updateAlert.transform.GetChild(0).gameObject.GetComponent<Text>().text = textToDisplay;
        updateAlert.SetActive(true);
        yield return new WaitForSeconds(3);
        updateAlert.SetActive(false);
    }

    IEnumerator FadeToBlack()
    {
        while (blackoutColor.a < 1)
        {
            fadeAmount = blackoutColor.a + (1 * Time.deltaTime);

            blackoutColor = new Color(blackoutColor.r, blackoutColor.g, blackoutColor.b, fadeAmount);
            blackoutPanel.GetComponent<Image>().color = blackoutColor;

            yield return null;
        }

        firstPersonController.GetComponent<FirstPersonController>().enabled = false;

        firstPersonController.transform.position = new Vector3(455, 212, 668);

        firstPersonController.GetComponent<FirstPersonController>().enabled = true;

        while (blackoutColor.a > 0)
        {
            fadeAmount = blackoutColor.a - (1 * Time.deltaTime);

            blackoutColor = new Color(blackoutColor.r, blackoutColor.g, blackoutColor.b, fadeAmount);
            blackoutPanel.GetComponent<Image>().color = blackoutColor;

            yield return null;
        }
    }

    public void markSolved()
    {
        solvedCount++;

        Interactable focus = firstPersonController.GetComponent<FirstPersonController>().focus;

        focus.transform.parent.gameObject.GetComponent<Task>().isSolved = true;
        focus.GetComponent<Interactable>().isInteractable = false;

        string taskNumber = focus.transform.parent.name;

        if (System.String.Equals(taskNumber, "FirstTask"))
        {
            healthBar.GetComponent<RawImage>().texture = Resources.Load<Texture2D>("healthBarFilled");
            healthBar.transform.GetChild(0).gameObject.GetComponent<Text>().text = "150/250";

            StartCoroutine(ShowAlert("150 health points gained!"));
        }
        else if (System.String.Equals(taskNumber, "SecondTask"))
        {
            moneyCount -= 3f;
            money.transform.GetChild(0).gameObject.GetComponent<Text>().text = (moneyCount).ToString();
            moveGate();

            StartCoroutine(ShowAlert("3 gold coins lost!"));
        }
        else if (System.String.Equals(taskNumber, "ThirdTask"))
        {
            moneyCount += 3.5f;
            Debug.Log(moneyCount);
            money.transform.GetChild(0).gameObject.GetComponent<Text>().text = (moneyCount).ToString();

            StartCoroutine(ShowAlert("3.5 gold coins gained!"));
        }
        else if (System.String.Equals(taskNumber, "FourthTask"))
        {
            money.transform.GetChild(1).gameObject.GetComponent<Text>().text = "+20";

            StartCoroutine(ShowAlert("You've won the dagger!"));
        }
        else if (System.String.Equals(taskNumber, "FifthTask"))
        {
            // Bandits task is solved
            taskObjects[5].transform.GetChild(0).gameObject.GetComponent<Interactable>().isInteractable = true;
            taskObjects[6].transform.GetChild(0).gameObject.GetComponent<Interactable>().isInteractable = true;

            StartCoroutine(FadeToBlack());

            StartCoroutine(ShowAlert("You've been injured!"));

            healthBar.GetComponent<RawImage>().texture = Resources.Load<Texture2D>("healthBarKindaFilled");
            healthBar.transform.GetChild(0).gameObject.GetComponent<Text>().text = "30/250";
        } else if (System.String.Equals(taskNumber, "SixthTask"))
        {
            healthBar.GetComponent<RawImage>().texture = Resources.Load<Texture2D>("healthBarFull");
            healthBar.transform.GetChild(0).gameObject.GetComponent<Text>().text = "250/250";

            StartCoroutine(ShowAlert("Healing steadily!"));
        }
    }

    public void insertAnswerOne()
    {
        Interactable focus = firstPersonController.GetComponent<FirstPersonController>().focus;
        string taskNumber = focus.transform.parent.name;

        switch (taskNumber)
        {
            case "FirstTask":
                taskNum = 0;
                break;
            case "SecondTask":
                taskNum = 1;
                break;
            case "ThirdTask":
                taskNum = 2;
                break;
            case "FourthTask":
                taskNum = 3;
                break;
            case "FifthTask":
                taskNum = 4;
                break;
            case "SixthTask":
                taskNum = 5;
                break;
            case "SeventhTask":
                taskNum = 6;
                break;
        }

        activeUser.answers[taskNum] = 1;
        checkIfAllSolved();
    }

    public void insertAnswerTwo()
    {
        Interactable focus = firstPersonController.GetComponent<FirstPersonController>().focus;
        string taskNumber = focus.transform.parent.name;

        switch (taskNumber)
        {
            case "FirstTask":
                taskNum = 0;
                break;
            case "SecondTask":
                taskNum = 1;
                break;
            case "ThirdTask":
                taskNum = 2;
                break;
            case "FourthTask":
                taskNum = 3;
                break;
            case "FifthTask":
                taskNum = 4;
                break;
            case "SixthTask":
                taskNum = 5;
                break;
            case "SeventhTask":
                taskNum = 6;
                break;
        }

        activeUser.answers[taskNum] = 2;
        checkIfAllSolved();
    }

    public void checkIfAllSolved()
    {
        if (solvedCount == 7 && !savedAnswers)
        {
            SaveAnswersToDatabase(activeUser);
            savedAnswers = true;
        }
    }

    public void gameOver()
    {
        if (solvedCount == 7)
        {
            firstPersonController.GetComponent<FirstPersonController>().toggleOutroPanel();
        }
    }

    public void moveGate()
    {
        float speed = 5000;
        Transform pivotPoint = gate.transform.GetChild(0).gameObject.transform;

        gate.transform.RotateAround(pivotPoint.position, Vector3.up, -speed * Time.deltaTime);
        gate.GetComponent<MeshCollider>().enabled = false;
    }

    public void addBonus()
    {
        money.transform.GetChild(1).gameObject.GetComponent<Text>().text = "+50";
    }

    public async void SaveAnswersToDatabase(UserAnswer user)
    {
        BsonArray answersArray = new BsonArray();

        foreach (var a in user.answers)
        {
            answersArray.Add(a);
        }

        var document = new BsonDocument
        {
            { "gender", user.gender },
            { "age", user.age },
            { "education", user.education },
            { "answers", answersArray }
        };

        await collection.InsertOneAsync(document);
    }
}
