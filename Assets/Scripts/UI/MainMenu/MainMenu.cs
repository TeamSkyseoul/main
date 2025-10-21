using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Games.System.Settings;
namespace GameUI
{
    public class MainMenu : UIPopUp
    {
        [Header("UI Tabs")]
        [SerializeField]  List<GameObject> tabObjects;

        [Header("Highlight Buttons")]
        [SerializeField] HighlightButton[] buttons;

         readonly List<ISettingPage> tabs = new();
         PlayerSettingData _data;
         const int DefaultTabIndex = 0;

        public override bool Init()
        {
            if (base.Init() == false) return false;
            EnsureTabs();
            LoadData();
            RegisterEvents();
            OpenTab(DefaultTabIndex);
            return true;
        }

        private void EnsureTabs()
        {
            tabs.Clear();

            for(int i =0; i< tabObjects.Count; i++)
            {
                if (tabObjects[i] == null)
                {
                    Debug.LogError("Null reference in _tabObjects list!");
                    continue;
                }
                var module = tabObjects[i].GetComponent<ISettingPage>();

                if (module != null) tabs.Add(module);
                else Debug.LogError($"{tabObjects[i].name} 에 ISettingsModule 구현체가 없습니다!");

            }
           
        }

        private void RegisterEvents()
        {
            for (int i = 0; i < tabs.Count; i++)
            {
                int index = i;
                tabs[index].OnSettingChanged += () => ApplySubData(tabs[index]);
            }
        }

        private void OpenTab(int index)
        {
            CheckHighlight(index);
            for (int i = 0; i < tabObjects.Count; i++)
            {
                if (tabObjects[i] != null)
                    tabObjects[i].gameObject.SetActive(i == index);
            }
        }

        private void LoadData()
        {
            _data =  Games.System.Settings.UserSettings.Settings;

            foreach (var tab in tabs)
            {
                switch (tab)
                {
                    case ISettingsPage<MouseKeyBoardData> mk:
                        mk.Init(_data.MouseKeyBoard);
                        break;

                    case ISettingsPage<AudioData> st:
                        st.Init(_data.Sound);
                        break;

                    case ISettingsPage<GraphicSettingData> gt:
                        gt.Init(_data.Graphic);
                        break;
                }
            }
        }

        private void ApplySubData(ISettingPage tab)
        {
            switch (tab)
            {
                case ISettingsPage<MouseKeyBoardData> mk:
                    _data.MouseKeyBoard = mk.GetSubData();
                    break;

                case ISettingsPage<AudioData> st:
                    _data.Sound = st.GetSubData();
                    break;

                case ISettingsPage<GraphicSettingData> gt:
                    _data.Graphic = gt.GetSubData();
                    break;
            }

          UserSettings.UploadSettings(_data);
        }
        void CheckHighlight(int index)
        {
            for(int i =0; i<buttons.Length; i++)
                buttons[i].SetHighlight(index == i);
        }
        public void OnClickTabButton(int index)=> OpenTab(index);

        public void SaveSetting()
        {
            for(int i=0; i<tabs.Count; i++)
            {
                int index = i;
                tabs[index].ApplySetting();
                ApplySubData(tabs[index]);
            }
  
            UserSettings.SaveSettings();
        }

        public void ResetSettingData()
        {
            UserSettings.ResetSettings();
            LoadData();
        }

        #region ButtonEvents
        public void OnClickBackButton() => UIController.Instance.ClosePopup<MainMenu>();

        public void OnClickLobbyButton()
        {
            // TODO: 로비 씬 이동 처리
        }

        public void OnClickGameExitButton()
        {
            //TODO : GAME EXIT 정산
            #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        #endregion
    }
}
