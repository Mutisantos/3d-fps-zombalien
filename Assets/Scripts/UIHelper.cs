using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/** referencias a los objetos del Canvas que compongan el HUD del jugador, con el fin de poder otorgarle al mismo la información correcta y
 * actualizada de su estado actual en terminos de Salud, Escudo, Cargador, Munición y objetos adquiridos.
 * Esteban.Hernandez
 */
public class UIHelper : MonoBehaviour
{

    public PlayerShooter PlayerHolder;
    public GameObject GameOverScreen;
    public GameObject GameFinishedScreen;
    public TextMeshProUGUI TotalAmmoText;
    public TextMeshProUGUI ClipAmmoText;
    public TextMeshProUGUI EventMessageText;
    public Slider HealthBar;
    public Slider ShieldBar;

    private void Awake()
    {
        HealthBar.value = PlayerHolder.MaxHealth;
        ShieldBar.value = 0;
        GameOverScreen.SetActive(false);
    }


    private void OnEnable()
    {
        PlayerHolder.HealthChanged += HealthChanged;
        PlayerHolder.ShieldChanged += ShieldChanged;
        PlayerHolder.AmmoChanged += AmmoChanged;
        PlayerHolder.ClipChanged += ClipChanged;
        PlayerHolder.ItemChanged += ItemChanged;
        PlayerHolder.FinishedChanged += GameFinished;
    }

    private void OnDisable()
    {
        PlayerHolder.HealthChanged -= HealthChanged;
        PlayerHolder.ShieldChanged -= ShieldChanged;
        PlayerHolder.AmmoChanged -= AmmoChanged;
        PlayerHolder.ClipChanged -= ClipChanged;
        PlayerHolder.ItemChanged -= ItemChanged;
        PlayerHolder.FinishedChanged -= GameFinished;
    }

    private void HealthChanged(float newValue)
    {
        HealthBar.value = newValue;
        if(newValue <= 0)
        {
            GameOverScreen.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            GameOverScreen.SetActive(false);
            Time.timeScale = 1;
        }
    }

    private void ShieldChanged(float newValue)
    {
        ShieldBar.value = newValue;
    }

    private void AmmoChanged(int newValue)
    {
        TotalAmmoText.text = $"{newValue}";
    }

    private void ClipChanged(int newValue)
    {
        ClipAmmoText.text = $"{newValue}";
    }


    private void ItemChanged(string newValue)
    {
        EventMessageText.text = $"Has obtenido {newValue}";
        StartCoroutine(FadeItemMessage());
    }

    private void GameFinished(bool newValue)
    {
        GameFinishedScreen.SetActive(newValue);
    }

    private IEnumerator FadeItemMessage()
    {
        yield return new WaitForSeconds(2);

        EventMessageText.text = "";
    }



}
