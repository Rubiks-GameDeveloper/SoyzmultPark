using UnityEngine;
using WebARFoundation; // Импорт из плагина

public class ARController : MonoBehaviour
{
    public GameObject cheburashkaObject; // 3D-объект для Чебурашки
    public GameObject wolfObject; // 3D-объект для Волка
    // Добавьте для других

    private MindARImageTrackingManager imageTracker;

    void Start()
    {
        imageTracker = GetComponent<MindARImageTrackingManager>();
        imageTracker.onTargetFoundEvent += OnTargetFound;
        imageTracker.onTargetLostEvent += OnTargetLost;
    }

    void OnTargetFound(int targetIndex)
    {
        // targetIndex — индекс в .mind (0 для Чебурашки, 1 для Волка)
        if (targetIndex == 0)
        {
            cheburashkaObject.SetActive(true); // Показать анимированный/статичный объект
            // Запустить анимацию: cheburashkaObject.GetComponent<Animator>().Play("AnimationName");
        }
        else if (targetIndex == 1)
        {
            wolfObject.SetActive(true);
        }
    }

    void OnTargetLost(int targetIndex)
    {
        // Скрыть объекты
        cheburashkaObject.SetActive(false);
        wolfObject.SetActive(false);
    }
}