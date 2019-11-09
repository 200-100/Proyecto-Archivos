using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoArchivos
{
    class Registro
    {
        //
        private long dirRegistro;
        //
        private List<object> informacion = new List<object>();
        //
        private long dirSigRegistro;

        //Constructor
        public Registro()
        {
        }
        //
        public long DirRegistro
        {
            get { return dirRegistro; }
            set { dirRegistro = value; }
        }
        //
        public List<object> Informacion
        {
            get { return informacion; }
            set { informacion = value; }
        }
        //
        public long DirSigRegistro
        {
            get { return dirSigRegistro; }
            set { dirSigRegistro = value; }
        }
    }
}
