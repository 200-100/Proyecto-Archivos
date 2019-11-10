using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoArchivos
{
    class HashEstatico_SubBloque
    {
        //
        private long dirsubbloque;
        //
        private object informacion;
        //
        private long dirInfo; //Esta es la direccion de el registro que tiene la informacion

        //
        public HashEstatico_SubBloque()
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
            get { return dirInfo; }
            set { dirInfo = value; }
        }
    }
}
