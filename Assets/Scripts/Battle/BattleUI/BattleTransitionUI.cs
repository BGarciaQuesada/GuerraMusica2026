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

    // Se ha partido el m�todo de PlayTransition en dos para poder esperar a que los personajes est�n en el plano de batalla
    public IEnumerator PlayEnter()
    {
        // Slide in (cubre pantalla)
        yield return Move(transitionImage, leftOutside, center);
    }

    public IEnumerator PlayExit()
    {
        // Slide out
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
