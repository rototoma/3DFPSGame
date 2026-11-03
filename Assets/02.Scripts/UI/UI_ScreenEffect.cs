using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_ScreenEffect : MonoBehaviour
{
    [SerializeField] private Image _hitScreen;

    [SerializeField] private float _flashTime = 0.06f;
    [SerializeField] private float _gradTime = 3f;

    // 피격 연출 색상 (알파는 연출에 따라 코드에서 결정한다)
    [SerializeField] private Color _hitColor = new Color(0.55f, 0.03f, 0.05f, 1f);

    private Coroutine _hitCoroutine;

    private void Awake()
    {
        _hitScreen.enabled = false;
    }

    public void Flash()
    {
        _hitCoroutine = StartCoroutine(Flash_Coroutine());
    }

    IEnumerator Flash_Coroutine()
    {
        _hitScreen.enabled = true;
        yield return new WaitForSeconds(_flashTime);

        _hitScreen.enabled = false;
    }

    public void Fade()
    {
        _hitCoroutine = StartCoroutine(Fade_Coroutine());
    }

    IEnumerator Fade_Coroutine()
    {
        _hitScreen.enabled = true;
        float timer = _flashTime;
        while (timer > 0)
        {
            _hitScreen.color = new Color(_hitColor.r, _hitColor.g, _hitColor.b, 2 * timer);
            timer -= Time.deltaTime;
            yield return null;
        }

        _hitScreen.enabled = false;
    }

    public void Grad()
    {
        StartCoroutine(Grad_Coroutine());
    }

    IEnumerator Grad_Coroutine()
    {
        _hitScreen.enabled = true;
        float timer = _gradTime;
        while (timer > 0)
        {
            _hitScreen.color = new Color(_hitColor.r, _hitColor.g, _hitColor.b, (3 - timer) / 3);
            timer -= Time.deltaTime;
            yield return null;
        }
    }
}