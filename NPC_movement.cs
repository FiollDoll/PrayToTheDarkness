using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_movement : MonoBehaviour
{
    public string totalLocation;
    [SerializeField] private float speed;
    [SerializeField] private AllScripts scripts;
    private bool _playerInCollider;
    private Transform _playerTransform;
    private SpriteRenderer _sr;
    private Animator _animator;

    [Header("Move to player")] public bool moveToPlayer;

    [Header("Move to point")] public bool moveToPoint;
    public bool waitPlayerAndMove;
    public Transform point;
    public string locationOfPointName;

    private void Start()
    {
        _playerTransform = GameObject.Find("Player").transform;
        _sr = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.gameObject.name)
        {
            case "floorChange" when totalLocation != locationOfPointName && moveToPoint:
                transform.position = scripts.locations.GetLocation(locationOfPointName).spawns[0].spawn.position;
                totalLocation = locationOfPointName;
                break;
            case "Player":
                _playerInCollider = true;
                break;
            default:
            {
                if (point != null & other.gameObject.name == point.gameObject.name)
                    moveToPoint = false;
                break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
            _playerInCollider = false;
    }

    private void MoveTo(Transform target) => scripts.main.MoveTo(target, speed, transform, _sr, _animator);

    private void FixedUpdate()
    {
        if (!_playerInCollider && moveToPlayer)
            MoveTo(_playerTransform);
        else if (moveToPoint)
        {
            if (!waitPlayerAndMove || (waitPlayerAndMove && _playerInCollider))
            {
                if (locationOfPointName == totalLocation)
                    MoveTo(point);
                else
                    MoveTo(scripts.locations.GetLocation(totalLocation).transformOfStairs);
            }
            else
                GetComponent<Animator>().SetBool("walk", false);
        }
        else
            GetComponent<Animator>().SetBool("walk", false);
    }
}