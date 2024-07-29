using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Opening : MonoBehaviour
{
    public Dropdown dropdown_map;
    public Dropdown dropdown_player1;
    public Dropdown dropdown_player2;

    void Start()
    {
        if (dropdown_map == null || dropdown_map.options.Count != 0) return;

        // 모든 .map 파일 검색
        string[] mapFiles = Directory.GetFiles("map", "*.map");

        // 파일 이름에서 경로 및 확장자 제거
        var mapNames = mapFiles.Select(file => Path.GetFileNameWithoutExtension(file)).ToList();

        // 드롭다운에 추가
        dropdown_map.AddOptions(new List<string> { "Select Map" });
        dropdown_map.AddOptions(mapNames);

        // 선택된 값 설정
        dropdown_map.value = PlayerPrefs.GetInt("dropdownMap", 0);
        dropdown_player1.value = PlayerPrefs.GetInt("dropdownPlayer1", 0);
        dropdown_player2.value = PlayerPrefs.GetInt("dropdownPlayer2", 0);
    }

    // Update is called once per frame
    public void openingGameStart()
    {
        int dropdown_index = dropdown_map.value;
        if (dropdown_index == 0)
        {
            dropdown_map.Show();
            return;
        }
        string dropdown_name = dropdown_map.options[dropdown_index].text;

        PlayerPrefs.SetString("Map", dropdown_name);

        dropdown_index = dropdown_player1.value;
        dropdown_name = dropdown_player1.options[dropdown_index].text;

        PlayerPrefs.SetString("Player 1", dropdown_name);

        dropdown_index = dropdown_player2.value;
        dropdown_name = dropdown_player2.options[dropdown_index].text;

        PlayerPrefs.SetString("Player 2", dropdown_name);

        SceneManager.LoadScene("GameScene");
    }

    public void openingGameSetting()
    {
        // 선택된 값 설정
        PlayerPrefs.SetInt("dropdownMap", dropdown_map.value);
        PlayerPrefs.SetInt("dropdownPlayer1", dropdown_player1.value);
        PlayerPrefs.SetInt("dropdownPlayer2", dropdown_player2.value);

        SceneManager.LoadScene("SettingScene");
    }

    public void openingGameExit()
    {
        UnityEngine.Application.Quit();
    }
}
