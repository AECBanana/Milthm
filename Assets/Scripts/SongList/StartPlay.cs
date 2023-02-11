using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPlay : MonoBehaviour
{
    public void Click()
    {
        DifficultyController.Ready = false;
        BeatmapLoader.PlayingUID = DifficultyController.Active.uid;
        BeatmapLoader.PlayingIndex = DifficultyController.Active.index;
        PlayerPrefs.SetInt(DifficultyController.Active.uid + ".lastPlay", DifficultyController.Active.index);
        BeatmapLoader.Playing = SongResources.Beatmaps[DifficultyController.Active.uid][DifficultyController.Active.index];
        Loading.Run("PlayScene", "PlayLoadingPrefab");
        MusicEffectBtn.CurrentSprite = null;
    }
}
