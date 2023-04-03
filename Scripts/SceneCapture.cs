using UnityEngine;
using System.IO;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class SceneCapture : MonoBehaviour
    {
        public int captureWidth = 1920; // the width of the captured image
        public int captureHeight = 1080; // the height of the captured image
        public string captureFilename = "screenshot.png"; // the name of the captured image file

        public void CaptureScene()
        {
            // Create a new texture to hold the captured image
            Texture2D captureTexture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);

            // Read the pixels from the screen and save them to the texture
            captureTexture.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
            captureTexture.Apply();

            // Encode the texture as a PNG file and save it to disk
            byte[] pngBytes = captureTexture.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/Snapshots/" + captureFilename, pngBytes);

            // Destroy the temporary texture
            Destroy(captureTexture);

            Debug.Log("Scene captured and saved as " + captureFilename);
        }

    }
}
