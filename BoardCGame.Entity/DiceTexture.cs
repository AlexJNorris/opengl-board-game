using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoardCGame.Entity.Enumerations;

namespace BoardCGame.Entity
{
    public class DiceTexture : Texture, ICloneable
    {
        public int FaceNumber { get; set; }
        public EnumDiceTextureType TextureTypeType { get; set; }

        public DiceTexture(int id, int width, int height, EnumDiceTextureType diceTextureType, int faceNumber)
            : base(id,width,height)
        {
            this.TextureTypeType = diceTextureType;
            this.FaceNumber = faceNumber;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
