using UnityEngine;
using UnityEngine.SceneManagement;
using Yashlan.manage;

namespace Yashlan.controller
{
    public class LoadingController : MonoBehaviour
    {
        void Start()
        {
            UserDataManager.Load();
            SceneManager.LoadScene(1);
        }
    }
}