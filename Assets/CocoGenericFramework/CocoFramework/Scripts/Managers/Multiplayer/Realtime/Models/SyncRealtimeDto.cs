using UnityEngine;
using System.Collections;
using System.IO;

namespace TabTale {

    public class SyncRealtimeDto : BaseDto {
        
        public int randomSeed;
        
        protected override void DoSerialize(BinaryWriter writer){
            writer.Write(randomSeed);
        }
        
        protected override void DoDesserialize(BinaryReader reader){
            randomSeed = reader.ReadInt32();
        }
        
    }
}