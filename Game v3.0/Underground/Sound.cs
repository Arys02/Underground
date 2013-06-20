using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using SharpDX.XAudio2;
using SharpDX.Multimedia;
using BufferFlags = SharpDX.XAudio2.BufferFlags;
using System.Security.Cryptography;

namespace Underground
{

    class Sound
    {
        static XAudio2 audio = new XAudio2();
        public static Thread pas = new Thread(bruitpas);
        public static MasteringVoice mastervoice = new MasteringVoice(audio, 3);
        public static SubmixVoice Voice = new SubmixVoice(audio, 1);
        public static Random Random = new Random();
        public static bool soundContinue = true;
        public static int random(int min, int max)
        {
            byte[] bytes = new byte[1];
            new RNGCryptoServiceProvider().GetBytes(bytes);
            return bytes[0] % (max - min) + min;
        }

        public static void main()
        {

            Thread ambiance2 = new Thread(Playbackground);
            Thread ambiance = new Thread(PlayEvenement);
            ambiance.Start();
            ambiance2.Start();
            

        }

        public static void bruitpas()
        {

            Playsound(audio,
                      (!Program.input.KeysDown.Contains(Camera.keyrun)
                           ? @"Ressources\Sound\pas.wav"
                           : @"Ressources\Sound\Cours.wav"), 5, false);
            
            pas.Abort();
            pas = new Thread(bruitpas);
        }

        public static void PlayEvenement()
        {
            string even = @"Ressources\Sound\bruitage\";
            string[] soundbruitage = new[]
                {
                    even+"cri.wav", even+"gore.wav",
                    even+"goute.wav", even+"impact.wav",
                    even+"kids.wav", even+"bidoch.wav",
                    even+"bouillon.wav", even+"Breath.wav",
                    even+"Breath1.wav", even+"Breath3.wav",
                    even+"Chains.wav",even+"fire.wav",
                    //even+"Growl.wav", even+"Snarl1.wav",
                    even+"Wind1.wav", even+"Wind2.wav",
                    even+"Wind3.wav", even+"Wind4.wav"
                };

            while (true)
            {
                int y = Random.Next(0, soundbruitage.Length);
                int time = Random.Next(50000, 120000);
                Console.WriteLine(soundbruitage[y] + " : " + y + " time : " + time);
                Thread.Sleep(time);
                switch (y)
                {
                    case 2:
                        for (int i = 0; i < soundbruitage.Length; i++)
                            Playsound(audio, soundbruitage[y], 2, true);
                        break;
                    case 11:
                        Playsound(audio, soundbruitage[y], 2, true);
                        Playsound(audio, soundbruitage[y], 2, true);
                        break;
                    default:
                        Playsound(audio, soundbruitage[y], 2, true);
                        break;
                }
            }
        }
        public static void Playbackground()
        {

            Voice.SetVolume(10);
            string ambiance = @"Ressources\Sound\ambiance\";
            string[] soundAmbiance = new[]
                {
                    @ambiance+@"a1.wav",
                    @ambiance+@"a3.wav",
                    @ambiance+@"a4.wav",
                    @ambiance+@"a5.wav",
                    @ambiance+@"a6.wav", 
                    @ambiance+@"a9.wav"
                };
            while (true)
            {

                int y = random(0, 5);
                Console.WriteLine("son no : " + soundAmbiance[y]);
                int time = random(10000, 30000);
                Console.WriteLine(time + " ms");

                Playsound(audio, soundAmbiance[y], 1, true);

                Thread.Sleep(time);

            }
        }


        static void Playsound(XAudio2 device, string path, int volume, bool ambiance)
        {

            var stream = new SoundStream(File.OpenRead(path));
            bool boolwhile = true;
            var waveformat = stream.Format;
            var buffer = new AudioBuffer
            {
                Stream = stream.ToDataStream(),
                AudioBytes = (int)stream.Length,
                Flags = BufferFlags.EndOfStream
            };
            stream.Close();

            var sourcevoice = new SourceVoice(device, waveformat, true);
            sourcevoice.SubmitSourceBuffer(buffer, stream.DecodedPacketsInfo);
            sourcevoice.SetVolume(volume);
            sourcevoice.Start();
            int count = 0;

            while (sourcevoice.State.BuffersQueued > 0  && boolwhile)
            {
                if (ambiance || soundContinue)
                {
                   if (count == 50)
                    {
                        Console.Out.Flush();
                        count = 0;
                    }

                    //Thread.Sleep(10);
                    count++;
                   
                }
                else
                {
                    boolwhile = false;
                }
               

            }
            sourcevoice.DestroyVoice();
            sourcevoice.Dispose();
            buffer.Stream.Dispose();
        }



    }
}

