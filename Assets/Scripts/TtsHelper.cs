using System;
using UnityEngine;
using System.Net.Http;

public class TtsHelper : MonoBehaviour
{
    private static HttpClient httpClient = new HttpClient {
        BaseAddress = new Uri("https://tts.maxhagman.se/api/tts"),
    };

    // TODO: Should be removed when we have more of the system in place
    private const string defaultInsult = "You <break time=\"150ms\"/> suck you little <break time=\"100ms\"/> twat.";
    
    private const string language = "en";
    private const string vocoder = "medium";

    public string voice = "larynx:southern_english_male-glow_tts";
    public float denoiser = 0.5f;

    static string insultContentWrapper = "<speak><voice><prosody rate=\"slow\" pitch=\"-5%\"><emphasis level=\"moderate\">{0}</emphasis></prosody></voice></speak>";

    public void CreateVoiceline(string insult) 
    {
        if (insult == null || insult.Length <= 0)
        {
            insult = defaultInsult;
        }
        var content = String.Format(insultContentWrapper, insult);
        SendRequest(denoiser, insult);
    }

    private async void SendRequest(float denoiserStrength, string content)
    {
        using (UnityWebRequest request = UnityWebRequest.Get())
        {
            // TODO: switch to this instead of the httpclient
        }

        var urlParameters = String.Format(
            "?voice={0}&lang={1}&vocoder={2}&denoiserStrength={3}&text={4}&speakerId=&ssml=true&ssmlNumbers=true&ssmlDates=true&ssmlCurrency=true&cache=false", 
            voice,
            language,
            vocoder,
            denoiserStrength,
            content
            );
        byte[] response = await httpClient.GetByteArrayAsync(urlParameters);

        if (response.StatusCode != 200)
        {
            Debug.Log("Error response from tts with code " + response.StatusCode + ": " + response.StatusMessage);
            return;
        }

        // TODO: Read bytearray to WAV
    }

// https://tts.maxhagman.se/api/tts?voice=larynx:southern_english_male-glow_tts&lang=en&vocoder=medium&denoiserStrength=0.005&text=<speak> <voice> <prosody rate="slow" pitch="-5%"> <emphasis level="moderate"> You <break time="150ms"/> suck you little <break time="100ms"/> twat. </emphasis> </prosody> </voice> </speak>&speakerId=&ssml=true&ssmlNumbers=true&ssmlDates=true&ssmlCurrency=true&cache=false
}