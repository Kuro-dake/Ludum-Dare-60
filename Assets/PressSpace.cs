using UnityEngine;

public class PressSpace : MonoBehaviour
{
    public static bool waiting_for_space;
    [SerializeField]
    private Vector2 waiting_pos, non_waiting_pos;

    [SerializeField] private float move_speed = 500f;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            waiting_for_space = false;
        }
        
        GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(GetComponent<RectTransform>().anchoredPosition,
                waiting_for_space ? waiting_pos : non_waiting_pos, Time.deltaTime * move_speed);

    }
}
