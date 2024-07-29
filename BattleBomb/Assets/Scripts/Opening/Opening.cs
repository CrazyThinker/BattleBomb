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

        // ��� .map ���� �˻�
        string[] mapFiles = Directory.GetFiles("map", "*.map");

        // ���� �̸����� ��� �� Ȯ���� ����
        var mapNames = mapFiles.Select(file => Path.GetFileNameWithoutExtension(file)).ToList();

        // ��Ӵٿ �߰�
        dropdown_map.AddOptions(new List<string> { "Select Map" });
        dropdown_map.AddOptions(mapNames);

        // ���õ� �� ����
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
        // ���õ� �� ����
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
