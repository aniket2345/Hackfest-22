using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.ObjdetectModule;
using OpenCVForUnity.UnityUtils;


public class Camera : MonoBehaviour
{
    // Start is called before the first frame update
    private bool camAvailable;
    private WebCamTexture backCam;
    private Texture defaultBackground;

    public RawImage background;
    public AspectRatioFitter fit;

    private string filename;

    private CascadeClassifier cascade;
    private MatOfRect faces;

    private Mat rgbaMat;
    private Mat grayMat;

    private Texture2D finalTexture;
    /*CascadeClassifier cascade; // open CV cascade
    Mat frame; // helps store current frame
    OpenCvSharp.Rect MyFace; // holds a face ( Rect )*/

    private void Start()
    {
        defaultBackground = background.texture;

        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.Log("Not camera was detected");
            camAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            //Debug.Log(devices[i].name);
            if (devices[i].isFrontFacing)
            {
                backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
            }
        }

        backCam.Play();
        background.texture = backCam;

        filename = "haarcascade_frontalface_default.xml";
        cascade = new CascadeClassifier();
        cascade.load(Utils.getFilePath(filename));
        faces = new MatOfRect();

        rgbaMat = new Mat(backCam.height, backCam.width, CvType.CV_8UC4);
        grayMat = new Mat(backCam.height, backCam.width, CvType.CV_8UC4);

        finalTexture = new Texture2D(rgbaMat.cols(), rgbaMat.rows(), TextureFormat.RGBA32, false);

        //cascade = new CascadeClassifier("C:/Users/Aniket/Documents/UnityProjects/cameraTrial/Assets/OpenCV+Unity/Demo/Face_Detector/haarcascade_frontalface_default.xml");

        camAvailable = true;
    }

    private void Update()
    {
        if (!camAvailable)
        {
            return;
        }

        // ratio
        float ratio = (float)backCam.width / (float)backCam.height;
        fit.aspectRatio = ratio;

        // scale
        float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        // orientation
        int orient = -backCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);


        Utils.webCamTextureToMat(backCam, rgbaMat);
        Imgproc.cvtColor(rgbaMat, grayMat, Imgproc.COLOR_RGBA2GRAY);
        cascade.detectMultiScale(grayMat, faces, 1.1, 4);
        OpenCVForUnity.CoreModule.Rect[] rects = faces.toArray();


        for (int i = 0; i < rects.Length; i++)
        {
            Debug.Log("detect faces " + rects[i].x);
            Imgproc.rectangle(rgbaMat, new Point(rects[i].x, rects[i].y), new Point(rects[i].x + rects[i].width, rects[i].y + rects[i].height), new Scalar(255, 0, 0, 255), 2);
        }

        Utils.fastMatToTexture2D(rgbaMat, finalTexture);
        background.texture = finalTexture;
        /*Mat frame = OpenCvSharp.Unity.TextureToMat(backCam);

        findNewFace(frame);
        display(frame);*/
    }

    /*
    void findNewFace(Mat frame)
    {
        var faces = cascade.DetectMultiScale(frame, 1.1, 2, HaarDetectionType.ScaleImage);
        if(faces.Length >= 1)
        {
            Debug.Log(faces[0].Location);
            MyFace = faces[0];
        }
    }

    void display(Mat frame)
    {
        if (MyFace != null)
        {
            frame.Rectangle(MyFace, new Scalar(250, 0, 0), 2); // drawing on MyFace
        }
        Texture newTexture = OpenCvSharp.Unity.MatToTexture(frame);
        background.texture = newTexture;
    }*/
}
