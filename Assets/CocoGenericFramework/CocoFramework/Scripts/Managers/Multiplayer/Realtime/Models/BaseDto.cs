using UnityEngine;
using System.Collections;
using System.IO;

namespace TabTale {

    public class BaseDto {
        
        
        public byte[] Serialize() {
            using (MemoryStream m = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(m)) {
                    DoSerialize(writer);
                }
                return m.ToArray();
            }
        }
        
        public void Desserialize(byte[] data) {
            using (MemoryStream m = new MemoryStream(data)) {
                using (BinaryReader reader = new BinaryReader(m)) {
                    DoDesserialize(reader);
                }
            }
        }
        
        protected virtual void DoSerialize(BinaryWriter writer){
            
        }
        
        protected virtual void DoDesserialize(BinaryReader reader){
            
        }
    }

}

