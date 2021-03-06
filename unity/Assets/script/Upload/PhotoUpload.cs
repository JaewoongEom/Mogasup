using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Windows.Forms;
using Ookii.Dialogs;
using UnityEngine.Networking;

public class PhotoUpload : MonoBehaviour
{
    VistaOpenFileDialog OpenDialog;
    Stream openStream = null;
    GameObject uiItem;

    UserController userController;
    private void Start()
    {
        userController = GameObject.Find("UserInfo").GetComponent<UserController>();

        OpenDialog = new VistaOpenFileDialog();
        OpenDialog.Filter = "jpg files (*.jpg) |*.jpg|png files (*.png) |*.jpg|All files  (*.*)|*.*";
        OpenDialog.FilterIndex = 3;
        OpenDialog.Title = "Image Dialog";
    }

    public string FileOpen()
    {
        if (OpenDialog.ShowDialog() == DialogResult.OK)
        {
            if ((openStream = OpenDialog.OpenFile()) != null)
            {
                return OpenDialog.FileName;
            }
        }
        return null;
    }

    public void Upload()
    {
        string fileName = FileOpen();

        if (!string.IsNullOrEmpty(fileName))
        {
            Debug.Log(fileName);
            StartCoroutine(Send(fileName));
        }
    }

    IEnumerator Send(string filename)
    {
        WWWForm form = new WWWForm();
        form.AddField("family_id", userController.family_id);
        WWW localfile = new WWW(filename);
        string url = "http://k4a102.p.ssafy.io:8080/picture";
        //string url = "http://localhost:8080/picture";

    
        form.AddBinaryData("file", localfile.bytes);
        // Upload via post request
        var www = UnityWebRequest.Post(url, form);
        // change the method name
        www.method = "POST";
        yield return www.SendWebRequest();
        if (www.error != null)
        {
            Debug.Log(www.error);
        }
        else
        {
            Response r = JsonUtility.FromJson<Response>(www.downloadHandler.text);

            ScrollRect scrollRect = GameObject.Find("PhotoListScroll").GetComponent<ScrollRect>();
            GameObject photoPrefab = Resources.Load("Prefabs/photoPrefab") as GameObject;
            GameObject photo = Instantiate(photoPrefab) as GameObject;
            RawImage item = photo.GetComponent<RawImage>();
            item.texture = localfile.texture;
            item.name = "photo_1_" + r.result;

            photo.transform.SetParent(scrollRect.content.transform, false);

        }
    }

    public void DeletePhoto()
    {
        StartCoroutine(delete());
    }

    IEnumerator delete()
    {
        string jsonResult;
        bool isOnLoading = true;
        string GetDataUrl = "http://k4a102.p.ssafy.io:8080/picture/" + GameObject.Find("picName").gameObject.GetComponent<Text>().text;
        // string GetDataUrl = "http://localhost:8080//picture/" + GameObject.Find("picName").gameObject.GetComponent<Text>().text;
        using (UnityWebRequest www = UnityWebRequest.Get(GetDataUrl))
        {
            //www.chunkedTransfer = false;
            www.method = "DELETE";
            yield return www.Send();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                if (www.isDone)
                {
                    GameObject photos = GameObject.Find("PhotoGroup");
                    RawImage[] temp = photos.GetComponentsInChildren<RawImage>();

                    for (int i = 0; i < temp.Length; i++)
                    {
                        if (temp[i].name.Contains(GameObject.Find("picName").gameObject.GetComponent<Text>().text))
                        {
                            Destroy(temp[i].gameObject);
                        }
                    }
                    isOnLoading = false;
                    jsonResult =
                    System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);

                    uiItem = GameObject.Find("PhotoItem");
                    uiItem.GetComponent<RectTransform>().anchoredPosition = Vector3.left * -2000;
                }
            }
        }
    }

    [System.Serializable]
    class Response
    {
        public string message;
        public string result;
    }

}
