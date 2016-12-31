﻿using System;
using System.Runtime.InteropServices;

namespace GraphicsExample
{
    public class Program
    {
        private const int GL_ARRAY_BUFFER = 0x8892;
        private const int GL_STATIC_DRAW = 0x88E4;
        private const int GL_FLOAT = 0x1406;
        private const int GL_TRIANGLES = 0x0004;
        private const string OPENGL_DLL = "opengl32";
        private const string GLFW_DLL = "glfw";

        // Triangle vertices
        private static float[] vertices = {
            -0.5f, -0.5f,
            0.5f, -0.5f,
            0.0f,  0.5f,
        };

        public static void Main(string[] args)
        {
            Initialise();
            var window = CreateWindow(1024, 768, ".NET Core Graphics Example", IntPtr.Zero, IntPtr.Zero);
            MakeContextCurrent(window);
            LoadFunctionPointers();
            // Create the VAO
            uint vao = 0;
            GenVertexArrays(1, ref vao);
            BindVertexArray(vao);
            // Create the VBO
            uint vbo = 0;
            GenBuffers(1, ref vbo);
            BindBuffer(GL_ARRAY_BUFFER, vbo);
            BufferData(GL_ARRAY_BUFFER, new IntPtr(sizeof(float) * vertices.Length), vertices, GL_STATIC_DRAW);
            // Draw the Triangle
            EnableVertexAttribArray(0);
            VertexAttribPointer(0, 2, GL_FLOAT, false, 0, IntPtr.Zero);
            DrawArrays(GL_TRIANGLES, 0, 3);
            SwapBuffers(window);
            // Wait for input in console
            Console.ReadLine();
        }

        // OpenGL Bindings
        [DllImport(OPENGL_DLL, EntryPoint = "glDrawArrays")] private static extern void DrawArrays(int mode, int first, int count);
        private delegate void glGenBuffers(int n, ref uint buffers);
        private delegate void glBindBuffer(uint target, uint buffer);
        private delegate void glBufferData(uint target, IntPtr size, float[] data, uint usage);
        private delegate void glEnableVertexAttribArray(uint index);
        private delegate void glVertexAttribPointer(uint indx, int size, uint type, bool normalized, int stride, IntPtr ptr);
        private delegate void glGenVertexArrays(int n, ref uint arrays);
        private delegate void glBindVertexArray(uint array);
        private static glGenBuffers GenBuffers;
        private static glBindBuffer BindBuffer;
        private static glBufferData BufferData;
        private static glEnableVertexAttribArray EnableVertexAttribArray;
        private static glVertexAttribPointer VertexAttribPointer;
        private static glGenVertexArrays GenVertexArrays;
        private static glBindVertexArray BindVertexArray;

        // GLFW Bindings
        [DllImport(GLFW_DLL, EntryPoint = "glfwInit")] private static extern bool Initialise();
        [DllImport(GLFW_DLL, EntryPoint = "glfwCreateWindow")] private static extern IntPtr CreateWindow(int width, int height, string title, IntPtr monitor, IntPtr share);
        [DllImport(GLFW_DLL, EntryPoint = "glfwMakeContextCurrent")] private static extern void MakeContextCurrent(IntPtr window);
        [DllImport(GLFW_DLL, EntryPoint = "glfwSwapBuffers")] private static extern void SwapBuffers(IntPtr window);
        [DllImport(GLFW_DLL, EntryPoint = "glfwGetProcAddress")] private static extern IntPtr GetProcAddress(string procname);

        private static T GetMethod<T>()
        {
            var funcPtr = GetProcAddress(typeof(T).Name);
            if (funcPtr == IntPtr.Zero)
            {
                Console.WriteLine($"Unable to load Function Pointer: {typeof(T).Name}");
                return default(T);
            }
            return Marshal.GetDelegateForFunctionPointer<T>(funcPtr);
        }

        private static void LoadFunctionPointers()
        {
            GenBuffers = GetMethod<glGenBuffers>();
            BindBuffer = GetMethod<glBindBuffer>();
            BufferData = GetMethod<glBufferData>();
            EnableVertexAttribArray = GetMethod<glEnableVertexAttribArray>();
            VertexAttribPointer = GetMethod<glVertexAttribPointer>();
            GenVertexArrays = GetMethod<glGenVertexArrays>();
            BindVertexArray = GetMethod<glBindVertexArray>();
        }
    }
}
