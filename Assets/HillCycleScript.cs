using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class HillCycleScript : MonoBehaviour
{

    public KMAudio Audio;
    public List<KMSelectable> keys;
    public GameObject[] dials;
    public TextMesh[] dialText;
    public TextMesh disp;

    private int r;
    private string[][] message = new string[2][]  { new string[100] { "ADVERTED", "ADVOCATE", "ALLOCATE", "ALTERING", "BINORMAL", "BINOMIAL", "BULKHEAD", "BULLETED", "CIPHERED", "CIRCUITS", "COMPILER", "COMMANDO", "DECIMATE", "DECEIVED", "DISCOVER", "DISPOSAL", "ENCIPHER", "ENTRANCE", "EQUATORS", "EQUALISE", "FINALISE", "FINNICKY", "FORTRESS", "FORWARDS", "GAUNTLET", "GAMBLING", "GATEPOST", "GATEWAYS", "HAZARDED", "HAZINESS", "HUNGRIER", "HUNTRESS", "INCOMING", "INDIRECT", "ILLUSION", "ILLUMINE", "JIGSAWED", "JIGGLING", "JUNCTION", "JUNKYARD", "KILOWATT", "KILOBYTE", "KNOCKOUT", "KNOCKING", "LINGERED", "LINEARLY", "LINKAGES", "LINKWORK", "MONOGRAM", "MONOMIAL", "MULTIPLY", "MULTITON", "NANOGRAM", "NANOWATT", "NUMEROUS", "NUMERALS", "ORDINALS", "ORDERING", "OBSERVED", "OBSCURED", "PROGRESS", "PROJECTS", "PROPHASE", "PROPHECY", "QUADRANT", "QUADRICS", "QUARTILE", "QUARTICS", "REVERSED", "REVOLVED", "ROTATORS", "RELAYING", "STANZAIC", "STANDOUT", "STOPPING", "STOPWORD", "TRIGONAL", "TRICKIER", "TOGGLING", "TOGETHER", "UNDERWAY", "UNDERLIE", "ULTRAHOT", "ULTRARED", "VICINITY", "VICELESS", "VOLITION", "VOLATILE", "WHATNESS", "WHATSITS", "WHATEVER", "WHATNOTS", "YEARLONG", "YEASAYER", "YOKOZUNA", "YOURSELF", "ZIPPERED", "ZYGOMATA", "ZUGZWANG", "ZYMOGENE" }
                                                  , new string[100] { "COMMANDO", "ENCIPHER", "NUMERALS", "CIPHERED", "HAZARDED", "MULTIPLY", "DECEIVED", "ULTRAHOT", "GATEWAYS", "TRICKIER", "DECIMATE", "OBSERVED", "ORDERING", "YOURSELF", "NUMEROUS", "UNDERWAY", "WHATNESS", "QUADRANT", "TOGETHER", "QUARTICS", "ROTATORS", "KILOBYTE", "WHATNOTS", "JIGGLING", "ENTRANCE", "EQUALISE", "INCOMING", "ALLOCATE", "RELAYING", "UNDERLIE", "ADVOCATE", "VICINITY", "PROGRESS", "QUARTILE", "HUNGRIER", "HUNTRESS", "REVOLVED", "MONOGRAM", "YOKOZUNA", "VOLITION", "ZUGZWANG", "PROJECTS", "JIGSAWED", "BULLETED", "ILLUSION", "GAUNTLET", "BINOMIAL", "ADVERTED", "LINKWORK", "ZIPPERED", "STANDOUT", "QUADRICS", "MONOMIAL", "BULKHEAD", "PROPHASE", "JUNCTION", "OBSCURED", "DISCOVER", "KNOCKING", "FINNICKY", "REVERSED", "TOGGLING", "ZYGOMATA", "CIRCUITS", "VOLATILE", "KNOCKOUT", "STOPPING", "ULTRARED", "STOPWORD", "LINKAGES", "HAZINESS", "ZYMOGENE", "ILLUMINE", "YEASAYER", "MULTITON", "NANOWATT", "WHATSITS", "PROPHECY", "COMPILER", "GAMBLING", "GATEPOST", "LINEARLY", "KILOWATT", "BINORMAL", "JUNKYARD", "TRIGONAL", "INDIRECT", "ALTERING", "DISPOSAL", "NANOGRAM", "FORTRESS", "YEARLONG", "WHATEVER", "ORDINALS", "STANZAIC", "LINGERED", "EQUATORS", "FINALISE", "FORWARDS", "VICELESS" }};
    private string[] ciphertext = new string[2];
    private string answer;
    private int[][] rot = new int[2][] { new int[8], new int[8] };
    private int pressCount;
    private bool moduleSolved;

    //Logging
    static int moduleCounter = 1;
    int moduleID;

    private void Awake()
    {
        moduleID = moduleCounter++;
        foreach (KMSelectable key in keys)
        {
            int k = keys.IndexOf(key);
            key.OnInteract += delegate () { KeyPress(k); return false; };
        }
    }

    void Start()
    {
        Reset();
    }

    private void KeyPress(int k)
    {
        keys[k].AddInteractionPunch(0.125f);
        if (moduleSolved == false)
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            if (k == 26)
            {
                pressCount = 0;
                answer = string.Empty;
            }
            else
            {
                pressCount++;
                answer = answer + "QWERTYUIOPASDFGHJKLZXCVBNM"[k];
            }
            disp.text = answer;
            if (pressCount == 8)
            {
                if (answer == ciphertext[1])
                {
                    moduleSolved = true;
                    Audio.PlaySoundAtTransform("InputCorrect", transform);
                    disp.color = new Color32(0, 255, 0, 255);
                }
                else
                {
                    GetComponent<KMBombModule>().HandleStrike();
                    disp.color = new Color32(255, 0, 0, 255);
                    Debug.LogFormat("[Hill Cycle #{0}]The submitted response was {1}: Resetting", moduleID, answer);
                }
                Reset();
            }
        }
    }

    private void Reset()
    {

        StopAllCoroutines();
        if (moduleSolved == false)
        {
            pressCount = 0;
            answer = string.Empty;
            int det = 0;
            int[] matrix = new int[4];
            r = Random.Range(0, 100);
            string[] roh = new string[8];
            List<string>[] ciph = new List<string>[] { new List<string> { }, new List<string> { } };
            for (int i = 0; i < 8; i++)
            {
                dialText[i].text = string.Empty;
                rot[1][i] = rot[0][i];
                rot[0][i] = Random.Range(0, 5);
                roh[i] = rot[0][i].ToString();
            }
            while(det % 2 == 0 || det == 13)
            {
                for(int i = 0; i < 4; i++)
                {
                    rot[0][(2 * i) + 1] = Random.Range(0, 5);
                    roh[(2 * i) + 1] = rot[0][(2 * i) + 1].ToString();
                }
                for (int i = 0; i < 4; i++)
                {
                    matrix[i] = 5 * rot[0][2 * i] + rot[0][(2 * i) + 1];
                }
                det = (matrix[0] * matrix[3] - matrix[1] * matrix[2]) % 26;
            }
            for(int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    int[] alphpos = new int[2] { "ZABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(message[i][r][2 * j]), "ZABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(message[i][r][(2 * j) + 1]) };
                    for(int k = 0; k < 2; k++)
                    {
                        ciph[i].Add("ZABCDEFGHIJKLMNOPQRSTUVWXYZ"[((matrix[2 * k] * alphpos[0]) + (matrix[(2 * k) + 1] * alphpos[1])) % 26].ToString());
                    }
                }
                ciphertext[i] = string.Join(string.Empty, ciph[i].ToArray());
            }
            Debug.LogFormat("[Hill Cycle #{0}]The encrypted message was {1}", moduleID, ciphertext[0]);
            Debug.LogFormat("[Hill Cycle #{0}]The dial rotations were {1}", moduleID, string.Join(", ", roh));
            Debug.Log("[Hill Cycle #" + moduleID + "] The keymatrix was:");
            Debug.Log("[Hill Cycle #" + moduleID + "] " + matrix[0] + " " + matrix[1]);
            Debug.Log("[Hill Cycle #" + moduleID + "] " + matrix[2] + " " + matrix[3]);
            Debug.LogFormat("[Hill Cycle #{0}]The deciphered message was {1}", moduleID, message[0][r]);
            Debug.LogFormat("[Hill Cycle #{0}]The response word was {1}", moduleID, message[1][r]);
            Debug.LogFormat("[Hill Cycle #{0}]The correct response was {1}", moduleID, ciphertext[1]);
        }
        StartCoroutine(DialSet());
    }

    private IEnumerator DialSet()
    {
        int[] spin = new int[8];
        bool[] set = new bool[8];
        for (int i = 0; i < 8; i++)
        {
            if (moduleSolved == false)
            {
                spin[i] = rot[0][i] - rot[1][i];
            }
            else
            {
                spin[i] = -rot[0][i];
            }
            if (spin[i] < 0)
            {
                spin[i] += 5;
            }
        }
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (spin[j] == 0)
                {
                    if (set[j] == false)
                    {
                        set[j] = true;
                        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, transform);
                        if (moduleSolved == false)
                        {
                            dialText[j].text = ciphertext[0][j].ToString();
                        }
                        else
                        {
                            switch (j)
                            {
                                case 0:
                                    dialText[j].text = "W";
                                    break;
                                case 2:
                                case 3:
                                    dialText[j].text = "L";
                                    break;
                                case 4:
                                    dialText[j].text = "D";
                                    break;
                                case 5:
                                    dialText[j].text = "O";
                                    break;
                                case 6:
                                    dialText[j].text = "N";
                                    break;
                                default:
                                    dialText[j].text = "E";
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    dials[j].transform.localEulerAngles += new Vector3(0, 0, 72);
                    spin[j]--;
                }
            }
            if (i < 7)
            {
                yield return new WaitForSeconds(0.25f);
            }
        }
        if (moduleSolved == true)
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
            GetComponent<KMBombModule>().HandlePass();
        }
        disp.text = string.Empty;
        disp.color = new Color32(255, 255, 255, 255);
        yield return null;
    }
#pragma warning disable 414
    private string TwitchHelpMessage = "!{0} QWERTYUI [Inputs letters] | !{0} cancel [Deletes inputs]";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {

        if (command.ToLowerInvariant() == "cancel")
        {
            KeyPress(26);
            yield return null;
        }
        else
        {
            command = command.ToUpperInvariant();
            var word = Regex.Match(command, @"^\s*([A-Z\-]+)\s*$");
            if (!word.Success)
            {
                yield break;
            }
            command = command.Replace(" ", string.Empty);
            foreach (char letter in command)
            {
                KeyPress("QWERTYUIOPASDFGHJKLZXCVBNM".IndexOf(letter));
                yield return new WaitForSeconds(0.125f);
            }
            yield return null;
        }
    }
}
