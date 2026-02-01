using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Esta clase existe para que haya una imagen que va de izquierda a derecha, tape la pantalla, y desaparece cuando se carga lo siguiente
public class BattleTransitionUI : MonoBehaviour
{
    [SerializeField] RectTransform transitionImage;
    [SerializeField] float slideDuration = 0.5f;

    Vector2 leftOutside;
    Vector2 center;
    Vector2 rightOutside;

    void Awake()
    {
        float width = transitionImage.rect.width;
        leftOutside = new Vector2(-width, 0);
        center = Vector2.zero;
        rightOutside = new Vector2(width, 0);

        transitionImage.anchoredPosition = leftOutside;
    }

    public IEnumerator PlayTransition()
    {
        // Entrar
        yield return Move(transitionImage, leftOutside, center);

        // Espera mientras se teletransporta
        yield return new WaitForSecondsRealtime(0.3f);

        // Salir
        yield return Move(transitionImage, center, rightOutside);
    }

    IEnumerator Move(RectTransform img, Vector2 from, Vector2 to)
    {
        float t = 0f;
        while (t < slideDuration)
        {
            t += Time.unscaledDeltaTime;
            img.anchoredPosition = Vector2.Lerp(from, to, t / slideDuration);
            yield return null;
        }
        img.anchoredPosition = to;
    }
}
