﻿using System;
using UnityEngine;

namespace NitroxClient.GameLogic.ChatUI
{
    public class ChatLogEntry
    {
        public string PlayerName { get; }
        public string MessageText { get; set; }
        public Color32 PlayerColor { get; }
        public string Time { get; }

        public ChatLogEntry(string playerName, string messageText, Color32 playerColor)
        {
            PlayerName = playerName;
            MessageText = messageText;
            PlayerColor = playerColor;
            Time = DateTime.Now.ToString("HH:mm");
        }
    }
}
