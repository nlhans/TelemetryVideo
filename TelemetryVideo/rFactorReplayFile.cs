using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace TelemetryVideo
{
    public class rFactorReplayFile
    {
        public string File { get; set; }
        public IEnumerable<rFactorReplayPlayer> Players { get; protected set; }
        public rFactorReplayFile(string File)
        {
            this.File = File;
        }

        public void Read()
        {
            var pls = new List<rFactorReplayPlayer>();
            // Thanks to:
            //http://isiforums.net/f/archive/index.php/t-116.html
            var read = new BinaryReader(System.IO.File.OpenRead(File));

            while (read.ReadByte() != 0x0A) ;

            // That was the header.
            var unknown1 = read.ReadChars(8);
            var rfmFileNameLength = read.ReadUInt32();
            var rfmFileName = read.ReadChars((int)rfmFileNameLength);
            var unknownStringLengnth = read.ReadUInt32();
            var unkownString = read.ReadChars((int)unknownStringLengnth);
            var scnFileNameLength = read.ReadUInt32();
            var scnFileName = read.ReadChars((int)scnFileNameLength);
            var aiwFileNameLength = read.ReadUInt32();
            var aiwFileName = read.ReadChars((int)aiwFileNameLength);
            var unknown21 = read.ReadUInt32();
            var playercount = (int)read.ReadUInt32();

            for (var player = 0; player < playercount; player++)
            {
                var pl = new rFactorReplayPlayer();
                pl.Slot = (int)read.ReadChar();
                var driverNameLength = read.ReadChar();
                var cName = read.ReadChars(driverNameLength);
                pl.Name = string.Join("", cName); // does this work?

                var driverName2Length = read.ReadChar();
                var cName2 = read.ReadChars(driverName2Length);
                //

                var car = read.ReadChars(32);
                pl.VehicleName = string.Join("", car);
                var skin = read.ReadChars(32);
                pl.Skin = string.Join("", skin);

                pl.Upgrades = read.ReadChars(8);
                pl.EntryTime = read.ReadSingle();
                pl.ExitTime = read.ReadSingle();
                pls.Add(pl);
            }

            byte[] garbage = read.ReadBytes(36);
            bool ok = true;
            byte[] allData = read.ReadBytes((int)(read.BaseStream.Length - read.BaseStream.Position));

            double lastTime = 0;
            int i = -1;
            while (i < allData.Length-5)
            {
                i++;
                var timestamp = BitConverter.ToSingle(allData, i);
                if (float.IsNaN(timestamp) || float.IsInfinity(timestamp)) continue;
               if (timestamp <= 0 || timestamp >=3600) continue;
                double dt = timestamp - lastTime;
                if(lastTime!= 0)
                {
                    if (dt < 0 || dt > 1) continue;
                }
                lastTime = timestamp;

                Debug.WriteLine(timestamp);
                var eventCount = allData[i + 4];
                var unknown = allData[i+5];
                if (eventCount == 0 || eventCount > 10) continue;

                i += 6;
                while (eventCount-- > 0)
                {
                    var eventHeader = BitConverter.ToUInt32(allData, i); i += 4;
                    var eventTimeAdjustment = allData[i]; i++;
                    var type = GetType(eventHeader);
                    var cls= GetClass(eventHeader);

                    if (cls == rFactorReplayEventClass.ECLASS_LOC)
                    {
                        var data = new byte[8 + 18 + 6*4];
                        Array.Copy(allData, i, data, 0, data.Length);
                        var d = new rFactorReplayData();
                        //
                        var tmp1 = BitConverter.ToUInt32(data, 0);

                        var rps = tmp1 >> 18;
                        Console.WriteLine(rps);
                        var rpm = rps*60;
                    }
                }
                if (read.BaseStream.Length - read.BaseStream.Position < 100) 
                    ok = false;

            }
        }
        rFactorReplayEventClass GetClass(uint mPack)
        {
            return (rFactorReplayEventClass)(mPack >> 29);
        }


        rFactorReplayEventType GetType(uint mPack)
        {
            return (rFactorReplayEventType)((mPack >> 18) & 0x03f);
        }

    }

    public enum rFactorReplayEventClass
    {
        ECLASS_LOC = 0,
        ECLASS_VFX,
        ECLASS_SFX,
        ECLASS_SYS,
        ECLASS_COMM,
        ECLASS_OTHER,
        ECLASS_MAXIMUM
    }
}