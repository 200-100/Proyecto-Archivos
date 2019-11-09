using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoArchivos
{
    class IndicePrimario
    {
        //
        private long dirIndice;
        //
        private object id;
        //
        private List<IndicePrimario_SubBloque> sub_bloque;
        //
        private long dirBloque;

        //CONSTRUCTOR
        public IndicePrimario()
        {
        }
        //
        public long DirIndice
        {
            get { return dirIndice; }
            set { dirIndice = value; }
        }
        //
        public object ID
        {
            get { return id; }
            set { id = value; }
        }
        //
        public List<IndicePrimario_SubBloque> Sub_Bloque
        {
            get { return sub_bloque; }
            set { sub_bloque = value; }
        }
        //
        public long DirBloque
        {
            get { return dirBloque; }
            set { dirBloque = value; }
        }
    }
}
