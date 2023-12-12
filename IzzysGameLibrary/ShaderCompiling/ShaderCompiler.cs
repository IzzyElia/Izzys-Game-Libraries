using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection.Metadata.Ecma335;
using System.Timers;
using Izzy;

namespace IzzysGameLibrary.ShaderCompiling
{
    /// <summary>
    /// Handles compiling shaders. Note that the compiled shaders will need to be shipped with the
    /// game. They cannot be compiled in release builds.
    /// </summary>
    public static class ShaderCompiler
    {
        static ShaderCompiler()
        {
            ReadShaderCompileLog();
        }
        static Dictionary<string, long> shaderCompileLog = new Dictionary<string, long>();
        static string shaderCompileLogFileName = "shader-compile-log.txt";
        /// <summary>
        /// Saves the shader compile log, a text file which tracks what shaders were compiled when.
        /// </summary>
        static void SaveShaderCompileLog()
        {
            StreamWriter compileLog = File.CreateText(Path.Combine(shadersSourcePath, shaderCompileLogFileName));
            compileLog.WriteLine(platform.ToString());
            foreach (KeyValuePair<string, long> kvp in shaderCompileLog)
            {
                compileLog.WriteLine($"{kvp.Key}:{kvp.Value}");
            }
            compileLog.Close();
        }
        /// <summary>
        /// Reads the shader compile log and updates the internal dictionary from it.
        /// The shader compile log tracks when each shader was last compiled
        /// </summary>
        static void ReadShaderCompileLog()
        {
            string compileLogPath = Path.Combine(shadersSourcePath, shaderCompileLogFileName);
            if (!File.Exists(compileLogPath))
                return;

            StreamReader compileLog = File.OpenText(compileLogPath);

            // If the shaders were compiled under a different platform, ignore the compile log (thus triggering the recompile of all shaders)
            if (Enum.TryParse(typeof(Platform), compileLog.ReadLine(), out object result))
            {
                if ((Platform)result != platform)
                    return;
            }
            else
            {
                return;
            }

            while (!compileLog.EndOfStream)
            {
                string line = compileLog.ReadLine();
                if (line.Length >= 3)
                {
                    string[] split = line.Split(':');
                    string shaderFileName = split[0];
                    string shaderFileLastCompile = split[1];
                    if (long.TryParse(shaderFileLastCompile, out long lastCompile))
                    {
                        if (shaderCompileLog.ContainsKey(shaderFileName))
                        {
                            shaderCompileLog[shaderFileName] = lastCompile;
                        }
                        else
                        {
                            shaderCompileLog.Add(shaderFileName, lastCompile);
                        }
                    }
                    else
                    {
                        DynamicLogger.LogWarning($"Issue reading shader compile log: Time of last compile {shaderFileLastCompile} could not be read");
                        continue;
                    }
                }
            }
            compileLog.Close();
        }

        // Core class
        static Platform platform = Platform.OpenGL;
        const string mbfxcFilePath = "C:\\Users\\emmie\\.nuget\\packages\\dotnet-mgcb-editor-windows\\3.8.1.303\\tools\\net6.0\\any\\mgcb-editor-windows-data\\mgfxc.exe";
        public const string shadersSourcePath = "C:\\Users\\emmie\\Documents\\Development\\Final Days of the Empire\\Shaders";
        public const string compiledShadersSourcePath = "C:\\Users\\emmie\\Documents\\Development\\Final Days of the Empire\\ShadersCompiled";
        static bool compilingAll = false; // Set to true when compiling all shaders to tell the compile shader method to hold off on saving the compile log
        public static void CompileShader(string source)
        {
#if DEBUG

            string sourceFullPath = GetShaderSourceFilePath(source);
            string outputFullPath = GetShaderOutputFilePath(sourceFullPath);
            Directory.CreateDirectory(Path.GetDirectoryName(outputFullPath));
            string profile;
            switch (platform)
            {
                case Platform.OpenGL:
                    profile = "OpenGL";
                    break;
                case Platform.DirectX_11:
                    profile = "DirectX_11";
                    break;
                default:
                    throw new UnsupportedShaderPlatformException(platform.ToString());
            }
            string command = $"\"{sourceFullPath}\" \"{outputFullPath}\" /Profile:{profile}";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Process compilerProcess = new Process();
            compilerProcess.StartInfo = new ProcessStartInfo
            {
                FileName = mbfxcFilePath,
                Arguments = command,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true

            };
            compilerProcess.Start();
            compilerProcess.WaitForExit();
            while (!compilerProcess.StandardOutput.EndOfStream)
            {
                DynamicLogger.Log(compilerProcess.StandardOutput.ReadLine());
            }
            while (!compilerProcess.StandardError.EndOfStream)
            {
                DynamicLogger.LogWarning(compilerProcess.StandardError.ReadLine());
            }
            stopwatch.Stop();
            Console.WriteLine($"Compiled {source} in {stopwatch.Elapsed.TotalMilliseconds}ms");
            LogCompile(new FileInfo(sourceFullPath));
            if (!compilingAll)
            {
                SaveShaderCompileLog();
            }
#elif RELEASE
            Console.WriteLine("Shader compiler not available in release builds");
#endif
        }
        static string GetShaderSourceFilePath(string source) => Path.Combine(shadersSourcePath, source + ".fx");
        static string GetShaderOutputFilePath(string source)
        {
            string extension;
            switch (platform)
            {
                case Platform.OpenGL:
                    extension = ".opengl_shader";
                    break;
                case Platform.DirectX_11:
                    extension = ".dx11_shader";
                    break;
                default:
                    throw new UnsupportedShaderPlatformException(platform.ToString());
            }

            string outputFileName = GetFileName(source, false) + extension;
            return Path.Combine(compiledShadersSourcePath, outputFileName);
        }
        public static void CompileAllShaders(bool ignoreAlreadyCompiledShaders)
        {
            compilingAll = true;
            foreach (string file in Directory.GetFiles(shadersSourcePath))
            {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Extension == ".fx")
                {
                    if (ignoreAlreadyCompiledShaders && !DoesShaderNeedRecompile(fileInfo))
                        return;
                    CompileShader($"{GetFileName(fileInfo.Name, false)}");
                }
            }
            compilingAll = false;
            SaveShaderCompileLog();
        }
        public static Effect LoadShader(string source, GraphicsDevice graphicsDevice, bool forceRecompile = false)
        {
#if DEBUG
            FileInfo shaderSourceFile = new FileInfo(GetShaderSourceFilePath(source));
            if (forceRecompile || DoesShaderNeedRecompile(shaderSourceFile))
                if (File.Exists(shaderSourceFile.FullName))
                    CompileShader(source);
                else
                    throw new InvalidOperationException($"Shader source file for '{source}' not found at '{shaderSourceFile.FullName}'");
#endif

            string shaderFilePath = GetShaderOutputFilePath(source);
            if (!File.Exists(shaderFilePath))
                throw new InvalidOperationException($"Shader '{source}' not found at '{shaderFilePath}'");
            byte[] shaderData = File.ReadAllBytes(shaderFilePath);
            Effect shader = new Effect(graphicsDevice, shaderData);
            return shader;
        }

        // Utility Functions
        static string GetFileName(string file, bool includeExtention = false)
        {
            string[] split = file.Split('\\');
            string nameWithExtension = split[split.Length - 1];
            if (includeExtention)
            {
                return nameWithExtension;
            }
            else
            {
                string nameWithoutExtension = nameWithExtension.Split('.')[0];
                return nameWithoutExtension;
            }

        }
        static void LogCompile(FileInfo shaderFile)
        {
            string fileName = GetFileName(shaderFile.FullName, true);
            DateTime lastModified = shaderFile.LastWriteTimeUtc;
            if (shaderCompileLog.ContainsKey(fileName))
            {
                shaderCompileLog[fileName] = lastModified.Ticks;
            }
            else
            {
                shaderCompileLog.Add(fileName, lastModified.Ticks);
            }
        }
        static bool DoesShaderNeedRecompile(FileInfo file)
        {
            long lastCompiled = shaderCompileLog.GetValueOrDefault(file.Name, 0);
            long lastModified = file.LastWriteTimeUtc.Ticks;
            return lastModified > lastCompiled;
        }


        enum Platform
        {
            OpenGL,
            DirectX_11
        }
    }
}
