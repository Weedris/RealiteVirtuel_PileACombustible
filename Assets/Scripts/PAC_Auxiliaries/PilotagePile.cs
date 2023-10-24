/* This script show theoretical value for the fuel cell
 * the value are not checked yet by "our fuel cell expert"
 * 
 * the fuction is verry simple and probably not super acurate but it should do
 * 
 * it makes a lerp between two value based on true value
 * (there might have a better way but i didn't find it in time)
 * 
 */

using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

public class PilotagePile : MonoBehaviour
{
    [SerializeField]
    private Slider sliderIntensite;
    private Slider sliderTemperature;

    [SerializeField]
    private TextMeshProUGUI screen;

    // Intensité = A
    // Tension = V
    //public TextMeshProUGUI TextPuissance;
    // Puissance = W = V * A


    private float intensite = 0;
    private float tension = 0;
    private float puissance = 0;
    private float temperature = 0;
    private float debitHydrogene = 0;
    private float debitAir = 0;
    private float debitEau = 0;
    private float rendement = 0;
    private GameManager gm;
    private void Start()
    {
        gm = FindFirstObjectByType<GameManager>();
    }
    // Update is called once per frame
    void Update()
    {
        

        if (gm.state == GameManager.State.Pilotage)
        {
            if (intensite <= 29.7f)
            {
                float A = (intensite - 20) / 9.7f;
                tension = Mathf.Lerp(18, 17.4f, A);
                debitEau = Mathf.Lerp(2.7f, 4, A);
                debitHydrogene = Mathf.Lerp(3.579f, 5.347f, A);
                rendement = Mathf.Lerp(60, 57.9f, A);
                debitAir = Mathf.Lerp(32.2f, 42.6f, A);
                puissance = tension * intensite;
            }
            else if (intensite <= 39.8f)
            {
                float A = (intensite - 29.7f) / 10.1f;
                tension = Mathf.Lerp(17.4f, 16.8f, A);
                debitEau = Mathf.Lerp(4f, 5.3f, A);
                debitHydrogene = Mathf.Lerp(5.347f, 7.113f, A);
                rendement = Mathf.Lerp(57.9f, 55.7f, A);
                debitAir = Mathf.Lerp(42.6f, 54.4f, A);
                puissance = tension * intensite;
            }
            else if (intensite <= 49.8f)
            {
                float A = (intensite - 39.8f) / 10f;
                tension = Mathf.Lerp(16.8f, 15.9f, A);
                debitEau = Mathf.Lerp(5.3f, 6.7f, A);
                debitHydrogene = Mathf.Lerp(7.113f, 8.85f, A);
                rendement = Mathf.Lerp(55.7f, 52.9f, A);
                debitAir = Mathf.Lerp(54.4f, 57.5f, A);
                puissance = tension * intensite;
            }
            else if (intensite <= 59.8f)
            {
                float A = (intensite - 49.8f) / 10f;
                tension = Mathf.Lerp(15.9f, 15.2f, A);
                debitEau = Mathf.Lerp(6.7f, 8f, A);
                debitHydrogene = Mathf.Lerp(8.85f, 10.57f, A);
                rendement = Mathf.Lerp(52.9f, 50.7f, A);
                debitAir = Mathf.Lerp(57.5f, 66.3f, A);
                puissance = tension * intensite;
            }
            /*
            V = 18
            A = 20
            H2O = 2,7 g/min
            H2 = 3,579 l/min
            rendement = 60%
            air = 32,2 l/min

            V = 17,4
            A = 29,7
            H2O = 4 g/min
            H2 = 5,347 l/min
            rendement = 57,9%
            air = 42,6 l/min

            V = 16,8
            A = 39,8
            H2O = 5,3 g/min
            H2 = 7,113 l/min
            rendement = 55,7%
            air = 54,4 l/min

            V = 15,9
            A = 49,8
            H2O = 6,7 g/min
            H2 = 8,85 l/min
            rendement = 52,9%
            air = 57,5 l/min

            V = 15,2
            A = 59,8
            H2O = 8 g/min
            H2 = 10,57 l/min
            rendement = 50,7%
            Air = 66,3 l/min         
             */

            string text = gm.language.GiveCorrectlanguage().tension + ": " + tension.ToString() + " V" + "\n";
            text += gm.language.GiveCorrectlanguage().intensite + ": " + intensite.ToString() + " A" + "\n";
            text += gm.language.GiveCorrectlanguage().puissance + ": " + puissance.ToString() + " W" + "\n";
            text += gm.language.GiveCorrectlanguage().debitEau + ": " + debitEau.ToString() + " g/min" + "\n";
            text += gm.language.GiveCorrectlanguage().debitHydrogene + ": " + debitHydrogene.ToString() + " L/min" + "\n";
            text += gm.language.GiveCorrectlanguage().debitAir + ": " + debitAir.ToString() + " L/min" + "\n";
            text += gm.language.GiveCorrectlanguage().rendement + ": " + rendement.ToString() + " %" + "\n";

            screen.text = text;
        }

        if (gm.state != GameManager.State.Initilazing)
        {
            screen.alignment = TextAlignmentOptions.Left;
        }
    }

    public void changeIntesite()
    {
        intensite = sliderIntensite.value;
        sliderIntensite.GetComponentInChildren<TextMeshProUGUI>().text = intensite.ToString().Substring(0, 4);
    }

    public void changeTemperature()
    {
        temperature = sliderTemperature.value;
    }
}
