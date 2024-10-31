using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class inventory : MonoBehaviour
{
    public List<item> playerItems = new List<item>();
    [SerializeField] private item[] gameItems = new item[0];
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private GameObject itemInfoMenu, mainWatch, onlyIconWatch;
    [SerializeField] private TextMeshProUGUI newItemText;
    [SerializeField] private allScripts scripts;

    public void ManageInventoryPanel(bool state)
    {
        GameObject invMenu = transform.Find("inventoryMenu").gameObject;
        invMenu.SetActive(state);
        if (state)
            UpdateInvUI();
    }

    public void AddItem(string name)
    {
        foreach (item item in gameItems)
        {
            if (item.nameInGame == name)
            {
                playerItems.Add(item);
                StartCoroutine(ActivateNotify(item.name));
                break;
            }
        }
    }

    public void UseItem(int slot)
    {
        if (playerItems[slot].activateNameDialog != "")
            scripts.dialogsManager.ActivateDialog(playerItems[slot].activateNameDialog);
        if (playerItems[slot].questName != "" && playerItems[slot].questNextStep)
        {
            if (scripts.quests.FindQuest(playerItems[slot].questName) == scripts.quests.totalQuest)
                scripts.quests.NextStep();
        }

        if (playerItems[slot].removeAfterUse)
            playerItems.RemoveAt(slot);

        scripts.player.playerMenu.gameObject.SetActive(false);
    }

    public void WatchItem(int id)
    {
        itemInfoMenu.gameObject.SetActive(!itemInfoMenu.gameObject.activeSelf);
        mainWatch.gameObject.SetActive(!playerItems[id].watchIconOnly);
        onlyIconWatch.gameObject.SetActive(playerItems[id].watchIconOnly);
        if (playerItems[id].watchIconOnly)
            onlyIconWatch.transform.Find("ItemIcon").GetComponent<Image>().sprite = playerItems[id].icon;
        else
        {
            mainWatch.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text = playerItems[id].name;
            mainWatch.transform.Find("TextDescription").GetComponent<TextMeshProUGUI>().text = playerItems[id].description;
            mainWatch.transform.Find("ItemIcon").GetComponent<Image>().sprite = playerItems[id].icon;
        }

        Button button = itemInfoMenu.transform.Find("ButtonActivate").GetComponent<Button>();
        button.interactable = true; // Дефолт значение
        if (playerItems[id].canUse && (playerItems[id].useInInventory || playerItems[id].useInCollider))
        {
            if (playerItems[id].useInInventory)
            {
                if (playerItems[id].questName != "")
                {
                    if (playerItems[id].questName != scripts.quests.totalQuest.nameInGame)
                        button.interactable = false;
                    if (scripts.quests.totalQuest.totalStep != playerItems[id].questStage && playerItems[id].questStage > 0)
                        button.interactable = false;
                }
            }
            else if (playerItems[id].useInCollider)
            {
                if (scripts.interactions.selectedEI == null
                 || (scripts.interactions.selectedEI != null && scripts.interactions.selectedEI.itemNameUse != playerItems[id].nameInGame))
                    button.interactable = false;
            }
        }
        else
            button.interactable = false;

        if (button.interactable)
        {
            button.onClick.AddListener(delegate { UseItem(id); });
            if (playerItems[id].activationText != "")
                button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = playerItems[id].activationText;
            else
                button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "???";
        }
        else
            button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "";
    }

    private void UpdateInvUI()
    {
        foreach (Transform child in transform.Find("inventoryMenu").transform.Find("items"))
            Destroy(child.gameObject);

        for (int i = 0; i < playerItems.Count; i++)
        {
            var obj = Instantiate(inventorySlotPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform.Find("inventoryMenu").transform.Find("items"));
            obj.GetComponent<Image>().sprite = playerItems[i].icon;
            int num = i;
            obj.GetComponent<Button>().onClick.AddListener(delegate { WatchItem(num); });
        }
    }

    private IEnumerator ActivateNotify(string name)
    {
        newItemText.gameObject.SetActive(true);
        newItemText.text = "+" + name;
        yield return new WaitForSeconds(2);
        newItemText.gameObject.SetActive(false);
    }
}

[System.Serializable]
public class item
{
    public string nameInGame;
    public string nameRu, nameEn;
    [HideInInspector]
    public string name
    {
        get
        {
            if (PlayerPrefs.GetString("language") == "ru")
                return nameRu;
            else
                return nameEn;
        }
    }
    public string descriptionRu, descriptionEn;
    [HideInInspector]
    public string description
    {
        get
        {
            if (PlayerPrefs.GetString("language") == "ru")
                return descriptionRu;
            else
                return descriptionEn;
        }
    }
    public string activationTextRu, activationTextEn;
    [HideInInspector]
    public string activationText
    {
        get
        {
            if (PlayerPrefs.GetString("language") == "ru")
                return activationTextRu;
            else
                return activationTextEn;
        }
    }
    [HideInInspector]
    public Sprite icon
    {
        get
        {
            if (iconRu != null) // iconEn дефолт
            {
                if (PlayerPrefs.GetString("language") == "ru")
                    return iconRu;
                else
                    return iconEn;
            }
            return iconEn;
        }
    }
    public Sprite iconRu, iconEn;
    public bool watchIconOnly;

    [Header("UseSettings")]
    public bool canUse;
    public bool removeAfterUse = true;
    public bool useInInventory;
    public bool useInCollider;

    [Header("UseInInventorySetting")]
    public string activateNameDialog;
    public string questName;
    public int questStage = -1;
    public bool questNextStep;
}
