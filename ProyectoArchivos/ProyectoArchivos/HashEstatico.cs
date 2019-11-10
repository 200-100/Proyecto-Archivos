using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoArchivos
{
    class HashEstatico
    {
        //VARIABLES
        //
        private long dirindice;
        //
        private List<HashEstatico_SubBloque> sub_bloque;
        //
        private long dirbloque;
        //
        private long desbordamiento;

        //Constructor
        public HashEstatico()
        {
        }

        //
        public long DirIndice
        {
            get { return dirindice; }
            set { dirindice = value; }
        }
        //
        public List<HashEstatico_SubBloque> Sub_Bloque
        {
            get { return sub_bloque; }
            set { sub_bloque = value; }
        }
        //
        public long DirBloque
        {
            get { return dirbloque; }
            set { dirbloque = value; }
        }
        //
        public long Desbordamiento
        {
            get { return desbordamiento; }
            set { desbordamiento = value; }
        }
    }
}
