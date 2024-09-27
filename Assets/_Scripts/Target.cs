using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private bool isResetTarget;
    [SerializeField] private Color hoverColor = Color.yellow;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color selectColor = Color.green;
    [SerializeField] private Color targetHighlightColor = Color.red;
    private SpriteRenderer sprite;
    private TargetManager targetManager;
    private bool onSelect;
    private float timer = 0f;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        targetManager = FindObjectOfType<TargetManager>();
    }
    void Update()
    {
        timer += Time.deltaTime;
    }

    public void Highlight()
    {
        defaultColor = targetHighlightColor;
        sprite.color = defaultColor;
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
        StartCoroutine(SpawnNextTarget(0.1f));
        targetManager.SpawnNextTarget(isResetTarget);
    }



    public IEnumerator SpawnNextTarget(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
