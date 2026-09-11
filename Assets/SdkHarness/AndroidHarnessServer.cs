#if UNITY_ANDROID
using System;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Unity.Pipeline;
using Unity.Pipeline.Config;
using UnityEngine;

namespace AdaptyExample.SdkHarness
{
    /// <summary>
    /// The Pipeline runtime server for an Android device.
    /// </summary>
    /// <remarks>
    /// The stock server writes its descriptor next to <c>Application.dataPath</c>, which on Android
    /// is the APK, so the write throws and the server stops itself before it has served anything.
    /// This one writes the descriptor into the app's internal files directory, with a token of its
    /// own (the package's token manager is internal). Read it with
    /// <c>adb shell run-as com.adaptytest cat files/.unity-pipeline-runtime-port</c> (a Development
    /// build is debuggable), put it in a directory on the host as <c>--runtime-path</c>, and bridge
    /// the port with <c>adb forward tcp:7900 tcp:7900</c>.
    /// </remarks>
    internal sealed class AndroidHarnessServer : RuntimePipelineServer
    {
        private const string DescriptorFileName = ".unity-pipeline-runtime-port";

        private readonly string m_DescriptorPath;
        private readonly string m_Token = NewToken();
        private DateTime m_StartedAt;
        private DateTime m_LastHeartbeat;
        private bool m_HasDescriptor;

        internal AndroidHarnessServer(RuntimePipelineConfig config, string filesDirectory)
            : base(config)
        {
            m_DescriptorPath = Path.Combine(filesDirectory, DescriptorFileName);
        }

        public override DateTime StartedAt => m_StartedAt;

        protected override void CreateInstanceDescriptor()
        {
            m_StartedAt = DateTime.UtcNow;
            m_LastHeartbeat = m_StartedAt;

            // The same shape the package writes, so the CLI reads it as any other runtime. The
            // capabilities are BasePipelineServer.Capabilities, which is internal, restated.
            var descriptor = new
            {
                capabilities = new[] { "exec.argv", "exec.commandLine" },
                pid = System.Diagnostics.Process.GetCurrentProcess().Id,
                port = Port,
                platform = Application.platform.ToString(),
                unityVersion = Application.unityVersion,
                buildGuid = Application.buildGUID,
                startedAt = m_StartedAt,
                lastHeartbeat = m_LastHeartbeat,
                workingDirectory = Directory.GetCurrentDirectory(),
                evalToken = m_Token,
            };

            File.WriteAllText(m_DescriptorPath, JsonConvert.SerializeObject(descriptor, Formatting.Indented));
            m_HasDescriptor = true;
            Debug.Log($"SdkHarness: runtime descriptor written to {m_DescriptorPath}");
        }

        protected override void DeleteInstanceDescriptor()
        {
            m_HasDescriptor = false;
            File.Delete(m_DescriptorPath);
        }

        // The host reads a pulled copy, so rewriting the file on the device buys nothing.
        protected override void UpdateHeartBeat() => m_LastHeartbeat = DateTime.UtcNow;

        protected override object GetServerStatus()
        {
            UpdateHeartBeat();
            return new
            {
                status = m_HasDescriptor ? "ready" : "error",
                lastHeartbeat = m_LastHeartbeat,
            };
        }

        protected override string GetToken() => m_Token;

        private static string NewToken()
        {
            var bytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }
    }

    /// <summary>
    /// Keeps the package's driver from starting its server and runs
    /// <see cref="AndroidHarnessServer"/> in its place, pumped the way the driver would.
    /// </summary>
    internal sealed class AndroidHarnessServerHost : MonoBehaviour
    {
        private AndroidHarnessServer m_Server;
        private int m_MaxWorkItemsPerFrame;

        // AfterSceneLoad runs once the bootstrap (BeforeSceneLoad) has created the driver, and
        // before the driver's Start, which is where it would start the stock server.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            var driver = RuntimePipelineBootstrap.Instance;
            if (driver == null)
            {
                Debug.LogError("SdkHarness: no Pipeline runtime driver - Enable In Builds is off in Project Settings > Pipeline > Runtime");
                return;
            }

            driver.Config.autoStart = false;

            var config = Instantiate(driver.Config);
            config.autoStart = true;

            var host = new GameObject("AndroidHarnessServer").AddComponent<AndroidHarnessServerHost>();
            DontDestroyOnLoad(host.gameObject);
            host.m_MaxWorkItemsPerFrame = config.maxWorkItemsPerFrame;
            host.m_Server = new AndroidHarnessServer(config, FilesDirectory());
            host.m_Server.Start(config.port);

            Debug.Log(
                host.m_Server.IsRunning
                    ? $"SdkHarness: Android runtime server listening on port {host.m_Server.Port}"
                    : "SdkHarness: Android runtime server failed to start"
            );
        }

        private static string FilesDirectory()
        {
            using var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
            using var dir = activity.Call<AndroidJavaObject>("getFilesDir");
            return dir.Call<string>("getAbsolutePath");
        }

        private void Update()
        {
            m_Server?.Dispatcher.ProcessWorkQueue(m_MaxWorkItemsPerFrame);
            m_Server?.WatchdogTick();
        }

        private void OnApplicationQuit() => m_Server?.Stop();
    }
}
#endif
