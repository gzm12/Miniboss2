using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }

    public void OpenMenu()
    {
        gameObject.SetActive(true);
    }
}
