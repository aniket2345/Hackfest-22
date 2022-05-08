using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.ObjdetectModule;
using OpenCVForUnity.UnityUtils;
using System.IO;
using UnityEngine.UI;

public class controller : MonoBehaviour
{
    public GameObject button;
    public RawImage cam;
    private int count;
    // Start is called before the first frame update
    void Start()
    {
        button.SetActive(false);
        count = 0;
    }

    public void ScreenShot()
    {
        /*
        WebCamTexture webcamTexture;

        //obtain cameras avialable
        WebCamDevice[] cam_devices = WebCamTexture.devices;
        //create camera texture
        webcamTexture = new WebCamTexture(cam_devices[0].name, 480, 640, 30);

        webcamTexture.Play();


        Color[] webcamPixels = webcamTexture.GetPixels(0,0,webcamTexture.width, webcamTexture.height);*/
        

        //texture.SetPixels(webcamPixels, 0);
        Texture2D texture = new Texture2D(128,128);
        texture = (Texture2D) cam.texture;
        texture.Apply();

        var bytes = texture.EncodeToPNG();

        var dirPath = Application.dataPath + "/SaveImages/";

        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        Debug.Log(dirPath + (int)count + "Image.png");
        File.WriteAllBytes(dirPath + (int)count + "-Image.png", bytes);
        count = count + 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
