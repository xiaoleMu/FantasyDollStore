using UnityEngine;
using System.Collections;

namespace TabTale {

    public class GameOverDto : BaseDto {
        
        public int score;
        
        protected override void DoSerialize (System.IO.BinaryWriter writer){
            writer.Write(score);
        }
        
        protected override void DoDesserialize (System.IO.BinaryReader reader){
            score=reader.ReadInt32();
        }
    }

}