using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Assets.Classes;
using TMPro;
using System.Linq;

public class DeckEditMenu : MonoBehaviour
{
    public TMP_Text DeckName;
    public GameObject ArrowLeft;
    public GameObject ArrowRight;
    public GameObject DeckEditBox;
    public GameObject NoDeckMsg;
    public CreateDeck DeckCollection;

    private DeckCollection CardDecks = new();
    private bool PathsInitialized = false;
    private readonly string JSON_RESOURCES_PATH = Path.Combine("CardDataBase","Decks");
    private string JSON_PATH;
    private string SELECTED_DECK_PATH;
    private int displayedDeck = 0;
    private int selectedDeckIndex = 0;

    void Start()
    {
        // initialize path (can't use it during serialization)
        InitializePaths();

        CardDecks = ReadJson<DeckCollection>(JSON_PATH, JSON_RESOURCES_PATH);
        ShowDeckToEdit();
    }

    private void InitializePaths()
    {
        if (!PathsInitialized)
        {
            PathsInitialized = true;
#if UNITY_STANDALONE_WIN
        JSON_PATH = Path.Combine("Assets","Resources","CardDataBase","Decks.json");
        SELECTED_DECK_PATH = Path.Combine("Assets","Resources","CardDataBase","selectedDeckIndex.txt");
#elif UNITY_ANDROID
            JSON_PATH = Path.Combine(Application.persistentDataPath, "CardDataBase", "Decks.json");
            SELECTED_DECK_PATH = Path.Combine(Application.persistentDataPath, "CardDataBase", "selectedDeckIndex.txt");
#endif
        }
    }

    public void ResetView()
    {
        displayedDeck = 0;
        InitializePaths();
        CardDecks = ReadJson<DeckCollection>(JSON_PATH, JSON_RESOURCES_PATH);
        ShowDeckToEdit();
    }

    private T ReadJson<T>(string path, string resource_path)
    {
        T wyn;
        if (!System.IO.File.Exists(path))
        {
            TextAsset json_file = Resources.Load<TextAsset>(resource_path);
            var jsonDB = json_file.text;
            wyn = JsonConvert.DeserializeObject<T>(jsonDB);

            if (Path.GetDirectoryName(path) != null && !Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            File.WriteAllText(path, jsonDB);
        }
        else
        {
            using StreamReader reader = new(path);
            var jsonDB = reader.ReadToEnd();
            wyn = JsonConvert.DeserializeObject<T>(jsonDB);
            reader.Close();
        }

        return wyn;
    }

    private void ShowDeckToEdit()
    {
        switch (CardDecks.Decks.Count)
        {
            case 0:
                DeckEditBox.SetActive(false);
                NoDeckMsg.SetActive(true);
                break;

            case 1:
                ArrowLeft.SetActive(false);
                ArrowRight.SetActive(false);
                break;

            default:
                DeckEditBox.SetActive(true);
                NoDeckMsg.SetActive(false);
                ArrowLeft.SetActive(true);
                ArrowRight.SetActive(true);
                break;
        }

        if (CardDecks.Decks.Count != 0)
            DeckName.text = CardDecks.Decks[displayedDeck].Name;
    }

    public void ShowRight()
    {
        displayedDeck++;
        
        if(displayedDeck > CardDecks.Decks.Count -1)
        {
            displayedDeck = 0;
        }
        ShowDeckToEdit();
    }
    public void ShowLeft()
    {
        displayedDeck--;
        if (displayedDeck < 0)
        {
            displayedDeck = CardDecks.Decks.Count - 1;
        }
        ShowDeckToEdit();
    }

    public void EditDeck()
    {
        DeckCollection.InitalizeDeck(CardDecks.Decks[displayedDeck].CardList.ToList(),CardDecks.Decks[displayedDeck].Name);
    }

    public void SelectDeck(){
        selectedDeckIndex = displayedDeck;
        using (var stream = new FileStream(SELECTED_DECK_PATH, FileMode.Create))
        {
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(selectedDeckIndex);
            }
        }
    }
}