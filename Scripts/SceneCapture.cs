using UnityEngine;
using System.IO;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class SceneCapture : MonoBehaviour
    {
        public int captureWidth = 1920;
        public int captureHeight = 1080;
        public string DefaultCaptureFilename = "screenshot.png";
        public void captureScene()
        {
            // Create a new texture to hold the captured image
            Texture2D captureTexture = new(captureWidth, captureHeight, TextureFormat.RGB24, false);

            // Read the pixels from the screen and save them to the texture
            captureTexture.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
            captureTexture.Apply();

            // Encode the texture as a PNG file and save it to disk
            byte[] pngBytes = captureTexture.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/Snapshots/" + DefaultCaptureFilename, pngBytes);

            // Destroy the temporary texture
            Destroy(captureTexture);
        }

    }
}
