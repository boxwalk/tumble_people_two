using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class medals_info : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void menu_change()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("menu_close"))
        {
            anim.SetTrigger("open_menu");
        }else if (anim.GetCurrentAnimatorStateInfo(0).IsName("menu_open"))
        {
            anim.SetTrigger("close_menu");
        }
    }
}
