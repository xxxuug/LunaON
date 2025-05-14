using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    private void Awake()
    {
        Initialize();
    }

    protected abstract void Initialize();
}
