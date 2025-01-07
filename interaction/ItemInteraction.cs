﻿using UnityEngine;

public class ItemInteraction : MonoBehaviour, IInteractable
{
    [Header("Main")] public string ruLabelName;
    public string enLabelName;
    public string interLabel => PlayerPrefs.GetString("language") == "ru" ? ruLabelName : enLabelName;

    [Header("Requires")] public bool AutoUse;
    public bool autoUse => AutoUse;
    public string NameQuestRequired;
    public string nameQuestRequired => NameQuestRequired;
    public int StageInter;
    public int stageInter => StageInter;

    [Header("Preferences")] public bool darkAfterUse { get; set; }
    public bool nextQuestStep { get; set; }

    private AllScripts _scripts;

    private void Awake()
    {
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
    }

    public void DoInteraction()
    {
        if (darkAfterUse)
            _scripts.main.ActivateNoVision(1f);
        if (nextQuestStep)
            _scripts.questsSystem.NextStep();

        _scripts.inventoryManager.AddItem(this.gameObject.name);
        Destroy(this.gameObject);
    }
}