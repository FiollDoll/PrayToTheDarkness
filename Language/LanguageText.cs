﻿using UnityEngine;
using TMPro;

public class LanguageText : MonoBehaviour
{
    public LanguageSetting languageSetting;
    private TextMeshProUGUI _textMeshProUGUI;


    public void OnEnable()
    {
        if (!_textMeshProUGUI)
            _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        _textMeshProUGUI.text = languageSetting.text;
    }
}