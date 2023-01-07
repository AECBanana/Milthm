using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public const float MoveArea = 50f;
    public float FlowSpeed;
    public BeatmapModel.LineDirection Direction;
    public List<MonoBehaviour> HitObjects = new List<MonoBehaviour>();
    public List<(SpriteRenderer, SpriteRenderer)> ObjectRenders = new List<(SpriteRenderer, SpriteRenderer)>();

    private void Update()
    {
        for(int i = 0; i < HitObjects.Count; i++)
        {
            if (i >= HitObjects.Count) break;
            if (!HitObjects[i].gameObject.activeSelf)
            {
                float x = 0;
                if (HitObjects[i] is TapController tap)
                {
                    x = (tap.Time - AudioUpdate.Time) * FlowSpeed * 5;
                }
                else if (HitObjects[i] is HoldController hold)
                {
                    x = (hold.From - AudioUpdate.Time) * FlowSpeed * 5 - 1.66f;
                }
                if (x <= MoveArea)
                {
                    HitObjects[i].gameObject.SetActive(true);
                    HitObjects.RemoveAt(i);
                    i--;
                }
                else
                {
                    return;
                }
            }
        }
    }
}
