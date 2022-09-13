using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class Type
    {
        string name;
        string imgSrc;

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                    name = value;
            }
        }

        public string ImgSrc
        {
            get { return imgSrc; }
            set
            {
                if (imgSrc != value)
                    imgSrc = value;
            }
        }

    }
}
