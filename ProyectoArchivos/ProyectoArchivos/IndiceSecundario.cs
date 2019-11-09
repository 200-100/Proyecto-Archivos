using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoArchivos
{
    class IndiceSecundario
    {
        //VARIABLES
        //
        private object id;
        //
        private long dirindice;
        //
        private List<IndiceSecundario_SubBloque> sub_bloque;
        //
        private long dirbloque;

        //Constructor
        public IndiceSecundario()
        {
        }
        //
        public object ID
        {
            get { return id; }
            set { id = value; }
        }
        //
        public long DirIndice
        {
            get { return dirindice; }
            set { dirindice = value; }
        }
        //
        public List<IndiceSecundario_SubBloque> Sub_Bloque
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
    }
}
