using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;
using System.IO;
using System.Runtime.InteropServices;

namespace Graphics
{
    class TextureCubeMap
    {
        uint mtexture;
        //int width, height;
        int TexUnit;
        List<Bitmap> bitmaps;
        public TextureCubeMap(List<string> path, int texUnit)
        {
            TexUnit = texUnit;
            Gl.glActiveTexture(texUnit);
            uint[] tex = { 0 };
            Gl.glGenTextures(1, tex);
            mtexture = tex[0];
            Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, mtexture);

            bitmaps = new List<Bitmap>();
            for (int i = 0; i < path.Count; ++i)
            { 
                bitmaps.Add((Bitmap)Bitmap.FromFile(path[i]));
                if (bitmaps[i] != null)
                {
                    Rectangle rect = new Rectangle(0, 0, bitmaps[i].Width, bitmaps[i].Height);
                    BitmapData bitmapData = bitmaps[i].LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    Gl.glTexImage2D(Gl.GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, 0,
                                         Gl.GL_RGBA, bitmaps[i].Width, bitmaps[i].Height, 0, Gl.GL_RGBA,
                                         Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);
                    if (bitmaps[i] != null)
                             {
                                 bitmaps[i].UnlockBits(bitmapData);
                                 bitmaps[i].Dispose();
                             }
                }
                else
                {
                    Console.WriteLine("Notfound");
                }
            }
            //width = bitmap.Width;
            //height = bitmap.Height;

            Gl.glTexParameteri(Gl.GL_TEXTURE_CUBE_MAP, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE);
            Gl.glTexParameteri(Gl.GL_TEXTURE_CUBE_MAP, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE);
            Gl.glTexParameteri(Gl.GL_TEXTURE_CUBE_MAP, Gl.GL_TEXTURE_WRAP_R, Gl.GL_CLAMP_TO_EDGE);//

            Gl.glTexParameteri(Gl.GL_TEXTURE_CUBE_MAP, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_CUBE_MAP, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);

           // Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);
            //Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, width, height, 0, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);

        }

        public void CleanUp()
        {
            Gl.glDeleteTextures(1, (IntPtr)mtexture);
        }
        public void Bind()
        {
            Gl.glActiveTexture(TexUnit);
            Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, mtexture);
        }
    }
}
