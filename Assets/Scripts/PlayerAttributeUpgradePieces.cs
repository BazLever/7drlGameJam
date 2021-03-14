using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributeUpgradePieces : MonoBehaviour
{
    public float playerInteractDistance = 3.5f;

    [Space]
    [Header("Referances:")]
    public SkinnedMeshRenderer headRenderer;
    public SkinnedMeshRenderer torsoRenderer;
    public SkinnedMeshRenderer legsRenderer;

    [Space]
    public AttributeTypes attributeType = AttributeTypes.nullAttribute;

    [Space]
    [Header("Attribute Types Character Names:")]
    public string damageCharacterName = "null";
    public string attackSpeedCharacterName = "null";
    public string jumpHeightCharacterName = "null";
    public string movementSpeedCharacterName = "null";
    public string numberOfJumpsCharacterName = "null";
    public string healOverTimeCharacterName = "null";
    public string wallClimbHeightCharacterName = "null";
    public string lifeStealCharacterName = "null";
    public string chanceToBlockDamageCharacterName = "null";
    //public string enemiesExplodeOnDeathCharacterName;

    private PlayerController playerController;
    private PieceSelectionUI pieceSelection;

    private void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        pieceSelection = GameObject.FindGameObjectWithTag("PieceSelection").GetComponent<PieceSelectionUI>();
    }

    void Update()
    {
        if (Input.GetKeyDown(playerController.interactKey) && !playerController.isDead && (playerController.transform.position - transform.position).sqrMagnitude <= (playerInteractDistance * playerInteractDistance))
        {
            switch (attributeType)
            {
                case AttributeTypes.damage:
                    pieceSelection.OpenPartSelection(damageCharacterName, attributeType);
                    break;

                case AttributeTypes.attackSpeed:
                    pieceSelection.OpenPartSelection(attackSpeedCharacterName, attributeType);
                    break;

                case AttributeTypes.jumpHeight:
                    pieceSelection.OpenPartSelection(jumpHeightCharacterName, attributeType);
                    break;

                case AttributeTypes.movementSpeed:
                    pieceSelection.OpenPartSelection(movementSpeedCharacterName, attributeType);
                    break;

                case AttributeTypes.numberOfJumps:
                    pieceSelection.OpenPartSelection(numberOfJumpsCharacterName, attributeType);
                    break;

                case AttributeTypes.healOverTime:
                    pieceSelection.OpenPartSelection(healOverTimeCharacterName, attributeType);
                    break;

                case AttributeTypes.wallClimbHeight:
                    pieceSelection.OpenPartSelection(wallClimbHeightCharacterName, attributeType);
                    break;

                case AttributeTypes.lifeSteal:
                    pieceSelection.OpenPartSelection(lifeStealCharacterName, attributeType);
                    break;

                case AttributeTypes.chanceToBlockDamage:
                    pieceSelection.OpenPartSelection(chanceToBlockDamageCharacterName, attributeType);
                    break;

                //case AttributeTypes.enemiesExplodeOnDeath:
                    //pieceSelection.OpenPartSelection(enemiesExplodeOnDeathCharacterName, attributeType);
                    //break;
            }

            Destroy(gameObject);
        }
    }
}
