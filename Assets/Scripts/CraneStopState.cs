using UnityEngine;

public class CraneStopState : MonoBehaviour, ICraneState
{
    private CraneController2 _craneController;

    public void Handle(CraneController2 craneController)
    {
        if (!_craneController)
            _craneController = craneController;

        _craneController.CurrentSpeed = 0;
    }
}