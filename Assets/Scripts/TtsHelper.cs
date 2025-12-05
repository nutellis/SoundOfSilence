using System;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class TtsHelper : MonoBehaviour
{
    public static string baseAddress = "https://tts.maxhagman.se/api/tts";
    public string voice = "larynx:southern_english_male-glow_tts";
    public float denoiser = 0.5f;
    public AudioSource audioSource;

    // TODO: Should be removed when we have more of the system in place
    private const string defaultInsult = "You <break time=\"150ms\"/> suck you little <break time=\"100ms\"/> twat.";
    
    private const string language = "en";
    private const string vocoder = "medium";
    private const string insultContentWrapper = "<speak><voice><prosody rate=\"slow\" pitch=\"-5%\"><emphasis level=\"moderate\">{0}</emphasis></prosody></voice></speak>";

    public IEnumerator PlayInsult(string insult) 
    {
        Debug.Log("Started generating insult");
        if (insult == null || insult.Length <= 0)
        {
            insult = defaultInsult;
        }
        var content = String.Format(insultContentWrapper, insult);

        var urlParameters = String.Format(
            "?voice={0}&lang={1}&vocoder={2}&denoiserStrength={3}&text={4}&speakerId=&ssml=true&ssmlNumbers=true&ssmlDates=true&ssmlCurrency=true&cache=false",
            voice,
            language,
            vocoder,
            denoiser,
            WebUtility.UrlEncode(content)
            );
        var requestUrl = baseAddress + urlParameters;
        Debug.Log("TTS request URL: " + requestUrl);
        using UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(requestUrl, AudioType.WAV);
        request.certificateHandler = new BypassCertificate();
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error response from tts with code " + request.responseCode + ": " + request.error);
            yield break;
        }

        var audio = DownloadHandlerAudioClip.GetContent(request);
        audioSource.clip = audio;
        audioSource.Play();
        Debug.Log("Playing TTS audio");
    }

// https://tts.maxhagman.se/api/tts?voice=larynx:southern_english_male-glow_tts&lang=en&vocoder=medium&denoiserStrength=0.005&text=<speak> <voice> <prosody rate="slow" pitch="-5%"> <emphasis level="moderate"> You <break time="150ms"/> suck you little <break time="100ms"/> twat. </emphasis> </prosody> </voice> </speak>&speakerId=&ssml=true&ssmlNumbers=true&ssmlDates=true&ssmlCurrency=true&cache=false
}

public class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}