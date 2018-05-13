using System;
using Tao.FreeGlut;
using OpenGL;

namespace OpenGLTutorial4
{
    class Program
    {
        private static int width = 1280, height = 720;
        private static ShaderProgram program;
        private static VBO<Vector3> triangle;
        private static VBO<Vector3> triangleColor;
        private static VBO<int> triangleElements;
        private static System.Diagnostics.Stopwatch watch;
        private static float angle;
        private static float angle2;

        private static VBO<Vector3> triangle2;
        private static VBO<Vector3> triangleColor2;
        private static VBO<int> triangleElements2;

        static void Main(string[] args)
        {
            // create an OpenGL window
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
            Glut.glutInitWindowSize(width, height);
            Glut.glutCreateWindow("OpenGL Tutorial");

            // provide the Glut callbacks that are necessary for running this tutorial
            Glut.glutIdleFunc(OnRenderFrame);
            Glut.glutDisplayFunc(OnDisplay);
            Glut.glutCloseFunc(OnClose);

            // enable depth testing to ensure correct z-ordering of our fragments
            Gl.Enable(EnableCap.DepthTest);

            // compile the shader program
            program = new ShaderProgram(VertexShader, FragmentShader);

            // set the view and projection matrix, which are static throughout this tutorial
            program.Use();
            program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 1000f));
            program["view_matrix"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 10), Vector3.Zero, new Vector3(0, 1, 0)));

            // create a triangle with vertices and colors
            triangle = new VBO<Vector3>(new Vector3[] { new Vector3(0, 2, 0), new Vector3(-1, 0, 0), new Vector3(1, 0, 0) });
            triangleColor = new VBO<Vector3>(new Vector3[] { new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1) });
            triangleElements = new VBO<int>(new int[] { 0, 1, 2 }, BufferTarget.ElementArrayBuffer);

            triangle2 = new VBO<Vector3>(new Vector3[] { new Vector3(-1, 0, 0), new Vector3(0, -2, 0), new Vector3(1, 0, 0) });
            triangleColor2 = new VBO<Vector3>(new Vector3[] { new Vector3(0, 0, 1), new Vector3(1, 0, 0), new Vector3(0, 1, 0) });
            triangleElements2 = new VBO<int>(new int[] { 0, 1, 2 }, BufferTarget.ElementArrayBuffer);

            watch = System.Diagnostics.Stopwatch.StartNew();

            Glut.glutMainLoop();
        }

        private static void OnClose()
        {
            // dispose of all of the resources that were created
            triangle.Dispose();
            triangleColor.Dispose();
            triangleElements.Dispose();

            triangle2.Dispose();
            triangleColor2.Dispose();
            triangleElements2.Dispose();

            program.DisposeChildren = true;
            program.Dispose();
        }

        private static void OnDisplay()
        {

        }

        private static void OnRenderFrame()
        {
            // calculate how much time has elapsed since the last frame
            watch.Stop();
            float deltaTime = (float)watch.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency;
            watch.Restart();

            // use the deltaTime to adjust the angle of the cube and pyramid
            angle += deltaTime;
            angle2 -= deltaTime;

            // set up the OpenGL viewport and clear both the color and depth bits
            Gl.Viewport(0, 0, width, height);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // use our shader program
            Gl.UseProgram(program);

            // transform the triangle
            program["model_matrix"].SetValue(Matrix4.CreateRotationY(angle) * Matrix4.CreateTranslation(new Vector3(0, 0, 0)));

            // bind the vertex positions, colors and elements of the triangle
            Gl.BindBufferToShaderAttribute(triangle, program, "vertexPosition");
            Gl.BindBufferToShaderAttribute(triangleColor, program, "vertexColor");
            Gl.BindBuffer(triangleElements);

            // draw the triangle
            Gl.DrawElements(BeginMode.Triangles, triangleElements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // bind the vertex positions, colors and elements of the triangle
            program["model_matrix"].SetValue(Matrix4.CreateRotationY(-2*angle2) * Matrix4.CreateTranslation(new Vector3(0, 0, 0)));
            Gl.BindBufferToShaderAttribute(triangle2, program, "vertexPosition");
            Gl.BindBufferToShaderAttribute(triangleColor2, program, "vertexColor");
            Gl.BindBuffer(triangleElements);

            // draw the triangle
            Gl.DrawElements(BeginMode.Triangles, triangleElements2.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

            Glut.glutSwapBuffers();
        }

        public static string VertexShader = @"
#version 130

in vec3 vertexPosition;
in vec3 vertexColor;

out vec3 color;

uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;

void main(void)
{
    color = vertexColor;
    gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);
}
";

        public static string FragmentShader = @"
#version 130

in vec3 color;

out vec4 fragment;

void main(void)
{
    fragment = vec4(color, 1);
}
";
    }
}
