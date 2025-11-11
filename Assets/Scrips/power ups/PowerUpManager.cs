using System.Data.SqlTypes;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class PowerUpManager : MonoBehaviour

{
    public PlayerPhysicsController pc;
    public DisplayData dd;
    private WeaponDemo dc;


    private bool perla;
    private bool miche;
    private bool wisky;
    private bool tequila;
    private bool absinthe;
    private bool mezcal;
    private bool pulque;
    private bool wine;

    private string cws;
    private float timer = 0f;
    private float intervalo = 2f;


    void Start()
    {
        

        perla = false; 
        miche = false; 
        wisky = false; 
        tequila = false;  
        absinthe = false;
        mezcal = false;
        pulque = false;
        wine = false;


    }

    // Update is called once per frame
    void Update()
    {

        timer += Time.deltaTime;

        if (timer >= intervalo)
        {
            timer = 0f; // reinicia el contador

            if (pulque)
            {
                pc.MasVida(20);
                dd.ShowVidaPickup(1);
            }
        }
    }
    public void NewWeapon(GameObject cw)
    {
        dc= cw.GetComponent<WeaponDemo>();
        perla = false;
        absinthe= false;
        tequila= false;
        mezcal= false;
        cws = dc.categoryName;
        dd.ResetVida();

    }

    public void ActivePower(int c)
    {
        switch (c)
        {
            case 1: Perla();break;
            case 2: Miche(); break;
            case 3: Wisky(); break;
            case 4: Tequila(); break;
            case 5: Absinthe(); break;
            case 6: Mezcal(); break;
            case 7: Pulque(); break;
            case 8: Wine(); break;
        }
    }

    private void Perla()
    {
        perla = true;
        dc.ChangeDmage(3);
        dd.MultiplicarVida(3,cws);
        Invoke(nameof(UnPerla), 30f);

    }
    private void Miche()
    {
        miche = true;
        pc.CompletarVida();
        dd.ShowVidaPickup(100);
        Invoke(nameof(UnMiche), 3f);
    }
    private void Wisky()
    {
        wisky = true;
        pc.Proteger(0);
        dd.SetProtectionAlpha(1f);
        Invoke(nameof(UnWisky), 15f);

    }
    private void Tequila()
    {
        tequila = true;
        dc.ChangeDmage(50);
        dd.MultiplicarVida(50, cws);
        Invoke(nameof(UnTequila), 5f);
    }
    private void Absinthe()
    {
        absinthe = true;
        dc.ChangeDmage(2);
        dd.MultiplicarVida(2, cws);
        Invoke(nameof(UnAbsinthe), 60f);

    }
    private void Mezcal()
    {
        mezcal = true;
        dc.ChangeDmage(10);
        dd.MultiplicarVida(10, cws);
        Invoke(nameof(UnMezcal), 15f);
    }
    private void Pulque()
    {
        pulque = true;
        Invoke(nameof(UnPulque), 30f);
    }
    private void Wine()
    {
        wine = true;
        pc.Proteger(0.5f);
        if(!wisky)dd.SetProtectionAlpha(0.5f);
        Invoke(nameof(UnWine), 30f);
    }
    private void UnPerla()
    {
        if (perla)
        {
            perla = false;
            dd.DividarVida(3);
            dc.RestaurarDamge(3);
        }
        

    }
    private void UnMiche()
    {
        if (miche)
        {
            if(wine) dd.SetProtectionAlpha(0.5f);
            miche = false;
        }
    }
    private void UnWisky()
    {
        if (wisky)
        {
            wisky = false;
            if(wine)dd.SetProtectionAlpha(0.5f);
            else dd.SetProtectionAlpha(0f);
            pc.DesProteger(wisky, wine);
        }

    }
    private void UnTequila()
    {
        if (tequila)
        {
            tequila = false;
            dd.DividarVida(50);
            dc.RestaurarDamge(50);
        }

    }
    private void UnAbsinthe()
    {
        if (absinthe)
        {
            absinthe = false;
            dd.DividarVida(2);
            dc.RestaurarDamge(2);
        }
    }
    private void UnMezcal()
    {
        if (mezcal)
        {
            mezcal = false;
            dd.DividarVida(10);
            dc.RestaurarDamge(10);
        }
    }
    private void UnPulque()
    {
        if (pulque)
        {
            pulque = false;
        }
    }
    private void UnWine()
    {
        if (wine)
        {
            wine = false;
            if(!wisky) dd.SetProtectionAlpha(0f);
            pc.DesProteger(wisky, wine);
        }
    }
   
}
