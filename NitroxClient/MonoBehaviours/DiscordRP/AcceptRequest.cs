﻿using System.Collections;
using NitroxClient.MonoBehaviours.DiscordRP;
using UnityEngine;

namespace NitroxClient.MonoBehaviours.Gui.MainMenu
{
    public class AcceptRequest : MonoBehaviour
    {
        public DiscordController DiscordRp;
        public DiscordRpc.DiscordUser Request;

        Rect discordWindowRect = new Rect(Screen.width / 2 - 250, 100, 500, 250);
        Texture avatar;

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void Start()
        {
            DiscordRp = gameObject.GetComponent<DiscordController>();
            StartCoroutine(LoadAvatar(Request.userId, Request.avatar));
        }

        public void OnGUI()
        {
            discordWindowRect = GUILayout.Window(0, discordWindowRect, DoDiscordWindow, "Server join request from Discord");
        }

        private void SetGUIStyle()
        {
            GUI.skin.textArea.fontSize = 18;
            GUI.skin.textArea.richText = false;
            GUI.skin.textArea.alignment = TextAnchor.MiddleLeft;
            GUI.skin.textArea.wordWrap = true;
            GUI.skin.textArea.stretchHeight = true;
            GUI.skin.textArea.padding = new RectOffset(10, 10, 5, 5);

            GUI.skin.label.fontSize = 14;
            GUI.skin.label.alignment = TextAnchor.MiddleRight;
            GUI.skin.label.stretchHeight = true;
            GUI.skin.label.fixedWidth = 140;
            GUI.skin.label.fixedHeight = 140;

            GUI.skin.button.fontSize = 14;
            GUI.skin.button.stretchHeight = true;
        }

        private void DoDiscordWindow(int windowId)
        {
            SetGUIStyle();
            using (GUILayout.VerticalScope v = new GUILayout.VerticalScope("Box"))
            {
                using (GUILayout.HorizontalScope h = new GUILayout.HorizontalScope())
                {
                    GUILayout.Label(avatar);
                    GUILayout.Label("Player " + Request.username + " wants to join your server.");
                }
                using (GUILayout.HorizontalScope b = new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Accept"))
                    {
                        CloseWindow(1);
                    }

                    if (GUILayout.Button("Deny"))
                    {
                        CloseWindow(0);
                    }
                }
            }
        }

        private void CloseWindow(int respond)
        {
            DiscordRp.RespondLastJoinRequest(respond);
            DiscordRp.ShowingWindow = false;
            Destroy(this);
        }

        private IEnumerator LoadAvatar(string id, string avatar_id)
        {
            WWW avatarURL = new WWW("https://cdn.discordapp.com/avatars/" + id + "/" + avatar_id + ".png");
            yield return avatarURL;

            if (avatarURL.texture != null)
            {
                avatar = avatarURL.texture;
            }
            else
            {
                //using (WWW standardAvatarURL = new WWW("https://discordapp.com/assets/6debd47ed13483642cf09e832ed0bc1b.png"))
                //{
                //    yield return standardAvatarURL;
                //    if (standardAvatarURL.texture != null)
                //    {
                //        avatar = standardAvatarURL.texture;
                //    }
                //}
            }

            yield return null;
        }
    }
}
