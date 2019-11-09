using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoArchivos
{
    class HashDinamico_Bloque
    {
        //
        private long dirhash;
        // 
        private string codigob;
        //
        private long dirsub_bloque;
        //
        private HashDinamico_SubBloque sub_bloque;

        //CONSTRUCTOR
        public HashDinamico_Bloque()
        {
        }
        //
        public long DirHash
        {
            get { return dirhash; }
            set { dirhash = value; }
        }
        //
        public string CodigoB
        {
            get { return codigob; }
            set { codigob = value; }
        }
        //
        public long DirSub_Bloque
        {
            get { return dirsub_bloque; }
            set { dirsub_bloque = value; }
        }
        //
        public HashDinamico_SubBloque Sub_Bloque
        {
            get { return sub_bloque; }
            set { sub_bloque = value; }
        }
    }
}
