using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPlay : MonoBehaviour
{
    public void Click()
    {
        SndPlayer.Play("UI_Buttons_Pack2\\Button_15_Pack2");
        DifficultyController.Ready = false;
        BeatmapLoader.PlayingUID = DifficultyController.Active.uid;
        BeatmapLoader.PlayingIndex = DifficultyController.Active.index;
        PlayerPrefs.SetInt(DifficultyController.Active.uid + ".lastPlay", DifficultyController.Active.index);
        BeatmapLoader.Playing = SongResources.Beatmaps[DifficultyController.Active.uid][DifficultyController.Active.index];
        Loading.Run("PlayScene", "PlayLoadingPrefab");
        MusicEffectBtn.CurrentSprite = null;
    }
}
