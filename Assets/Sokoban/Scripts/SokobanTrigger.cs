using System.Collections;
using UnityEngine;

public class SokobanTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Box")
        {
            GameManager.TargetsAchived++;
            Debug.Log("Ящик поставлен на палету (всего " + GameManager.TargetsAchived.ToString() + "из " + GameManager.TargetsToWin.ToString() + " )");
        }
        
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Box")
        {
            GameManager.TargetsAchived--;
            Debug.Log("Ящик убран с палеты (всего " + GameManager.TargetsAchived.ToString() + "из " + GameManager.TargetsToWin.ToString() + " )");
        }
    }
}

