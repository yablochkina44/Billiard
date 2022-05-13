using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

namespace CG_OpenGL
{
    public class Game : GameWindow
    {
        Vector3 LightPos = new Vector3(0.0f, 2.6f, 0.0f);
        Shader shader;
        

        Texture DiffuseBoll1, SpecularBoll1;
        Texture DiffuseBoll2, SpecularBoll2;
        Texture DiffuseBoll3, SpecularBoll3;

        List<ObjectRender> ObjectRenderList = new List<ObjectRender>();


        double Time;
        int Side = 1;
        const double Degrees = 40;

        //float[] vertices = {
        //    -0.5f, -0.5f, 0.0f, //Bottom-left vertex
        //     0.5f, -0.5f, 0.0f, //Bottom-right vertex
        //     0.0f,  0.5f, 0.0f  //Top vertex
        //};
        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
           : base(gameWindowSettings, nativeWindowSettings) { }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused) // Check to see if the window is focused
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }


        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Time += 35.0 * e.Time * Side * Math.Cos(Time / 30);

            if (Math.Abs(Time) > Degrees) Side *= -1;

            var RotationMatrixZ = Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(Time));
            var RotationMatrixY = Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(90));
            var TranslationMatrix = Matrix4.CreateTranslation(0, 0, (float)(Time / 80));

            var model = Matrix4.Identity * RotationMatrixZ * TranslationMatrix * RotationMatrixY;

            foreach (var Obj in ObjectRenderList)
            {
                Obj.Bind();
                Obj.ApplyTexture();
                Obj.UpdateShaderModel(model);
                Obj.ShaderAttribute();
                Obj.Render();
            }

            SwapBuffers();
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            //запускается каждый раз как меняется окно
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
            
        }
        private void DefineShader(Shader Shader)
        {
            Shader.SetInt("material.diffuse", 0);
            Shader.SetInt("material.specular", 1);
            Shader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
            Shader.SetFloat("material.shininess", 100000.0f);
            Shader.SetVector3("light.position", LightPos);
            Shader.SetFloat("light.constant", 0.1f);
            Shader.SetFloat("light.linear", 0.09f);
            Shader.SetFloat("light.quadratic", 0.032f);
            Shader.SetVector3("light.ambient", new Vector3(0.2f));
            Shader.SetVector3("light.diffuse", new Vector3(0.5f));
            Shader.SetVector3("light.specular", new Vector3(1.0f));
            Shader.Use();
        }
        protected override void OnLoad()
        {

           base.OnLoad();
           GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);//цвет окна
           GL.Enable(EnableCap.DepthTest);
            ////VertexBufferObject = GL.GenBuffer();//генерация индетиф буфера и запись в переменную для хранения дескриптора

            //Sphere Butt = new Sphere(0.2f, 0.0f, 0.5f, 0f);
            Sphere Boll1 = new Sphere(0.05f, 0.0f, 0.0f, 0f);
            Sphere Boll2 = new Sphere(0.05f, 0.0f, -0.5f, 0f);
            Sphere Boll3 = new Sphere(0.05f, -0.5f, 0.0f, 0f);

            shader = new Shader(@"C:/Users/nnatu/source/repos/CG_OpenGL/shader/shader.vert", @"C:/Users/nnatu/source/repos/CG_OpenGL/shader/lighting.frag");
            DefineShader(shader);

            DiffuseBoll1 = Texture.LoadFromFile(@"C:/Users/nnatu/source/repos/CG_OpenGL/resourse/red.jpg");
            SpecularBoll1 = Texture.LoadFromFile(@"C:/Users/nnatu/source/repos/CG_OpenGL/resourse/red_specular.jpg");
            DiffuseBoll2 = Texture.LoadFromFile(@"C:/Users/nnatu/source/repos/CG_OpenGL/resourse/red.jpg");
            SpecularBoll2 = Texture.LoadFromFile(@"C:/Users/nnatu/source/repos/CG_OpenGL/resourse/red_specular.jpg");
            DiffuseBoll3 = Texture.LoadFromFile(@"C:/Users/nnatu/source/repos/CG_OpenGL/resourse/red.jpg");
            SpecularBoll3 = Texture.LoadFromFile(@"C:/Users/nnatu/source/repos/CG_OpenGL/resourse/red_specular.jpg");

            //DiffuseTail = Texture.LoadFromFile("../../../Resources/head.jpg");
            //SpecularTail = Texture.LoadFromFile("../../../Resources/head_specular.jpg");

            // var ButtVert = Butt.GetAll(); var ButtInd = Butt.GetIndices();
            var Vert_1 = Boll1.GetAll(); var Ind_1 = Boll1.GetIndices();
            var Vert_2 = Boll2.GetAll(); var Ind_2 = Boll2.GetIndices();
            var Vert_3 = Boll3.GetAll(); var Ind_3 = Boll3.GetIndices();

            //ObjectRenderList.Add(new ObjectRender(ButtVert, ButtInd, Shader, DiffuseTail, SpecularTail));
          
            ObjectRenderList.Add(new ObjectRender(Vert_1, Ind_1, shader, DiffuseBoll1, SpecularBoll1));
            ObjectRenderList.Add(new ObjectRender(Vert_2, Ind_2, shader, DiffuseBoll2, SpecularBoll2));
            ObjectRenderList.Add(new ObjectRender(Vert_3, Ind_3, shader, DiffuseBoll3, SpecularBoll3));
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
        }

    }
}
