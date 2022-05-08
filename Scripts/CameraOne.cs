using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.ObjdetectModule;
using OpenCVForUnity.UnityUtils;

public class CameraOne : MonoBehaviour
{

    public RawImage background;
    public AspectRatioFitter fit;
    public GameObject button;

    private WebCamTexture webcamTexture;
    private string filename;
    private CascadeClassifier cascade;
    private MatOfRect faces;
    private Texture2D texture;
    private Mat rgbaMat;
    private Mat grayMat;

    void initializeData()
    {
        //rotate RawImage according to rotation of webcamtexture
        this.GetComponent<RectTransform>().Rotate(new Vector3(0, 0, 360 - webcamTexture.videoRotationAngle));
        //store name of xml file
        filename = "haarcascade_frontalface_default.xml";
        //initaliaze cascade classifier
        cascade = new CascadeClassifier();
        //load the xml file data
        cascade.load(Utils.getFilePath(filename));

        //initalize faces matofrect
        faces = new MatOfRect();
        //initialize rgb and gray Mats
        rgbaMat = new Mat(webcamTexture.height, webcamTexture.width, CvType.CV_8UC4);
        grayMat = new Mat(webcamTexture.height, webcamTexture.width, CvType.CV_8UC4);

        //initialize texture2d
        texture = new Texture2D(rgbaMat.cols(), rgbaMat.rows(), TextureFormat.RGBA32, false);
    }

    // Start is called before the first frame update
    void Start()
    {
        //obtain cameras avialable
        WebCamDevice[] cam_devices = WebCamTexture.devices;
        //create camera texture
        webcamTexture = new WebCamTexture(cam_devices[0].name, 480, 640, 30);
        //start camera
        webcamTexture.Play();

        background.texture = webcamTexture;
        // initialize members;
        initializeData();
    }

    // Update is called once per frame
    void Update()
    {
        // ratio
        float ratio = (float)webcamTexture.width / (float)webcamTexture.height;
        fit.aspectRatio = ratio;

        // scale
        float scaleY = webcamTexture.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        // orientation
        int orient = -webcamTexture.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);


        //convert webcamtexture to rgb mat
        Utils.webCamTextureToMat(webcamTexture, rgbaMat);

        //convert rgbmat to grayscale
        Imgproc.cvtColor(rgbaMat, grayMat, Imgproc.COLOR_RGBA2GRAY);

        //extract faces
        cascade.detectMultiScale(grayMat, faces, 1.1, 4);

        //store faces in array
        OpenCVForUnity.CoreModule.Rect[] rects = faces.toArray();

        //draw rectangle over all faces
        if (rects.Length > 0)
        {
            button.SetActive(true);
        }
        else
        {
            button.SetActive(false);
        }
        for (int i = 0; i < rects.Length; i++)
        {
            Imgproc.rectangle(rgbaMat, new Point(rects[i].x, rects[i].y), new Point(rects[i].x + rects[i].width, rects[i].y + rects[i].height), new Scalar(255, 0, 0, 255), 2);
        }

        //convert rgb mat back to texture
        Utils.fastMatToTexture2D(rgbaMat, texture);

        //set rawimage texture
        background.texture = texture;

    }
}
