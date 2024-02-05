/* This script show theoretical value for the fuel cell
 * the value are not checked yet by "our fuel cell expert"
 * 
 * the fuction is verry simple and probably not super acurate but it should do
 * 
 * it makes a lerp between two value based on true value
 * (there might have a better way but i didn't find it in time)
 * 
 */

using System;
using TMPro;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

public class PilotagePile : MonoBehaviour
{
	[SerializeField]
	private Slider IntensitySlider;
	private Slider sliderTemperature;

	[SerializeField]
	private TextMeshProUGUI screen;

	// Intensité = A
	// Tension = V
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

	public static float roundUp(float num)
    {
        return (float)(Math.Ceiling(num * 100) / 100);
    }

    // Update is called once per frame
    void Update()
	{
		if (gm.state == GameManager.State.Pilotage)
        {
            float A = Mathf.Clamp((intensite - 20) / 39.8f, 0f, 1f);

            tension = roundUp(Mathf.Lerp(18 - (0.6f * A), 15.2f, A));
            debitEau = roundUp(Mathf.Lerp(2.7f + (1.3f * A), 8f, A));
            debitHydrogene = roundUp(Mathf.Lerp(3.579f + (1.778f * A), 10.57f, A));
            rendement = roundUp(Mathf.Lerp(60 - (2.1f * A), 50.7f, A));
            debitAir = roundUp(Mathf.Lerp(32.2f + (10.2f * A), 66.3f, A));
            puissance = roundUp(tension * intensite);
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

            var lang = gm.language.GiveCorrectlanguage();

			string text = lang.tension + ": " + tension.ToString() + " V" + "\n"
				+ lang.intensite + ": " + roundUp(intensite).ToString() + " A" + "\n"
				+ lang.puissance + ": " + puissance.ToString() + " W" + "\n"
				+ lang.debitEau + ": " + debitEau.ToString() + " g/min" + "\n"
				+ lang.debitHydrogene + ": " + debitHydrogene.ToString() + " L/min" + "\n"
				+ lang.debitAir + ": " + debitAir.ToString() + " L/min" + "\n"
				+ lang.rendement + ": " + rendement.ToString() + " %" + "\n";
			screen.text = text;
		}

		if (gm.state != GameManager.State.Initilazing)
		{
			screen.alignment = TextAlignmentOptions.Left;
		}
	}

	public void changeIntensite()
	{
		intensite = IntensitySlider.value;
		IntensitySlider.GetComponentsInChildren<TextMeshProUGUI>()[0].text = intensite.ToString().Substring(0, 4);
	}

	public void changeTemperature()
	{
		temperature = sliderTemperature.value;
	}
}
