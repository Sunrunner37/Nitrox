﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NitroxClient.GameLogic.ChatUI;
using NitroxModel.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NitroxClient.MonoBehaviours.Gui.Chat
{
    public class PlayerChat : uGUI_InputGroup
    {
        private const int LINE_CHAR_LIMIT = 255;
        private const int MESSAGES_LIMIT = 64;
        private const float TOGGLED_TRANSPARENCY = 0.4f;
        public const float CHAT_VISIBILITY_TIME_LENGTH = 10f;

        public static bool IsLoading = true;
        private PlayerChatManager playerChatManager;
        private CanvasGroup canvasGroup;
        private HorizontalOrVerticalLayoutGroup[] layoutGroups;
        private GameObject logEntryPrefab;
        private Image[] backgroundImages;
        private bool transparent;
        private InputField inputField;
        public string inputText
        {
            get { return inputField.text; }
            set { inputField.text = value; }
        }

        private static readonly Queue<ChatLogEntry> entries = new Queue<ChatLogEntry>();

        public IEnumerator SetupChatComponents()
        {
            playerChatManager = NitroxServiceLocator.LocateService<PlayerChatManager>();

            canvasGroup = GetComponent<CanvasGroup>();
            layoutGroups = GetComponentsInChildren<HorizontalOrVerticalLayoutGroup>();

            logEntryPrefab = GameObject.Find("ChatLogEntryPrefab");
            logEntryPrefab.AddComponent<PlayerChatLogItem>();
            logEntryPrefab.SetActive(false);

            GetComponentsInChildren<Button>()[0].onClick.AddListener(ToggleBackgroundTransparency);
            GetComponentsInChildren<Button>()[1].gameObject.AddComponent<PlayerChatPinButton>();

            inputField = GetComponentInChildren<InputField>();
            inputField.gameObject.AddComponent<PlayerChatInputField>();
            inputField.GetComponentInChildren<Button>().onClick.AddListener(playerChatManager.SendMessage);

            backgroundImages = new[]
            {
                transform.GetChild(0).GetComponent<Image>(), transform.GetChild(1).GetComponent<Image>(), transform.GetChild(3).GetComponent<Image>()
            };

            yield return new WaitForEndOfFrame(); //Needed so Select() works on initialization
            IsLoading = false;
        }

        public void WriteLogEntry(string playerName, string message, Color color)
        {
            if (entries.Count == MESSAGES_LIMIT)
            {
                Destroy(entries.Dequeue().EntryObject);
            }

            ChatLogEntry chatLogEntry;
            GameObject chatLogEntryObject;
            if (entries.Count != 0 && entries.Last().PlayerName == playerName)
            {
                chatLogEntry = entries.Last();
                chatLogEntry.MessageText += "\n" + message;
                chatLogEntry.UpdateTime();
                chatLogEntryObject = chatLogEntry.EntryObject;
            }
            else
            {
                chatLogEntry = new ChatLogEntry(playerName, SanitizeMessage(message), color);
                chatLogEntryObject = Instantiate(logEntryPrefab, logEntryPrefab.transform.parent, false);
                chatLogEntry.EntryObject = chatLogEntryObject;
                entries.Enqueue(chatLogEntry);
            }

            chatLogEntryObject.GetComponent<PlayerChatLogItem>().ApplyOnPrefab(chatLogEntry);
            StartCoroutine(UpdateChatEntrySpacing());
        }


        /// Updates the layout sorting algorithm from Unity to prevent "loss" of text messages.
        private IEnumerator UpdateChatEntrySpacing()
        {
            yield return null;
            foreach (HorizontalOrVerticalLayoutGroup layoutGroup in layoutGroups)
            {
                layoutGroup.enabled = false;
            }
            yield return null;
            foreach (HorizontalOrVerticalLayoutGroup layoutGroup in layoutGroups)
            {
                layoutGroup.enabled = true;
            }
        }

        public void Show()
        {
            PlayerChatInputField.ResetTimer();
            StartCoroutine(ToggleChatFade(true));
        }
        public void Hide()
        {
            StartCoroutine(ToggleChatFade(false));
        }

        public void Select()
        {
            base.Select(true);
            inputField.Select();
            inputField.ActivateInputField();
        }

        public void Deselect()
        {
            base.Deselect();
            EventSystem.current.SetSelectedGameObject(null);
        }

        private static string SanitizeMessage(string message)
        {
            message = message.Trim();
            return message.Length < LINE_CHAR_LIMIT ? message : message.Substring(0, LINE_CHAR_LIMIT);
        }

        private void ToggleBackgroundTransparency()
        {
            float alpha = transparent ? 1f : TOGGLED_TRANSPARENCY;
            transparent = !transparent;

            foreach (Image backgroundImage in backgroundImages)
            {
                backgroundImage.CrossFadeAlpha(alpha, 0.5f, false);
            }
        }

        private IEnumerator ToggleChatFade(bool fadeIn)
        {
            if (fadeIn)
            {
                while (canvasGroup.alpha < 1f)
                {
                    canvasGroup.alpha += 0.01f;
                    yield return new WaitForSeconds(0.0005f);
                }
            }
            else
            {
                while (canvasGroup.alpha > 0f)
                {
                    canvasGroup.alpha -= 0.01f;
                    yield return new WaitForSeconds(0.005f);
                }
            }
        }
    }
}
