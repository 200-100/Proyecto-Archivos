using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoArchivos
{
    class IndicePrimario_SubBloque
    {
        //
        private long dirsubbloque;
        //
        private object informacion;
        //
        private long dirinformacion;

        //CONSTRUCTOR
        public IndicePrimario_SubBloque()
        {
        }
        //
        public long DirSubBloque
        {
            get { return dirsubbloque; }
            set { dirsubbloque = value; }
        }
        //
        public object Informacion
        {
            get { return informacion; }
            set { informacion = value; }
        }
        //
        public long DirInformacion
        {
            get { return dirinformacion; }
            set { dirinformacion = value; }
        }
    }
}
