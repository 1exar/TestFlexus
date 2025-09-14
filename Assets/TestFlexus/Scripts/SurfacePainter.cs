using UnityEngine;

namespace TestFlexus.Scripts
{
    public class SurfacePainter : MonoBehaviour
    {
        [SerializeField] private RenderTexture renderTexture;
        [SerializeField] private Material drawMaterial;
        [SerializeField] private Texture2D decalTexture;
        [SerializeField] private float decalSize = 0.05f;

        private void Awake()
        {
            if (renderTexture == null)
            {
                renderTexture = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGB32);
                renderTexture.Create();

                RenderTexture.active = renderTexture;
                GL.Clear(true, true, new Color(0,0,0,0));
                RenderTexture.active = null;
            }

            GetComponent<Renderer>().material.SetTexture("_MaskRT", renderTexture);
        }

        public void Paint(RaycastHit hit)
        {
            Vector2 uv = hit.textureCoord;

            drawMaterial.SetTexture("_DecalTex", decalTexture);
            drawMaterial.SetVector("_UV", new Vector4(uv.x, uv.y, 0, 0));
            drawMaterial.SetFloat("_Size", decalSize);

            int x = (int)(uv.x * renderTexture.width);
            int y = (int)(uv.y * renderTexture.height);
            int size = (int)(decalSize * renderTexture.width);

            RenderTexture.active = renderTexture;
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, renderTexture.width, 0, renderTexture.height);

            drawMaterial.SetPass(0);

            GL.Begin(GL.QUADS);
            GL.TexCoord2(0, 0);
            GL.Vertex3(x - size / 2, y - size / 2, 0);
            GL.TexCoord2(1, 0);
            GL.Vertex3(x + size / 2, y - size / 2, 0);
            GL.TexCoord2(1, 1);
            GL.Vertex3(x + size / 2, y + size / 2, 0);
            GL.TexCoord2(0, 1);
            GL.Vertex3(x - size / 2, y + size / 2, 0);
            GL.End();

            GL.PopMatrix();
            RenderTexture.active = null;
        }
    }
}