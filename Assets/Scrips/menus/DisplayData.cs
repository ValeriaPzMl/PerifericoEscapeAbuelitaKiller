using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayData : MonoBehaviour
{
    [Header("Trackers")]
    public PositionManager tracker;
    public PlayerPhysicsController vidaGeter;

    [Header("UI Texts")]
    public TextMeshProUGUI differenceText;
    public TextMeshProUGUI vida;
    public TextMeshProUGUI carrosM;

    [Header("Power UI Elements")]
    public TextMeshProUGUI powerVidaText;   // Texto que muestra cuánto se curó (aparece 1s)
    public TextMeshProUGUI powerDanoText;   // Texto para multiplicador de daño (ej: "x3")

    [Header("UI - sprites")]
    public Image projectileSpriteImage;     // sprite que representa el proyectil (mapa por nombre)
    public Image lifeSpriteImage;           // sprite para power de vida (se muestra brevemente)
    public Image protectSpriteImage;

    [Header("Projectile Sprite Mapping (Inspector)")]
    public List<string> projectileNames;   // lista de nombres (arrastrar/teclear)
    public List<Sprite> projectileSprites; // misma size que names, arrastrar sprites

    private Dictionary<string, Sprite> projectileMap;
    [HideInInspector]public int carrosMuertos = 0;
    private Coroutine vidaCoroutine;
    private int multi;

    private void Awake()
    {
        BuildProjectileMap();
    }

    private void Start()
    {
        // ocultar UI al inicio
        if (powerDanoText) powerDanoText.gameObject.SetActive(false);
        if (powerVidaText) { powerVidaText.gameObject.SetActive(false); powerVidaText.text = ""; }
        if (projectileSpriteImage) projectileSpriteImage.gameObject.SetActive(false);
        if (lifeSpriteImage) lifeSpriteImage.gameObject.SetActive(false);
        if (protectSpriteImage) protectSpriteImage.gameObject.SetActive(false);
    }


    private void Update()
    {
        // Actualizar textos principales
        if (differenceText && tracker != null)
            differenceText.text = tracker.differenceX.ToString("F2") + "Km";

        if (vida && vidaGeter != null)
            vida.text = vidaGeter.getLife().ToString();

        if (carrosM != null)
            carrosM.text = carrosMuertos.ToString();
    }

    private void BuildProjectileMap()
    {
        projectileMap = new Dictionary<string, Sprite>();
        int count = Mathf.Min(projectileNames.Count, projectileSprites.Count);
        for (int i = 0; i < count; i++)
        {
            var name = projectileNames[i];
            var sp = projectileSprites[i];
            if (!string.IsNullOrEmpty(name) && sp != null)
            {
                projectileMap[name] = sp;
            }
        }

        if (projectileNames.Count != projectileSprites.Count)
            Debug.LogWarning("[DisplayData] projectileNames y projectileSprites no tienen la misma longitud. Se mapearon " + count + " elementos.");
    }

    // devuelve sprite por nombre o null si no existe
    public Sprite GetProjectileSprite(string projectileName)
    {
        if (string.IsNullOrEmpty(projectileName)) return null;
        if (projectileMap == null) BuildProjectileMap();
        projectileMap.TryGetValue(projectileName, out Sprite s);
        return s;
    }

    // --- MULTIPLICADOR (vida/daño según tu uso) ---
    // Sobrecarga: deja como antes si solo pasas float
    public void MultiplicarVida(float vida)
    {
        MultiplicarVida(vida, null);
    }

    // version nueva: permite pasar el nombre del proyectil para mostrar su sprite
    public void MultiplicarVida(float vida, string projectileName)
    {
        int v = Mathf.Max(1, (int)vida); // proteger contra 0 o negativos
        // actualizar multi (multiplicador entero)
        long temp = (long)multi * v; // usar long para evitar overflow momentáneo
        if (temp > int.MaxValue) temp = int.MaxValue;
        multi = (int)temp;

        // mostrar texto multiplicador
        if (powerDanoText != null)
        {
            powerDanoText.text = $"x{multi}";
            powerDanoText.gameObject.SetActive(true);
        }

        // mostrar sprite de proyectil si existen datos
        if (projectileSpriteImage != null)
        {
            Sprite s = GetProjectileSprite(projectileName);
            if (s != null)
            {
                projectileSpriteImage.sprite = s;
                SetImageAlpha(projectileSpriteImage, 1f);
                projectileSpriteImage.gameObject.SetActive(true);
            }
            else
            {
                // si no se pasó nombre o no existe, dejamos el sprite anterior si había; si no, lo ocultamos
                if (projectileSpriteImage.sprite == null) projectileSpriteImage.gameObject.SetActive(false);
            }
        }
    }

    // --- DIVIDIR ---
    public void DividarVida(float vida)
    {
        DividarVida(vida, true);
    }

    // allow optional hideSpriteWhenOne to control behavior
    public void DividarVida(float vida, bool hideSpriteWhenOne)
    {
        int v = Mathf.Max(1, (int)vida); // evitar div por 0
        // dividir (entero)
        if (v != 0)
        {
            multi = Mathf.Max(1, multi / v); // nunca bajar de 1
        }

        // si multi == 1 ocultamos todo lo relacionado al multiplicador
        if (multi <= 1)
        {
            multi = 1;
            if (powerDanoText) powerDanoText.gameObject.SetActive(false);
            if (projectileSpriteImage != null && hideSpriteWhenOne)
                projectileSpriteImage.gameObject.SetActive(false);
        }
        else
        {
            if (powerDanoText != null)
            {
                powerDanoText.text = $"x{multi}";
                powerDanoText.gameObject.SetActive(true);
            }
        }
    }

    public void ResetVida()
    {
        multi = 1;
        if (powerDanoText) powerDanoText.gameObject.SetActive(false);
        if (projectileSpriteImage) projectileSpriteImage.gameObject.SetActive(false);
    }

    // --- VIDA: mostrar sprite y texto por 1 segundo ---
    // amount: cantidad curada (entera)
    public void ShowVidaPickup(int amount)
    {
        if (vidaCoroutine != null) StopCoroutine(vidaCoroutine);
        vidaCoroutine = StartCoroutine(VidaCoroutine(amount));
    }

    private IEnumerator VidaCoroutine(int amount)
    {
        if (lifeSpriteImage) lifeSpriteImage.gameObject.SetActive(true);
        if (powerVidaText)
        {
            if(amount==100)
            powerVidaText.text = $"{amount}%";
            if(amount==1)
            powerVidaText.text = $"+{amount}";
            powerVidaText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(1f);

        if (lifeSpriteImage) lifeSpriteImage.gameObject.SetActive(false);
        if (powerVidaText) powerVidaText.gameObject.SetActive(false);

        vidaCoroutine = null;
    }

    // --- PROTECCIÓN: ajustar alpha (0..1). Si alpha == 0 se oculta ---
    public void SetProtectionAlpha(float alpha)
    {
        alpha = Mathf.Clamp01(alpha);
        if (protectSpriteImage == null) return;

        if (alpha <= 0f)
        {
            protectSpriteImage.gameObject.SetActive(false);
        }
        else
        {
            protectSpriteImage.gameObject.SetActive(true);
            SetImageAlpha(protectSpriteImage, alpha);
        }
    }

    // helper para setear alpha
    private void SetImageAlpha(Image img, float alpha)
    {
        if (img == null) return;
        Color c = img.color;
        c.a = Mathf.Clamp01(alpha);
        img.color = c;
    }

public void CarroMuerto()
    {
        carrosMuertos++;
    }
    
}
