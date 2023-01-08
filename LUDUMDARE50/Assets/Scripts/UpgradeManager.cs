using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public GameLogic gameLogic;
    public TMP_Text pointText;
    
    public Upgrade[] upgrades;
    
    int points;

    [System.Serializable]
    public struct Upgrade
    {
        public string name;
        public string description;
        public int cost;
        public GameObject container;
        public bool isBought;
    }

    private void Start()
    {
        Instance = this;
        points = gameLogic.GetPoints();
    }

    void Update()
    {
        points = gameLogic.GetPoints();
        pointText.text = "Points: " + points;

        foreach (Upgrade upgrade in upgrades)
        {
            if (upgrade.container.activeSelf)
            {
                if (upgrade.isBought)
                {
                    upgrade.container.GetComponentInChildren<TMP_Text>().text = upgrade.name + "\nYou already have this";
                    upgrade.container.GetComponent<Button>().interactable = false;
                }
                else
                {
                    if (points < upgrade.cost)
                    {
                        upgrade.container.GetComponent<Button>().interactable = false;
                    }
                    else
                        upgrade.container.GetComponent<Button>().interactable = true;

                    upgrade.container.GetComponentInChildren<TMP_Text>().text = upgrade.name + "\nCosts " + upgrade.cost + " points";
                }
            }
        }
    }
}
