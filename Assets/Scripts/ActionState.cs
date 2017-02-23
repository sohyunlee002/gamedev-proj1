using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public interface ActionState
{
    void Enter();
    void Exit();
    void FixedUpdate();
    void Update();
    void HandleInput();
}
