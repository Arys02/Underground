using System;
using System.Collections.Generic;
using System.IO;
//using System.Linq;
using System.Text;
using System.Threading;
using SharpDX.Direct3D9;
using SharpDX.DirectSound;
using SharpDX.XAudio2;
using SharpDX.Multimedia;
using BufferFlags = SharpDX.XAudio2.BufferFlags;

namespace Underground
{
    class Sound
    {
        public static void main()
        {
            /*var audio = new XAudio2();
            var mastervoice = new MasteringVoice(audio);
            //Playsound(audio, "dark.wav");*/
        }
        /*static void Playsound(XAudio2 device, string path)
        {
            var stream = new SoundStream(File.OpenRead(path));
            var waveformat = stream.Format;
            var buffer = new AudioBuffer
                {
                    Stream = stream.ToDataStream(),
                    AudioBytes = (int) stream.Length,
                    Flags = BufferFlags.EndOfStream
                };
            stream.Close();
            var sourcevoice = new SourceVoice(device, waveformat, true);
            
            
            
            while (true)
            {
                sourcevoice.SubmitSourceBuffer(buffer, stream.DecodedPacketsInfo);
                sourcevoice.Start();
                int count = 0;
                while (sourcevoice.State.BuffersQueued > 0)
                {
                    if (count == 50)
                    {
                        Console.Out.Flush();
                        count = 0;
                    }
                    Thread.Sleep(10);
                    count++;
                }
            
            }
           
            sourcevoice.DestroyVoice();
            sourcevoice.Dispose();
            buffer.Stream.Dispose();
        }*/
    }
}
