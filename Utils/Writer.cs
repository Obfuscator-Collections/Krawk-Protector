using System;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;
using dnlib.IO;
using dnlib.PE;
namespace Krawk_Protector.Utils
{

	public class Writer : IModuleWriterListener {
		public static NativeModuleWriterOptions Run(Context ctx) {
			return new Writer().DoIt(ctx);
		}

		NativeModuleWriterOptions DoIt(Context ctx) {
            var mod = ctx.ManifestModule;
                var opts = new NativeModuleWriterOptions(mod as ModuleDefMD);
                opts.MetaDataOptions.Flags = MetaDataFlags.PreserveAll;
                opts.MetaDataLogger = DummyLogger.NoThrowInstance;
                opts.Cor20HeaderOptions.Flags -= dnlib.DotNet.MD.ComImageFlags.ILOnly;
                opts.Cor20HeaderOptions.Flags = dnlib.DotNet.MD.ComImageFlags._32BitRequired;
                opts.Listener = this;
                return opts;
    }
    dnlib.DotNet.Writer.MethodBody codeChunk;
    public void OnWriterEvent(ModuleWriterBase writer, ModuleWriterEvent evt)
            {
                if (evt == ModuleWriterEvent.MDEndWriteMethodBodies)
                {
                    byte[] native_ = Encode_();
                    codeChunk = writer.MethodBodies.Add(new dnlib.DotNet.Writer.MethodBody(native_));
                }
                else if (evt == ModuleWriterEvent.EndCalculateRvasAndFileOffsets)
                {
                    uint rid = writer.MetaData.GetRid(Protections.Constants.ConstantsProtection.native_);
                    writer.MetaData.TablesHeap.MethodTable[rid].RVA = (uint)codeChunk.RVA;
                }
		}
        public static byte[] Encode_()
            {
                var stream = new MemoryStream();
                using (var writer = new BinaryWriter(stream))
                {
                   //push ebp
                   //mov ebp,esp
                   //mov eax,0x1A = 26
                   //pop ebp
                   //ret
                    writer.Write(new byte[] { 0x55, 0x48 });
                    writer.Write(new byte[] { 0x89, 0xE5});
                    writer.Write(new byte[] { 0xB8, 0x1A, 0x00, 0x00, 0x00});
                    writer.Write(new byte[] { 0x5D });
                    writer.Write(new byte[] { 0xC3 });

                }
                return stream.ToArray();
            }
    }
}