using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceSelectionUI : MonoBehaviour
{
    [Header("Referances:")]
    public Text headDescriptionText;
    public Text torsoDescriptionText;
    public Text legsDescriptionText;
    public Image headImage;
    public Image torsoImage;
    public Image legsImage;

    [Space]
    [Header("Piece Images:")]
    public Sprite damageHeadPieceImage;
    public Sprite damageTorsoPieceImage;
    public Sprite damageLegsPieceImage;
    [Space]
    public Sprite attackSpeedHeadPieceImage;
    public Sprite attackSpeedTorsoPieceImage;
    public Sprite attackSpeedLegsPieceImage;
    [Space]
    public Sprite jumpHeightHeadPieceImage;
    public Sprite jumpHeightTorsoPieceImage;
    public Sprite jumpHeightLegsPieceImage;
    [Space]
    public Sprite movementSpeedHeadPieceImage;
    public Sprite movementSpeedTorsoPieceImage;
    public Sprite movementSpeedLegsPieceImage;
    [Space]
    public Sprite wallClimbHeightHeadPieceImage;
    public Sprite wallClimbHeightTorsoPieceImage;
    public Sprite wallClimbHeightLegsPieceImage;
    [Space]
    public Sprite numberOfJumpsHeadPieceImage;
    public Sprite numberOfJumpsTorsoPieceImage;
    public Sprite numberOfJumpsLegsPieceImage;
    [Space]
    public Sprite healOverTimeHeadPieceImage;
    public Sprite healOverTimeTorsoPieceImage;
    public Sprite healOverTimeLegsPieceImage;
    [Space]
    public Sprite lifeStealHeadPieceImage;
    public Sprite lifeStealTorsoPieceImage;
    public Sprite lifeStealLegsPieceImage;
    [Space]
    public Sprite chanceToBlockDamageHeadPieceImage;
    public Sprite chanceToBlockDamageTorsoPieceImage;
    public Sprite chanceToBlockDamageLegsPieceImage;
    //[Space]
    //public Sprite enemiesExplodeOnDeathHeadPieceImage;
    //public Sprite enemiesExplodeOnDeathTorsoPieceImage;
    //public Sprite enemiesExplodeOnDeathLegsPieceImage;

    private bool gamePaused = false;

    private AttributeTypes attributeType = AttributeTypes.nullAttribute;

    private PlayerAttributes playerAttributes;
    private PlayerController playerController;
    private Animator partSelectAnim;

    // Start is called before the first frame update
    void Start()
    {
        playerAttributes = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttributes>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        partSelectAnim = GetComponent<Animator>();

        partSelectAnim.SetBool("Open", false);
    }


    public void HeadPieceSelectButton()
    {
        // do nothing if the game is paused or the player is dead
        if (gamePaused || playerController.isDead)
            return;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // continue to let the player be able to look around
        playerController.canMoveCamera = true;

        playerAttributes.ChangeAttributePiece(new PlayerAttributePiece(PieceTypes.head, attributeType));
        partSelectAnim.SetBool("Open", false);
    }
    public void TorsoPieceSelectButton()
    {
        // do nothing if the game is paused or the player is dead
        if (gamePaused || playerController.isDead)
            return;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // continue to let the player be able to look around
        playerController.canMoveCamera = true;

        playerAttributes.ChangeAttributePiece(new PlayerAttributePiece(PieceTypes.torso, attributeType));
        partSelectAnim.SetBool("Open", false);
    }
    public void LegsPieceSelectButton()
    {
        // do nothing if the game is paused or the player is dead
        if (gamePaused || playerController.isDead)
            return;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // continue to let the player be able to look around
        playerController.canMoveCamera = true;

        playerAttributes.ChangeAttributePiece(new PlayerAttributePiece(PieceTypes.legs, attributeType));
        partSelectAnim.SetBool("Open", false);
    }

    public void OpenPartSelection(string characterName, AttributeTypes attributeType)
    {
        this.attributeType = attributeType;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // stop the player from being able to look around
        playerController.canMoveCamera = false;

        // set the description text and image
        switch (attributeType)
        {
            case AttributeTypes.damage:
                headImage.sprite = damageHeadPieceImage;
                torsoImage.sprite = damageTorsoPieceImage;
                legsImage.sprite = damageLegsPieceImage;

                headDescriptionText.text = characterName + ", LVL: " + playerAttributes.headPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Damage";
                torsoDescriptionText.text = characterName + ", LVL: " + playerAttributes.torsoPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Damage";
                legsDescriptionText.text = characterName + ", LVL: " + playerAttributes.legsPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Damage";
                break;

            case AttributeTypes.attackSpeed:
                headImage.sprite = attackSpeedHeadPieceImage;
                torsoImage.sprite = attackSpeedTorsoPieceImage;
                legsImage.sprite = attackSpeedLegsPieceImage;

                headDescriptionText.text = characterName + ", LVL: " + playerAttributes.headPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Attack Speed";
                torsoDescriptionText.text = characterName + ", LVL: " + playerAttributes.torsoPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Attack Speed";
                legsDescriptionText.text = characterName + ", LVL: " + playerAttributes.legsPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Attack Speed";
                break;

            case AttributeTypes.jumpHeight:
                headImage.sprite = jumpHeightHeadPieceImage;
                torsoImage.sprite = jumpHeightTorsoPieceImage;
                legsImage.sprite = jumpHeightLegsPieceImage;

                headDescriptionText.text = characterName + ", LVL: " + playerAttributes.headPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Jump Height";
                torsoDescriptionText.text = characterName + ", LVL: " + playerAttributes.torsoPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Jump Height";
                legsDescriptionText.text = characterName + ", LVL: " + playerAttributes.legsPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Jump Height";
                break;

            case AttributeTypes.movementSpeed:
                headImage.sprite = movementSpeedHeadPieceImage;
                torsoImage.sprite = movementSpeedTorsoPieceImage;
                legsImage.sprite = movementSpeedLegsPieceImage;

                headDescriptionText.text = characterName + ", LVL: " + playerAttributes.headPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Movement Speed";
                torsoDescriptionText.text = characterName + ", LVL: " + playerAttributes.torsoPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Movement Speed";
                legsDescriptionText.text = characterName + ", LVL: " + playerAttributes.legsPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Movement Speed";
                break;

            case AttributeTypes.numberOfJumps:
                headImage.sprite = numberOfJumpsHeadPieceImage;
                torsoImage.sprite = numberOfJumpsTorsoPieceImage;
                legsImage.sprite = numberOfJumpsLegsPieceImage;

                headDescriptionText.text = characterName + ", LVL: " + playerAttributes.headPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Number Of Jumps";
                torsoDescriptionText.text = characterName + ", LVL: " + playerAttributes.torsoPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Number Of Jumps";
                legsDescriptionText.text = characterName + ", LVL: " + playerAttributes.legsPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Number Of Jumps";
                break;

            case AttributeTypes.healOverTime:
                headImage.sprite = healOverTimeHeadPieceImage;
                torsoImage.sprite = healOverTimeTorsoPieceImage;
                legsImage.sprite = healOverTimeLegsPieceImage;

                headDescriptionText.text = characterName + ", LVL: " + playerAttributes.headPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Heal Over Time";
                torsoDescriptionText.text = characterName + ", LVL: " + playerAttributes.torsoPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Heal Over Time";
                legsDescriptionText.text = characterName + ", LVL: " + playerAttributes.legsPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Heal Over Time";
                break;

            case AttributeTypes.wallClimbHeight:
                headImage.sprite = wallClimbHeightHeadPieceImage;
                torsoImage.sprite = wallClimbHeightTorsoPieceImage;
                legsImage.sprite = wallClimbHeightLegsPieceImage;

                headDescriptionText.text = characterName + ", LVL: " + playerAttributes.headPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Wall Climb Height";
                torsoDescriptionText.text = characterName + ", LVL: " + playerAttributes.torsoPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Wall Climb Height";
                legsDescriptionText.text = characterName + ", LVL: " + playerAttributes.legsPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Wall Climb Height";
                break;

            case AttributeTypes.lifeSteal:
                headImage.sprite = lifeStealHeadPieceImage;
                torsoImage.sprite = lifeStealTorsoPieceImage;
                legsImage.sprite = lifeStealLegsPieceImage;

                headDescriptionText.text = characterName + ", LVL: " + playerAttributes.headPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Life Steal";
                torsoDescriptionText.text = characterName + ", LVL: " + playerAttributes.torsoPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Life Steal";
                legsDescriptionText.text = characterName + ", LVL: " + playerAttributes.legsPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Life Steal";
                break;

            case AttributeTypes.chanceToBlockDamage:
                headImage.sprite = chanceToBlockDamageHeadPieceImage;
                torsoImage.sprite = chanceToBlockDamageTorsoPieceImage;
                legsImage.sprite = chanceToBlockDamageLegsPieceImage;

                headDescriptionText.text = characterName + ", LVL: " + playerAttributes.headPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Chance To Block Damage";
                torsoDescriptionText.text = characterName + ", LVL: " + playerAttributes.torsoPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Chance To Block Damage";
                legsDescriptionText.text = characterName + ", LVL: " + playerAttributes.legsPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Chance To Block Damage";
                break;

            //case AttributeTypes.enemiesExplodeOnDeath:
                //headImage.sprite = enemiesExplodeOnDeathHeadPieceImage;
                //torsoImage.sprite = enemiesExplodeOnDeathTorsoPieceImage;
                //legsImage.sprite = enemiesExplodeOnDeathLegsPieceImage;
                //
                //headDescriptionText.text = characterName + ", LVL: " + playerAttributes.headPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Enemies Explode On Death";
                //torsoDescriptionText.text = characterName + ", LVL: " + playerAttributes.torsoPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Enemies Explode On Death";
                //legsDescriptionText.text = characterName + ", LVL: " + playerAttributes.legsPieceLevel + "/" + playerAttributes.maxPieceLevel + "\n + Enemies Explode On Death";
                //break;
        }

        partSelectAnim.SetBool("Open", true);
    }

    /// <summary>
    /// sets if the game is paused for this object
    /// </summary>
    public void SetGamePaused(bool pauseState)
    {
        gamePaused = pauseState;
    }

    /// <summary>
    /// returns true if the menu is open
    /// </summary>
    public bool IsOpen()
    {
        return partSelectAnim.GetBool("Open");
    }
}
