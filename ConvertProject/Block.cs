using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertProject
{
    class Block
    {
        //TODO: make private and add proper public vars to mutate 
        public string name { get; set; }
        public string texture_image { get; set; }
        public string game_ID { get; set; }
        public string game_ID13 { get; set; }
        public int block_ID { get; set; }
        public int data_ID { get; set; }
        public bool luminance { get; set; }
        public bool transparency { get; set; }
        public bool falling { get; set; }
        public bool redstone { get; set; }
        public bool survival { get; set; }
        public int version { get; set; }
        public int id { get; set; }

        public Block(string name, string texture_image, string game_ID, string game_ID13, int block_ID, int data_ID, bool luminance, bool transparency, bool falling, bool redstone, bool survival, int version, int id)
        {
            this.name = name;
            this.texture_image = texture_image;
            this.game_ID = game_ID;
            this.game_ID13 = game_ID13;
            this.block_ID = block_ID;
            this.data_ID = data_ID;
            this.luminance = luminance;
            this.transparency = transparency;
            this.falling = falling;
            this.redstone = redstone;
            this.survival = survival;
            this.version = version;
            this.id = id;
        }
    }
}
