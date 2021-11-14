using HarmonyLib;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using VRage;
using VRageMath;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace EnhancedUI.Gui.Render
{
    public class BatchDataPlayer : IVideoPlayer
    {
        private static readonly Func<Device> DeviceInstance =
            AccessTools.MethodDelegate<Func<Device>>(AccessTools.PropertyGetter(
                Type.GetType("VRage.Platform.Windows.Render.MyPlatformRender, VRage.Platform.Windows", true),
                "DeviceInstance"));

        private readonly Vector2I videoSize;
        private readonly Func<byte[]> dataGetter;
        private Texture2D? texture;
        private ShaderResourceView? shaderResourceView;

        public BatchDataPlayer(Vector2I size, Func<byte[]> dataGetter)
        {
            videoSize = size;
            this.dataGetter = dataGetter;
        }

        public void Init(string filename)
        {
            Texture2DDescription texture2DDescription = new Texture2DDescription
            {
                Width = VideoWidth,
                Height = VideoHeight,
                Format = Format.B8G8R8A8_UNorm_SRgb,
                ArraySize = 1,
                MipLevels = 1,
                BindFlags = BindFlags.ShaderResource,
                Usage = ResourceUsage.Dynamic,
                CpuAccessFlags = CpuAccessFlags.Write,
                SampleDescription =
                {
                    Count = 1,
                    Quality = 0
                },
                OptionFlags = ResourceOptionFlags.None
            };

            texture = new Texture2D(DeviceInstance(), texture2DDescription)
            {
                DebugName = "BatchDataPlayer.Texture"
            };

            ShaderResourceViewDescription shaderResourceViewDescription = new ShaderResourceViewDescription
            {
                Format = Format.B8G8R8A8_UNorm_SRgb,
                Dimension = ShaderResourceViewDimension.Texture2D,
                Texture2D =
                {
                    MipLevels = 1,
                    MostDetailedMip = 0
                }
            };

            shaderResourceView = new ShaderResourceView(DeviceInstance(), texture, shaderResourceViewDescription)
            {
                DebugName = texture.DebugName
            };
        }

        public void Dispose()
        {
            Stop();

            shaderResourceView?.Dispose();
            texture?.Dispose();
        }

        public void Play()
        {
            CurrentState = VideoState.Playing;
        }

        public void Stop()
        {
            CurrentState = VideoState.Stopped;
        }

        public void Update(object context)
        {
            if (CurrentState == VideoState.Playing && dataGetter() is { } data)
            {
                OnFrame((DeviceContext)context, data);
            }
        }

        private void OnFrame(DeviceContext context, byte[] data)
        {
            DataBox dataBox = context.MapSubresource(texture, 0, MapMode.WriteDiscard, MapFlags.None);

            if (dataBox.IsEmpty)
            {
                return;
            }

            Utilities.Write(dataBox.DataPointer, data, 0, data.Length);

            context.UnmapSubresource(texture, 0);
        }

        public int VideoWidth => videoSize.X;

        public int VideoHeight => videoSize.Y;

        public float Volume { get; set; }

        public VideoState CurrentState { get; private set; }

        public IntPtr TextureSrv => shaderResourceView?.NativePointer ?? IntPtr.Zero;
    }
}