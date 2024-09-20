using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private SpriteRenderer sprite;
    private bool onSelect;
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        
    }

    public void OnHighlight()
    {

    }

    public void OnHoverEnter()
    {
        if(onSelect)return;
        sprite.color = Color.yellow;
    }

    public void OnHoverExit()
    {
        if(onSelect)return;
        sprite.color = Color.white;
    }

    public void OnSelect()
    {
        onSelect = true;
        sprite.color = Color.green;
        StartCoroutine(DestroyGameObject(0.1f));
    }

    public IEnumerator DestroyGameObject(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
