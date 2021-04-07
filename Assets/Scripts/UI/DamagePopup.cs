using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private static int SortingOrder = 0;
    private const float DISAPPEAR_TIMER_MAX = 1f;
    private const float DISAPPEAR_SPEED = 3f;

    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;


    public static DamagePopup Create(Vector3 position, float damage)
    {
        GameObject damagePopupGO =  Instantiate(GameManager.Instance.damagePopupPrefab, position, Quaternion.identity);
        DamagePopup damagePopup = damagePopupGO.GetComponent<DamagePopup>();
        damagePopup.Setup(damage);
        return damagePopup;
    }

    // Start is called before the first frame update
    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

 
    public void Setup(float damage)
    {
        textMesh.SetText(damage.ToString());
        textColor = textMesh.color;
        disappearTimer = DISAPPEAR_TIMER_MAX;

        // TODO: enlever valeurs random
        moveVector = new Vector3(.7f, 1) * 8f;

        SortingOrder++;
        textMesh.sortingOrder = SortingOrder;
    }

    private void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        
        // TODO: enlever valeur random
        moveVector -= moveVector * 5f * Time.deltaTime;
            
        //first half
        if (disappearTimer > DISAPPEAR_TIMER_MAX * 0.5f)
        {
            float increaseScaleAmount = 0.5f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;

        } 

        //second half
        else
        {
            float decreaseScaleAmount = 1.5f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }

        disappearTimer -= Time.deltaTime;
        
        if (disappearTimer < 0)
        {
            //start fading
            textColor.a -= DISAPPEAR_SPEED * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }


}
