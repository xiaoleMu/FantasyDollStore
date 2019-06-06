using UnityEngine;
using System.Collections;
using System.IO;

namespace TabTale {

    public class ReadyDto : BaseDto {
        public int level;
        
        protected override void DoSerialize (System.IO.BinaryWriter writer){
            writer.Write(level);
        }
        
        protected override void DoDesserialize (System.IO.BinaryReader reader){
            level=reader.ReadInt32();
        }
        
    }

}