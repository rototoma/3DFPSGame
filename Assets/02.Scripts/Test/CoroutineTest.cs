using System.Collections;
using UnityEngine;

public class CoroutineTest : MonoBehaviour
{
    // Coroutine이란?
    // 비동기 실행과 제어를 효율적으로 제공하는 기능이다.
    // 1. 비동기 실행
    // 2. 시간에 따른 실행 제어

    private void Start()
    {
        Debug.Log("Start");

        StartCoroutine(Co_Countdown());
    }

    private IEnumerator Co_Countdown()
    {
        yield return new WaitForSeconds(1f);
    }
}