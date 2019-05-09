using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;
using GlmNet;
using System.IO;
using Graphics._3D_Models;
using System.Drawing;
using System.Windows.Forms;
namespace Graphics
{
    class Renderer
    {
        int mode = 0;
        int SkyBoxEyePositionID;
        int SkyBoxAmbientLightID;
        int SkyBoxDataID;

        int TerrainEyePositionID;
        int TerrainAmbientLightID;
        int TerrainDataID;


        int SimpleEyePositionID;
        int SimpleAmbientLightID;
        int SimpleDataID;

        int TreesEyePositionID;
        int TreesAmbientLightID;
        int TreesDataID;

        //shaders
        Shader SimpleShader, SkyboxShader, TerrainShader, TreesShader;



        float Red = 1, Green = 1, Blue = 1;

        //skbox buffer
        uint SkyboxPositionsBufferID;

        //skybox Textures
        TextureCubeMap skyboxTex;
        List<string> SkyboxFacesPath;


        Model3D [] Trees =  new Model3D[100];
        float[] TreesHeights = new float[100];
        public Model Terrain;
        public Model[] GrassList = new Model[1600];



        public Dictionary<Tuple<int, int>, int> HeightsDict = new Dictionary<Tuple<int, int>, int>();
            
            
        //Terrain Textures
        Texture SandTexture, RockTexture, IceTexture, GrassTexture;

        Texture GrassTexture1, GrassTexture2, GrassTexture3, GrassTexture4;

        //Terrain Textures IDs
        int SandTexturetureID, RockTexturetureID, IceTexturetureID, GrassTexturetureID;

        //Terrain Heights
        float[,] TerrainHeights = new float[400, 400];

        int TerrainModelMatrixID, TerrainViewMatrixID, TerrainProjectionMatrixID;


        mat4 ProjectionMatrix, ViewMatrix, ScaleMatrix;

        public float Speed = 1;

        public Camera cam;

        int SkyboxViewMatrixID, SkyboxProjectionMatrixID, SkyboxModelMatrixID;
        int SimpleViewMatrixID, SimpleProjectionMatrixID, SimpleModelMatrixID;

        int TreesViewMatrixID, TreesProjectionMatrixID, TreesModelMatrixID;

        string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

        int GrassPointer = 0;
        int TreesPointer = 0;

        public void Initialize()
        {

            SimpleShader = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");

            TerrainShader = new Shader(projectPath + "\\Shaders\\TerrainVertexShader.vertexshader", projectPath + "\\Shaders\\TerrainFragmentShader.fragmentshader");

            TreesShader = new Shader(projectPath + "\\Shaders\\TreesVertexShader.vertexshader", projectPath + "\\Shaders\\TreesFragmentShader.fragmentshader");


            SandTexture = new Texture(projectPath + "\\Textures\\sand.jpg", 1, true);
            RockTexture = new Texture(projectPath + "\\Textures\\rock.jpg", 2, true);
            GrassTexture = new Texture(projectPath + "\\Textures\\grasss.jpg", 3, true);
            IceTexture = new Texture(projectPath + "\\Textures\\ice.jpg", 4, true);


            GrassTexture1 = new Texture(projectPath + "\\Textures\\grass1.png", 5, true);
            GrassTexture2 = new Texture(projectPath + "\\Textures\\grass2.png", 6, true);
            GrassTexture3 = new Texture(projectPath + "\\Textures\\grass3.png", 7, true);
            GrassTexture4 = new Texture(projectPath + "\\Textures\\grass4.png", 8, true);
            


          

            try
            {

                Bitmap bitmap = (Bitmap)Bitmap.FromFile(projectPath + "\\Textures\\map.png");
                int width = bitmap.Width;
                int height = bitmap.Height;
                for (int i = 0; i < bitmap.Height; ++i)
                {
                    for (int j = 0; j < bitmap.Width; ++j)
                    {
                        float y = ((float)bitmap.GetPixel(i, j).R / (float)(256)) * (float)200;
                        TerrainHeights[i, j] = y;
                       
                    }
                }
                List<List<vec3>> Vertices = new List<List<vec3>>();
                List<List<vec3>> Normals = new List<List<vec3>>();
                List<List<vec2>> UV = new List<List<vec2>>();
                List<int> inds = new List<int>();
                int counter = 0;

                double MinX = 1e9;
                double MaxX = -1 * 1e9;

                double MinZ = 1e9;
                double MaxZ = -1 * 1e9;

                for (int i = 0; i < bitmap.Height; i++)
                {
                    List<vec3> VerticesList = new List<vec3>();
                    List<vec3> NormalsList = new List<vec3>();
                    List<vec2> UVList = new List<vec2>();
                    for (int j = 0; j < bitmap.Width; j++)
                    {
                        float xCoordinate = (float)j / ((float)bitmap.Height - 1) * (float)200;
                        float yCoordinate = TerrainHeights[i, j];
                        float zCoordinate = (float)i / ((float)bitmap.Width - 1) * (float)200;

                        List<mat4> modelmatrices = new List<mat4>() { glm.scale(new mat4(1), new vec3(7000, 4000, 7000))
                                                            , new mat4(1)
                                                            ,  glm.translate(new mat4(1), new vec3(-500000,-900000 , -500000))
                                                            ,  new mat4(1) };
                        mat4 modelmatrix = MathHelper.MultiplyMatrices(modelmatrices);



                        vec4 pos = new vec4(new vec3(xCoordinate, yCoordinate, zCoordinate), 1);
                        vec4 finalpos = modelmatrix * pos;





                        xCoordinate = finalpos.x;
                        yCoordinate = finalpos.y;
                        zCoordinate = finalpos.z;
                        if (xCoordinate % 1000 != 0)
                            xCoordinate = ((int)(xCoordinate / 1000) + 1) * 1000;
                        if (yCoordinate % 1000 != 0)
                            yCoordinate = ((int)(yCoordinate / 1000) + 1) * 1000;
                        if (zCoordinate % 1000 != 0)
                            zCoordinate = ((int)(zCoordinate / 1000) + 1) * 1000;

                        Tuple<int, int> XZTuple = new Tuple<int, int>((int)xCoordinate, (int)zCoordinate);

                        HeightsDict[XZTuple] = (int)yCoordinate;


                        MinX = Math.Min(xCoordinate, MinX);

                        MaxX = Math.Max(xCoordinate, MaxX);
                        MinZ = Math.Min(zCoordinate, MinZ);
                        MaxZ = Math.Max(zCoordinate, MaxZ);


                        ++counter;
                        if (counter % 100 == 0)
                        {
                           
                            List<vec3> ver = new List<vec3>();
                            ver.Add(new vec3(-xCoordinate, yCoordinate, 0));
                            ver.Add(new vec3(xCoordinate, yCoordinate, 0));
                            ver.Add(new vec3(xCoordinate, -yCoordinate, 0));
                            ver.Add(new vec3(-xCoordinate, -yCoordinate, 0));

                            List<vec2> uv = new List<vec2>();
                            uv.Add(new vec2(0, 0));
                            uv.Add(new vec2(1, 0));
                            uv.Add(new vec2(1, 1));
                            uv.Add(new vec2(0, 1));
                            GrassList[GrassPointer] = new Model(ver, uv);
                            GrassList[GrassPointer].Initialize();
                            ++GrassPointer;
                        }

                        if (counter % 10000 == 0)
                        {
                          

                            TreesHeights[TreesPointer] = finalpos.y;

                            List<vec3> ver = new List<vec3>();
                            Trees[TreesPointer] = new Model3D();
                            Trees[TreesPointer].LoadFile(projectPath + "\\ModelFiles\\static\\palmtree", "palmtree.obj", TreesPointer + 9);
                            Trees[TreesPointer].scalematrix = glm.scale(new mat4(1), new vec3(400, 400, 400));
                            Trees[TreesPointer].rotmatrix = glm.rotate(3.1412f, new vec3(0, 1, 1));
                            ++TreesPointer;
                        
                        }

                        VerticesList.Add(new vec3((int)xCoordinate, (int)yCoordinate, (int)zCoordinate));
                        NormalsList.Add(new vec3(0,1,0));
                        float U = (float)j / ((float)bitmap.Height - 1);
                        float V = (float)i / ((float)bitmap.Width - 1);
                        UVList.Add(new vec2(U * 10, V * 10));
                    }
                    Vertices.Add(VerticesList);
                    Normals.Add(NormalsList);
                    UV.Add(UVList);
                }
                List<vec3> finalVerticesList = new List<vec3>();
                List<vec3> finalNormalsList = new List<vec3>();
                List<vec2> finalUVList = new List<vec2>();
                for (int i = 0; i < bitmap.Height-1; i++)
                {
                    for (int j = 0; j < bitmap.Width -1 ; j++)
                    {
                        finalVerticesList.Add(Vertices[i][j]);
                        finalVerticesList.Add(Vertices[i+1][j]);
                        finalVerticesList.Add(Vertices[i + 1][j+1]);
                        finalVerticesList.Add(Vertices[i][j + 1]);

                        finalNormalsList.Add(Normals[i][j]);
                        finalNormalsList.Add(Normals[i + 1][j]);
                        finalNormalsList.Add(Normals[i + 1][j + 1]);
                        finalNormalsList.Add(Normals[i][j + 1]);

                        finalUVList.Add(UV[i][j]);
                        finalUVList.Add(UV[i + 1][j]);
                        finalUVList.Add(UV[i + 1][j + 1]);
                        finalUVList.Add(UV[i][j + 1]);

                    }
                }


                Terrain = new Model(finalVerticesList, finalUVList, finalNormalsList);
                Terrain.Initialize();
              //  MessageBox.Show(MinX.ToString() + " " + MaxX.ToString() + " " + MinZ.ToString() + " " + MaxZ.ToString());

            }
            catch
            {
                MessageBox.Show("error");
            }








            SkyboxShader = new Shader(projectPath + "\\Shaders\\SkyBox.vertexshader", projectPath + "\\Shaders\\SkyBox.fragmentshader");

            SkyboxFacesPath = new List<string>();
            SkyboxFacesPath.Add(projectPath + "\\Textures\\Sky\\cloudtop_rt.png");
            SkyboxFacesPath.Add(projectPath + "\\Textures\\Sky\\cloudtop_lf.png");
            SkyboxFacesPath.Add(projectPath + "\\Textures\\Sky\\cloudtop_up.png");
            SkyboxFacesPath.Add(projectPath + "\\Textures\\Sky\\cloudtop_dn.png");
            SkyboxFacesPath.Add(projectPath + "\\Textures\\Sky\\cloudtop_bk.png");
            SkyboxFacesPath.Add(projectPath + "\\Textures\\Sky\\cloudtop_ft.png");
            skyboxTex = new TextureCubeMap(SkyboxFacesPath,22 );
          

            float size = 900000f;
            float[] skyboxVertices = {
                // Positions
                -size,  size, -size,
                -size, -size, -size,
                size, -size, -size,
                size, -size, -size,
                size,  size, -size,
                -size,  size, -size,

                -size, -size,  size,
                -size, -size, -size,
                -size,  size, -size,
                -size,  size, -size,
                -size,  size,  size,
                -size, -size,  size,

                size, -size, -size,
                size, -size,  size,
                size,  size,  size,
                size,  size,  size,
                size,  size, -size,
                size, -size, -size,

                -size, -size,  size,
                -size,  size,  size,
                size,  size,  size,
                size,  size,  size,
                size, -size,  size,
                -size, -size,  size,

                -size,  size, -size,
                size,  size, -size,
                size,  size,  size,
                size,  size,  size,
                -size,  size,  size,
                -size,  size, -size,

                -size, -size, -size,
                -size, -size,  size,
                size, -size, -size,
                size, -size, -size,
                -size, -size,  size,
                size, -size,  size
            };
            SkyboxPositionsBufferID = GPU.GenerateBuffer(skyboxVertices);

            cam = new Camera();
            cam.Reset(0, 0, 0, 0, 0, 0, 0, 1, 0);

            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();
            ScaleMatrix = glm.scale(new mat4(1), new vec3(1, 1, 1));


            SkyboxShader.UseShader();


            SkyboxModelMatrixID = Gl.glGetUniformLocation(SkyboxShader.ID, "trans");
            SkyboxProjectionMatrixID = Gl.glGetUniformLocation(SkyboxShader.ID, "projection");
            SkyboxViewMatrixID = Gl.glGetUniformLocation(SkyboxShader.ID, "view");


            SkyBoxDataID = Gl.glGetUniformLocation(SkyboxShader.ID, "data");
            int LightPositionID = Gl.glGetUniformLocation(SkyboxShader.ID, "LightPosition_worldspace");
            vec3 lightPosition = new vec3(0, 0, 0);
            Gl.glUniform3fv(LightPositionID, 1, lightPosition.to_array());
            SkyBoxAmbientLightID = Gl.glGetUniformLocation(SkyboxShader.ID, "ambientLight");
            SkyBoxEyePositionID = Gl.glGetUniformLocation(SkyboxShader.ID, "EyePosition_worldspace");

            TerrainShader.UseShader();
            TerrainModelMatrixID = Gl.glGetUniformLocation(TerrainShader.ID, "model");
            TerrainProjectionMatrixID = Gl.glGetUniformLocation(TerrainShader.ID, "projection");
            TerrainViewMatrixID = Gl.glGetUniformLocation(TerrainShader.ID, "view");

            Gl.glUniform1i(Gl.glGetUniformLocation(TerrainShader.ID, "Sand"), 1);
            Gl.glUniform1i(Gl.glGetUniformLocation(TerrainShader.ID, "Rock"), 2);
            Gl.glUniform1i(Gl.glGetUniformLocation(TerrainShader.ID, "Grass"), 3);
            Gl.glUniform1i(Gl.glGetUniformLocation(TerrainShader.ID, "Ice"), 4);




            TerrainDataID = Gl.glGetUniformLocation(TerrainShader.ID, "data");
            LightPositionID = Gl.glGetUniformLocation(TerrainShader.ID, "LightPosition_worldspace");
            lightPosition = new vec3(0, 0, 0);
            Gl.glUniform3fv(LightPositionID, 1, lightPosition.to_array());
            TerrainAmbientLightID = Gl.glGetUniformLocation(TerrainShader.ID, "ambientLight");
            TerrainEyePositionID = Gl.glGetUniformLocation(TerrainShader.ID, "EyePosition_worldspace");

            SimpleShader.UseShader();
            Gl.glUniform1i(Gl.glGetUniformLocation(SimpleShader.ID, "myTextureSampler"), 5);
            SimpleModelMatrixID = Gl.glGetUniformLocation(SimpleShader.ID, "model");
            SimpleProjectionMatrixID = Gl.glGetUniformLocation(SimpleShader.ID, "projection");
            SimpleViewMatrixID = Gl.glGetUniformLocation(SimpleShader.ID, "view");



            SimpleDataID = Gl.glGetUniformLocation(SimpleShader.ID, "data");
            LightPositionID = Gl.glGetUniformLocation(SimpleShader.ID, "LightPosition_worldspace");
            lightPosition = new vec3(0, 0, 0);
            Gl.glUniform3fv(LightPositionID, 1, lightPosition.to_array());
            SimpleAmbientLightID = Gl.glGetUniformLocation(SimpleShader.ID, "ambientLight");
            SimpleEyePositionID = Gl.glGetUniformLocation(SimpleShader.ID, "EyePosition_worldspace");





            TreesShader.UseShader();

            TreesModelMatrixID = Gl.glGetUniformLocation(TreesShader.ID, "model");
            TreesProjectionMatrixID = Gl.glGetUniformLocation(TreesShader.ID, "projection");
            TreesViewMatrixID = Gl.glGetUniformLocation(TreesShader.ID, "view");




            TreesDataID = Gl.glGetUniformLocation(TreesShader.ID, "data");
            LightPositionID = Gl.glGetUniformLocation(TreesShader.ID, "LightPosition_worldspace");
            lightPosition = new vec3(0, 0, 0);
            Gl.glUniform3fv(LightPositionID, 1, lightPosition.to_array());
            TreesAmbientLightID = Gl.glGetUniformLocation(TreesShader.ID, "ambientLight");
            TreesEyePositionID = Gl.glGetUniformLocation(TreesShader.ID, "EyePosition_worldspace");

          //  Gl.glUniform1i(Gl.glGetUniformLocation(TreesShader.ID, "myTextureSampler"), 9);



            Gl.glClearColor(0,0,0.4f,1);
            Gl.glEnable(Gl.GL_DEPTH_TEST);

        }
        public void Draw()
        {

            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glDepthFunc(Gl.GL_LESS);


            SkyboxShader.UseShader();
            skyboxTex.Bind();


            Gl.glUniformMatrix4fv(SkyboxViewMatrixID, 1, Gl.GL_FALSE, ViewMatrix.to_array());
            Gl.glUniformMatrix4fv(SkyboxProjectionMatrixID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());
            Gl.glUniformMatrix4fv(SkyboxModelMatrixID, 1, Gl.GL_FALSE, ScaleMatrix.to_array());
            Gl.glUniform3fv(SkyBoxEyePositionID, 1, cam.GetCameraPosition().to_array());




            vec3 ambientLight = new vec3(Red, Green, Blue);
            Gl.glUniform3fv(SkyBoxAmbientLightID, 1, ambientLight.to_array());
            vec2 data = new vec2(50, 50);
            Gl.glUniform2fv(SkyBoxDataID, 1, data.to_array());
           



            if (mode%2 == 0)
            {
                Red -= 0.0003f;
                Green -= 0.0003f;
                Blue -= 0.0003f;
                if (Blue <= 0.02f)
                {
                    ++mode;
                }
            }
            else
            {

                Red += 0.0003f;
                Green += 0.0003f;
                Blue += 0.0003f;
                if (Blue > 1)
                {
                    ++mode;
                }
            }

            GPU.BindBuffer(SkyboxPositionsBufferID);
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 3 * sizeof(float), IntPtr.Zero);
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 36);


            TerrainShader.UseShader();

            Gl.glUniform3fv(TerrainAmbientLightID, 1, ambientLight.to_array());
            Gl.glUniform2fv(TerrainDataID, 1, data.to_array());

            Gl.glUniformMatrix4fv(TerrainModelMatrixID, 1, Gl.GL_FALSE, ScaleMatrix.to_array());
            Gl.glUniformMatrix4fv(TerrainProjectionMatrixID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());
            Gl.glUniformMatrix4fv(TerrainViewMatrixID, 1, Gl.GL_FALSE, ViewMatrix.to_array());
            Gl.glUniform3fv(TerrainEyePositionID, 1, cam.GetCameraPosition().to_array());


            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1); 
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, Texture.TexturesHandles[1]);

            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, Texture.TexturesHandles[2]);

            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, Texture.TexturesHandles[3]);

            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 4);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, Texture.TexturesHandles[4]);


            Terrain.Draw(TerrainModelMatrixID, glm.scale(new mat4(1), new vec3(1, 1, 1)), new mat4(1), glm.translate(new mat4(1), new vec3(0,0 , 0)));



            int p = 0;

            TreesShader.UseShader();
            Gl.glUniform3fv(TreesAmbientLightID, 1, ambientLight.to_array());
            Gl.glUniform2fv(TreesDataID, 1, data.to_array());

            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 9);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, Texture.TexturesHandles[9]);
            Gl.glUniformMatrix4fv(TreesViewMatrixID, 1, Gl.GL_FALSE, ViewMatrix.to_array());
            Gl.glUniformMatrix4fv(TreesProjectionMatrixID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());
            Gl.glUniformMatrix4fv(TreesModelMatrixID, 1, Gl.GL_FALSE, ScaleMatrix.to_array());
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {


                    if (p == 0)
                        Trees[p].transmatrix = glm.translate(new mat4(1), new vec3((i * 300000) , -700000, (j * 400000) - 50000));
                    else if(p == 1)
                        Trees[p].transmatrix = glm.translate(new mat4(1), new vec3((i * 300000) + 200000, -630000, (j * 400000) - 50000));
                    else if( p == 2 )
                        Trees[p].transmatrix = glm.translate(new mat4(1), new vec3((i * 300000) + 200000, -550000, (j * 400000) - 50000));
                    else if(p == 3)
                        Trees[p].transmatrix = glm.translate(new mat4(1), new vec3((i * 300000) + 200000, -500000, (j * 400000) - 200000));
                    else if(p == 4)
                        Trees[p].transmatrix = glm.translate(new mat4(1), new vec3((i * 300000) + 200000, -600000, (j * 400000) + 20000));
                    else if(p == 5)
                        Trees[p].transmatrix = glm.translate(new mat4(1), new vec3((i * 300000) + 200000, -600000, (j * 400000) - 90000));
                    else if(p == 6)
                        Trees[p].transmatrix = glm.translate(new mat4(1), new vec3((i * 300000) + 200000, -550000, (j * 400000) + 20000));
                    else if(p == 7)
                        Trees[p].transmatrix = glm.translate(new mat4(1), new vec3((i * 300000) + 200000, -700000, (j * 400000) + 20000));
                    else if(p == 8)
                        Trees[p].transmatrix = glm.translate(new mat4(1), new vec3((i * 300000) + 200000, -550000, (j * 400000) + 20000));
                    else if(p == 9)
                        Trees[p].transmatrix = glm.translate(new mat4(1), new vec3((i * 300000) - 1000000, -700000, (j * 400000) - 300000));
                    else if(p == 10)
                        Trees[p].transmatrix = glm.translate(new mat4(1), new vec3((i * 300000) - 950000, -600000, (j * 400000)+50000 ));
                    else if(p==11)
                        Trees[p].transmatrix = glm.translate(new mat4(1), new vec3((i * 300000) - 1300000, -600000, (j * 400000) - 100000));

                    Trees[p].Draw(TreesModelMatrixID);
                                       
                    ++p;
                }
            }
            



            SimpleShader.UseShader();



            Gl.glUniform3fv(SimpleAmbientLightID, 1, ambientLight.to_array());
            Gl.glUniform2fv(SimpleDataID, 1, data.to_array());
            Gl.glUniformMatrix4fv(SimpleViewMatrixID, 1, Gl.GL_FALSE, ViewMatrix.to_array());
            Gl.glUniformMatrix4fv(SimpleProjectionMatrixID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());
            Gl.glUniformMatrix4fv(SimpleModelMatrixID, 1, Gl.GL_FALSE, ScaleMatrix.to_array());
            Gl.glUniform3fv(SimpleEyePositionID, 1, cam.GetCameraPosition().to_array());

            

             p = 0;
            int counter = 0;
          //  MessageBox.Show(GrassPointer.ToString());
            for (int i = 0; i < 35; ++i)
            {
                for (int j = 0; j < 34; ++j)
                {


                    ++p;
                    if (p%3 ==0 || p % 5 == 0 || p%8 == 0 || p%11 == 0 || p%14 ==0) continue;
                    ++counter;
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 5);
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, Texture.TexturesHandles[5]);
                    
                    if (counter % 9 == 0 || counter % 7 == 0)
                    {

                        GrassTexture1.Bind();

                    }
                    else if (counter % 5 == 0 || counter % 6 == 0)
                    {

                        GrassTexture2.Bind();
                    }
                    else if (counter % 8 == 0 || counter % 3 == 0)
                    {


                        GrassTexture3.Bind();
                    }
                    else
                    {

                        GrassTexture4.Bind();

                    }
                  
                    if(counter%2==0)
                        GrassList[i * 17 + j].Draw(SimpleModelMatrixID, glm.scale(new mat4(1), new vec3(0.1f, 0.1f, 0.1f)), new mat4(1), glm.translate(new mat4(1), new vec3((i * 40000) - 390000, -500000, (j * 20000) - 40000)));
                    else
                        GrassList[i * 17 + j].Draw(SimpleModelMatrixID, glm.scale(new mat4(1), new vec3(0.1f, 0.1f, 0.1f)), new mat4(1), glm.translate(new mat4(1), new vec3((i * 40000) - 390000, -500000, (j * 20000) - 40000)));
                    
               }
            }
           
          

        }
        public void Update(float deltaTime)
        {
            cam.UpdateViewMatrix();
            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();            
        }
     
        public void CleanUp()
        {
            SimpleShader.DestroyShader();
        }
    }
}

