using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 게임 상태를 리스닝하는 인터페이스
public interface IGameStateListener
{
    void OnStateChanged(GameState state);
}
