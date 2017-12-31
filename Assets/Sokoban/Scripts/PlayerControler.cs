using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    public Transform player;
    [SerializeField]
    private float stepLength = 1;
    [SerializeField]
    private float speed = 0.05f;

    private bool isMove;
    private Vector3 direction, targetPos;
    private Transform pushedObject;

    void Start()
    {
        targetPos = player.position;
    }

    void Update()
    {
        if (isMove)
        {
            // Продолжаем движение в заданном направлении
            KeepMove();

            // Player зашел в клетку-назначение - прекращаем движение
            if (targetPos == AlignPosition(player.position))
            {
                StopMove();
            }

            return;
        }

        HandleUserInput();
    }

    private void KeepMove()
    {
        // движение персонажа и ящика, если он есть
        player.position = Vector3.MoveTowards(player.position, targetPos, speed);

        // Толкаем ящик, если он перед нами
        if (pushedObject != null)
        {
            pushedObject.position = Vector3.MoveTowards(pushedObject.position, targetPos + direction * stepLength, speed);
        }
    }

    private void StopMove()
    {
        isMove = false;

        // выравнивание позиции
        player.position = AlignPosition(player.position);

        if (pushedObject != null)
        {
            pushedObject.position = AlignPosition(pushedObject.position);
        }

        ScoreManager.Instance.AddScore(1);
        Debug.Log("Движений: " + ScoreManager.Instance.Score + "(лучший результат: " + ScoreManager.Instance.HighScore + ")");
    }

    private void HandleUserInput()
    {
        var vertical = Input.GetAxisRaw("Vertical");
        var horizontal = Input.GetAxisRaw("Horizontal");

        if (vertical != 0)
        {
            direction = vertical > 0
                ? Vector3.up
                : Vector3.down;
        }
        else if (horizontal != 0)
        {
            direction = horizontal > 0
                ? Vector3.right
                : Vector3.left;
        }
        else
        {
            direction = Vector3.zero;
        }

        if (direction.magnitude != 0)
        {
            Move();
        }
    }

    private void Move()
    {
        // поиск объектов в направлении движения, на два хода вперед
        Transform front = GetTransform(new Vector2(player.position.x + stepLength * direction.x, player.position.y + stepLength * direction.y));
        Transform nextToFront = GetTransform(new Vector2(player.position.x + stepLength * direction.x * 2f, player.position.y + stepLength * direction.y * 2f));

        if (MovementIsPosible(front, nextToFront))
        {
            isMove = true;

            pushedObject = TryGetPushedObject(front);

            targetPos = new Vector3(player.position.x + stepLength * direction.x, player.position.y + stepLength * direction.y, player.position.z);
        }
    }

    private bool MovementIsPosible(Transform front, Transform nextToFront)
    {
        return !(front == null                         // Некуда идти
            || front.tag.CompareTo("Wall") == 0        // Нельзя идти через стену
            || front != null && front.tag.CompareTo("Box") == 0 && nextToFront != null && nextToFront.tag.CompareTo("Box") == 0      // Нельзя толкать две коробки
            || front != null && front.tag.CompareTo("Box") == 0 && nextToFront != null && nextToFront.tag.CompareTo("Wall") == 0);   // Нельзя толкать коробку в стену
    }

    private Transform TryGetPushedObject(Transform front)
    {
        return (front != null && front.tag.CompareTo("Box") == 0)
            ? front
            : null;
    }

    private Transform GetTransform(Vector2 point)
    {
        RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero);

        return hit.collider != null
            ? hit.transform
            : null;
    }

    private Vector3 AlignPosition(Vector3 pos)
    {
        return new Vector3(
            AlignCoordinate(pos.x),
            AlignCoordinate(pos.y),
            AlignCoordinate(pos.z));
    }

    private float AlignCoordinate(float coord)
    {
        return Mathf.Round(coord * 100f) / 100f; ;
    }
}
