using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public bool IsIsMainTarget => isMainTarget;
    [SerializeField] private bool isResetTarget;
    [SerializeField] private Color hoverColor = Color.yellow;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color selectColor = Color.green;
    [SerializeField] private Color targetHighlightColor = Color.red;
    private SpriteRenderer sprite;
    private TargetManager targetManager;
    private StudyBehavior studyBehavior;
    private bool onSelect;
    private float timer = 0f;
    private bool isMainTarget = false;
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        targetManager = FindObjectOfType<TargetManager>();
        studyBehavior = FindObjectOfType<StudyBehavior>();
    }
    void Update()
    {
        timer += Time.deltaTime;
    }

    public void SetMainTarget()
    {
        defaultColor = targetHighlightColor;
        sprite.color = defaultColor;
        isMainTarget = true;
    }

    public void OnHoverEnter()
    {
        if(onSelect)return;
        sprite.color = hoverColor;
    }

    public void OnHoverExit()
    {
        if(onSelect)return;
        sprite.color = defaultColor;
    }

    public void OnSelect()
    {
        onSelect = true;
        sprite.color = selectColor;

        if (isMainTarget || !isResetTarget)
        {
            studyBehavior.NextTrial();
            targetManager.SpawnNextTarget(isResetTarget);
        }
        else //If a distractor target is selected
        {
            sprite.color = defaultColor;
            studyBehavior.HandleMisClick();
        }

    }
}
