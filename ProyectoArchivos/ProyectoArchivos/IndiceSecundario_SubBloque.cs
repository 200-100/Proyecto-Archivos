using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoArchivos
{
    class IndiceSecundario_SubBloque
    {
        //
        private long dirsubbloque;
        //
        private long dirInfo;

        //
        public IndiceSecundario_SubBloque()
        {
        }
        //
        public long DirSubBloque
        {
            get { return dirsubbloque; }
            set { dirsubbloque = value; }
        }
        //
        public long DirInformacion
        {
            get { return dirInfo; }
            set { dirInfo = value; }
        }
    }
}
