using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace MyLib
{
    public static class ExtensionMethods
    {
        private static byte[] LoadBytes(this Stream s, int count)
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            if (count < 1)
                throw new ArgumentOutOfRangeException(nameof(count));
            byte[] result = new byte[count];
            s.Read(result, 0, count);
            return result;
        }

        private static bool LoadBoolean(this Stream s)
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            return Convert.ToBoolean(s.ReadByte());
        }

        private static short LoadInt16(this Stream s)
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            return BitConverter.ToInt16(s.LoadBytes(2), 0);
        }

        private static int LoadInt32(this Stream s)
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            return BitConverter.ToInt32(s.LoadBytes(4), 0);
        }

        private static float LoadFloat(this Stream s)
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            return BitConverter.ToSingle(s.LoadBytes(4), 0);
        }

        private static double LoadDouble(this Stream s)
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            return BitConverter.ToDouble(s.LoadBytes(8), 0);
        }

        private static string LoadString(this Stream s)
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            byte[] data = LoadBytes(s, LoadInt32(s));
            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (string)bf.Deserialize(ms);
            }

        }

        private static T LoadEnum<T>(this Stream s)
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            return (T)Enum.Parse(typeof(T), s.ReadByte().ToString(CultureInfo.InvariantCulture));
        }

        private static RectangleF LoadRectangleF(this Stream s)
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            return new RectangleF(LoadFloat(s), LoadFloat(s), LoadFloat(s), LoadFloat(s));
        }

        private static void LoadPathItem(this Stream s)
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            var e = s.LoadEnum<PathItemType>();
            switch (e)
            {
                case PathItemType.Point:
                    var x = s.LoadFloat();
                    var y = s.LoadFloat();
                    return;
                case PathItemType.Arc:
                    RectangleF rect = s.LoadRectangleF();
                    var startAngle = s.LoadFloat();
                    var sweepAngle = s.LoadFloat();
                    return;
                default: throw new NotSupportedException($"{nameof(PathItemType)} not supported.");
            }
        }

        private static void LoadPathInstruction(this Stream s)
        {
            var v = s.LoadInt32();
            var count = s.LoadInt32();
            for (int i = 0; i < count; i++)
                s.LoadPathItem();
            var name = s.LoadString();
            var desc = s.LoadString();
            var b = s.LoadBoolean();
            count = s.LoadInt32();
            for (int i = 0; i < count; i++)
            {
                var l = s.LoadDouble();
            }

        }

        private static byte[] DecompressBytesSharpZipLib(this byte[] data)
        {
            using (var ms_in = new MemoryStream(data))
            {
                using (var ms_out = new MemoryStream())
                {
                    using (var szl = new GZipStream(ms_in, CompressionMode.Decompress))
                    {
                        szl.CopyTo(ms_out);
                        return ms_out.ToArray();
                    }
                }
            }
        }

        private static void LoadDetailDimension(this Stream stream, int version)
        {
            var label = stream.LoadString();
            var labelOffset = stream.LoadDouble();
            var dimension = stream.LoadDouble();
            var dimensionOffset = version > 2 ? stream.LoadDouble() : 0.0;
            var orientation = stream.LoadEnum<DimensionOrientation>();
            var location = version > 3 ? stream.LoadEnum<DimensionLocation>() : DimensionLocation.Top;
        }

        private static void LoadDrawing(this Stream stream, int version)
        {
            var inputFile = stream.LoadString();
            var blockName = stream.LoadString();
            var rotateX = stream.LoadDouble();
            var rotateY = stream.LoadDouble();
            var rotateZ = stream.LoadDouble();
            var translateX = stream.LoadDouble();
            var translateY = stream.LoadDouble();
            var translateZ = stream.LoadDouble();
            var scaleX = stream.LoadDouble();
            var scaleY = stream.LoadDouble();
            var scaleZ = stream.LoadDouble();
            var PMUPath = version > 5 ? stream.LoadString() : string.Empty;
            int countComp = stream.LoadInt32();
            for (int i = 0; i < countComp; i++)
            {
                var c = stream.LoadString();
            }
            int countDim = stream.LoadInt32();
            for (int i = 0; i < countDim; i++)
            {
                LoadDetailDimension(stream, version);
            }
            int countDimLine = stream.LoadInt32();
            for (int i = 0; i < countDimLine; i++)
            {
                LoadDetailDimension(stream, version);
            }
        }


        private static void LoadDetailInstruction(this Stream stream)
        {
            var v = stream.LoadInt32();
            var l = stream.LoadInt32();
            var data = stream.LoadBytes(l);
            var decompressed = data.DecompressBytesSharpZipLib();
            using (var ms = new MemoryStream(decompressed))
            {
                var name = ms.LoadString();
                var fromX = ms.LoadDouble();
                var fromY = ms.LoadDouble();
                var toX = ms.LoadDouble();
                var toY = ms.LoadDouble();
                var extension = v > 4 ? ms.LoadDouble() : 0.0;
                var sort = v > 1 ? ms.LoadInt32() : 0;
                var PMUPath = v > 5 ? ms.LoadString() : string.Empty;
                var count = ms.LoadInt32();
                for (int i = 0; i < count; i++)
                    LoadDrawing(ms, v);
            }
        }

        private static PointF LoadPointF(this Stream s)
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            return new PointF(LoadFloat(s), LoadFloat(s));
        }

        private static void LoadDimensionInstruction(this Stream stream)
        {
            var v = stream.LoadInt32();
            var p1 = stream.LoadPointF();
            var orientation = stream.LoadEnum<DimensionOrientation>();
            var labelOffset = stream.LoadDouble();
            var label = stream.LoadString();
            var level = stream.LoadEnum<DimensionLevel>();
            var location = stream.LoadEnum<DimensionLocation>();
            var measureType = stream.LoadEnum<DimensionMeasure>();
            var dim = stream.LoadFloat();
            var radialCenter = stream.LoadPointF();
            var radialEnd = stream.LoadPointF();
        }

        public static void LoadResource()
        {
            using (var s = typeof(ExtensionMethods).Assembly.GetManifestResourceStream("MyLib.XA2JJBEJ_LIF_IMAGE.bin"))
            {
                if (s is null) Debug.Fail("not able to load resource.");
                var compressed = s.LoadBoolean();
                if (!compressed) Debug.Fail("resource is not compressed.");

                var inputStream = new DeflateStream(s, CompressionMode.Decompress) as Stream;

                var workAround = false;
                if (workAround)
                {
                    // the work around here is to fully decompress the resource
                    // as the bug seems to be related to a GZip operation on deflate stream.
                    var ms = new MemoryStream();
                    inputStream.CopyTo(ms);
                    inputStream.Dispose();
                    ms.Position = 0;
                    inputStream = ms;
                }

                using (inputStream)
                {
                    var version = inputStream.LoadInt16();
                    if (version < 2) Debug.Fail("");
                    var mostSigBytes = inputStream.ReadByte();
                    Debug.Assert(2 == mostSigBytes);
                    var leastSigBytes = inputStream.ReadByte();
                    Debug.Assert(3 == leastSigBytes);
                    var bounds = inputStream.LoadRectangleF();
                    if (version >= 5)
                    {
                        var count = inputStream.LoadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            var k = inputStream.LoadString();
                            var v = inputStream.LoadRectangleF();
                        }
                    }
                    if (version >= 3)
                    {
                        var caption = inputStream.LoadString();
                    }
                    if (version >= 5)
                    {
                        var count = inputStream.LoadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            var k = inputStream.LoadString();
                            var v = inputStream.LoadString();
                        }
                    }
                    if (version >= 4)
                    {
                        var count = inputStream.LoadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            var e = inputStream.LoadEnum<InstructionType>();
                            switch (e)
                            {
                                case InstructionType.CadDetail:
                                    inputStream.LoadDetailInstruction();
                                    break;
                                case InstructionType.CadDimension:
                                    inputStream.LoadDimensionInstruction();
                                    break;
                                case InstructionType.CadPath:
                                case InstructionType.CadOperation:
                                    inputStream.LoadPathInstruction();
                                    break;

                            }
                        }
                    }
                }
                    
            }
                
        }

    }
}
