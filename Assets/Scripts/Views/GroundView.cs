
using Services;
using Static;
using UnityEngine;
using Zenject;

public class GroundView : MonoBehaviour
{
    [Inject]
    public void Init(MainService s)
    {
        StaticValues.GroundYPos = transform.position.y;
    }
}
