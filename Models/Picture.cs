using System;
using System.Collections.Generic;

namespace SAVINAFILMS
{
    public partial class Picture
    {
        public int PictureId { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }

        public virtual Film PictureNavigation { get; set; }
    }
}
