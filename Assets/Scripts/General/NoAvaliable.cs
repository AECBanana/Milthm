using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoAvaliable : MonoBehaviour
{
    public void Click()
    {
        DialogController.Show("敬请期待", "该功能尚在制作中！");
    }
}
