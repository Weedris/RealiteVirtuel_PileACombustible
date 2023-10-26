/* pretty self explanatory I think
 * I should have done a better job to make it more lisible in the editor but i works
 * 
 */

using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Language")]
public class Language : ScriptableObject
{

    public string language;

    public string welcome;
    public string objectif;
    public string suivant;
    [TextArea(3, 10)]
    public string instruction;
    [TextArea(3, 10)]
    public string suite;
    [TextArea(3, 10)]
    public string warning;
    public string montage;

    [TextArea(3, 10)]
    public string stack;
    [TextArea(3, 10)]
    public string H2;
    [TextArea(3, 10)]
    public string compresseur;
    [TextArea(3, 10)]
    public string humidificateur;
    [TextArea(3, 10)]
    public string N2;
    [TextArea(3, 10)]
    public string ventilateur;
    [TextArea(3, 10)]
    public string H2O;
    [TextArea(3, 10)]
    public string radiateur;

    [TextArea(3, 10)]
    public string pilotage;

    public string intensityBarName;
    public string intensityBarValue;

    public string intensite;
    public string tension;
    public string puissance;
    public string debitHydrogene;
    public string debitAir;
    public string debitEau;
    public string rendement;
    public string temperature;

    public string endtitle;
    public string endtext;
    public string endbutton;
}
