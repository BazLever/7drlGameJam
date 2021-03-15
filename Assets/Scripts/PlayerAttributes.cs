using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceTypes { head, torso, legs };
public enum AttributeTypes { 
    nullAttribute,
    damage, attackSpeed, jumpHeight, movementSpeed,
    wallClimbHeight, numberOfJumps, healOverTime,
    lifeSteal, chanceToBlockDamage, enemiesExplodeOnDeath };

public class PlayerAttributePiece
{
    public PieceTypes type;

    public AttributeTypes attributeType = AttributeTypes.nullAttribute;

    public PlayerAttributePiece(PieceTypes pieceType)
    {
        type = pieceType;
        attributeType = AttributeTypes.nullAttribute;
    }
    public PlayerAttributePiece(PieceTypes pieceType, AttributeTypes attributeType)
    {
        type = pieceType;
        this.attributeType = attributeType;
    }
}

public class PlayerAttributes : MonoBehaviour
{

    public int headPieceLevel = 0;
    public int torsoPieceLevel = 0;
    public int legsPieceLevel = 0;
    public int maxPieceLevel = 10;

    [Space]
    [Header("Referances:")]
    public SkinnedMeshRenderer headRenderer;
    public SkinnedMeshRenderer torsoRenderer;
    public SkinnedMeshRenderer legsRenderer;

    [Space]
    [Header("Base Attribute Upgrade Amount:")]
    public float damage;
    public float attackSpeed;
    public float jumpHeight;
    public float movementSpeed;
    public float wallClimbHeight;
    public float numberOfJumps;
    public float healOverTime;
    public float lifeSteal;
    public float chanceToBlockDamage;
    //public float enemiesExplodeOnDeath;

    [Space]
    [Header("Attribute Piece Materials:")]
    public Material nullHeadPieceMaterial;
    public Material nullTorsoPieceMaterial;
    public Material nullLegsPieceMaterial;
    [Space]
    public Material damageHeadPieceMaterial;
    public Material damageTorsoPieceMaterial;
    public Material damageLegsPieceMaterial;
    [Space]
    public Material attackSpeedHeadPieceMaterial;
    public Material attackSpeedTorsoPieceMaterial;
    public Material attackSpeedLegsPieceMaterial;
    [Space]
    public Material jumpHeightHeadPieceMaterial;
    public Material jumpHeightTorsoPieceMaterial;
    public Material jumpHeightLegsPieceMaterial;
    [Space]
    public Material movementSpeedHeadPieceMaterial;
    public Material movementSpeedTorsoPieceMaterial;
    public Material movementSpeedLegsPieceMaterial;
    [Space]
    public Material wallClimbHeightHeadPieceMaterial;
    public Material wallClimbHeightTorsoPieceMaterial;
    public Material wallClimbHeightLegsPieceMaterial;
    [Space]
    public Material numberOfJumpsHeadPieceMaterial;
    public Material numberOfJumpsTorsoPieceMaterial;
    public Material numberOfJumpsLegsPieceMaterial;
    [Space]
    public Material healOverTimeHeadPieceMaterial;
    public Material healOverTimeTorsoPieceMaterial;
    public Material healOverTimeLegsPieceMaterial;
    [Space]
    public Material lifeStealHeadPieceMaterial;
    public Material lifeStealTorsoPieceMaterial;
    public Material lifeStealLegsPieceMaterial;
    [Space]
    public Material chanceToBlockDamageHeadPieceMaterial;
    public Material chanceToBlockDamageTorsoPieceMaterial;
    public Material chanceToBlockDamageLegsPieceMaterial;
    //[Space]
    //public Material enemiesExplodeOnDeathHeadPieceMaterial;
    //public Material enemiesExplodeOnDeathTorsoPieceMaterial;
    //public Material enemiesExplodeOnDeathLegsPieceMaterial;

    private PlayerAttributePiece headPiece = new PlayerAttributePiece(PieceTypes.head);
    private PlayerAttributePiece torsoPiece = new PlayerAttributePiece(PieceTypes.torso);
    private PlayerAttributePiece legsPiece = new PlayerAttributePiece(PieceTypes.legs);

    /// <summary>
    /// changes the equiped piece to a new one, increases the pieces type level and changes the players materials and death particle materials
    /// </summary>
    public void ChangeAttributePiece(PlayerAttributePiece newAttributePiece)
    {
        switch (newAttributePiece.type)
        {
            case PieceTypes.head:
                if (headPieceLevel < maxPieceLevel)
                    headPieceLevel++;
                headPiece = newAttributePiece;

                // change the players head material
                switch (newAttributePiece.attributeType)
                {
                    case AttributeTypes.damage:
                        headRenderer.material = damageHeadPieceMaterial;
                        break;

                    case AttributeTypes.attackSpeed:
                        headRenderer.material = attackSpeedHeadPieceMaterial;
                        break;

                    case AttributeTypes.jumpHeight:
                        headRenderer.material = jumpHeightHeadPieceMaterial;
                        break;

                    case AttributeTypes.movementSpeed:
                        headRenderer.material = movementSpeedHeadPieceMaterial;
                        break;

                    case AttributeTypes.numberOfJumps:
                        headRenderer.material = numberOfJumpsHeadPieceMaterial;
                        break;

                    case AttributeTypes.healOverTime:
                        headRenderer.material = healOverTimeHeadPieceMaterial;
                        break;

                    case AttributeTypes.wallClimbHeight:
                        headRenderer.material = wallClimbHeightHeadPieceMaterial;
                        break;

                    case AttributeTypes.lifeSteal:
                        headRenderer.material = lifeStealHeadPieceMaterial;
                        break;

                    case AttributeTypes.chanceToBlockDamage:
                        headRenderer.material = chanceToBlockDamageHeadPieceMaterial;
                        break;

                    //case AttributeTypes.enemiesExplodeOnDeath:
                        //headRenderer.material = enemiesExplodeOnDeathHeadPieceMaterial;
                        //break;
                }

                // change the players head death particle effect material
                GetComponent<PlayerController>().headDeathParticle.GetComponent<ParticleSystemRenderer>().material = headRenderer.material;

                break;

            case PieceTypes.torso:
                if (torsoPieceLevel < maxPieceLevel)
                    torsoPieceLevel++;
                torsoPiece = newAttributePiece;

                // change the players torso material
                switch (newAttributePiece.attributeType)
                {
                    case AttributeTypes.damage:
                        torsoRenderer.material = damageTorsoPieceMaterial;
                        break;

                    case AttributeTypes.attackSpeed:
                        torsoRenderer.material = attackSpeedTorsoPieceMaterial;
                        break;

                    case AttributeTypes.jumpHeight:
                        torsoRenderer.material = jumpHeightTorsoPieceMaterial;
                        break;

                    case AttributeTypes.movementSpeed:
                        torsoRenderer.material = movementSpeedTorsoPieceMaterial;
                        break;

                    case AttributeTypes.numberOfJumps:
                        torsoRenderer.material = numberOfJumpsTorsoPieceMaterial;
                        break;

                    case AttributeTypes.healOverTime:
                        torsoRenderer.material = healOverTimeTorsoPieceMaterial;
                        break;

                    case AttributeTypes.wallClimbHeight:
                        torsoRenderer.material = wallClimbHeightTorsoPieceMaterial;
                        break;

                    case AttributeTypes.lifeSteal:
                        torsoRenderer.material = lifeStealTorsoPieceMaterial;
                        break;

                    case AttributeTypes.chanceToBlockDamage:
                        torsoRenderer.material = chanceToBlockDamageTorsoPieceMaterial;
                        break;

                        //case AttributeTypes.enemiesExplodeOnDeath:
                        //torsoRenderer.material = enemiesExplodeOnDeathTorsoPieceMaterial;
                        //break;
                }

                // change the players torso death particle effect material
                GetComponent<PlayerController>().torsoDeathParticle.GetComponent<ParticleSystemRenderer>().material = torsoRenderer.material;

                break;

            case PieceTypes.legs:
                if (legsPieceLevel < maxPieceLevel)
                    legsPieceLevel++;
                legsPiece = newAttributePiece;

                // change the players legs material
                switch (newAttributePiece.attributeType)
                {
                    case AttributeTypes.damage:
                        legsRenderer.material = damageLegsPieceMaterial;
                        break;

                    case AttributeTypes.attackSpeed:
                        legsRenderer.material = attackSpeedLegsPieceMaterial;
                        break;

                    case AttributeTypes.jumpHeight:
                        legsRenderer.material = jumpHeightLegsPieceMaterial;
                        break;

                    case AttributeTypes.movementSpeed:
                        legsRenderer.material = movementSpeedLegsPieceMaterial;
                        break;

                    case AttributeTypes.numberOfJumps:
                        legsRenderer.material = numberOfJumpsLegsPieceMaterial;
                        break;

                    case AttributeTypes.healOverTime:
                        legsRenderer.material = healOverTimeLegsPieceMaterial;
                        break;

                    case AttributeTypes.wallClimbHeight:
                        legsRenderer.material = wallClimbHeightLegsPieceMaterial;
                        break;

                    case AttributeTypes.lifeSteal:
                        legsRenderer.material = lifeStealLegsPieceMaterial;
                        break;

                    case AttributeTypes.chanceToBlockDamage:
                        legsRenderer.material = chanceToBlockDamageLegsPieceMaterial;
                        break;

                        //case AttributeTypes.enemiesExplodeOnDeath:
                        //legsRenderer.material = enemiesExplodeOnDeathLegsPieceMaterial;
                        //break;
                }

                // change the players legs death particle effect material
                GetComponent<PlayerController>().legsDeathParticle.GetComponent<ParticleSystemRenderer>().material = legsRenderer.material;

                break;
        }
    }

    /// <summary>
    /// returns the modifier amount of an attribute type
    /// </summary>
    public float GetAttributeModifier(AttributeTypes attributeType)
    {
        float value = 0;

        // get the head pieces modifier
        if (headPiece.attributeType == attributeType)
        {
            switch (headPiece.attributeType)
            {
                case AttributeTypes.damage:
                    value += damage * headPieceLevel;
                    break;

                case AttributeTypes.attackSpeed:
                    value += attackSpeed * headPieceLevel;
                    break;

                case AttributeTypes.jumpHeight:
                    value += jumpHeight * headPieceLevel;
                    break;

                case AttributeTypes.movementSpeed:
                    value += movementSpeed * headPieceLevel;
                    break;

                case AttributeTypes.wallClimbHeight:
                    value += wallClimbHeight * headPieceLevel;
                    break;

                case AttributeTypes.numberOfJumps:
                    value += numberOfJumps * headPieceLevel;
                    break;

                case AttributeTypes.healOverTime:
                    value += healOverTime * headPieceLevel;
                    break;

                case AttributeTypes.lifeSteal:
                    value += lifeSteal * headPieceLevel;
                    break;

                case AttributeTypes.chanceToBlockDamage:
                    value += chanceToBlockDamage * headPieceLevel;
                    break;

                    // this one just doesn't make sense to work with the upgrade system currently
                    //case AttributeTypes.enemiesExplodeOnDeath:
                    //value += enemiesExplodeOnDeath * headPieceLevel;
                    //break;
            }
        }

        // get the torso pieces modifier
        if (torsoPiece.attributeType == attributeType)
        {
            switch (torsoPiece.attributeType)
            {
                case AttributeTypes.damage:
                    value += damage * torsoPieceLevel;
                    break;

                case AttributeTypes.attackSpeed:
                    value += attackSpeed * torsoPieceLevel;
                    break;

                case AttributeTypes.jumpHeight:
                    value += jumpHeight * torsoPieceLevel;
                    break;

                case AttributeTypes.movementSpeed:
                    value += movementSpeed * torsoPieceLevel;
                    break;

                case AttributeTypes.wallClimbHeight:
                    value += wallClimbHeight * torsoPieceLevel;
                    break;

                case AttributeTypes.numberOfJumps:
                    value += numberOfJumps * torsoPieceLevel;
                    break;

                case AttributeTypes.healOverTime:
                    value += healOverTime * torsoPieceLevel;
                    break;

                case AttributeTypes.lifeSteal:
                    value += lifeSteal * torsoPieceLevel;
                    break;

                case AttributeTypes.chanceToBlockDamage:
                    value += chanceToBlockDamage * torsoPieceLevel;
                    break;

                    // this one just doesn't make sense to work with the upgrade system currently
                    //case AttributeTypes.enemiesExplodeOnDeath:
                    //value += enemiesExplodeOnDeath * torsoPieceLevel;
                    //break;
            }
        }

        // get the legs pieces modifier
        if (legsPiece.attributeType == attributeType)
        {
            switch (legsPiece.attributeType)
            {
                case AttributeTypes.damage:
                    value += damage * legsPieceLevel;
                    break;

                case AttributeTypes.attackSpeed:
                    value += attackSpeed * legsPieceLevel;
                    break;

                case AttributeTypes.jumpHeight:
                    value += jumpHeight * legsPieceLevel;
                    break;

                case AttributeTypes.movementSpeed:
                    value += movementSpeed * legsPieceLevel;
                    break;

                case AttributeTypes.wallClimbHeight:
                    value += wallClimbHeight * legsPieceLevel;
                    break;

                case AttributeTypes.numberOfJumps:
                    value += numberOfJumps * legsPieceLevel;
                    break;

                case AttributeTypes.healOverTime:
                    value += healOverTime * legsPieceLevel;
                    break;

                case AttributeTypes.lifeSteal:
                    value += lifeSteal * legsPieceLevel;
                    break;

                case AttributeTypes.chanceToBlockDamage:
                    value += chanceToBlockDamage * legsPieceLevel;
                    break;

                    // this one just doesn't make sense to work with the upgrade system currently
                    //case AttributeTypes.enemiesExplodeOnDeath:
                    //value += enemiesExplodeOnDeath * legsPieceLevel;
                    //break;
            }
        }

        return value;
    }
}
