using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using HarmonyLib;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using VRage;
using VRageMath;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace EnhancedUI
{
    public class BatchDataPlayer : IVideoPlayer
    {
        private static readonly Func<Device> _deviceInstance =
            AccessTools.MethodDelegate<Func<Device>>(AccessTools.PropertyGetter(
                Type.GetType("VRage.Platform.Windows.Render.MyPlatformRender, VRage.Platform.Windows", true),
                "DeviceInstance"));

        private readonly Vector2I _size;
        private readonly Func<byte[]> _dataGetter;
        private Texture2D _texture;
        private ShaderResourceView _srv;
#pragma warning disable 8618
        public BatchDataPlayer(Vector2I size, Func<byte[]> dataGetter)
#pragma warning restore 8618
        {
            _size = size;
            _dataGetter = dataGetter;
        }

        public void Init(string filename)
        {
            var texture2DDescription = new Texture2DDescription
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
                OptionFlags = ResourceOptionFlags.None,
            };
            _texture = new(_deviceInstance(), texture2DDescription);
            var shaderResourceViewDescription = new ShaderResourceViewDescription
            {
                Format = Format.B8G8R8A8_UNorm_SRgb,
                Dimension = ShaderResourceViewDimension.Texture2D,
                Texture2D =
                {
                    MipLevels = 1,
                    MostDetailedMip = 0
                }
            };
            _srv = new(_deviceInstance(), _texture, shaderResourceViewDescription);
            _texture.DebugName = _srv.DebugName = "BatchDataPlayer.Texture";
        }

        public void Dispose()
        {
            Stop();
            _srv.Dispose();
            _texture.Dispose();
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
            if (CurrentState == VideoState.Playing && _dataGetter() is { } data)
                OnFrame((DeviceContext)context, data);
        }

        private void OnFrame(DeviceContext context, byte[] data)
        {
            var dataBox = context.MapSubresource(_texture, 0, MapMode.WriteDiscard, MapFlags.None);

            if (dataBox.IsEmpty)
                return;

            Utilities.Write(dataBox.DataPointer, data, 0, data.Length);

            context.UnmapSubresource(_texture, 0);
        }

        public int VideoWidth => _size.X;

        public int VideoHeight => _size.Y;

        public float Volume { get; set; }

        public VideoState CurrentState { get; private set; }

        public IntPtr TextureSrv => _srv.NativePointer;
    }
}