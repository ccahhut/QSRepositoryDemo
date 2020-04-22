using System;
using System.Collections.Generic; 
using System.Text;
using System.Runtime.InteropServices;   
namespace QManage
{
     public class SoundPlay
    {
        protected const int SND_SYNC = 0x0;
        protected const int SND_ASYNC = 0x1;
        protected const int SND_NODEFAULT = 0x2;
        protected const int SND_MEMORY = 0x4;
        protected const int SND_LOOP = 0x8;
        protected const int SND_NOSTOP = 0x10;
        protected const int SND_NOWAIT = 0x2000;
        protected const int SND_ALIAS = 0x10000;
        protected const int SND_ALIAS_ID = 0x110000;
        protected const int SND_FILENAME = 0x20000;
        protected const int SND_RESOURCE = 0x40004;
        protected const int SND_PURGE = 0x40;
        protected const int SND_APPLICATION = 0x80;
        [DllImport("Winmm.dll", CharSet = CharSet.Auto, EntryPoint = "PlaySound")]
        protected extern static bool PlaySound(string strFile, IntPtr hMod, int flag);

        /// <summary>
        /// 播放声音函数
        /// </summary>
        /// <param name="strSoundFile">声音文件</param>
        /// <param name="bSynch">是否同步，如果为True，则播放声音完毕再执行后面的操作，为False，则播放声音的同时继续执行后面的操作</param>
        /// <returns></returns>
        public static bool PlaySoundFile(string strSoundFile, bool bSynch)
        {
            if (!System.IO.File.Exists(strSoundFile))
                return false;
            int Flags;
            if (bSynch)
                Flags = SND_FILENAME | SND_SYNC;
            else
                Flags = SND_FILENAME | SND_ASYNC;

            return PlaySound(strSoundFile, IntPtr.Zero, Flags);
        }
    }
     
}
