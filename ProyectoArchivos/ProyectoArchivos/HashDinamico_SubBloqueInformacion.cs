using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoArchivos
{
    class HashDinamico_SubBloqueInformacion
    {
        //
        private long dirsub_bloque;
        //
        private int valor;
        //
        private long dirregistro;

        //CONSTRUCTOR
        public HashDinamico_SubBloqueInformacion()
        {
        }
        //
        public long DirSub_Bloque
        {
            get { return dirsub_bloque; }
            set { dirsub_bloque = value; }
        }
        //
        public int Valor
        {
            get { return valor; }
            set { valor = value; }
        }
        //
        public long DirRegistro
        {
            get { return dirregistro; }
            set { dirregistro = value; }
        }
    }
}
