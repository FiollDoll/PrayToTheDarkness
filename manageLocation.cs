using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;
public class manageLocation : MonoBehaviour
{
    public location totalLocation;
    public location[] locations = new location[0];
    [SerializeField] private Image noViewPanel;
    [SerializeField] private allScripts scripts;
    private GameObject player;

    private void Start()
    {
        totalLocation = GetLocation("mainMark");
        player = GameObject.Find("Player");
    }

    public void ActivateLocation(string name, string spawn, bool withFade = true)
    {
        void locationSetup(location location)
        {
            player.transform.position = location.GetSpawn(spawn).position;
            scripts.player.virtualCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = location.wallsForCamera as PolygonCollider2D;
            scripts.player.canMove = false;
            scripts.player.virtualCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = scripts.main.startCameraSize + location.modifCamera;

            NPC_movement[] NPCs = GameObject.FindObjectsOfType<NPC_movement>();
            foreach (NPC_movement totalNPC in NPCs)
            {
                if (totalNPC.moveToPlayer)
                    totalNPC.gameObject.transform.position = location.GetSpawn(spawn).position;
            }
        }

        totalLocation = GetLocation(name);
        if (withFade)
        {
            Sequence sequence = DOTween.Sequence();
            Tween fadeAnimation = noViewPanel.DOFade(100f, 0.2f).SetEase(Ease.InQuart);
            fadeAnimation.OnComplete(() =>
            {
                locationSetup(totalLocation);
            });
            sequence.Append(fadeAnimation);
            sequence.Append(noViewPanel.DOFade(0f, 0.5f).SetEase(Ease.OutQuart));
            sequence.Insert(0, transform.DOScale(new Vector3(1, 1, 1), sequence.Duration()));
            sequence.OnComplete(() => { scripts.player.canMove = true; });
        }
        else
        {
            locationSetup(totalLocation);
            scripts.player.canMove = true;
        }
    }

    public void FastMoveToLocation(string name)
    {
        ActivateLocation(name, "fromStairs"); // Для кнопок лестницы
        scripts.interactions.floorChangeMenu.gameObject.SetActive(false);
    }

    public void SetLockToLocation(string nameLocation, bool lockLocation) => GetLocation(nameLocation).locked = lockLocation;


    public location GetLocation(string name)
    {
        foreach (location location in locations)
        {
            if (location.gameName == name)
                return location;
        }
        return null;
    }
}


[System.Serializable]
public class location
{
    [System.Serializable]
    public class spawnInLocation
    {
        public string name;
        public Transform spawn;
    }

    public string gameName;
    [HideInInspector]
    public string name
    {
        get
        {
            if (PlayerPrefs.GetString("language") == "ru")
                return ruName;
            else
                return enName;
        }
    }
    public string ruName, enName;
    public bool locked, autoEnter;
    public Collider2D wallsForCamera;
    public float modifCamera;
    public spawnInLocation[] spawns = new spawnInLocation[0];
    public Transform transformOfStairs = null; // Если есть лестница
    public Transform GetSpawn(string name)
    {
        foreach (spawnInLocation spawn in spawns)
        {
            if (spawn.name == name)
            {
                return spawn.spawn;
            }
        }
        return null;
    }
}