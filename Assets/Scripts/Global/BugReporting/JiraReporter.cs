using System.Net.Http;
using TMPro;
using UnityEngine;
using Palmmedia.ReportGenerator.Core.Common;
using System.Text;
using System.Collections;

public class JiraReporter : MonoBehaviour
{
    [SerializeField] TMP_InputField header;
    [SerializeField] TMP_InputField body;
    [SerializeField] GameObject form;
    [SerializeField] TMP_Text successAlert;

    public readonly string apiToken = "ATATT3xFfGF06BV-yJpKNE7Q4znG66HGScgo2-zg7_uYA_WJvaDuCJ-jOzXxrG-zGHJgBhjyeoVD4-1cmREEgp_SalPoQPAXyWa4CsRgz1fJRifqs8mcsNcMjE3sO0W2qIs36toqpKS61vGcHrZX5AdkAHtJaWoxmwnZe_zuS5ehZGTdWm0R9og=63052D57";
    private string authEncode = "ZXRoZXJlb3NAZ21haWwuY29tOkFUQVRUM3hGZkdGMDZCVi15SnBLTkU3UTR6bkc2NkhHU2NnbzItemc3X3VZQV9XSnZhRHVDSi1qT3pYeHJHLXpHSEpnQmhqeWVvVkQ0LTFjbVJFRWdwX1NhbFBvUVBBWHlXYTRDc1JnejFmSlJpZnFzOG1jc05jTWpFM3NPMFcycUlzMzZ0b3FwS1M2MXZHY0hyWlg1QWRrQUh0SmFXb3htd25aZV96dVM1ZWhaR1RkV20wUjlvZz02MzA1MkQ1Nw==";
    private string postURI = "https://yorblesnoot.atlassian.net/rest/api/3/issue";

    private static readonly HttpClient client = new HttpClient();

    private void Awake()
    {
        client.DefaultRequestHeaders.Add("Authorization", "Basic " + authEncode);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    }
    public async void SendReport()
    {
        var response = await client.PostAsync(postURI, GenerateJSONPayload(header.text, body.text));
        var responseString = await response.Content.ReadAsStringAsync();
        Debug.Log(responseString);
        body.text = "";
        form.SetActive(false);
        
        StartCoroutine(FadeSuccess(successAlert));
    }

    readonly float fadeDuration;
    readonly Color32 invisWhite = new(255,255, 255, 0);
    IEnumerator FadeSuccess(TMP_Text words)
    {
        words.gameObject.SetActive(true);
        float timeElapsed = 0;
        while (timeElapsed < fadeDuration)
        {
            words.color = Color32.Lerp(Color.white, invisWhite, timeElapsed / fadeDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        words.gameObject.SetActive(false);
        words.color = Color.white;
    }

    HttpContent GenerateJSONPayload(string summary, string body)
    {
        var obj = new
        {
            fields = new
            {
                project = new
                {
                    key = "JBLU"
                },
                summary = summary,
                issuetype = new
                {
                    name = "Task"
                },
                description = new
                {
                    version = 1,
                    type = "doc",
                    content = new[]
                    {
                        new
                        {
                            type = "paragraph",
                            content = new[]
                            {
                                new
                                {
                                    type = "text",
                                    text = body
                                }
                            }
                        }
                    }
                }
            }
        };
        string json = JsonSerializer.ToJsonString(obj);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
